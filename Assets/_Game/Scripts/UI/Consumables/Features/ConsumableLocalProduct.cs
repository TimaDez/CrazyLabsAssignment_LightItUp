using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LightItUp.UI;


namespace LightItUp.Currency
{

public class ConsumableLocalProduct : MonoBehaviour {

		public ConsumableLocalProductInfo consumableLocalProductInfo;
		public Text productPrice;
		public GameObject buyButton;
		public TextMeshProUGUI amount;

		public GameObject magnifiedBooster;
		public int magnifyBoosterID;

		[SerializeField] private UI_BoosterLockedText unlockMessage;
		
		void OnEnable()
		{
			InitializeConsumableProduct ();
		}

		void OnDisable()
		{
			buyButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}


		void InitializeConsumableProduct()
		{
			
			SetPrice ();
			AddOnClickEvent ();
			
		}
	
	
		void SetPrice()
		{
			string productPriceString = consumableLocalProductInfo.currencyCostAmount.ToString ();
			productPrice.text = productPriceString;
			amount.text = "x"+consumableLocalProductInfo.boosterAmount.ToString();
		}

		void AddOnClickEvent ()
		{
			buyButton.GetComponent<Button> ().onClick.AddListener (OnButtonClicked);
		}

		void OnButtonClicked()
		{
			
			
			if (CurrencyService.Instance.GetCurrentAmount (CurrencyType.Gems) >= consumableLocalProductInfo.currencyCostAmount)
			{
				if (!IsBoosterUnlocked())
				{
					int unlockAtLevel = BoosterService.Instance.GetBoosterUnlockLevel(consumableLocalProductInfo.boosters[0]);
					unlockMessage.ShowBoosterLocked(unlockAtLevel);
				}
				else
				{
					CurrencyService.Instance.ConsumeCurrency (CurrencyType.Gems, consumableLocalProductInfo.currencyCostAmount);
					RewardPurchase ();
				}
			} 
			else
			{
				unlockMessage.ShowNotEnoughGold();
				FailPurchase ();
			}
		}

		private bool IsBoosterUnlocked()
		{
			List<BoosterType> boosters = consumableLocalProductInfo.boosters;
			if (boosters.Count > 0)
			{
				foreach (var booster in boosters)
				{
					if (!BoosterService.Instance.IsLevelReachedForBooster(booster, GameData.PlayerData.unlockedLevelIdx))
						return false;
				}
			}

			return true;
		}
		void OnPurchaseResolved(bool isSuccessful)
		{
			if (isSuccessful) {
				RewardPurchase ();
			} else {
				FailPurchase ();
			}

		}

		void RewardPurchase()
		{
			MagnifyBooster();
			
            string itemSpent = consumableLocalProductInfo.currencyType.ToString();
            int amountSpent = consumableLocalProductInfo.currencyCostAmount;

            for (int i = 0; i < consumableLocalProductInfo.boosters.Count; i++) 
            {
				BoosterService.Instance.SetBoosterPurchased (consumableLocalProductInfo.boosters [i]);
				BoosterService.Instance.AddRegularBoosterCharges (consumableLocalProductInfo.boosters[i], consumableLocalProductInfo.boosterAmount);
			}
		}

		private void MagnifyBooster()
		{
			magnifiedBooster.SetActive(true);
			magnifiedBooster.GetComponent<CloseBoosterMagnified>().MagnifyBooster(magnifyBoosterID);
		}

		void FailPurchase()
		{
			CanvasController.GetPopup<UI_BoosterShopPopup> ().ShowConsumableProducts ();
		}
}

}