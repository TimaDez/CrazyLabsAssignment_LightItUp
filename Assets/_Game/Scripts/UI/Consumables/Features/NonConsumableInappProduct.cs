using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LightItUp.Currency
{

	public class NonConsumableInappProduct : MonoBehaviour,IInappProduct {

		public NonConsumableProductInfo nonConsumableProductInfo;
		public Text productPrice;
		public GameObject buyButton;
		public List<GameObject> nonConsumableActiveIcons;

		void OnEnable()
		{
			AnnounceToService (true);
			InitializeConsumableProduct ();
		}

		void OnDisable()
		{
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
			if (!ShouldBeActive ()) {
				DeactivateProduct ();
				return;
			}
			SetPrice ();
			AddOnClickEvent ();
			
		}


		void DeactivateProduct()
		{
			for (int i = 0; i < nonConsumableActiveIcons.Count; i++) {
				nonConsumableActiveIcons [i].gameObject.SetActive (false);
			}
		}
		bool ShouldBeActive()
		{
			if (ConsumableService.Instance.IsNoAdsProductPurchased()) 
			{
				return false;
			}
			return true;
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
			ConsumableService.Instance.AttemptPurchase (nonConsumableProductInfo.inappName, this);
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
			

//			SendDeltaEvent.ShopClicked (
//				productInfo.inappName,
//				inappInfo.analyticsString,
//				(float)PSDKWrapper.Instance.GetPriceInLocalCurrency (inappInfo.inappName),
//				PSDKWrapper.Instance.GetLocalizedPriceString(inappInfo.inappName),
//				1);
		
		}


		void FailPurchase()
		{
		}
}

}