using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public abstract class DebugConsoleItem : MonoBehaviour
	{
		#region Abstract Methods

		public abstract string GetTabName();
		public abstract void Initialize();

		#endregion
	}
}
