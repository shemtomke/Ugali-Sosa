using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

#if BBG_MT_IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension; 
#endif

#pragma warning disable 0414 // Reason: Some inspector variables are only used in specific platforms and their usages are removed using #if blocks

namespace BBG.MobileTools
{
	public class IAPManager : SingletonComponent<IAPManager>
	#if BBG_MT_IAP
	, IStoreListener
	#endif
	{
		#region Inspector Variables

		public GameObject loadingObj;

		#endregion

		#region Member Variables

		private const string LogTag = "IAPManager";

		#if BBG_MT_IAP
		private IStoreController	storeController;
		private IExtensionProvider 	extensionProvider;
		#endif

		private HashSet<string> purchasedNonConsumables;

		#endregion

		#region Properties

		/// <summary>
		/// Callback that is invoked when the IAPManager has successfully initialized and has retrieved the list of products/prices
		/// </summary>
		public System.Action<bool> OnIAPInitialized { get; set; }

		/// <summary>
		/// Callback that is invoked when a product is purchased, passes the product id that was purchased
		/// </summary>
		public System.Action<string> OnProductPurchased { get; set; }

		/// <summary>
		/// Callback that is invoked when a product purchase fails
		/// </summary>
		public System.Action<string> OnProductPurchasedFailed { get; set; }

		/// <summary>
		/// Returns true if IAP is initialized
		/// </summary>
		public bool IsInitialized
		{
			#if BBG_MT_IAP
			get { return storeController != null && extensionProvider != null; }
			#else
			get { return false; }
			#endif
		}

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			purchasedNonConsumables	= new HashSet<string>();

			LoadSave();
		}

		private void Start()
		{
			#if BBG_MT_IAP

			// Initialize IAP
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			// Add all the product ids to teh builder
			for (int i = 0; i < IAPSettings.Instance.productInfos.Count; i++)
			{
				IAPSettings.ProductInfo productInfo = IAPSettings.Instance.productInfos[i];

				GameDebugManager.Log(LogTag, "Adding product to builder, id: " + productInfo.productId + ", consumable: " + productInfo.consumable);

				builder.AddProduct(productInfo.productId, productInfo.consumable ? ProductType.Consumable : ProductType.NonConsumable);
			}

			GameDebugManager.Log(LogTag, "Initializing IAP now...");

			UnityPurchasing.Initialize(this, builder);

			#endif
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

		#if BBG_MT_IAP

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			GameDebugManager.Log(LogTag, "Initialization successful!");

			storeController		= controller;
			extensionProvider	= extensions;

			if (OnIAPInitialized != null)
			{
				OnIAPInitialized(true);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason failureReason)
		{
			GameDebugManager.LogError(LogTag, "Initializion failed! Reason: " + failureReason);

			if (OnIAPInitialized != null)
			{
				OnIAPInitialized(false);
			}
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			loadingObj.SetActive(false);

			GameDebugManager.LogError(LogTag, "Purchase failed for product id: " + product.definition.id + ", reason: " + failureReason);

			OnProductPurchasedFailed?.Invoke(product.definition.id);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			loadingObj.SetActive(false);

			Product product = args.purchasedProduct;

			GameDebugManager.Log(LogTag, "Purchase successful for product id: " + product.definition.id);

			SetProductPurchased(product);

			return PurchaseProcessingResult.Complete;
		}

		/// <summary>
		/// Starts the buying process for the given product id
		/// </summary>
		public void BuyProduct(string productId)
		{
			if (string.IsNullOrEmpty(productId))
			{
				return;
			}

			GameDebugManager.Log(LogTag, "BuyProduct: Purchasing product with id: " + productId);

			if (IsInitialized)
			{
				Product product = storeController.products.WithID(productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product == null)
				{
					GameDebugManager.LogError(LogTag, "BuyProduct: product with id \"" + productId + "\" does not exist.");
				}
				else if (!product.availableToPurchase)
				{
					GameDebugManager.LogError(LogTag, "BuyProduct: product with id \"" + productId + "\" is not available to purchase.");
				}
				else
				{
					loadingObj.SetActive(true);
					storeController.InitiatePurchase(product);
				}
			}
			else
			{
				GameDebugManager.LogWarning(LogTag, "BuyProduct: IAPManager not initialized.");
			}
		}

		/// <summary>
		/// Sets the given Product as purchased in the IAPManager, this also invokes and events registered to the product
		/// </summary>
		public void SetProductPurchased(Product product)
		{
			if (product.definition.type != ProductType.Consumable)
			{
				purchasedNonConsumables.Add(product.definition.id);
			}

			OnProductPurchased?.Invoke(product.definition.id);
		}

		/// <summary>
		/// Gets the products store information
		/// </summary>
		public Product GetProductInformation(string productId)
		{
			if (IsInitialized)
			{
				return storeController.products.WithID(productId);
			}

			return null;
		}

		#endif

		/// <summary>
		/// Sets the given product as purchased if it is an available product
		/// </summary>
		public void SetProductPurchased(string productId)
		{
			#if BBG_MT_IAP

			Product product = GetProductInformation(productId);

			if (product != null)
			{
				SetProductPurchased(product);
			}

			#endif
		}

		/// <summary>
		/// Returns true if the given product id has been purchased, only for non-consumable products, consumable products will always return false.
		/// </summary>
		public bool IsProductPurchased(string productId)
		{
			return purchasedNonConsumables.Contains(productId);
		}

		/// <summary>
		/// Restores the purchases if platform is iOS or OSX
		/// </summary>
		public void RestorePurchases()
		{
			GameDebugManager.Log(LogTag, "RestorePurchases: Restoring purchases");

			#if BBG_MT_IAP
			if (IsInitialized)
			{
				if ((Application.platform == RuntimePlatform.IPhonePlayer ||
				     Application.platform == RuntimePlatform.OSXPlayer))
				{
					extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result) => {});
				}
				else
				{
					GameDebugManager.LogWarning(LogTag, "RestorePurchases: Device is not iOS, no need to call this method.");
				}
			}
			else
			{
				GameDebugManager.LogWarning(LogTag, "RestorePurchases: IAPManager not initialized.");
			}
			#endif
		}

		#endregion

		#region Save Methods

		private void Save()
		{
			Dictionary<string, object> json = new Dictionary<string, object>();

			json["purchases"] = new List<string>(purchasedNonConsumables);

			Utils.SaveToFile(json, Utils.SaveFolderPath, "iap");
		}

		private void LoadSave()
		{
			JSONNode json = Utils.LoadSaveFile(Utils.SaveFolderPath, "iap");

			if (json != null)
			{
				JSONArray purchasesJson = json["purchases"].AsArray;

				for (int i = 0; i < purchasesJson.Count; i++)
				{
					purchasedNonConsumables.Add(purchasesJson[i].Value);
				}
			}
		}

		#endregion
	}
}