using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using BBG.MobileTools;

namespace BBG.MobileTools
{
	public class MobileAdsManager : SingletonComponent<MobileAdsManager>
	{
		#region Enums

		public enum ConsentType
		{
			Unknown,
			Personalized,
			NonPersonalized
		}

		public enum UserLocation
		{
			Unknown,
			InEEA,
			NotInEEA
		}

		#endregion

		#region Inspector Variables

		public GameObject consentPopup = null;
		public GameObject loadingObj = null;

		#endregion

		#region Member Variables

		private const string LogTag = "MobileAdsManager";

		private bool			isAdsInitialized;
		private UserLocation	userLocation;
		private bool			adsRemoved;

		private Dictionary<string, AdNetworkHandler> networkHandlers;

		private System.Action onInterstitialAdClosedCallback;
		private System.Action onRewardAdClosedCallback;
		private System.Action onRewardGrantedCallback;

		private IEnumerator bannerRetryRoutine;
		private IEnumerator interstitialRetryRoutine;
		private IEnumerator rewardRetryRoutine;

		#endregion

		#region Properties

		public bool IsInitialized				{ get { return initialized && isAdsInitialized; } }
		public bool AreAdsEnabled				{ get { return MobileAdsSettings.Instance.adsEnabled; } }
		public bool AreBannerAdsEnabled			{ get { return AreAdsEnabled && MobileAdsSettings.Instance.AreBannerAdsEnabled && !adsRemoved; } }
		public bool AreInterstitialAdsEnabled	{ get { return AreAdsEnabled && MobileAdsSettings.Instance.AreInterstitialAdsEnabled && !adsRemoved; } }
		public bool AreRewardAdsEnabled			{ get { return AreAdsEnabled && MobileAdsSettings.Instance.AreRewardAdsEnabled && (!adsRemoved || MobileAdsSettings.Instance.dontRemoveRewardAds); } }

		/// <summary>
		/// Gets a value indicating whether ads are completely removed (dontRemoveRewardAds is set to false and ads have been removed)
		/// </summary>
		public bool AdsRemoved { get { return adsRemoved; } }
		
		/// <summary>
		/// If ads are removed and dontRemoveRewardAds is selected then reward ads will still load and appear in the game.
		/// </summary>
		public bool DontRemoveRewardAds { get { return MobileAdsSettings.Instance.dontRemoveRewardAds; } }

		/// <summary>
		/// Gets the ConsentType that has been set, returns ConsentType.Unknown it consent has not been set
		/// </summary>
		public ConsentType ConsentStatus { get; private set; }

		/// <summary>
		/// Gets the banner ad network.
		/// </summary>
		public AdNetworkHandler BannerAdHandler { get { return GetAdNetworkHandler(MobileAdsSettings.Instance.SelectedBannerAdNetworkId); } }

		/// <summary>
		/// Gets the banner ad network.
		/// </summary>
		public AdNetworkHandler InterstitialAdHandler { get { return GetAdNetworkHandler(MobileAdsSettings.Instance.SelectedInterstitialAdNetworkId); } }

		/// <summary>
		/// Gets the banner ad network.
		/// </summary>
		public AdNetworkHandler RewardAdHandler { get { return GetAdNetworkHandler(MobileAdsSettings.Instance.SelectedRewardAdNetworkId); } }

		/// <summary>
		/// Returns the AdState for the interstitial ad
		/// </summary>
		public AdNetworkHandler.AdState BannerAdState
		{
			get { return BannerAdHandler != null ? BannerAdHandler.BannerAdState : AdNetworkHandler.AdState.None; }
		}

		/// <summary>
		/// Returns the AdState for the interstitial ad
		/// </summary>
		public AdNetworkHandler.AdState InterstitialAdState
		{
			get { return InterstitialAdHandler != null ? InterstitialAdHandler.InterstitialAdState : AdNetworkHandler.AdState.None; }
		}

		/// <summary>
		/// Returns the AdState for the reward ad
		/// </summary>
		public AdNetworkHandler.AdState RewardAdState
		{
			get { return RewardAdHandler != null ? RewardAdHandler.RewardAdState : AdNetworkHandler.AdState.None; }
		}

		#endregion

		#region Events

		public System.Action OnInitialized					{ get; set; }
		public System.Action OnAdsRemoved					{ get; set; }

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

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			LoadSave();
		}

		private void Start()
		{
			// Check if ads have already been initialized
			if (!initialized || isAdsInitialized)
			{
				return;
			}

			// Check if ads have been removed for this user
			if (adsRemoved && !MobileAdsSettings.Instance.dontRemoveRewardAds)
			{
				GameDebugManager.Log(LogTag, "Ads have been removed, ads will not be initialized or requested");

				return;
			}

			// Check if ads are even enabled in settings
			if (!AreAdsEnabled)
			{
				GameDebugManager.Log(LogTag, "Ads are not enabled in settings, ads will not be initialized or requested");

				return;
			}

			StartCoroutine(BeginAdsInitialization());
		}

		protected override void OnDestroy()
		{
			Save();

			base.OnDestroy();
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				Save();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Loads the banner ad
		/// </summary>
		public void LoadBannerAd()
		{
			if (!isAdsInitialized || !AreBannerAdsEnabled)
			{
				return;
			}

			if (BannerAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Banner ads are not enabled");

				return;
			}

			if (bannerRetryRoutine == null)
			{
				StopCoroutine(bannerRetryRoutine);

				bannerRetryRoutine = null;
			}

			BannerAdHandler.LoadBannerAd();
		}

		/// <summary>
		/// Shows the banner ad
		/// </summary>
		public void ShowBannerAd()
		{
			if (!isAdsInitialized || !AreBannerAdsEnabled)
			{
				return;
			}

			if (BannerAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Banner ads are not enabled");

				return;
			}

			BannerAdHandler.ShowBannerAd();
		}

		/// <summary>
		/// Hides the banner ad
		/// </summary>
		public void HideBannerAd()
		{
			if (!isAdsInitialized || !AreBannerAdsEnabled)
			{
				return;
			}

			if (BannerAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Banner ads are not enabled");

				return;
			}

			BannerAdHandler.HideBannerAd();
		}

		/// <summary>
		/// Loads an interstitial ad if one is not already loaded, loading, or showing
		/// </summary>
		public void LoadInterstitialAd()
		{
			if (!isAdsInitialized || !AreInterstitialAdsEnabled)
			{
				return;
			}

			if (InterstitialAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Interstitial ads are not enabled");

				return;
			}

			if (interstitialRetryRoutine == null)
			{
				StopCoroutine(interstitialRetryRoutine);

				interstitialRetryRoutine = null;
			}

			InterstitialAdHandler.LoadInterstitialAd();
		}

		/// <summary>
		/// Shows an interstitial ad
		/// </summary>
		public void ShowInterstitialAd()
		{
			ShowInterstitialAd(null);
		}

		/// <summary>
		/// Shows the interstitial ad. Returns true if the ad was successfully shown, false otherwise. If the ad was shown then onFinished will be invoked
		/// when the ad finishes.
		/// </summary>
		public bool ShowInterstitialAd(System.Action onFinished)
		{
			if (!isAdsInitialized || !AreInterstitialAdsEnabled)
			{
				return false;
			}

			if (InterstitialAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Interstitial ads are not enabled");
				
				return false;
			}

			onInterstitialAdClosedCallback = onFinished;  
			
			bool showing = InterstitialAdHandler.ShowInterstitialAd();

			if (loadingObj != null) loadingObj.SetActive(showing);

			return showing;
		}

		/// <summary>
		/// Loads an interstitial ad if one is not already loaded, loading, or showing
		/// </summary>
		public void LoadRewardAd()
		{
			if (!isAdsInitialized || !AreRewardAdsEnabled)
			{
				return;
			}

			if (RewardAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Reward ads are not enabled");

				return;
			}

			if (rewardRetryRoutine == null)
			{
				StopCoroutine(rewardRetryRoutine);

				rewardRetryRoutine = null;
			}

			RewardAdHandler.LoadRewardAd();
		}

		/// <summary>
		/// Shows the reward ad
		/// </summary>
		public void ShowRewardAd()
		{
			ShowRewardAd(null, null);
		}

		/// <summary>
		/// Shows the reward ad. Returns true if the ad was successfully shown, false otherwise.
		/// The onClosedCallback will be invoked when the ad closes, use this to resume game play.
		/// The onRewardGrantedCallback will be invoked when the player should be given the reward.
		/// </summary>
		public bool ShowRewardAd(System.Action onClosedCallback, System.Action onRewardGrantedCallback)
		{
			if (!isAdsInitialized || !AreRewardAdsEnabled)
			{
				return false;
			}

			if (RewardAdHandler == null)
			{
				GameDebugManager.LogWarning(LogTag, "Reward ads are not enabled");

				return false;
			}

			this.onRewardAdClosedCallback	= onClosedCallback;
			this.onRewardGrantedCallback	= onRewardGrantedCallback;

			bool showing = RewardAdHandler.ShowRewardAd();

			if (loadingObj != null) loadingObj.SetActive(showing);

			return showing;
		}

		/// <summary>
		/// Sets the consent status to use when requesting ads, 0 for non-personalized ads and 1 for personalized ads
		/// </summary>
		public void SetConsentStatus(int consentStatus)
		{
			switch (consentStatus)
			{
				case 0:
					SetConsentStatus(ConsentType.NonPersonalized);
					break;
				case 1:
					SetConsentStatus(ConsentType.Personalized);
					break;
				default:
					GameDebugManager.LogError(LogTag, "Invalid constent status: " + consentStatus + ". Must be either 0 (non-personalized) or 1 (personalized)");
					break;
			}

		}

		/// <summary>
		/// Sets the consent status to use when requesting ads
		/// </summary>
		public void SetConsentStatus(ConsentType consentStatus)
		{
			GameDebugManager.Log(LogTag, "Setting consent status to: " + consentStatus.ToString());

			ConsentStatus = consentStatus;

			if (!isAdsInitialized)
			{
				// If ads have not been initialized yet then do it now
				InitializeAds();
			}
			else
			{
				// Notify all AdNetworkHandlers that the consent status has changed
				foreach (KeyValuePair<string, AdNetworkHandler> pair in networkHandlers)
				{
					pair.Value.SetConsentStatus(consentStatus);
				}
			}
		}

		/// <summary>
		/// Removes ads for this user
		/// </summary>
		public void RemoveAds()
		{
			if (adsRemoved)
			{
				GameDebugManager.Log(LogTag, "RemoveAds: Ads already removed");

				return;
			}

			GameDebugManager.Log(LogTag, "Removing ads");

			adsRemoved = true;

			if (isAdsInitialized)
			{
				foreach (KeyValuePair<string, AdNetworkHandler> pair in networkHandlers)
				{
					pair.Value.AdsRemoved(MobileAdsSettings.Instance.dontRemoveRewardAds);
				}
			}

			if (OnAdsRemoved != null)
			{
				OnAdsRemoved();
			}
		}

		/// <summary>
		/// Gets the banner height in pixels.
		/// </summary>
		public float GetBannerHeightInPixels()
		{
			if (BannerAdHandler != null)
			{
				return BannerAdHandler.GetBannerHeightInPixels();
			}

			return 0f;
		}

		/// <summary>
		/// Gets the banner height in pixels.
		/// </summary>
		public MobileAdsSettings.BannerPosition GetBannerPosition()
		{
			if (BannerAdHandler != null)
			{
				return BannerAdHandler.GetBannerPosition();
			}

			return MobileAdsSettings.BannerPosition.Bottom;
		}

		/// <summary>
		/// Starts a coroutine, used by AdNetworkHandlers
		/// </summary>
		public void BeginCoroutine(IEnumerator routine)
		{
			StartCoroutine(routine);
		}

		/// <summary>
		/// Stops a coroutine, used by AdNetworkHandlers
		/// </summary>
		public void EndCoroutine(IEnumerator routine)
		{
			StopCoroutine(routine);
		}

		#endregion

		#region Private Methods

		protected override void OnInitialize()
		{
			DontDestroyOnLoad(this);
		}

		private IEnumerator BeginAdsInitialization()
		{
			GameDebugManager.Log(LogTag, "Waiting for internet connection...");

			// First check if the users device has access to the internet
			while (true)
			{
				string url = "https://www.google.com";

				// Check if we can ping google.com
				using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
				{
					// Request and wait for google to response
					yield return webRequest.SendWebRequest();

					if (webRequest.result == UnityWebRequest.Result.Success)
					{
						break;
					}

					GameDebugManager.LogError(LogTag, "Network error: " + webRequest.error);
				}

				// Wait until trying again
				yield return new WaitForSeconds(10);
			}

			if (ConsentStatus == ConsentType.Unknown && MobileAdsSettings.Instance.consentSetting == MobileAdsSettings.ConsentSetting.RequireOnlyInEEA)
			{
				GameDebugManager.Log(LogTag, "User consent is unknown and the consent setting is \"require only in EEA\", checking is user is in EEA now...");

				// If the consent status is unknown and we are only requiring consent if the user is in the EEA then before initializing ads
				// lets try and determine if the user is in the EEA or not
				yield return CheckIsInEEA();
			}
			
			InitializeAds();
		}

		/// <summary>
		/// Initializes ad networks and pre-loads ads
		/// </summary>
		private void InitializeAds()
		{
			// Check if consent is required before initializing ads
			if (RequireUserConsent())
			{
				if (ConsentStatus == ConsentType.Unknown)
				{
					GameDebugManager.Log(LogTag, "Consent is required and has not been given, ads will not be initialized or requested");

					if (consentPopup != null)
					{
						consentPopup.gameObject.SetActive(true);
					}

					// Consent has not been set, do not initialize ads until consent has been set
					return;
				}

				GameDebugManager.Log(LogTag, "Consent type is set to: " + ConsentStatus);
			}

			CreateAdNetworkHandlers();

			isAdsInitialized = true;

			if (OnInitialized != null)
			{
				OnInitialized();
			}
		}

		/// <summary>
		/// Creates the AdNetworkHandlers that will handle shwoing banner/interstitial/reward for the selected network
		/// </summary>
		private void CreateAdNetworkHandlers()
		{
			networkHandlers = new Dictionary<string, AdNetworkHandler>();

			// Create the AdNetworkHandler that should be used by banner ads
			if (AreBannerAdsEnabled)
			{
				try
				{
					CreateAdNetworkHandler(MobileAdsSettings.Instance.SelectedBannerAdNetworkId);

					BannerAdHandler.bannerAdsEnabled = true;

					// Add this instance event listeners
					BannerAdHandler.OnBannerAdLoading		+= BannerAdLoaded;
					BannerAdHandler.OnBannerAdLoaded		+= BannerAdLoaded;
					BannerAdHandler.OnBannerAdFailedToLoad	+= BannerAdFailedToLoad;
					BannerAdHandler.OnBannerAdShown			+= BannerAdShown;
					BannerAdHandler.OnBannerAdHidden		+= BannerAdHidden;
				}
				catch(System.Exception ex)
				{
					GameDebugManager.LogError(LogTag, "Could not create banner AdNetworkHandler: " + ex.Message);
				}
			}

			// Create the AdNetworkHandler that should be used by interstitial ads
			if (AreInterstitialAdsEnabled)
			{
				try
				{
					CreateAdNetworkHandler(MobileAdsSettings.Instance.SelectedInterstitialAdNetworkId);

					InterstitialAdHandler.interstitialAdsEnabled = true;

					// Add this instance event listeners
					InterstitialAdHandler.OnInterstitialAdLoading		+= InterstitialAdLoading;
					InterstitialAdHandler.OnInterstitialAdLoaded		+= InterstitialAdLoaded;
					InterstitialAdHandler.OnInterstitialAdFailedToLoad	+= InterstitialAdFailedToLoad;
					InterstitialAdHandler.OnInterstitialAdShowing		+= InterstitialAdShowing; 
					InterstitialAdHandler.OnInterstitialAdShown			+= InterstitialAdShown;
					InterstitialAdHandler.OnInterstitialAdClosed		+= InterstitialAdClosed;
				}
				catch(System.Exception ex)
				{
					GameDebugManager.LogError(LogTag, "Could not create interstitial AdNetworkHandler: " + ex.Message);
				}
			}

			// Create the AdNetworkHandler that should be used by reward ads
			if (AreRewardAdsEnabled)
			{
				try
				{
					CreateAdNetworkHandler(MobileAdsSettings.Instance.SelectedRewardAdNetworkId);

					RewardAdHandler.rewardAdsEnabled = true;

					// Add event listeners
					RewardAdHandler.OnRewardAdLoaded		+= RewardAdLoaded;
					RewardAdHandler.OnRewardAdLoading		+= RewardAdLoading;
					RewardAdHandler.OnRewardAdFailedToLoad	+= RewardAdFailedToLoad;
					RewardAdHandler.OnRewardAdShowing		+= RewardAdShowing;
					RewardAdHandler.OnRewardAdShown			+= RewardAdShown;
					RewardAdHandler.OnRewardAdClosed		+= RewardAdClosed;
					RewardAdHandler.OnRewardAdGranted		+= RewardAdGranted;
				}
				catch(System.Exception ex)
				{
					GameDebugManager.LogError(LogTag, "Could not create reward AdNetworkHandler: " + ex.Message);
				}
			}

			// Initialize each ad handler
			foreach (KeyValuePair<string, AdNetworkHandler> pair in networkHandlers)
			{
				pair.Value.SetConsentStatus(ConsentStatus);
				pair.Value.Initialize();
			}
		}

		/// <summary>
		/// Creates the AdNetworkHandler with the given network id
		/// </summary>
		private void CreateAdNetworkHandler(string networkId)
		{
			if (!networkHandlers.ContainsKey(networkId))
			{
				AdNetworkHandler networkHandler = null;

				switch (networkId)
				{
					case MobileAdsSettings.AdMobNetworkId:
						networkHandler = new AdMobNetworkHandler();
						break;
					case MobileAdsSettings.UnityAdsNetworkId:
						networkHandler = new UnityAdsNetworkHandler();
						break;
					default:
						throw new System.Exception("Unknown ad network id: " + networkId);
				}

				networkHandlers.Add(networkId, networkHandler);
			}
		}

		private IEnumerator CheckIsInEEA()
		{
			string url = "http://adservice.google.com/getconfig/pubvendors";

			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				// Request and wait for the desired page.
				yield return webRequest.SendWebRequest();

				bool isError = false;

				isError = webRequest.result != UnityWebRequest.Result.Success;

				if (isError)
				{
					GameDebugManager.Log(LogTag, "Error when checking users location, error: " + webRequest.error);

					userLocation = UserLocation.Unknown;
				}
				else
				{
					// Example response: {"is_request_in_eea_or_unknown":false}
					JSONNode json = JSON.Parse(webRequest.downloadHandler.text);

					if (json.IsObject && (json.AsObject).HasKey("is_request_in_eea_or_unknown") && (json.AsObject)["is_request_in_eea_or_unknown"].IsBoolean)
					{
						userLocation = (json.AsObject)["is_request_in_eea_or_unknown"].AsBool ? UserLocation.InEEA : UserLocation.NotInEEA;
					}
					else
					{
						GameDebugManager.Log(LogTag, "Could not parse response, invalid or unexpected format");

						userLocation = UserLocation.Unknown;
					}
				}

				GameDebugManager.Log(LogTag, "User location: " + userLocation);
			}
		}

		/// <summary>
		/// Checks if ads will not be requested until consent is given
		/// </summary>
		private bool RequireUserConsent()
		{
			if (MobileAdsSettings.Instance.consentSetting == MobileAdsSettings.ConsentSetting.NotRequired)
			{
				return false;
			}

			if (MobileAdsSettings.Instance.consentSetting == MobileAdsSettings.ConsentSetting.RequireAll)
			{
				return true;
			}

			return (userLocation == UserLocation.InEEA || userLocation == UserLocation.Unknown);
		}

		/// <summary>
		/// Gets the ad network handler if it has been created
		/// </summary>
		private AdNetworkHandler GetAdNetworkHandler(string networkId)
		{
			if (networkHandlers != null && networkHandlers.ContainsKey(networkId))
			{
				return networkHandlers[networkId];
			}

			return null;
		}

		/// <summary>
		/// Invoked when the interstitial ad failed to load an ad.
		/// </summary>
		private void BannerAdFailedToLoad()
		{
			if (MobileAdsSettings.Instance.retryLoadIfFailed)
			{
				if (bannerRetryRoutine != null)
				{
					StopCoroutine(bannerRetryRoutine);
				}

				StartCoroutine(bannerRetryRoutine = RetryBannerAdLoad());
			}

			if (OnBannerAdFailedToLoad!= null)
			{
				OnBannerAdFailedToLoad();
			}
		}

		/// <summary>
		/// Waits the specified amount of time before trying to load a new banner ad
		/// </summary>
		private IEnumerator RetryBannerAdLoad()
		{
			yield return new WaitForSeconds(MobileAdsSettings.Instance.retryWaitTime);

			if (BannerAdState == AdNetworkHandler.AdState.None)
			{
				LoadBannerAd();
			}
		}

		/// <summary>
		/// Invoked when a banner ad starts loading
		/// </summary>
		private void BannerAdLoading()
		{
			if (OnBannerAdLoading!= null)
			{
				OnBannerAdLoading();
			}
		}

		/// <summary>
		/// Invoked when a banner ad has loaded
		/// </summary>
		private void BannerAdLoaded()
		{
			if (OnBannerAdLoaded!= null)
			{
				OnBannerAdLoaded();
			}
		}

		/// <summary>
		/// Invoked when the banner ad is shown
		/// </summary>
		private void BannerAdShown()
		{
			if (OnBannerAdShown!= null)
			{
				OnBannerAdShown();
			}
		}

		/// <summary>
		/// Invoked when the banner ad is hidden
		/// </summary>
		private void BannerAdHidden()
		{
			if (OnBannerAdHidden!= null)
			{
				OnBannerAdHidden();
			}
		}

		/// <summary>
		/// Invoked when the interstitial ad failed to load an ad.
		/// </summary>
		private void InterstitialAdFailedToLoad()
		{
			if (MobileAdsSettings.Instance.retryLoadIfFailed)
			{
				if (interstitialRetryRoutine != null)
				{
					StopCoroutine(interstitialRetryRoutine);
				}

				StartCoroutine(interstitialRetryRoutine = RetryInterstitialAdLoad());
			}

			if (OnInterstitialAdFailedToLoad!= null)
			{
				OnInterstitialAdFailedToLoad();
			}
		}

		/// <summary>
		/// Waits the specified amount of time before pre-loading an interstitial ad
		/// </summary>
		private IEnumerator RetryInterstitialAdLoad()
		{
			yield return new WaitForSeconds(MobileAdsSettings.Instance.retryWaitTime);

			if (InterstitialAdState == AdNetworkHandler.AdState.None)
			{
				LoadInterstitialAd();
			}
		}

		/// <summary>
		/// Invoked when the interstitial ad closes
		/// </summary>
		private void InterstitialAdClosed()
		{
			if (loadingObj != null) loadingObj.SetActive(false);

			if (onInterstitialAdClosedCallback != null)
			{
				onInterstitialAdClosedCallback();
			}

			if (OnInterstitialAdClosed!= null)
			{
				OnInterstitialAdClosed();
			}
		}

		/// <summary>
		/// Invoked when an iterstitial ad starts loading
		/// </summary>
		private void InterstitialAdLoading()
		{
			if (OnInterstitialAdLoading!= null)
			{
				OnInterstitialAdLoading();
			}
		}

		/// <summary>
		/// Invoked when an interstitial ad has loaded successfully
		/// </summary>
		private void InterstitialAdLoaded()
		{
			if (OnInterstitialAdLoaded!= null)
			{
				OnInterstitialAdLoaded();
			}
		}

		/// <summary>
		/// Invoked when an interstitial ad is about to show
		/// </summary>
		private void InterstitialAdShowing()
		{
			if (OnInterstitialAdShowing!= null)
			{
				OnInterstitialAdShowing();
			}
		}

		/// <summary>
		/// Invoked when an interstitial ad is shown on the screen
		/// </summary>
		private void InterstitialAdShown()
		{
			if (OnInterstitialAdShown!= null)
			{
				OnInterstitialAdShown();
			}
		}

		/// <summary>
		/// Invoked when a reward ad failes to load
		/// </summary>
		private void RewardAdFailedToLoad()
		{
			if (MobileAdsSettings.Instance.retryLoadIfFailed)
			{
				if (rewardRetryRoutine != null)
				{
					StopCoroutine(rewardRetryRoutine);
				}

				StartCoroutine(rewardRetryRoutine = RetryRewardAdLoad());
			}

			if (OnRewardAdFailedToLoad!= null)
			{
				OnRewardAdFailedToLoad();
			}
		}

		/// <summary>
		/// Waits the specified amount of time before pre-loading an reward ad
		/// </summary>
		private IEnumerator RetryRewardAdLoad()
		{
			yield return new WaitForSeconds(MobileAdsSettings.Instance.retryWaitTime);

			if (RewardAdState == AdNetworkHandler.AdState.None)
			{
				LoadRewardAd();
			}
		}

		/// <summary>
		/// Invoked when a reward ad closes
		/// </summary>
		private void RewardAdClosed()
		{
			if (loadingObj != null) loadingObj.SetActive(false);

			if (onRewardAdClosedCallback != null)
			{
				onRewardAdClosedCallback();
			}

			if (OnRewardAdClosed!= null)
			{
				OnRewardAdClosed();
			}
		}

		/// <summary>
		/// Invoked when the player should be rewarded for watching a reward ad
		/// </summary>
		private void RewardAdGranted()
		{
			onRewardGrantedCallback?.Invoke();
			OnRewardAdGranted?.Invoke();
		}

		/// <summary>
		/// Invoked when a reward ad starts loading
		/// </summary>
		private void RewardAdLoading()
		{
			if (OnRewardAdLoading!= null)
			{
				OnRewardAdLoading();
			}
		}

		/// <summary>
		/// Invoked when a reward ad has successfully loaded
		/// </summary>
		private void RewardAdLoaded()
		{
			if (OnRewardAdLoaded!= null)
			{
				OnRewardAdLoaded();
			}
		}

		/// <summary>
		/// Invoked when a reward ad is about to show
		/// </summary>
		private void RewardAdShowing()
		{
			if (OnRewardAdShowing!= null)
			{
				OnRewardAdShowing();
			}
		}

		/// <summary>
		/// Invoked when a reward ad is shown on the screen
		/// </summary>
		private void RewardAdShown()
		{
			if (OnRewardAdShown!= null)
			{
				OnRewardAdShown();
			}
		}

		#endregion

		#region Save Methods

		private void Save()
		{
			Dictionary<string, object> json = new Dictionary<string, object>();

			json["ads_removed"]		= adsRemoved;
			json["consent_status"]	= (int)ConsentStatus;

			Utils.SaveToFile(json, Utils.SaveFolderPath, "ads");
		}

		private void LoadSave()
		{
			JSONNode json = Utils.LoadSaveFile(Utils.SaveFolderPath, "ads");

			if (json != null)
			{
				adsRemoved		= json["ads_removed"].AsBool;
				ConsentStatus	= (ConsentType)json["consent_status"].AsInt;
			}
		}

		#endregion
	}
}
