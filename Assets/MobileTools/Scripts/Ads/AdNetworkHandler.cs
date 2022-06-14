using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public abstract class AdNetworkHandler
	{
		#region Enums

		public enum AdState
		{
			None,
			Loading,
			Loaded,
			Showing,
			Shown,
			Wait	// Used by some AdNetworkHandler implementations to signify it is waiting for something to happen so don't call any of the Do methods
		}

		#endregion

		#region Member Variables

		public bool isInitialized;
		public bool bannerAdsEnabled;
		public bool interstitialAdsEnabled;
		public bool rewardAdsEnabled;
		public bool preLoadBannerAds		= true;
		public bool preLoadInterstitialAds	= true;
		public bool preLoadRewardAds		= true;

		protected bool							adsRemoved;
		protected bool							disabled;
		protected MobileAdsManager.ConsentType	consentStatus;

		#endregion

		#region Properties

		public AdState BannerAdState		{ get; protected set; }
		public AdState InterstitialAdState	{ get; protected set; }
		public AdState RewardAdState		{ get; protected set; }

		protected abstract string LogTag { get; }

		#endregion

		#region Events

		public System.Action OnBannerAdLoading				{ get; set; }
		public System.Action OnBannerAdLoaded				{ get; set; }
		public System.Action OnBannerAdFailedToLoad			{ get; set; }
		public System.Action OnBannerAdShown				{ get; set; }
		public System.Action OnBannerAdHidden				{ get; set; }

		public System.Action OnInterstitialAdLoading		{ get; set; }
		public System.Action OnInterstitialAdLoaded			{ get; set; }
		public System.Action OnInterstitialAdFailedToLoad	{ get; set; }
		public System.Action OnInterstitialAdShowing		{ get; set; }
		public System.Action OnInterstitialAdShown			{ get; set; }
		public System.Action OnInterstitialAdClosed			{ get; set; }

		public System.Action OnRewardAdLoading				{ get; set; }
		public System.Action OnRewardAdLoaded				{ get; set; }
		public System.Action OnRewardAdFailedToLoad			{ get; set; }
		public System.Action OnRewardAdShowing				{ get; set; }
		public System.Action OnRewardAdShown				{ get; set; }
		public System.Action OnRewardAdClosed				{ get; set; }
		public System.Action OnRewardAdGranted				{ get; set; }

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Invoked when the AdNetworkHandler needs to initialize
		/// </summary>
		protected abstract void DoInitialize();

		/// <summary>
		/// Invoked when the banner ad needs to load
		/// </summary>
		protected abstract void DoLoadBannerAd();

		/// <summary>
		/// Invoked when the banner ad needs to show
		/// </summary>
		protected abstract void DoShowBannerAd();

		/// <summary>
		/// Invoked when the banner ad needs to hide
		/// </summary>
		protected abstract void DoHideBannerAd();

		/// <summary>
		/// Invoked when an interstitial ad needs to load
		/// </summary>
		protected abstract void DoLoadInterstitialAd();

		/// <summary>
		/// Invoked when an interstitial ad is loaded and needs to show
		/// </summary>
		protected abstract void DoShowInterstitialAd();

		/// <summary>
		/// Invoked when an reward ad needs to load
		/// </summary>
		protected abstract void DoLoadRewardAd();

		/// <summary>
		/// Invoked when an reward ad is loaded and needs to show
		/// </summary>
		protected abstract void DoShowRewardAd();

		/// <summary>
		/// Invoked when ads are removed and ads need to stop being requested
		/// </summary>
		protected abstract void DoAdsRemoved(bool dontRemoveRewardAds);

		/// <summary>
		/// Invoked when the constentType has been set
		/// </summary>
		protected abstract void ConsentStatusUpdated();

		/// <summary>
		/// Gets the banner height in pixels.
		/// </summary>
		protected abstract float DoGetBannerHeightInPixels();

		/// <summary>
		/// Gets the banner position
		/// </summary>
		protected abstract MobileAdsSettings.BannerPosition DoGetBannerPosition();

		#endregion

		#region Public Methods

		/// <summary>
		/// Initialize this network handler
		/// </summary>
		public void Initialize()
		{
			if (adsRemoved)
			{
				return;
			}

			if (!isInitialized)
			{
				DoInitialize();
			}
		}

		public bool LoadBannerAd()
		{
			if (adsRemoved || disabled)
			{
				return false;
			}

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot show banner, network handler has not been initialized");
				return false;
			}

			if (!bannerAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot show banner, banner ads have not been selected for this ad network");
				return false;
			}

			DoLoadBannerAd();

			return true;
		}

		/// <summary>
		/// Shows the banner ad
		/// </summary>
		public void ShowBannerAd()
		{
			if (adsRemoved || disabled)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Showing banner ad");

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot show banner, network handler has not been initialized");
				return;
			}

			if (!bannerAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot show banner, banner ads have not been selected for this ad network");
				return;
			}

			DoShowBannerAd();
		}

		/// <summary>
		/// Hides the banner ad
		/// </summary>
		public void HideBannerAd()
		{
			if (adsRemoved || disabled)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Hiding banner ad");

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot show/hide banner, network handler has not been initialized");
				return;
			}

			if (!bannerAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot show/hide banner, banner ads have not been selected for this ad network");
				return;
			}

			DoHideBannerAd();
		}

		/// <summary>
		/// Loads an interstitial ad if an interstitial ad has not been loaded, loading, or shown
		/// </summary>
		public bool LoadInterstitialAd()
		{
			if (adsRemoved || disabled)
			{
				return false;
			}

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot pre-load interstitial ad, network handler has not been initialized");
				return false;
			}

			if (!interstitialAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot pre-load interstitial ad, interstitial ads have not been selected for this ad network");
				return false;
			}

			if (InterstitialAdState != AdState.None)
			{
				GameDebugManager.Log(LogTag, "Interstitial ad will not be pre-loaded because the AdState is not None, AdState: " + InterstitialAdState);
				return false;
			}

			DoLoadInterstitialAd();

			return true;
		}

		/// <summary>
		/// Shows an interstitial ad. Returns true it an interstitial ad is shown, false if it wasn't shown because there was no pre-loaded interstitial ad to show.
		/// </summary>
		public bool ShowInterstitialAd()
		{
			if (adsRemoved || disabled)
			{
				return false;
			}

			GameDebugManager.Log(LogTag, "Showing interstitial ad");

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot show interstitial ad, network handler has not been initialized");
				return false;
			}

			if (!interstitialAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot show interstitial ad, interstitial ads have not been selected for this ad network");
				return false;
			}

			if (RewardAdState == AdState.Showing || RewardAdState == AdState.Shown)
			{
				GameDebugManager.Log(LogTag, "Interstitial ad will not show because the reward ad is showing");
				return false;
			}

			switch (InterstitialAdState)
			{
				case AdState.None:
					GameDebugManager.Log(LogTag, "Interstitial ad will not show because there is no loaded interstitial ad to show, loading one now");
					// Load a new interstitial ad now
					DoLoadInterstitialAd();
					break;
				case AdState.Loaded:
					// Interstitial ad is loaded and ready to show
					DoShowInterstitialAd();
					return true;
				case AdState.Loading:
					GameDebugManager.Log(LogTag, "Interstitial ad will not show because there is no loaded interstitial ad to show, the interstitial ad is currently being loaded");
					break;
				case AdState.Showing:
					GameDebugManager.Log(LogTag, "Interstitial ad is about to show");
					break;
				case AdState.Shown:
					GameDebugManager.Log(LogTag, "Interstitial ad is already showing");
					break;
				case AdState.Wait:
					GameDebugManager.Log(LogTag, "Interstitial ad will not show because the AdState is set to Wait");
					break;
			}

			return false;
		}

		/// <summary>
		/// Loads an reward ad if an reward ad has not been loaded, loading, or shown
		/// </summary>
		public bool LoadRewardAd()
		{
			if (adsRemoved || disabled)
			{
				return false;
			}

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot pre-load reward ad, network handler has not been initialized");
				return false;
			}

			if (!rewardAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot pre-load reward ad, reward ads have not been selected for this ad network");
				return false;
			}

			if (RewardAdState != AdState.None)
			{
				GameDebugManager.Log(LogTag, "Reward ad will not be pre-loaded because the AdState is not None, AdState: " + RewardAdState);
				return false;
			}

			DoLoadRewardAd();

			return true;
		}

		/// <summary>
		/// Shows a reward ad. Returns true it a reward ad is shown, false if it wasn't shown because there was no pre-loaded reward ad to show.
		/// </summary>
		public bool ShowRewardAd()
		{
			if (adsRemoved || disabled)
			{
				return false;
			}

			GameDebugManager.Log(LogTag, "Showing reward ad");

			if (!isInitialized)
			{
				GameDebugManager.LogError(LogTag, "Cannot show reward ad, network handler has not been initialized");
				return false;
			}

			if (!rewardAdsEnabled)
			{
				GameDebugManager.LogError(LogTag, "Cannot show reward ad, reward ads have not been selected for this ad network");
				return false;
			}

			if (InterstitialAdState == AdState.Showing || InterstitialAdState == AdState.Shown)
			{
				GameDebugManager.Log(LogTag, "Reward ad will not show because the interstitial ad is showing");
				return false;
			}

			switch (RewardAdState)
			{
				case AdState.None:
					GameDebugManager.Log(LogTag, "Reward ad will not show because there is no loaded reward ad to show, loading one now");
					// Load a new reward ad now
					DoLoadRewardAd();
					break;
				case AdState.Loaded:
					// Reward ad is loaded and ready to show
					DoShowRewardAd();
					return true;
				case AdState.Loading:
					GameDebugManager.Log(LogTag, "Reward ad will not show because there is no loaded reward ad to show, the reward ad is currently being loaded");
					break;
				case AdState.Showing:
					GameDebugManager.Log(LogTag, "Reward ad is about to show");
					break;
				case AdState.Shown:
					GameDebugManager.Log(LogTag, "Reward ad is already showing");
					break;
				case AdState.Wait:
					GameDebugManager.Log(LogTag, "Reward ad will not show because the AdState is set to Wait");
					break;
			}

			return false;
		}

		/// <summary>
		/// Notifies this instance when ads have been removed or un-removed.
		/// </summary>
		public void AdsRemoved(bool dontRemoveRewardAds)
		{
			if (!isInitialized || disabled)
			{
				return;
			}

			adsRemoved = true;

			DoAdsRemoved(dontRemoveRewardAds);
		}

		/// <summary>
		/// Sets the consent status selected by the user
		/// </summary>
		public void SetConsentStatus(MobileAdsManager.ConsentType consentStatus)
		{
			if (this.consentStatus == consentStatus)
			{
				return;
			}

			this.consentStatus = consentStatus;

			if (isInitialized && !disabled)
			{
				ConsentStatusUpdated();
			}
		}

		/// <summary>
		/// Gets the banner height in pixels.
		/// </summary>
		public float GetBannerHeightInPixels()
		{
			if (!isInitialized || disabled)
			{
				return 0f;
			}

			return DoGetBannerHeightInPixels();
		}

		/// <summary>
		/// Gets the banner position
		/// </summary>
		public MobileAdsSettings.BannerPosition GetBannerPosition()
		{
			return DoGetBannerPosition();
		}

		#endregion

		#region Protected Methods

		protected void PreLoadBannerAd()
		{
			if (preLoadBannerAds)
			{
				LoadBannerAd();
			}
		}

		protected void PreLoadInterstitialAd()
		{
			if (preLoadInterstitialAds)
			{
				LoadInterstitialAd();
			}
		}

		protected void PreLoadRewardAd()
		{
			if (preLoadRewardAds)
			{
				LoadRewardAd();
			}
		}

		protected void NotifyBannerAdLoading()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Banner Ad loading");

			InvokeEvent(OnBannerAdLoading);
		}

		protected void NotifyBannerAdLoaded()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Banner Ad loaded");

			InvokeEvent(OnBannerAdLoaded);
		}

		protected void NotifyBannerAdFailedToLoad(string error = "")
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.LogError(LogTag, "Banner Ad failed to load" + (string.IsNullOrEmpty(error) ? "" : ", error: " + error));

			InvokeEvent(OnBannerAdFailedToLoad);
		}

		protected void NotifyBannerAdShown()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Banner Ad shown");

			InvokeEvent(OnBannerAdShown);
		}

		protected void NotifyBannerAdHidden()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Banner Ad hidden");

			InvokeEvent(OnBannerAdHidden);
		}

		protected void NotifyInterstitialAdLoading()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Interstitial Ad loading");

			InterstitialAdState = AdState.Loading;

			InvokeEvent(OnInterstitialAdLoading);
		}

		protected void NotifyInterstitialAdLoaded()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Interstitial Ad loaded");

			InterstitialAdState = AdState.Loaded;

			InvokeEvent(OnInterstitialAdLoaded);
		}

		protected void NotifyInterstitialAdFailedToLoad(string error = "")
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.LogError(LogTag, "Interstitial Ad failed to load" + (string.IsNullOrEmpty(error) ? "" : ", error: " + error));

			InterstitialAdState = AdState.None;

			InvokeEvent(OnInterstitialAdFailedToLoad);
		}

		protected void NotifyInterstitialAdShowing()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Interstitial Ad showing");

			InterstitialAdState = AdState.Showing;

			InvokeEvent(OnInterstitialAdShowing);
		}

		protected void NotifyInterstitialAdShown()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Interstitial Ad shown");

			InterstitialAdState = AdState.Shown;

			InvokeEvent(OnInterstitialAdShown);
		}

		protected void NotifyInterstitialAdClosed()
		{
			GameDebugManager.Log(LogTag, "Interstitial Ad closed");

			InterstitialAdState = AdState.None;

			InvokeEvent(OnInterstitialAdClosed);
		}

		protected void NotifyRewardAdLoading()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Reward Ad loading");

			RewardAdState = AdState.Loading;

			InvokeEvent(OnRewardAdLoading);
		}

		protected void NotifyRewardAdLoaded()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Reward Ad loaded");

			RewardAdState = AdState.Loaded;

			InvokeEvent(OnRewardAdLoaded);
		}

		protected void NotifyRewardAdFailedToLoad(string error = "")
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.LogError(LogTag, "Reward Ad failed to load" + (string.IsNullOrEmpty(error) ? "" : ", error: " + error));

			RewardAdState = AdState.None;

			InvokeEvent(OnRewardAdFailedToLoad);
		}

		protected void NotifyRewardAdShowing()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Reward Ad showing");

			RewardAdState = AdState.Showing;

			InvokeEvent(OnRewardAdShowing);
		}

		protected void NotifyRewardAdShown()
		{
			if (adsRemoved)
			{
				return;
			}

			GameDebugManager.Log(LogTag, "Reward Ad shown");

			RewardAdState = AdState.Shown;

			InvokeEvent(OnRewardAdShown);
		}

		protected void NotifyRewardAdClosed()
		{
			GameDebugManager.Log(LogTag, "Reward Ad closed");

			if (RewardAdState == AdState.Shown || RewardAdState == AdState.Showing)
			{
				RewardAdState = AdState.None;
			}

			InvokeEvent(OnRewardAdClosed);
		}

		protected void NotifyRewardAdGranted(string rewardId, double rewardAmount)
		{
			GameDebugManager.Log(LogTag, string.Format("Reward Ad reward granted, rewardId: {0}, rewardAmount: {1}", rewardId, rewardAmount));

			if (OnRewardAdGranted != null)
			{
				OnRewardAdGranted.Invoke();
			}
		}

		private void InvokeEvent(System.Action adEvent)
		{
			if (adEvent != null)
			{
				adEvent.Invoke();
			}
		}

		#endregion
	}
}
