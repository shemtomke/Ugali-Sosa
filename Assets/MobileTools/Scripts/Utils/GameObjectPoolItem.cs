using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public class GameObjectPoolItem : MonoBehaviour
	{
		#region Member Variables

		public bool				isInPool;
		public GameObjectPool	pool;
		public CanvasGroup		canvasGroup;

		#endregion
	}
}
