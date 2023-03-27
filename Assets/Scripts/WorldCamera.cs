using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oville
{
	public class WorldCamera : MonoBehaviour
	{		
		public AspectRatio CurrentAspectRatio;

		[Header("Calculate vertical size from aspect")]
		public bool DisableAutoAspect;
        public bool DisableAutoOffset = true;

		public float SizeFor16_9 = 5.75f;
		public Vector3 OffsetFor16_9 = Vector3.zero;

        public float SizeFor195_9 = 5.75f;
        public Vector3 OffsetFor195_9 = Vector3.zero;

        public float SizeFor4_3 = 7.68f;
		public Vector3 OffsetFor4_3 = Vector3.zero;

        public float SizeFor43_3 = 7.1f;
        public Vector3 OffsetFor43_3 = Vector3.zero;
        
		[HideInInspector]
		public Vector3 originalPosition = Vector3.zero;


		private Camera _cam;
		public Camera Cam
		{
			get
			{
				if (this._cam == false)
				{
					this._cam = gameObject.GetComponent<Camera>();
				}

				if (this._cam == false)
				{
					Debug.LogError("[WorldCamera] Camera component not found.");
					return null;
				}

				return this._cam;
			}
		}

		//private HUDController _hud;
		//private HUDController HUD
		//{
		//	get
		//	{
		//		if (this._hud == false)
		//		{
		//			this._hud = GameObject.FindObjectOfType<HUDController>();
		//		}

		//		if (this._hud == false)
		//		{
		//			Debug.LogWarning("[WorldCamera] HUDController component not found.");
		//			return null;
		//		}

		//		return this._hud;
		//	}
		//}

		private void Awake()
		{
			// THE MAIN AUDIO LISTENER IS ON AN OBJECT IN THE MAIN SCENE.
			// HOWEVER DEVELOPERS NEED AN AUDIO LISTENER ON THIS CAMERA DURING DEVELOPMENT.
			// LEAVE THE AUDIO LISTENER ON THIS CAMERA, AND WE WILL AUTO-DESTROY IT WHEN
			// THE SCENE RUNS SO IT DOES NOT SHOW ERRORS IN THE CONSOLE.
			if (GameObject.FindObjectsOfType<AudioListener>().Length > 1)
			{
				Destroy(gameObject.GetComponent<AudioListener>());
			}
		}

		private void Start()
		{
			this.originalPosition = gameObject.transform.position;
			//Debug.Log("Aspect:" + this.Cam.aspect);
			calculateAspectRatio();
			//calculateHudAspect();
        }

//#if UNITY_EDITOR
//		private void Update()
//		{
//			calculateAspectRatio();
//			calculateHudAspect();
//		}
//#endif

        //private void LateUpdate()
        //{
        //    if (this.CurrentAspectRatio == AspectRatio.Aspect19_5by9)
        //    {
        //        PostionIPhoneXBorders();
        //    }
        //}

        // float ratio = (float)Screen.height / (float)Screen.width;
        public static AspectRatio CalculateAspectRatio(float ratio)
		{			
			//Debug.Log(ratio);
			if (ratio < 1.4)
			{
				return AspectRatio.Aspect4by3;
			}
            else if (ratio < 1.5)
            {
                return AspectRatio.Aspect43_3by3;
            }
			else if (ratio < 1.8)
			{
				return AspectRatio.Aspect16by9;
			}
			else if (ratio < 2.17)
			{
				return AspectRatio.Aspect19_5by9;
			}
			else
			{
				return AspectRatio.Aspect19_5by9;
			}
		}

		private void calculateAspectRatio()
		{
			if (this.DisableAutoAspect) return;

			AspectRatio ratio = CalculateAspectRatio(this.Cam.aspect);

			switch (ratio)
			{
				case AspectRatio.Aspect4by3:
					this.CurrentAspectRatio = ratio;
					SetForIpad();
					break;

                case AspectRatio.Aspect43_3by3:
                    this.CurrentAspectRatio = ratio;
                    SetForIpadPro();
                    break;

				case AspectRatio.Aspect16by9:
					this.CurrentAspectRatio = ratio;
					SetForiPhone8AndPrevious();
					break;

				case AspectRatio.Aspect19_5by9:
					this.CurrentAspectRatio = ratio;
					SetForiPhoneXAndNewer();
					break;
			}
		}
		
		//private void calculateHudAspect()
		//{
		//	if (this.HUD) this.HUD.SetAspectRatioFitter(CalculateAspectRatio(this.Cam.aspect), this.Cam.aspect);
		//}

		public void SetForIpad()
		{          
			this.Cam.orthographicSize = this.SizeFor4_3;
            if (!DisableAutoOffset) gameObject.transform.position = this.originalPosition + this.OffsetFor4_3;
        }

        public void SetForIpadPro()
        {
            this.Cam.orthographicSize = this.SizeFor43_3;
            if (!DisableAutoOffset) gameObject.transform.position = this.originalPosition + this.OffsetFor43_3;
        }

		public void SetForiPhone8AndPrevious()
		{
            this.Cam.orthographicSize = this.SizeFor16_9;
            if (!DisableAutoOffset) gameObject.transform.position = this.originalPosition + this.OffsetFor16_9;
        }

        public void SetForiPhoneXAndNewer()
        {
            this.Cam.orthographicSize = this.SizeFor195_9;
            if (!DisableAutoOffset) gameObject.transform.position = this.originalPosition + this.OffsetFor195_9;
        }		
	}

	public enum AspectRatio
	{
		None,
		AspectOthers,  // Undefined aspect ratios
		Aspect4by3,
		Aspect16by9, 
		Aspect19_5by9, // iphone X
        Aspect43_3by3, // iPad Pro 11
	}


    public static class CameraExtensions
    {
        public static Bounds OrthographicBounds(this Camera camera)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }
    }


}