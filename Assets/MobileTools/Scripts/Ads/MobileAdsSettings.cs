using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public class MobileAdsSettings : SingletonScriptableObject<MobileAdsSettings>
	{
		#region Classes

		[System.Serializable]
		public class PlatformSettings
		{
			public string bannerNetworkId		= "";
			public string interstitialNetworkId	= "";
			public string rewardNetworkId		= "";
		}

		/// <summary>
		/// Settings for AdMob
		/// </summary>
		[System.Serializable]
		public class AdMobConfig
		{
			[System.Serializable]
			public class Config
			{
				public enum BannerSize
				{
					Banner,
					MediumRectangle,
					IABBanner,
					Leaderboard,
					SmartBanner
				}

				public string		bannerUnitId;
				public string		interstitialUnitId;
				public string		rewardUnitId;
				public bool			showBannerOnStart;
				public List<string> deviceTestIds;

				public BannerSize		bannerSize;
				public BannerPosition	bannerPosition;
			}

			public Config	androidConfig	= new Config();
			public Config	iosConfig		= new Config();

			public string				BannerPlacementId		{ get { return ReturnPlatformValue(androidConfig.bannerUnitId, iosConfig.bannerUnitId).Trim(); } }
			public string				InterstitialPlacementId	{ get { return ReturnPlatformValue(androidConfig.interstitialUnitId, iosConfig.interstitialUnitId).Trim(); } }
			public string				RewardPlacementId		{ get { return ReturnPlatformValue(androidConfig.rewardUnitId, iosConfig.rewardUnitId).Trim(); } }
			public bool					ShowBannerOnStart		{ get { return ReturnPlatformValue(androidConfig.showBannerOnStart, iosConfig.showBannerOnStart); } }
			public Config.BannerSize	BannerSize				{ get { return ReturnPlatformValue(androidConfig.bannerSize, iosConfig.bannerSize); } }
			public BannerPosition		BannerPosition			{ get { return ReturnPlatformValue(androidConfig.bannerPosition, iosConfig.bannerPosition); } }
			public List<string>			DeviceTestIds			{ get { return ReturnPlatformValue(androidConfig.deviceTestIds, iosConfig.deviceTestIds); } }
		}

		/// <summary>
		/// Settings for Unity Ads
		/// </summary>
		[System.Serializable]
		public class UnityAdsConfig
		{
			[System.Serializable]
			public class Config
			{
				public string	gameId;
				public string	bannerPlacement;
				public string	interstitialPlacement;
				public string	rewardPlacement;
				public bool		showBannerOnAppStart;

				public BannerPosition bannerPosition;
			}

			public Config 	androidConfig	= new Config();
			public Config 	iosConfig		= new Config();
			public float	bannerHeight	= 50;

			public string			GameId					{ get { return ReturnPlatformValue(androidConfig.gameId, iosConfig.gameId); } }
			public string			BannerPlacement			{ get { return ReturnPlatformValue(androidConfig.bannerPlacement, iosConfig.bannerPlacement); } }
			public string			InterstitialPlacement	{ get { return ReturnPlatformValue(androidConfig.interstitialPlacement, iosConfig.interstitialPlacement); } }
			public string			RewardPlacement			{ get { return ReturnPlatformValue(androidConfig.rewardPlacement, iosConfig.rewardPlacement); } }
			public bool				ShowBannerOnAppStart	{ get { return ReturnPlatformValue(androidConfig.showBannerOnAppStart, iosConfig.showBannerOnAppStart); } }
			public BannerPosition	BannerPosition			{ get { return ReturnPlatformValue(androidConfig.bannerPosition, iosConfig.bannerPosition); } }
		}

		#endregion

		#region Enums

		public enum BannerPosition
		{
			Top,
			TopLeft,
			TopRight,
			Bottom,
			BottomLeft,
			BottomRight
		}

		public enum ConsentSetting
		{
			NotRequired,
			RequireOnlyInEEA,
			RequireAll
		}

		#endregion

		#region Member Variables

		public const string Name = "MobileAdSettings";

		public const string DisabledNetworkId	= "None";
		public const string AdMobNetworkId		= "AdMob";
		public const string UnityAdsNetworkId	= "UnityAds";

		/// <summary>
		/// List of networks ids
		/// </summary>
		public static readonly string[] NetworkIds = 
		{
			DisabledNetworkId,
			AdMobNetworkId,
			UnityAdsNetworkId
		};

		public bool				adsEnabled			= false;
		public ConsentSetting	consentSetting		= ConsentSetting.NotRequired;
		public bool				dontRemoveRewardAds	= false;
		public bool				retryLoadIfFailed	= true;
		public float			retryWaitTime		= 3;

		public PlatformSettings androidSettings;
		public PlatformSettings iosSettings;

		public AdMobConfig		adMobConfig		= new AdMobConfig();
		public UnityAdsConfig	unityAdsConfig	= new UnityAdsConfig();

		// Editor window only values
		public bool			isDetectedSdksHeaderExpanded;
		public List<string>	editorExpandedBoxes = new List<string>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current platforms ad network settings
		/// </summary>
		public PlatformSettings CurrentPlatformSettings
		{
			get
			{
				#if UNITY_ANDROID
				return androidSettings;
				#elif UNITY_IOS
				return iosSettings;
				#else
				return new PlatformSettings();
				#endif
			}
		}

		public bool AreBannerAdsEnabled
		{
			get { return CurrentPlatformSettings.bannerNetworkId != DisabledNetworkId; }
		}

		public bool AreInterstitialAdsEnabled
		{
			get { return CurrentPlatformSettings.interstitialNetworkId != DisabledNetworkId; }
		}

		public bool AreRewardAdsEnabled
		{
			get { return CurrentPlatformSettings.rewardNetworkId != DisabledNetworkId; }
		}

		/// <summary>
		/// Network id that is selected for banner ads
		/// </summary>
		public string SelectedBannerAdNetworkId
		{
			get { return CurrentPlatformSettings.bannerNetworkId; }
		}

		/// <summary>
		/// Network id that is selected for interstitial ads
		/// </summary>
		public string SelectedInterstitialAdNetworkId
		{
			get { return CurrentPlatformSettings.interstitialNetworkId; }
		}

		/// <summary>
		/// Network id that is selected for reward ads
		/// </summary>
		public string SelectedRewardAdNetworkId
		{
			get { return CurrentPlatformSettings.rewardNetworkId; }
		}

		#endregion

		#region Private Methods

		private static T ReturnPlatformValue<T>(T androidValue, T iosValue)
		{
			#if UNITY_ANDROID
			return androidValue;
			#elif UNITY_IOS
			return iosValue;
			#else
			return default(T);
			#endif
		}

		#endregion
	}
}
