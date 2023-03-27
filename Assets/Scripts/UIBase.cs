using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oville.Core.UI
{
	// USED FOR UI ELEMENTS
	public abstract class UIBase : MonoBase
	{
		public virtual void SetActive(bool b)
		{
			gameObject.SetActive(b);
		}


#region UI Callbacks
		// Use SetActive(FALSE) on this object to hide it. Override this to change functionality.
		public virtual void OnCloseButton()
		{
			gameObject.SetActive(false);
			if (this.onCloseEvent != null) this.onCloseEvent(this);
		}
		public virtual void OnCloseButtonWithDestroy(float timeToDie = 0)
		{
			if (this.onCloseEvent != null) this.onCloseEvent(this);
			Destroy(gameObject, timeToDie);
		}
		#endregion

		#region UI Events

		public Action<UIBase> onCloseEvent;
		public virtual void OnCloseEvent(Action<UIBase> callback)
		{
			this.onCloseEvent = callback;
		}


        /// <summary>
        /// Sometimes you want to force a player to use the confirmation buttons.
        /// Call this before displaying this window to disable 
        /// modal-style closing of the window. (tapping outside the window).
        /// </summary>
        public virtual void DisableOnCloseButton()
        {
            this.onCloseEvent = reenableWindow;
        }
        protected void reenableWindow(UIBase window)
        {
            window.SetActive(true);
        }

		#endregion
	}
}