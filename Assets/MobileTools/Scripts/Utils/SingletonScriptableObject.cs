using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		#region Member Variables

		private static T instance;

		#endregion

		#region Properties

		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = ScriptableObjectUtilities.CreateFromResources<T>(typeof(T).Name);
				}

				return instance;
			}
		}

		#endregion
	}
}
