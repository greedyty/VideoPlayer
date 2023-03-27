using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oville
{
	public class DragCamera : MonoBehaviour
	{
		private Camera _cam;
		private Camera Cam
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

		private WorldCamera worldCamera;
		public WorldCamera WorldCamera
		{
			get
			{
				if (this.worldCamera == false)
				{
					this.worldCamera = gameObject.GetComponentInParent<WorldCamera>();
				}
				if (this.worldCamera == false)
				{
					Debug.LogError("[DragCamera] Cannot find WorldCamera object.");
				}

				return this.worldCamera;
			}
		}


		private void Start()
		{
			calculateAspectRatio();
		}

#if UNITY_EDITOR
		private void Update()
		{
			calculateAspectRatio();
		}
#endif


		private void calculateAspectRatio()
		{
			if (this.WorldCamera == null) return;
			AspectRatio ratio = WorldCamera.CalculateAspectRatio(this.Cam.aspect);

			switch (ratio)
			{
				case AspectRatio.Aspect4by3:
					this.Cam.orthographicSize = this.WorldCamera.SizeFor4_3;
					break;

                case AspectRatio.Aspect43_3by3:
                    this.Cam.orthographicSize = this.worldCamera.SizeFor43_3;
                    break;

				case AspectRatio.Aspect16by9:				
					this.Cam.orthographicSize = this.WorldCamera.SizeFor16_9;
					break;
                case AspectRatio.Aspect19_5by9:
                    this.Cam.orthographicSize = this.WorldCamera.SizeFor195_9;
                    break;
            }
		}

	}
}