using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public static class ScriptableObjectUtilities
	{
		#region Public Methods
		
		public static T CreateFromResources<T>(string name) where T: ScriptableObject
		{
			T obj = Resources.Load<T>(name);

			if (obj == null)
			{
				Debug.Log("Creating ScriptableObject: Resources/" + name + ".asset");   

				obj = ScriptableObject.CreateInstance<T>();

				#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
					{
						System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
					}

					UnityEditor.AssetDatabase.CreateAsset(obj, "Assets/Resources/" + name + ".asset");
					UnityEditor.AssetDatabase.SaveAssets();
				}
				#endif
			}

			return obj;
		}

		public static T CreateFromAssetPath<T>(string assetPath) where T: ScriptableObject
		{
			#if UNITY_EDITOR
			T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);

			if (obj == null)
			{
				Debug.Log("Creating ScriptableObject: " + assetPath);   

				obj = ScriptableObject.CreateInstance<T>();

				#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);
					UnityEditor.AssetDatabase.SaveAssets();
				}
				#endif
			}

			return obj;
			#else
			Debug.LogError("[ScriptableObjectUtilities] CreateFromAssetPath | Cannot create from asset on device, this method only works in the Unity Editor.");
			return ScriptableObject.CreateInstance<T>();
			#endif
		}
		
		#endregion // Public Methods
	}
}
