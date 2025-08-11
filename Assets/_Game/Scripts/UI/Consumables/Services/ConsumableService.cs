using System.Collections.Generic;
using LightItUp.Singletons;
using System;

namespace LightItUp.Currency
{

	public class ConsumableService : SingletonCreate<ConsumableService>  
	{
		private bool isPurchaseActive;
		private IInappProduct currentProduct;
		public static Action<string, bool> PurchaseAttempted = (strVal, boolVal)=>{}; 
		public List <string> productsContainingNoAds;

		private List<IInappProduct> currentActiveProducts;
		private bool handlerAttached;

		void OnEnable()
		{
			handlerAttached = false;
			currentActiveProducts = new List<IInappProduct> ();
//			PSDKWrapper.BillingPurchaseCompleted += PsdkWrapperOnBillingPurchaseCompleted;
//			PSDKWrapper.BillingPurchaseFailed += PsdkWrapperOnBillingPurchaseFailed;
//			PSDKWrapper.NotifyOnBillingPurchaseRestored += NotifyOnBillingPurchaseRestored;
			isPurchaseActive = false;
			handlerAttached = true;
			
		}

		public bool IsNoAdsProductPurchased()
		{
			return false;
		}

		void PsdkWrapperOnBillingPurchaseCompleted(string id)
		{
			if (isPurchaseActive) 
			{
				isPurchaseActive = false;
				if (currentProduct!=null) {
					currentProduct.OnPurchaseResolved (true);
				}
				RefreshAllProducts ();
				PurchaseAttempted (id, true);
				return;
			}

		}


		void RefreshAllProducts ()
		{
			for (int i = 0; i < currentActiveProducts.Count; i++) 
			{
				currentActiveProducts [i].RefreshProduct ();
			}
		}
		void PsdkWrapperOnBillingPurchaseFailed(string id)
		{ 			
			isPurchaseActive = false;
			
			if (currentProduct!=null) 
			{
				currentProduct.OnPurchaseResolved (false);
			}
			PurchaseAttempted (id, false);
		}


		void NotifyOnBillingPurchaseRestored(bool value)
		{
			isPurchaseActive = false;
		}

	
		public void AttemptPurchase(string inappId, IInappProduct currentInappProduct)
		{
			if (inappId == null) {
				return;
			}

			if (isPurchaseActive) {
				return;
			}

			isPurchaseActive = true;
			this.currentProduct = currentInappProduct;
			
//			PSDKWrapper.Instance.PurchaseItem (inappId);

		}


		public void RegisterProduct(IInappProduct product, bool shouldRegister)
		{
			if (shouldRegister) {
				currentActiveProducts.Add (product);
			} else {
				currentActiveProducts.Remove (product);
			}
		}


	}
}
