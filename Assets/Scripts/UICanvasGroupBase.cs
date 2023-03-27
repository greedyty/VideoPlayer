using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oville.Core.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class UICanvasGroupBase : UIBase
	{
		[HideInInspector]
		public bool hideOnAwake;
		[HideInInspector]
		public bool MakeCanvasVisible = true;
		[HideInInspector]
		public CanvasGroup canvasGroup;
		protected bool canvasShowUi = true;
		public virtual bool ShowPanel
		{
			get
			{
				return this.canvasShowUi;
			}
			set
			{
				if (this.canvasGroup == false)
				{
					this.canvasGroup = gameObject.GetComponent<CanvasGroup>();
					if (this.canvasGroup == false)
					{
						Debug.LogError(gameObject.name + " is missing a required CanvasGroup component.");
					}
				}

				if (this.canvasGroup)
				{
					if (value == true)
					{
						this.canvasGroup.alpha = 1;
						this.canvasGroup.interactable = true;
						this.canvasGroup.blocksRaycasts = true;
						this.canvasShowUi = true;
						this.MakeCanvasVisible = true;

						OnVisible();
					}
					else
					{
						this.canvasGroup.alpha = 0;
						this.canvasGroup.interactable = false;
						this.canvasGroup.blocksRaycasts = false;
						this.canvasShowUi = false;
						this.MakeCanvasVisible = false;

						OnVisible();
					}
				}
			}
		}
		public bool isShowing
		{
			get { return this.canvasShowUi; }
		}
		
		// Use this for initialization
		protected override void Awake()
		{
			base.Awake();
			this.canvasGroup = gameObject.GetComponent<CanvasGroup>();
			if (this.canvasGroup == false)
			{
				Debug.LogError(gameObject.name + " is missing a required CanvasGroup component.");
			}

			this.canvasShowUi = (this.canvasGroup.alpha == 1);

			if (this.hideOnAwake) ShowPanel = false;
		}

		protected virtual void OnVisible() { }
		protected virtual void OnInvisible() { }
		protected virtual void OnEnable() { }

#region UI Callbacks
		public virtual void MakeVisible()
		{
			this.ShowPanel = true;
		}
		public virtual void MakeInvisible()
		{
			this.ShowPanel = false;
			if (this.onCloseEvent != null) this.onCloseEvent(this);
		}
        public virtual void VisibilitySwitch()
        {
            this.ShowPanel = !this.ShowPanel;
        }
		#endregion
	}
}