using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBG.MobileTools
{
	public class MobileAdsSettingsWindow : CustomEditorWindow
	{
		#region Inspector Variables

		private SerializedObject settingsSerializedObject;

		#endregion

		#region Member Variables

		#endregion

		#region Properties

		private SerializedObject SettingsSerializedObject
		{
			get
			{
				if (settingsSerializedObject == null)
				{
					settingsSerializedObject = new SerializedObject(MobileAdsSettings.Instance);
				}

				return settingsSerializedObject;
			}
		}

		#endregion

		#region Delegates

		private delegate void DrawNetworkConfig(SerializedProperty networkConfig);
		private delegate void DrawConfig(SerializedProperty platformConfig, params bool[] typeEnabled);

		#endregion

		#region Public Methods

		[MenuItem("Tools/Bizzy Bee Games/Mobile Ads Settings", priority = 51)]
		public static void Open()
		{
			EditorWindow.GetWindow<MobileAdsSettingsWindow>("Mobile Ads Settings");
		}

		#endregion

		#region Draw Methods

		public override void DoGUI()
		{
			SettingsSerializedObject.Update();

			DrawSettings();

			GUI.enabled = true;

			GUILayout.Space(5);

			DrawSDKSettings();

			GUILayout.Space(5);

			GUI.enabled = SettingsSerializedObject.FindProperty("adsEnabled").boolValue;

			DrawAdNetworkBoxes();

			GUI.enabled = true;

			SettingsSerializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Draws the general window settings
		/// </summary>
		private void DrawSettings()
		{
			BeginBox("Mobile Ads Settings");

			#if !UNITY_ANDROID && !UNITY_IOS
			EditorGUILayout.HelpBox("Please set your platform to either Android or iOS in the build settings.", MessageType.Error);
			EditorGUILayout.Space();
			#endif

			EditorGUILayout.PropertyField(SettingsSerializedObject.FindProperty("adsEnabled"));

			bool enableGUI = SettingsSerializedObject.FindProperty("adsEnabled").boolValue;

			GUI.enabled = enableGUI;

			EditorGUILayout.Space();

			DrawPlatformSettings("Android", SettingsSerializedObject.FindProperty("androidSettings"));

			EditorGUILayout.Space();

			DrawPlatformSettings("iOS", SettingsSerializedObject.FindProperty("iosSettings"));

			EditorGUILayout.Space();

			DrawGeneralSettings();

			GUILayout.Space(2);

			EndBox();
		}

		/// <summary>
		/// Draws the settings for the given PlatformSettings
		/// </summary>
		private void DrawPlatformSettings(string platformName, SerializedProperty platformSettings)
		{
			DrawBoldLabel(platformName);

			SerializedProperty bannerNetworkIdProp			= platformSettings.FindPropertyRelative("bannerNetworkId");
			SerializedProperty interstitialNetworkIdProp	= platformSettings.FindPropertyRelative("interstitialNetworkId");
			SerializedProperty rewardNetworkIdProp			= platformSettings.FindPropertyRelative("rewardNetworkId");

			bannerNetworkIdProp.stringValue			= DrawNetworkIdDropdown("Banner Ad Network", bannerNetworkIdProp.stringValue, MobileAdsSettings.NetworkIds);
			interstitialNetworkIdProp.stringValue	= DrawNetworkIdDropdown("Interstitial Ad Network", interstitialNetworkIdProp.stringValue, MobileAdsSettings.NetworkIds);
			rewardNetworkIdProp.stringValue			= DrawNetworkIdDropdown("Reward Ad Network", rewardNetworkIdProp.stringValue, MobileAdsSettings.NetworkIds);
		}

		/// <summary>
		/// Draws a dropdown to select a network id 
		/// </summary>
		private string DrawNetworkIdDropdown(string text, string selectedNetworkId, string[] networkIds)
		{
			int index = 0;

			for (int i = 0; i < networkIds.Length; i++)
			{
				if (selectedNetworkId == networkIds[i])
				{
					index = i;

					break;
				}
			}

			index = EditorGUILayout.Popup(text, index, networkIds);

			return networkIds[index];
		}

		/// <summary>
		/// Draws the consent settings
		/// </summary>
		private void DrawGeneralSettings()
		{
			DrawBoldLabel("General");

			SerializedProperty consentSettingsProp = SettingsSerializedObject.FindProperty("consentSetting");

			EditorGUILayout.PropertyField(consentSettingsProp);

			switch ((MobileAdsSettings.ConsentSetting)consentSettingsProp.enumValueIndex)
			{
				case MobileAdsSettings.ConsentSetting.NotRequired:
					EditorGUILayout.HelpBox("Ads will load and display regardless of consent. Ad requests will not specify if consent for personalized ads has been given.", MessageType.None);
					break;
				case MobileAdsSettings.ConsentSetting.RequireOnlyInEEA:
					EditorGUILayout.HelpBox("Ads will not be requested until consent has been set using the SetConsentStatus method in MobileAdsManager. This will only apply to users that are in the European Economic Area. If the users location cannot be determined then it will default to requiring user consent in order for ads to be requested.", MessageType.None);
					break;
				case MobileAdsSettings.ConsentSetting.RequireAll:
					EditorGUILayout.HelpBox("Ads will not be requested until consent has been set using the SetConsentStatus method in MobileAdsManager. This will apply to all users of the app.", MessageType.None);
					break;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SettingsSerializedObject.FindProperty("dontRemoveRewardAds"));
			EditorGUILayout.HelpBox("If selected, removing ads (such as for an IAP purchase) will not remove the ability to load and display reward ads, only interstitial and banner ares will be removed.", MessageType.None);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SettingsSerializedObject.FindProperty("retryLoadIfFailed"));
			EditorGUILayout.HelpBox("If selected, the system will automatically attempt to pre-load another ad if the previous load failed for any reason.", MessageType.None);
			
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SettingsSerializedObject.FindProperty("retryWaitTime"));
			EditorGUILayout.HelpBox("Amount of time to wait before attempting to pre-load another at after a fail.", MessageType.None);
		}

		/// <summary>
		/// Draws the SDK settings
		/// </summary>
		private void DrawSDKSettings()
		{
			BeginBox("Detected SDKs");

			if (GUILayout.Button("Detect SDKs"))
			{
				MobileAdsUtils.DetectSDKs();

				AssetDatabase.Refresh();
			}

			SerializedProperty isDetectedSdksHeaderExpandedProp = SettingsSerializedObject.FindProperty("isDetectedSdksHeaderExpanded");

			isDetectedSdksHeaderExpandedProp.boolValue = DrawBoldFoldout(isDetectedSdksHeaderExpandedProp.boolValue, "SDKs");

			if (isDetectedSdksHeaderExpandedProp.boolValue)
			{
				EditorGUI.indentLevel++;

				DrawDetectedSDKs();

				EditorGUI.indentLevel--;

				GUILayout.Space(3);
			}

			EndBox();
		}

		/// <summary>
		/// Draws the list of detected SDKs
		/// </summary>
		private void DrawDetectedSDKs()
		{
			bool anySDKsDetected = false;

			#if BBG_ADMOB
			DrawDetectedSDK(MobileAdsSettings.AdMobNetworkId, MobileAdsUtils.AdMobNetworkInfo);
			anySDKsDetected = true;
			#endif

			#if BBG_UNITYADS
			DrawDetectedSDK(MobileAdsSettings.UnityAdsNetworkId, MobileAdsUtils.UnityAdsNetworkInfo);
			anySDKsDetected = true;
			#endif

			if (!anySDKsDetected)
			{
				EditorGUILayout.LabelField("There are no detected SDKs");
			}
		}

		/// <summary>
		/// Draws the label and remove button for the detected SDK
		/// </summary>
		private void DrawDetectedSDK(string adNetwordId, MobileAdsUtils.NetworkInfo networkInfo)
		{
			EditorGUILayout.LabelField(adNetwordId);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(18);

			if (GUILayout.Button(string.Format("Remove [{0}] Scripting Define Symbol", adNetwordId)))
			{
				MobileAdsUtils.ClearNetworkInfo(networkInfo);
				 
				AssetDatabase.Refresh();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawAdNetworkBoxes()
		{
			List<string> networkIds = new List<string>();

			string androidBannerNetworkId		= SettingsSerializedObject.FindProperty("androidSettings").FindPropertyRelative("bannerNetworkId").stringValue;
			string androidInterstitialNetworkId = SettingsSerializedObject.FindProperty("androidSettings").FindPropertyRelative("interstitialNetworkId").stringValue;
			string androidRewardNetworkId		= SettingsSerializedObject.FindProperty("androidSettings").FindPropertyRelative("rewardNetworkId").stringValue;
			string iosBannerNetworkId			= SettingsSerializedObject.FindProperty("iosSettings").FindPropertyRelative("bannerNetworkId").stringValue;
			string iosInterstitialNetworkId		= SettingsSerializedObject.FindProperty("iosSettings").FindPropertyRelative("interstitialNetworkId").stringValue;
			string iosRewardNetworkId			= SettingsSerializedObject.FindProperty("iosSettings").FindPropertyRelative("rewardNetworkId").stringValue;

			// Get all the ad network ids that were selected
			AddNetworkId(networkIds, androidBannerNetworkId);
			AddNetworkId(networkIds, androidInterstitialNetworkId);
			AddNetworkId(networkIds, androidRewardNetworkId);
			AddNetworkId(networkIds, iosBannerNetworkId);
			AddNetworkId(networkIds, iosInterstitialNetworkId);
			AddNetworkId(networkIds, iosRewardNetworkId);

			networkIds.Sort();

			// Draw each of the boxes for the selected networks
			for (int i = 0; i < networkIds.Count; i++)
			{
				DrawAdNetworkSettings(networkIds[i]);
			}
		}

		/// <summary>
		/// Helper to DrawAdNetworkBoxes, adds all string values from listProperty to networkIds list
		/// </summary>
		private void AddNetworkIds(List<string> networkIds, SerializedProperty listProperty)
		{
			for (int i = 0; i < listProperty.arraySize; i++)
			{
				AddNetworkId(networkIds, listProperty.GetArrayElementAtIndex(i).stringValue);
			}
		}

		/// <summary>
		/// Helper to DrawAdNetworkBoxes, adds the networkId to the list if it has not already been added
		/// </summary>
		private void AddNetworkId(List<string> networkIds, string networkId)
		{
			if (networkId != MobileAdsSettings.DisabledNetworkId && !networkIds.Contains(networkId))
			{
				networkIds.Add(networkId);
			}
		}

		/// <summary>
		/// Handles calling the proper draw method for the given network id
		/// </summary>
		private void DrawAdNetworkSettings(string networkId)
		{
			bool isExpanded = BeginFoldoutBox(networkId + " Settings");

			if (isExpanded)
			{
				if (!SettingsSerializedObject.FindProperty("adsEnabled").boolValue)
				{
					EditorGUILayout.LabelField("Ads are not enabled. To edit these settings please enable ads in Mobile Ads Settings above.");
				}
				else if (!MobileAdsUtils.CheckAdNetwork(networkId))
				{
					EditorGUILayout.HelpBox(string.Format("{0} SDK not detected. Please import the {1} SDK and click the \"Detect SDKs\" button in the panel above. Check the documentation for information on how to import the SDK.", networkId, networkId), MessageType.Warning);
				}
				else
				{
					bool androidBanner			= IsNetworkSelected(networkId, "androidSettings", "androidConfig", "bannerNetworkId", "selectedBannerNetworks");
					bool androidInterstitial	= IsNetworkSelected(networkId, "androidSettings", "androidConfig", "interstitialNetworkId", "selectedInterstitialNetworks");
					bool androidReward			= IsNetworkSelected(networkId, "androidSettings", "androidConfig", "rewardNetworkId", "selectedRewardNetworks");

					bool iosBanner				= IsNetworkSelected(networkId, "iosSettings", "iosConfig", "bannerNetworkId", "selectedBannerNetworks");
					bool iosInterstitial		= IsNetworkSelected(networkId, "iosSettings", "iosConfig", "interstitialNetworkId", "selectedInterstitialNetworks");
					bool iosReward				= IsNetworkSelected(networkId, "iosSettings", "iosConfig", "rewardNetworkId", "selectedRewardNetworks");

					switch (networkId)
					{
						case MobileAdsSettings.AdMobNetworkId:
							DrawAdNetworkSettings("adMobConfig", androidBanner, androidInterstitial, androidReward, iosBanner, iosInterstitial, iosReward, DrawAdMobSettings, DrawAdMobNetworkSettings);
							break;
						case MobileAdsSettings.UnityAdsNetworkId:
							DrawAdNetworkSettings("unityAdsConfig", androidBanner, androidInterstitial, androidReward, iosBanner, iosInterstitial, iosReward, DrawUnityConfig, DrawUnityAdsNetworkSettings);
							break;
					}
				}
			}

			EndBox();
		}

		private bool IsNetworkSelected(string networkId, string platformSettings, string platformConfig, string str1, string str2)
		{
			string selectedNetworkId = SettingsSerializedObject.FindProperty(platformSettings).FindPropertyRelative(str1).stringValue;

			if (networkId == selectedNetworkId)
			{
				return true;
			}

			return false;
		}

		private bool CheckListForNetworkId(string networkId, SerializedProperty listProperty)
		{
			for (int i = 0; i < listProperty.arraySize; i++)
			{
				if (networkId == listProperty.GetArrayElementAtIndex(i).stringValue)
				{
					return true;
				}
			}

			return false;
		}

		private void DrawAdNetworkSettings(
			string		configPropertyName,
			bool		androidBanner,
			bool		androidInterstitial,
			bool		androidReward,
			bool		iosBanner,
			bool		iosInterstitial,
			bool		iosReward,
			DrawConfig	drawConfig,
			DrawNetworkConfig drawNetworkConfig = null)
		{
			SerializedProperty config			= SettingsSerializedObject.FindProperty(configPropertyName);
			SerializedProperty androidConfig	= config.FindPropertyRelative("androidConfig");
			SerializedProperty iosConfig		= config.FindPropertyRelative("iosConfig");

			GUILayout.Space(2);

			if (drawNetworkConfig != null)
			{
				drawNetworkConfig(config);

				GUILayout.Space(2);
				DrawLine();
				GUILayout.Space(2);
			}
			
			if (androidBanner || androidInterstitial || androidReward)
			{
				DrawBoldLabel("Android Settings");
				drawConfig(androidConfig, androidBanner, androidInterstitial, androidReward);
			}

			if (iosBanner || iosInterstitial || iosReward)
			{
				if (androidBanner || androidInterstitial || androidReward)
				{
					GUILayout.Space(2);
					DrawLine();
					GUILayout.Space(2);
				}

				DrawBoldLabel("iOS Settings");
				drawConfig(iosConfig, iosBanner, iosInterstitial, iosReward);
			}

			GUILayout.Space(2);
		}

		private void DrawUnityAdsNetworkSettings(SerializedProperty config)
		{
			DrawPropertyField(config, "bannerHeight");
		}

		private void DrawAdMobNetworkSettings(SerializedProperty config)
		{
			EditorGUILayout.HelpBox("Don't forget to add you AdMob App Id to the GoogleMobileAdsSettings by selecting focusing the Inspector window and selecting the menu item: Assets > Google Mobile Ads > Settings...", MessageType.Info);
		}

		private void DrawAdMobSettings(SerializedProperty config, params bool[] enabledTypes)
		{
			if (IsBannerEnabled(enabledTypes))
			{
				DrawPropertyField(config, "bannerUnitId");
				DrawPropertyField(config, "showBannerOnStart");
				DrawPropertyField(config, "bannerSize");
				DrawPropertyField(config, "bannerPosition");
			}

			if (IsInterstitalEnabled(enabledTypes))
			{
				DrawPropertyField(config, "interstitialUnitId");
			}

			if (IsRewardEnabled(enabledTypes))
			{
				DrawPropertyField(config, "rewardUnitId");
			}

			DrawPropertyField(config, "deviceTestIds", true);
		}

		private void DrawUnityConfig(SerializedProperty config, params bool[] enabledTypes)
		{
			DrawPropertyField(config, "gameId");

			if (IsBannerEnabled(enabledTypes))
			{
				DrawPropertyField(config, "bannerPlacement");
				DrawPropertyField(config, "showBannerOnAppStart");
				DrawPropertyField(config, "bannerPosition");
			}

			if (IsInterstitalEnabled(enabledTypes))
			{
				DrawPropertyField(config, "interstitialPlacement");
			}

			if (IsRewardEnabled(enabledTypes))
			{
				DrawPropertyField(config, "rewardPlacement");
			}
		}

		private bool IsBannerEnabled(bool[] enabledTypes)
		{
			return enabledTypes[0];
		}

		private bool IsInterstitalEnabled(bool[] enabledTypes)
		{
			return enabledTypes[1];
		}

		private bool IsRewardEnabled(bool[] enabledTypes)
		{
			return enabledTypes[2];
		}

		#endregion
	}
}
