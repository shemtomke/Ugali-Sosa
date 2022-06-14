using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if BBG_UNITYADS

using UnityEngine.Advertisements;

#endif

namespace BBG.MobileTools
{
	public class UnityAdsNetworkHandler : AdNetworkHandler
        #if BBG_UNITYADS
        , IUnityAdsListener
        #endif
	{
		#region Member Variables

		#if BBG_UNITYADS

		private bool	showBanner;
		private float	extraWaitTime = 0;

		#endif

		#endregion

		#region Properties

		protected override string LogTag { get { return "UnityAds"; } }

		private string GameId					{ get { return MobileAdsSettings.Instance.unityAdsConfig.GameId; } }
		private string BannerPlacement			{ get { return MobileAdsSettings.Instance.unityAdsConfig.BannerPlacement; } }
		private string InterstitialPlacement	{ get { return MobileAdsSettings.Instance.unityAdsConfig.InterstitialPlacement; } }
		private string RewardPlacement			{ get { return MobileAdsSettings.Instance.unityAdsConfig.RewardPlacement; } }

		#endregion

		#region Protected Methods

		protected override void DoInitialize()
		{
			GameDebugManager.Log(LogTag, "Initializing Unity Ads");

			#if BBG_UNITYADS

			GameDebugManager.Log(LogTag, "Game Id: " + GameId);

			Advertisement.AddListener(this);
			Advertisement.Initialize(GameId); 

			isInitialized = true;

			SetConsentStatus();

			if (bannerAdsEnabled)
			{
				GameDebugManager.Log(LogTag, "Banner ads are enabled, placement: " + BannerPlacement);

				showBanner		= MobileAdsSettings.Instance.unityAdsConfig.ShowBannerOnAppStart;
				extraWaitTime	= 3f;

				PreLoadBannerAd();
			}

			if (interstitialAdsEnabled)
			{
				GameDebugManager.Log(LogTag, "Interstitial ads are enabled, placement: " + InterstitialPlacement);

				PreLoadInterstitialAd();
			}

			if (rewardAdsEnabled)
			{
				GameDebugManager.Log(LogTag, "Reward ads are enabled, placement: " + RewardPlacement);

				PreLoadRewardAd();
			}

			#else

			GameDebugManager.LogError(LogTag, "Unity Ads has not been setup in Mobile Ads Settings");

			#endif
		}

		#if BBG_UNITYADS

		public void OnUnityAdsReady(string placementId)
		{
			if (placementId == InterstitialPlacement && InterstitialAdState != AdState.Loaded)
			{
				NotifyInterstitialAdLoaded();
			}
			else if (placementId == RewardPlacement && RewardAdState != AdState.Loaded)
			{
				NotifyRewardAdLoaded();
			}
		}

		public void OnUnityAdsDidStart(string placementId)
		{
			if (placementId == InterstitialPlacement)
			{
				NotifyInterstitialAdShown();
			}
			else if (placementId == RewardPlacement)
			{
				NotifyRewardAdShown();
			}
		} 

		public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
		{
			if (placementId == InterstitialPlacement)
			{
				NotifyInterstitialAdClosed();
				PreLoadInterstitialAd();
			}
			else if (placementId == RewardPlacement)
			{
				if (showResult == ShowResult.Finished)
				{
					NotifyRewardAdGranted("", 0);
				}

				NotifyRewardAdClosed();

				PreLoadRewardAd();
			}
		}

		public void OnUnityAdsDidError(string message)
		{
			GameDebugManager.LogError(LogTag, "OnUnityAdsDidError: " + message);
		}

		#endif

		protected override void DoLoadBannerAd()
		{
			#if BBG_UNITYADS

			BannerAdState = AdState.Loading;
			NotifyBannerAdLoading();

			MobileAdsManager.Instance.BeginCoroutine(WaitForBannerAdReady(extraWaitTime));

			extraWaitTime = 0;

			#endif
		}

		protected override void DoShowBannerAd()
		{
			#if BBG_UNITYADS

			showBanner = true;

			switch (BannerAdState)
			{
				case AdState.Loaded:
					Advertisement.Banner.SetPosition(GetUnityAdsBannerPosition());
					Advertisement.Banner.Show(BannerPlacement);
					BannerAdState = AdState.Shown;
					NotifyBannerAdShown();
					break;
				case AdState.None:
					LoadBannerAd();
					break;
				default:
					GameDebugManager.Log(LogTag, "DoShowBannerAd: Nothing will happen because BannerAdState is: " + BannerAdState);
					break;
			}

			#endif
		}

		protected override void DoHideBannerAd()
		{
			#if BBG_UNITYADS

			showBanner = false;

			if (BannerAdState == AdState.Shown)
			{
				Advertisement.Banner.Hide();
				
				BannerAdState = AdState.Loaded;
				NotifyBannerAdHidden();
			}
			else
			{
				GameDebugManager.Log(LogTag, "DoHideBannerAd: Nothing will happen because BannerAdState is: " + BannerAdState);
			}

			#endif
		}

		protected override void DoLoadInterstitialAd()
		{
			#if BBG_UNITYADS

			NotifyInterstitialAdLoading();

			Advertisement.Load(InterstitialPlacement);

			#endif
		}

		protected override void DoShowInterstitialAd()
		{
			#if BBG_UNITYADS

			NotifyInterstitialAdShowing();

			Advertisement.Show(InterstitialPlacement);

			#endif
		}

		protected override void DoLoadRewardAd()
		{
			#if BBG_UNITYADS

			NotifyRewardAdLoading();

			Advertisement.Load(RewardPlacement);

			#endif
		}

		protected override void DoShowRewardAd()
		{
			#if BBG_UNITYADS

			NotifyRewardAdShowing();

			Advertisement.Show(RewardPlacement);

			#endif
		}

		protected override void DoAdsRemoved(bool dontRemoveRewardAds)
		{
			#if BBG_UNITYADS

			if (BannerAdState == AdState.Shown)
			{
				Advertisement.Banner.Hide(true);
			}

			BannerAdState		= AdState.None;
			InterstitialAdState	= AdState.None;

			if (!dontRemoveRewardAds)
			{
				RewardAdState = AdState.None;
			}

			#endif
		}

		protected override void ConsentStatusUpdated()
		{
			#if BBG_UNITYADS

			SetConsentStatus();

			#endif
		}

		protected override float DoGetBannerHeightInPixels()
		{
			return Mathf.RoundToInt(MobileAdsSettings.Instance.unityAdsConfig.bannerHeight * UnityEngine.Screen.dpi / 160);
		}

		protected override MobileAdsSettings.BannerPosition DoGetBannerPosition()
		{
			return MobileAdsSettings.Instance.unityAdsConfig.BannerPosition;
		}

		#endregion

		#region Private Methods

		#if BBG_UNITYADS

		private void SetConsentStatus()
		{
			MetaData advMetaData = new MetaData("gdpr");

			switch (MobileAdsManager.Instance.ConsentStatus)
			{
				case MobileAdsManager.ConsentType.NonPersonalized:
					advMetaData.Set("consent", "false");
					Advertisement.SetMetaData(advMetaData);
					break;
				case MobileAdsManager.ConsentType.Personalized:
					advMetaData.Set("consent", "true");
					Advertisement.SetMetaData(advMetaData);
					break;
			}

		}

		private BannerPosition GetUnityAdsBannerPosition()
		{
			// Set the ads position
			switch (MobileAdsManager.Instance.BannerAdHandler.GetBannerPosition())
			{
				case MobileAdsSettings.BannerPosition.Top:
					return BannerPosition.TOP_CENTER;
				case MobileAdsSettings.BannerPosition.TopLeft:
					return BannerPosition.TOP_LEFT;
				case MobileAdsSettings.BannerPosition.TopRight:
					return BannerPosition.TOP_RIGHT;
				case MobileAdsSettings.BannerPosition.Bottom:
					return BannerPosition.BOTTOM_CENTER;
				case MobileAdsSettings.BannerPosition.BottomLeft:
					return BannerPosition.BOTTOM_LEFT;
				case MobileAdsSettings.BannerPosition.BottomRight:
					return BannerPosition.BOTTOM_RIGHT;
			}

			return BannerPosition.BOTTOM_CENTER;
		}

		private IEnumerator WaitForBannerAdReady(float waitTime)
		{
			if (!Advertisement.IsReady(BannerPlacement))
			{
				yield return null;
			}

			// We need to wait a couple more seconds right after the app starts before showing the banner or it wont actually show
			if (waitTime > 0)
			{
				yield return new WaitForSeconds(waitTime);
			}

			BannerAdState = AdState.Loaded;
			NotifyBannerAdLoaded();

			if (showBanner && preLoadBannerAds)
			{
				ShowBannerAd();
			}
		}

		#endif

		#endregion
	}
}
