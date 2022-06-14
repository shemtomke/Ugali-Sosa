using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBG.MobileTools
{
	public class IAPSettingsWindow : CustomEditorWindow
	{
		#region Inspector Variables

		private SerializedObject	settingsSerializedObject;
		private bool				showNoPluginError;

		#endregion

		#region Properties

		private SerializedObject SettingsSerializedObject
		{
			get
			{
				if (settingsSerializedObject == null)
				{
					settingsSerializedObject = new SerializedObject(IAPSettings.Instance);
				}

				return settingsSerializedObject;
			}
		}

		#endregion

		#region Delegates
		#endregion

		#region Public Methods

		[MenuItem("Tools/Bizzy Bee Games/IAP Settings", priority = 51)]
		public static void Open()
		{
			EditorWindow.GetWindow<IAPSettingsWindow>("IAP Settings");
		}

		#endregion

		#region Draw Methods

		public override void DoGUI()
		{
			SettingsSerializedObject.Update();

			EditorGUILayout.Space();

			BeginBox("IAP Settings");

			#if !UNITY_ANDROID && !UNITY_IOS
			EditorGUILayout.HelpBox("Please set your platform to either Android or iOS in the build settings.", MessageType.Error);
			EditorGUILayout.Space();
			GUI.enabled = false;
			#endif

			DrawEnableDisableButtons();

			EditorGUILayout.PropertyField(SettingsSerializedObject.FindProperty("productInfos"), true);

			EndBox();

			EditorGUILayout.Space();

			GUI.enabled = true;
			GUI.enabled = true;

			SettingsSerializedObject.ApplyModifiedProperties();
		}

		private void DrawEnableDisableButtons()
		{
			if (!IAPSettings.IsIAPEnabled)
			{
				EditorGUILayout.HelpBox("IAP is not enabled, please import the IAP SDK using the Package Manager window then click the button below.", MessageType.Info);

				if (GUILayout.Button("Enable IAP"))
				{
					if (!EditorUtilities.CheckNamespacesExists("UnityEngine.Purchasing"))
					{
						showNoPluginError = true;
					}
					else
					{
						showNoPluginError = false;

						EditorUtilities.SyncScriptingDefineSymbols("BBG_MT_IAP", true);
					}
				}

				if (showNoPluginError)
				{
					EditorGUILayout.HelpBox("The Unity IAP SDK was not been detected. Please import the Unity IAP SDK using the Package Manager window and make sure there are no compiler errors in your project. Check the documentation for more information.", MessageType.Error);
				}
			}
			else
			{
				if (GUILayout.Button("Disable IAP"))
				{
					// Remove BBG_MT_IAP from scripting define symbols
					EditorUtilities.SyncScriptingDefineSymbols("BBG_MT_IAP", false);
				}
			}
		}

		#endregion
	}
}
