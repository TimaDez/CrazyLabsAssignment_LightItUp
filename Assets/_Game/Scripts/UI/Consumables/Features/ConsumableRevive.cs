using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LightItUp.UI;

namespace LightItUp.Currency
{
	public class ConsumableRevive : MonoBehaviour 
	{
		private Button reviveButton;
		private int consumableCost = 40;
		private CurrencyType currencyType;

		private void Awake()
		{
			reviveButton = GetComponent<Button>();
			reviveButton.onClick.AddListener (OnRequestRevive);
		}

		private void OnDestroy()
		{
			reviveButton.onClick.RemoveListener(OnRequestRevive);
		}

		void OnRequestRevive()
		{
			if (CurrencyService.Instance.GetCurrentAmount (currencyType) >= consumableCost) 
			{
				CurrencyService.Instance.ConsumeCurrency (CurrencyType.Gems, consumableCost);
				CanvasController.GetPopup<UI_GameOver> ().GemsReviveSuccess ();
			}
		}

    }
}
