using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oville.Units
{
	public class Unit : Oville.Core.MonoBase
	{

		protected new T findComponentRecursively<T>()
		{
            if (this != null)
            {
                return gameObject.GetComponentInChildren<T>();
            }
            return default(T);
		}

	}
}