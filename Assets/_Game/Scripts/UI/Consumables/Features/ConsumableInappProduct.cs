using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LightItUp.Currency
{

	public class ConsumableInappProduct : MonoBehaviour, IInappProduct {

		public ConsumableProductInfo consumableProductInfo;
		public ConsumableProductInfo nonConsumableProductInfo;
		public ConsumableProductInfo discountProductInfo;
		public Text productPrice;
		public GameObject buyButton;
		public List<GameObject> nonConsumableActiveIcons;
		private bool isUsingNonConsumable;

		private ConsumableProductInfo currentActiveProductInfo;

		void OnEnable()
		{
			InitializeConsumableProduct ();
			AnnounceToService (true);
		}

		void OnDisable()
		{
//			PSDKWrapper.Instance.networkCheck.AnnounceNetworkStatus -= OnAnnounceNetworkStatus;
			buyButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
			AnnounceToService (false);
		}

		void AnnounceToService(bool val)
		{
			ConsumableService.Instance.RegisterProduct (this, val);
		}
		public void RefreshProduct()
		{
			buyButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
			InitializeConsumableProduct ();
		}

		void InitializeConsumableProduct()
		{
			SetCorrectProduct ();
			ActivateCorrectIcons ();
			SetPrice ();
			AddOnClickEvent ();
//			PSDKWrapper.Instance.networkCheck.AnnounceNetworkStatus += OnAnnounceNetworkStatus;

		}


		void ActivateCorrectIcons()
		{
			if (!isUsingNonConsumable) {
				for (int i = 0; i < nonConsumableActiveIcons.Count; i++) {
					nonConsumableActiveIcons [i].SetActive (false);	
				}
			}
		}

		ConsumableProductInfo GetCorrectConsumableProduct ()
		{
			return discountProductInfo;
		}	
		void SetCorrectProduct()
		{
			if (nonConsumableProductInfo == null) {
				currentActiveProductInfo = GetCorrectConsumableProduct ();
				isUsingNonConsumable = false;
				return;
			}

			bool isNoadsPurchased = ConsumableService.Instance.IsNoAdsProductPurchased ();
			if (isNoadsPurchased) 
			{
				currentActiveProductInfo = GetCorrectConsumableProduct ();
				isUsingNonConsumable = false;
			}
		}
		void SetPrice()
		{
			string productPriceString = "Unavailable";

			productPrice.text = productPriceString;
		}

		void AddOnClickEvent ()
		{
			buyButton.GetComponent<Button> ().onClick.AddListener (OnButtonClicked);
			buyButton.GetComponent<Button> ().interactable = true;
		}

		void OnButtonClicked()
		{
			ConsumableService.Instance.AttemptPurchase (currentActiveProductInfo.inappName, this);
			//RewardPurchase();
		}

		public void OnPurchaseResolved(bool isSuccessful)
		{
			if (isSuccessful) {
				RewardPurchase ();
			} else {
				FailPurchase ();
			}

		}

		void RewardPurchase()
		{
            string inappName = currentActiveProductInfo.inappName;
            string itemSpent = "buy_" + inappName;
//            int amountSpent = (int)PSDKWrapper.Instance.GetPriceInLocalCurrency(inappName);
			int amountSpent = 0;
            for (int i = 0; i < currentActiveProductInfo.boosters.Count; i++) 
            {
				BoosterService.Instance.SetBoosterPurchased (currentActiveProductInfo.boosters [i]);
				BoosterService.Instance.AddRegularBoosterCharges (currentActiveProductInfo.boosters[i], currentActiveProductInfo.boosterAmount);
			}

			if (currentActiveProductInfo.currencyAmount > 0) 
            {
                CurrencyService.Instance.AddCurrency(currentActiveProductInfo.currencyType, currentActiveProductInfo.currencyAmount, itemSpent, amountSpent); 

            }

		
		}

		void FailPurchase()
		{
			
		}
}

}