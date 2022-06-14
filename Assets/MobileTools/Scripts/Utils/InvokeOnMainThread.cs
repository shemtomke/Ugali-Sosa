using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public class InvokeOnMainThread : MonoBehaviour
	{
		#region Inspector Variables


		#endregion

		#region Member Variables

		private static InvokeOnMainThread	instance	= null;
		private static object				lockObj		= new object();

		private Queue<System.Action<object[]>>	actions;
		private Queue<object[]>					args;

		#endregion

		#region Unity Methods

		private void Update()
		{
			lock (lockObj)
			{
				while (actions.Count > 0)
				{
					actions.Dequeue()(args.Dequeue());
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the InvokeOnMainThread instance, must be called on the main unity thread prior to any Action calls
		/// </summary>
		public static void CreateInstance()
		{
			if (instance == null)
			{
				instance = new GameObject("InvokeOnMainThread").AddComponent<InvokeOnMainThread>();

				DontDestroyOnLoad(instance.gameObject);

				instance.Init();
			}
		}

		public static void Action(System.Action<object[]> action, params object[] arg)
		{
			instance.QueueAction(action, arg);
		}

		#endregion

		#region Private Methods

		private void Init()
		{
			actions	= new Queue<System.Action<object[]>>();
			args	= new Queue<object[]>();
		}

		private void QueueAction(System.Action<object[]> action, object[] arg)
		{
			lock (lockObj)
			{
				actions.Enqueue(action);
				args.Enqueue(arg);
			}
		}

		#endregion
	}
}
