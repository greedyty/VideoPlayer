using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Oville.Core.InputEvents
{
	public class ClickCapture : MonoBase,
		IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
		IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler,
		IMoveHandler, IScrollHandler, ICancelHandler
	{
		public bool Use2DPhysics;
		
		public static bool Enable;
		public static bool IsDragging;

		private Camera raycastCamera;
		public Camera RaycastCamera
		{
			get
			{
				if (this.raycastCamera == false)
				{
					this.raycastCamera = Camera.main;
				}
				if (this.raycastCamera == false)
				{
					Debug.LogError("[ClickCapture] RaycastCamera field is not set.");
				}
				return this.raycastCamera;
			}
		}
		public GameObject ActiveHoverObject; // the object the pointer is over
		//public GameObject ActiveDestinationObject;
		private static GameObject _activeDragObject;
        public static GameObject ActiveDragObject
		{
			get
			{
				return _activeDragObject;
			}
			set
			{
				_activeDragObject = null;
				_activeDragObject = value;
			}
		}

        //public Action StartDrag, FinishDrag;
		//private enum ClickState { Click, LongPress, DoubleClick }

		//public bool OverUI;
		//public static bool IsPointerOverGameObject = false;

		protected override void Awake()
		{
			base.Awake();
			//if (RaycastCamera == false) Debug.LogError("[ClickCapture] RaycastCamera field is not set.");

			Enable = true; // DEBUG.  Remove this.
		}

		public void OnPointerClick(PointerEventData eventData)
		{	
			GameObject go = RaycastWorld(eventData.position);
			if (go)
			{
				// what type of click?
				if (this.clickType == ClickState.Click)
				{
					//Debug.Log("[ClickCapture] CLICK");

					IInputClickHandler[] clicks = go.GetComponents<IInputClickHandler>();
					for (int i = 0; i < clicks.Length; i++)
					{
						if (((MonoBehaviour)clicks[i]).enabled) clicks[i].OnInputClick(eventData);
					}
					clicks = null;
				}
				else if (this.clickType == ClickState.LongPress)
				{
					//Debug.Log("[ClickCapture] LONG PRESS");

					IInputLongPressHandler[] longpresses = go.GetComponents<IInputLongPressHandler>();
					for (int i = 0; i < longpresses.Length; i++)
					{
						if (((MonoBehaviour)longpresses[i]).enabled) longpresses[i].OnInputLongPress(eventData);
					}
					longpresses = null;
				}

				this.clickType = ClickState.Click; //  RESET

				eventData.rawPointerPress = go;
				eventData.pointerPress = go;				
			}

            if (InputPointerClickEvent != null) InputPointerClickEvent(eventData);
        }
						
		public void OnPointerDown(PointerEventData eventData)
		{
			//Debug.Log("[ClickCapture] POINTER DOWN");
			if (this.pointerDownTimer != null) StopCoroutine(this.pointerDownTimer);
			this.pointerDownTimer = OnWhilePointerIsDown();
			StartCoroutine(this.pointerDownTimer);
						
			GameObject go = RaycastWorld(eventData.position);
			if (go)
			{
				ClearActiveHoverObject();
				ActiveHoverObject = go;
				IInputDownHandler[] clicks = go.GetComponents<IInputDownHandler>();
				for (int i = 0; i < clicks.Length; i++)
				{
					if (((MonoBehaviour)clicks[i]).enabled) clicks[i].OnInputDown(eventData);
				}
				clicks = null;
			}

			if (InputPointerDownEvent != null) InputPointerDownEvent(eventData);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			//Debug.Log("[ClickCapture] POINTER UP");
			if (ActiveHoverObject)
			{
				// clear last touched object
				IInputUpHandler[] up = ActiveHoverObject.GetComponents<IInputUpHandler>();
				for (int i = 0; i < up.Length; i++)
				{
					if (((MonoBehaviour)up[i]).enabled) up[i].OnInputUp();
				}
				up = null;
			}
			ClearActiveHoverObject();
			if (InputPointerUpEvent != null) InputPointerUpEvent(eventData);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			//IsPointerOverGameObject = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			//IsPointerOverGameObject = false;
		}
		
		public void OnInitializePotentialDrag(PointerEventData eventData) { }

		public void OnBeginDrag(PointerEventData eventData)
        {
			IsDragging = true;
			if (ActiveDragObject != null) return;
						
            GameObject go = RaycastWorld(eventData.position);

            if (go && go.GetComponents<IInputDragHandler>().Length != 0)
            {
                ClearActiveDragObject();
                ActiveDragObject = go;
                IInputDragHandler[] clicks = go.GetComponents<IInputDragHandler>();
                for (int i = 0; i < clicks.Length; i++)
                {
                    if (((MonoBehaviour)clicks[i]).enabled) clicks[i].OnBeginDrag(eventData);
                }
				clicks = null;
				if (InputDragStartEvent != null) InputDragStartEvent(eventData);
            }
        }

		public void OnDrag(PointerEventData eventData)
		{
            if (ActiveDragObject)
            {
                IInputDragHandler[] clicks = ActiveDragObject.GetComponents<IInputDragHandler>();
                for (int i = 0; i < clicks.Length; i++)
                {
                    if (((MonoBehaviour)clicks[i]).enabled) clicks[i].OnDrag(eventData);
                }
            }

            if (InputDragEvent != null)
			{
				InputDragEvent(eventData);
			}

			Vector2 pos = Vector2.zero;
			if (Input.touches.Length > 0)
			{
				pos = Input.touches[0].position;
			}
			else
			{
				pos = eventData.position;
			}
			
			
			// HOVER
			GameObject go = RaycastWorld(pos);
			if (go)
			{
				// enter & exit objects
				if (ActiveHoverObject != null && go != ActiveHoverObject)
				{
					IInputHoverHandler[] exits = ActiveHoverObject.GetComponents<IInputHoverHandler>();
					for (int i = 0; i < exits.Length; i++)
					{
						if (((MonoBehaviour)exits[i]).enabled) exits[i].OnInputHoverExit(eventData);
					}
				}

				if (ActiveHoverObject != go)
				{ 
					IInputHoverHandler[] enters = go.GetComponents<IInputHoverHandler>();
					for (int i = 0; i < enters.Length; i++)
					{
						if (((MonoBehaviour)enters[i]).enabled) enters[i].OnInputHoverEnter(eventData);
					}
				}
				
				ActiveHoverObject = go;
				IInputHoverHandler[] hovers = go.GetComponents<IInputHoverHandler>();

				for (int i = 0; i < hovers.Length; i++)
				{
					if (((MonoBehaviour)hovers[i]).enabled) hovers[i].OnInputHover(eventData);
				}				
			}
			else
			{
				if (ActiveHoverObject != null)
				{
					IInputHoverHandler[] exits = ActiveHoverObject.GetComponents<IInputHoverHandler>();
					for (int i = 0; i < exits.Length; i++)
					{
						if (((MonoBehaviour)exits[i]).enabled) exits[i].OnInputHoverExit(eventData);
					}
				}
				ClearActiveHoverObject();
			}

			/*
            #region Oville Ed
            GameObject obj = RaycastInteractiveObj(eventData.position);

            if (obj)
            {
                if (this.ActiveDestinationObject != null && obj != this.ActiveDestinationObject)
                {
                    IInputDestinationHandler[] exits = ActiveDestinationObject.GetComponents<IInputDestinationHandler>();

                    for (int i = 0; i < exits.Length; i++)
                    {
                        if (((MonoBehaviour)exits[i]).enabled) exits[i].OnDestinationExit();
                    }
                }

                if (obj != this.ActiveDestinationObject)
                {
                    IInputDestinationHandler[] enters = obj.GetComponents<IInputDestinationHandler>();
                    for (int i = 0; i < enters.Length; i++)
                    {
                        if (((MonoBehaviour)enters[i]).enabled) enters[i].OnDestinationEnter(eventData);
                    }
                }
             
                ActiveDestinationObject = obj;
                IInputDestinationHandler[] stays = ActiveDestinationObject.GetComponents<IInputDestinationHandler>();

                for (int i = 0; i < stays.Length; i++)
                {
                    if (((MonoBehaviour)stays[i]).enabled) stays[i].OnDestinationStay(eventData);
                }
            }
            else
            {
                if(ActiveDestinationObject != null)
                {
                    IInputDestinationHandler[] exits = ActiveDestinationObject.GetComponents<IInputDestinationHandler>();

                    for (int i = 0; i < exits.Length; i++)
                    {
                        if (((MonoBehaviour)exits[i]).enabled) exits[i].OnDestinationExit();
                    }
                }
                ActiveDestinationObject = null;
            }

            #endregion
			*/
            /*
			RaycastHit2D hit2d = Physics2D.Raycast(this.RaycastCamera.ScreenToWorldPoint(eventData.position), Vector2.zero);
			if (hit2d.collider != null)
			{
				ClearActiveObject();
				this.ActiveObject = hit2d.transform.gameObject;

				//Debug.Log("[INputMangaer] HIT");
				IInputHoverHandler hover = hit2d.transform.GetComponent<IInputHoverHandler>();
				if (hover != null)
				{
					hover.OnInputHover(eventData);
				}
			}
			else
			{
				ClearActiveObject();
			}
			*/
        }

        public void OnDrop(PointerEventData eventData) { }

		public void OnEndDrag(PointerEventData eventData)
        {
			IsDragging = false;

			if (ActiveDragObject)
            {
                IInputDragHandler[] clicks = ActiveDragObject.GetComponents<IInputDragHandler>();
                for (int i = 0; i < clicks.Length; i++)
                {
                    if (((MonoBehaviour)clicks[i]).enabled) clicks[i].OnEndDrag(eventData);
                }

                if (InputDragEndEvent != null) InputDragEndEvent(eventData);
            }
            ClearActiveDragObject();
        }

		public void OnScroll(PointerEventData eventData) { }

		public void OnCancel(BaseEventData eventData) { }

		public void OnMove(AxisEventData eventData)
		{
			//IsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
			//this.OverUI = EventSystem.current.IsPointerOverGameObject();
			//if (InputMoveEvent != null) InputMoveEvent(eventData);
		}       

		private GameObject RaycastWorld(Vector2 pos)
		{
			if (this.Use2DPhysics)
			{
				RaycastHit2D hit2d = Physics2D.Raycast(this.RaycastCamera.ScreenToWorldPoint(pos), Vector2.zero, Mathf.Infinity, Physics.DefaultRaycastLayers);
				if (hit2d.collider != null)
				{
					return hit2d.transform.gameObject;
				}
			}
			else
			{
				RaycastHit hit;
				Ray ray = this.RaycastCamera.ScreenPointToRay(pos);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers)) 
				{
					return hit.transform.gameObject;
				}
			}

			return null;
		}

        

		private void ClearActiveHoverObject()
		{
			if (ActiveHoverObject)
			{
				// clear last touched object
				//IInputUpHandler[] up = this.ActiveHoverObject.GetComponents<IInputUpHandler>();
				//for (int i = 0; i < up.Length; i++)				
				//{
				//	up[i].OnInputUp();
				//}

				//IInputHoverHandler[] hovers = this.ActiveHoverObject.GetComponents<IInputHoverHandler>();
				//for (int i = 0; i < hovers.Length; i++)
				//{
				//	hovers[i].OnInputHoverExit();
				//}
				
				ActiveHoverObject = null;
			}
		}

        private void ClearActiveDragObject()
        {
            if (ActiveDragObject)
            {
                ActiveDragObject = null;
            }
        }

		// LONG PRESS
		//public bool drag = false;
		//public bool zoom = false;
		private float TapDuration = 0.4f; // seconds until duration tap is triggered
		private IEnumerator pointerDownTimer;
		private ClickState clickType = ClickState.Click;
		private IEnumerator OnWhilePointerIsDown()
		{
			this.clickType = ClickState.Click;			
			yield return new WaitForSeconds(this.TapDuration);
			this.clickType = ClickState.LongPress;
		}


		//private void Update()
		//{
		//	Debug.Log( EventSystem.current.currentSelectedGameObject.name);
		//	IsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
		//	this.OverUI = EventSystem.current.IsPointerOverGameObject();
		//}

		// ////////////////////////////////////////////////////////////////////////////////
		/*
		public bool OverUI = false;
		private void Update()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				this.OverUI = true;
				return;
			}
			else
			{
				this.OverUI = false;
			}
		}
		*/
		/************
		 * AN EXAMPLE OF DRAGGING OBJECTS IN 3D-SPACE
		 ************
		private Vector3 prevPointWorldSpace;
		private Vector3 thisPointWorldSpace;
		private Vector3 realWorldTravel;

		private int drawFinger;
		private bool drawFingerAlreadyDown
		public void OnDrag3D(PointerEventData data)
		{
			if (drawFingerAlreadyDown == false)
				return;
			if (drawFinger != data.pointerId)
				return;

			thisPointWorldSpace = data.pointerCurrentRaycast.worldPosition;

			realWorldTravel = thisPointWorldSpace - prevPointWorldSpace;
			_processRealWorldtravel();

			prevPointWorldSpace = thisPointWorldSpace;
		}
		private void _processRealWorldtravel()
		{
			Vector3 pot = moveMe.position;

			pot.x += realWorldTravel.x;
			pot.y += realWorldTravel.y;
			moveMe.position = pot;
		}
		*/
				
		#region 3D Tools
		public static GameObject WhatDidIHit(Vector3 pointerPos)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(pointerPos);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				return hit.transform.gameObject;
			}
			return null;
		}

		public static GameObject WhatDidIHit(Vector3 pointerPos, LayerMask mask)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(pointerPos);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
			{
				return hit.transform.gameObject;
			}
			return null;
		}

		public GameObject[] WhatAllDidIHit(Vector3 pointerPos)
		{

			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay(pointerPos);
			hits = Physics.RaycastAll(ray, Mathf.Infinity);
			GameObject[] gos = new GameObject[hits.Length];
			for (int i = 0; i < hits.Length; i++)
			{
				gos[i] = hits[i].transform.gameObject;
			}
			return gos;
		}

		public GameObject WhatAllDidIHit(Vector3 pointerPos, string tag)
		{
			GameObject[] gos = WhatAllDidIHit(pointerPos);

			for (int i = 0; i < gos.Length; i++)
			{
				if (gos[i].tag == tag)
				{
					return gos[i].transform.gameObject;
				}
			}
			return null;
		}
		public GameObject WhatAllDidIHit(GameObject[] gos, string tag)
		{
			//if (gos == null) return null;
			for (int i = 0; i < gos.Length; i++)
			{
				if (gos[i].tag == tag) return gos[i];
			}
			return null;
		}

		public Vector3 WhereDidIHit(Vector3 pointerPos, int layer = 0)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(pointerPos);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (layer == 0) return hit.point;
				if (hit.transform.gameObject.layer == layer)
				{
					return hit.point;
				}
			}
			return Vector3.zero;
		}
		public Vector3 WhereDidIHit(Vector3 pointerPos, LayerMask mask)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(pointerPos);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
			{
				return hit.point;
			}
			return Vector3.zero;
		}
		#endregion
		#region 2D Tools
		public static GameObject WhatDidIHit2D(Vector3 pointerPos)
		{
			RaycastHit2D hit2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPos), Vector2.zero);
			if (hit2d.collider != null)
			{
				return hit2d.transform.gameObject;
			}
			return null;
		}

		public static GameObject WhatDidIHit2D(Vector3 pointerPos, LayerMask mask)
		{
			RaycastHit2D hit2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPos), Vector2.zero, Mathf.Infinity, mask);
			if (hit2d.collider != null)
			{
				return hit2d.transform.gameObject;
			}
			return null;
		}

		public GameObject[] WhatAllDidIHit2D(Vector3 pointerPos)
		{
			RaycastHit2D[] hits2d = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(pointerPos), Vector2.zero);
			GameObject[] gos = new GameObject[hits2d.Length];
			for (int i = 0; i < hits2d.Length; i++)
			{
				gos[i] = hits2d[i].transform.gameObject;
			}
			return gos;
		}

		public GameObject WhatAllDidIHit2D(Vector3 pointerPos, string tag)
		{
			GameObject[] gos = WhatAllDidIHit2D(pointerPos);
			for (int i = 0; i < gos.Length; i++)
			{
				if (gos[i].tag == tag)
				{
					return gos[i].transform.gameObject;
				}
			}
			return null;
		}
		public GameObject WhatAllDidIHit2D(GameObject[] gos, string tag)
		{
			if (gos == null) return null;
			for (int i = 0; i < gos.Length; i++)
			{
				if (gos[i].tag == tag) return gos[i];
			}
			return null;
		}

		public Vector3 WhereDidIHit2D(Vector3 pointerPos, int layer = 0)
		{
			RaycastHit2D hit2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPos), Vector2.zero, Mathf.Infinity);
			if (hit2d.collider != null)
			{
				if (layer == 0) return hit2d.point;
				if (hit2d.transform.gameObject.layer == layer)
				{
					return hit2d.point;
				}
			}
			return Vector3.zero;
		}
		public Vector3 WhereDidIHit2D(Vector3 pointerPos, LayerMask mask)
		{
			RaycastHit2D hit2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pointerPos), Vector2.zero, Mathf.Infinity, mask);
			if (hit2d.collider != null)
			{
				return hit2d.point;
			}
			return Vector3.zero;
		}
		#endregion
		#region DELEGATES
		public delegate void OnInputPointerClickDelegate(PointerEventData eventData);
		public static OnInputPointerClickDelegate InputPointerClickEvent;

		public delegate void OnInputPointerUpDelegate(PointerEventData eventData);
		public static OnInputPointerUpDelegate InputPointerUpEvent;

		public delegate void OnInputPointerDownDelegate(PointerEventData eventData);
		public static OnInputPointerDownDelegate InputPointerDownEvent;
		
		public delegate void OnInputDragStartDelegate(PointerEventData eventData);
		public static OnInputDragStartDelegate InputDragStartEvent;

		public delegate void OnInputDragDelegate(PointerEventData eventData);
		public static OnInputDragDelegate InputDragEvent;
		
		public delegate void OnInputDragEndDelegate(PointerEventData eventData);
		public static OnInputDragEndDelegate InputDragEndEvent;

		public delegate void OnInputDropDelegate(PointerEventData eventData);
		public static OnInputDropDelegate InputDropEvent;

		#endregion

		protected override void OnDestroy()
		{
			base.OnDestroy();

			// NULLIFY all static references

			_activeDragObject = null;
			InputPointerClickEvent = null;
			InputPointerUpEvent = null;
			InputPointerDownEvent = null;
			InputDragStartEvent = null;
			InputDragEvent = null;
			InputDragEndEvent = null;
			InputDropEvent = null;
		}

	}

	public interface IInputDownHandler
	{
		void OnInputDown(PointerEventData eventData);
	}	
	public interface IInputUpHandler
	{
		void OnInputUp();
	}
	public interface IInputClickHandler
	{
		void OnInputClick(PointerEventData eventData);
	}	
	public interface IInputHoverHandler
	{
		void OnInputHover(PointerEventData eventData);
		void OnInputHoverExit(PointerEventData eventData);
		void OnInputHoverEnter(PointerEventData eventData);
	}

	public interface IInputDoubleClickHandler { }
	public interface IInputLongPressHandler
	{
		void OnInputLongPress(PointerEventData eventData);
	}
	public interface IInputEnterHandler { }
	public interface IInputExitHandler { }	
  
    public interface IInputDragHandler
    {
        void OnBeginDrag(PointerEventData eventData);
        void OnDrag(PointerEventData eventData);
        void OnEndDrag(PointerEventData eventData);
    }
	public interface IDraggable
	{
		//void StartDraggable();
		//void EndDraggable();

		void OnHoverableObjectEnter(MonoBehaviour obj);
		void OnHoverableObjectExit(MonoBehaviour obj);

	}
}

public enum ClickState
{
	Click,
	LongPress,
	DoubleClick
}

public enum DragDirection
{
	None,
	Up,
	Down,
	Right,
	Left
}
public enum DragOrientation
{
	None,
	Horizontal,
	Vertical
}