using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oville.Core
{
	public class MonoBase : MonoBehaviour
	{
		protected virtual void Awake() { }
		protected virtual void Start() { }
		protected virtual void Update() { }
		protected virtual void OnDestroy() { }



		protected T findComponentRecursively<T>()
		{
			T c = gameObject.GetComponentInChildren<T>();
			if (c == null) Debug.LogError("could not find component " + c.GetType());

			return gameObject.GetComponentInChildren<T>();
		}
	}
}