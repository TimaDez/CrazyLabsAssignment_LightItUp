using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LightItUp.Currency
{
	public class ConsumableRVButton : MonoBehaviour {

        private const string ITEM_SPENT = "watchRv";
        private static readonly string BUTTON_NAME = "watchRVStoreButton";
        private static readonly string SOURCE_LOCATION = "shop";
        private const int AMOUNT_SPENT = 0;

        public GameObject availabilityTextObject;
		public GameObject currencyAmountObject;
		public GameObject currencyIconObject;
		//public GameObject outlineObject;
		public GameObject availableInTextObject;
		public GameObject freeGemsButton;
		public GameObject watchNowIcon;
		
		private TextMeshProUGUI currencyAmountText;
		private TextMeshProUGUI availabilityText;
		private Image currencyIconImage;
		//private Image outlineImage;
		private bool isPeriodicCheckActive;
		private float currentChecktime = 0f;


        void OnEnable()
		{
			CashComponents ();
			InitializeButton ();
		}

		void CashComponents()
		{
			currencyAmountText = currencyAmountObject.GetComponent<TextMeshProUGUI> ();
			availabilityText = availabilityTextObject.GetComponent<TextMeshProUGUI> ();
			currencyIconImage = currencyIconObject.GetComponent<Image> ();
			//outlineImage = outlineObject.GetComponent<Image> ();
			currencyAmountText.text = "+"+RVConsumableService.Instance.GetRVConfig ().currencyReward.ToString();
		}

		void InitializeButton()
		{
			currentChecktime = 0f;
			isPeriodicCheckActive = true;

			var isAvailable = RVConsumableService.Instance.IsCooldownFinished () && RVConsumableService.Instance.IsRvAvailable();
			if (isAvailable) {
				ShowActiveState ();
			} else {
				ShowInactiveState ();
			}

		}

		void AddButtonListener()
		{
			freeGemsButton.GetComponent<Button> ().onClick.AddListener (FreeGemsClicked);
		}

		void FreeGemsClicked()
		{
			ResolveResult(true);
		}

		void ResolveResult(bool val)
		{
			if (!val) return;
			
			CurrencyService.Instance.AddCurrency (CurrencyType.Gems, RVConsumableService.Instance.GetRVConfig ().currencyReward, ITEM_SPENT, AMOUNT_SPENT);
			RVConsumableService.Instance.SetCurrentRVWatchedTime ();
			InitializeButton ();
			isPeriodicCheckActive = true;
			freeGemsButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}

        void ShowActiveState()
		{
			availabilityTextObject.SetActive (false);
			currencyAmountText.color = new Color (currencyAmountText.color.r, currencyAmountText.color.g, currencyAmountText.color.b, 1);
			currencyIconImage.color = new Color (currencyIconImage.color.r, currencyIconImage.color.g, currencyIconImage.color.b, 1);
			//outlineImage.color = new Color (outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, 1);
			availableInTextObject.SetActive (false);
			watchNowIcon.SetActive(true);
			isPeriodicCheckActive = false;
			AddButtonListener ();
		}


		void ShowInactiveState()
		{
			currencyAmountText.color = new Color (currencyAmountText.color.r, currencyAmountText.color.g, currencyAmountText.color.b, 0.5f);
			currencyIconImage.color = new Color (currencyIconImage.color.r, currencyIconImage.color.g, currencyIconImage.color.b, 0.5f);
			//outlineImage.color = new Color (outlineImage.color.r, outlineImage.color.g, outlineImage.color.b, 0.5f);
			availabilityTextObject.SetActive (true);
			availableInTextObject.SetActive (true);
			watchNowIcon.SetActive(false);
			if (RVConsumableService.Instance.IsCooldownFinished())
			{
				availabilityText.text = "";
				availableInTextObject.GetComponent<TextMeshProUGUI>().text = "";
			} else {
				availabilityText.text = RVConsumableService.Instance.GetRemainingTime ();
				availableInTextObject.GetComponent<TextMeshProUGUI>().text = "AVAILABLE IN:";
			}

		}

		void Update()
		{
			if (!isPeriodicCheckActive) return;
			
			currentChecktime += Time.unscaledDeltaTime;

			if (currentChecktime >= 0.9f)
				InitializeButton ();
		}

		void OnDisable()
		{
			freeGemsButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
	}
}
