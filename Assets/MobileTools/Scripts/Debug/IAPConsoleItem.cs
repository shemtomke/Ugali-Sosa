using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if BBG_MT_IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension; 
#endif

namespace BBG.MobileTools
{
	public class IAPConsoleItem : DebugConsoleItem
	{
		#region Inspector Variables
		
		[SerializeField] private Text			enabledText				= null;
		[SerializeField] private Text			initializedText			= null;
		[SerializeField] private IAPTestButton	iapTestButtonTemplate	= null;
		[SerializeField] private Transform		iapTestButtonContainer	= null;
		
		#endregion // Inspector Variables

		#region Member Variables

		private GameObjectPool testButtonPool = null;

		#endregion

		#region Unity Methods
		
		private void Update()
		{
			if (IAPManager.Exists())
			{
				string	color	= IAPManager.Instance.IsInitialized ? "00ff00" : "ff0000";
				string	text	= IAPManager.Instance.IsInitialized ? "True" : "False";

				initializedText.text = string.Format("IAP Initialized: <color=#{0}>{1}</color>", color, text);
			}
		}
		
		#endregion // Unity Methods

		#region Public Methods

		public override string GetTabName()
		{
			return "IAP";
		}

		public override void Initialize()
		{
			if (IAPManager.Exists())
			{
				string	color	= IAPSettings.IsIAPEnabled ? "00ff00" : "ff0000";
				string	text	= IAPSettings.IsIAPEnabled ? "True" : "False";

				enabledText.text = string.Format("IAP Enabled: <color=#{0}>{1}</color>", color, text);

				CreateIapButtons();

				iapTestButtonTemplate.gameObject.SetActive(false);
			}
			else
			{
				enabledText.text = string.Format("<color=#{0}>{1}</color>", "ff0000", "IAPManager does not exist in the scene");
			}
		}

		#endregion

		#region Private Methods
		
		private void CreateIapButtons()
		{
			testButtonPool = new GameObjectPool(iapTestButtonTemplate.gameObject, 1, iapTestButtonContainer);

			if (IAPManager.Instance.IsInitialized)
			{
				SetupButtons();
			}
			else
			{
				IAPManager.Instance.OnIAPInitialized += OnIAPInitialized;
			}

			IAPManager.Instance.OnProductPurchased += OnProductPurchased;
		}

		private void OnIAPInitialized(bool success)
		{
			SetupButtons();
		}

		private void SetupButtons()
		{
			testButtonPool.ReturnAllObjectsToPool();

			for (int i = 0; i < IAPSettings.Instance.productInfos.Count; i++)
			{
				IAPSettings.ProductInfo	productInfo	= IAPSettings.Instance.productInfos[i];
				IAPTestButton			testButton	= testButtonPool.GetObject<IAPTestButton>();

				SetupTestButton(productInfo.productId, testButton);
			}
		}

		private void SetupTestButton(string productId, IAPTestButton testButton)
		{
			testButton.idText.text = productId;

			if (IAPSettings.IsIAPEnabled)
			{
				#if BBG_MT_IAP

				Product product = IAPManager.Instance.GetProductInformation(productId);

				if (product == null)
				{
					SetErrorText(testButton, "Product does not exist");
				}
				else
				{
					if (!product.availableToPurchase)
					{
						SetErrorText(testButton, "Product is not available to purchase");
					}
					else
					{
						testButton.errorText.gameObject.SetActive(false);
					}

					testButton.nameText.gameObject.SetActive(true);
					testButton.descText.gameObject.SetActive(true);
					testButton.priceText.gameObject.SetActive(true);
					testButton.consumableText.gameObject.SetActive(true);

					testButton.nameText.text		= "Title: " + product.metadata.localizedTitle;
					testButton.descText.text		= "Description: " + product.metadata.localizedDescription;
					testButton.priceText.text		= "Price: " + product.metadata.localizedPriceString;
					testButton.consumableText.text	= "Type: " + product.definition.type + (product.definition.type != ProductType.Consumable ? " - Purchased: " + IAPManager.Instance.IsProductPurchased(productId) : "");

					testButton.Data					= productId;
					testButton.OnListItemClicked	= OnButtonClicked;
				}

				#endif
			}
			else
			{
				SetErrorText(testButton, "IAP is not enabled");
			}
		}

		private void SetErrorText(IAPTestButton testButton, string message)
		{
			testButton.errorText.text = message;

			testButton.errorText.gameObject.SetActive(true);
			testButton.nameText.gameObject.SetActive(false);
			testButton.descText.gameObject.SetActive(false);
			testButton.priceText.gameObject.SetActive(false);
			testButton.consumableText.gameObject.SetActive(false);

			testButton.OnListItemClicked = null;
		}

		private void OnButtonClicked(int index, object data)
		{
			#if BBG_MT_IAP

			IAPManager.Instance.BuyProduct(data as string);

			#endif
		}

		private void OnProductPurchased(string productId)
		{
			// Refresh the buttons
			SetupButtons();
		}
		
		#endregion // Private Methods
	}
}
