using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.MobileTools
{
	public class AdsConsoleItem : DebugConsoleItem
	{
		#region Inspector Variables
		
		[SerializeField] private Text		textTemplate	= null;
		[SerializeField] private Transform	textContainer	= null;
		
		#endregion // Inspector Variables

		#region Member Variables
		
		private Text adsInitializedText;
		private Text adsRemovedText;
		private Text consentStatusText;
		private Text bannerText;
		private Text interstitialText;
		private Text rewardText;
		
		#endregion // Member Variables

		#region Unity Methods
		
		private void Update()
		{
			if (MobileAdsManager.Exists())
			{
				adsInitializedText.text	= string.Format("Ads Initialized: " + AddColorTags(MobileAdsManager.Instance.IsInitialized));
				adsRemovedText.text		= string.Format("Ads Removed: " + MobileAdsManager.Instance.AdsRemoved);
				consentStatusText.text	= string.Format("Consent Status: " + MobileAdsManager.Instance.ConsentStatus);
				bannerText.text			= string.Format("Banner Ad State: " + MobileAdsManager.Instance.BannerAdState);
				interstitialText.text	= string.Format("Interstitial Ad State: " + MobileAdsManager.Instance.InterstitialAdState);
				rewardText.text			= string.Format("Reward Ad State: " + MobileAdsManager.Instance.RewardAdState);
			}
		}
		
		#endregion // Unity Methods

		#region Public Methods

		public override string GetTabName()
		{
			return "Ads";
		}

		public override void Initialize()
		{
			if (MobileAdsManager.Exists())
			{
				CreateTextItem(string.Format("Ads Enabled: " + AddColorTags(MobileAdsManager.Instance.AreAdsEnabled)));

				adsInitializedText	= CreateTextItem("");
				adsRemovedText		= CreateTextItem("");
				consentStatusText	= CreateTextItem("");
				bannerText			= CreateTextItem("");
				interstitialText	= CreateTextItem("");
				rewardText			= CreateTextItem("");
			}
			else
			{
				CreateTextItem(string.Format("<color=#{0}>{1}</color>", "ff0000", "MobileAdsManager does not exist in the scene"));
			}
		}

		public void OnShowBanner()
		{
			if (MobileAdsManager.Exists())
			{
				MobileAdsManager.Instance.ShowBannerAd();
			}
		}

		public void OnHideBanner()
		{
			if (MobileAdsManager.Exists())
			{
				MobileAdsManager.Instance.HideBannerAd();
			}
		}

		public void OnShowInterstitial()
		{
			if (MobileAdsManager.Exists())
			{
				MobileAdsManager.Instance.ShowInterstitialAd();
			}
		}

		public void OnShowReward()
		{
			if (MobileAdsManager.Exists())
			{
				MobileAdsManager.Instance.ShowRewardAd();
			}
		}

		#endregion

		#region Private Methods
		
		private Text CreateTextItem(string text)
		{
			Text textComponent = Instantiate(textTemplate, textContainer, false);

			textComponent.text = text;

			textComponent.gameObject.SetActive(true);

			return textComponent;
		}

		private string AddColorTags(bool b)
		{
			return string.Format("<color=#{0}>{1}</color>", b ? "00ff00" : "ff0000", b ? "True" : "False");
		}
		
		#endregion // Private Methods
	}
}
