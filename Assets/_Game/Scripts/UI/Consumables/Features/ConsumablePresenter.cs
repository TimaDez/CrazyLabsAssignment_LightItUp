using System;
using System.Collections;
using UnityEngine;
using TMPro;
using LightItUp.UI;

namespace LightItUp.Currency
{

	public class ConsumablePresenter : MonoBehaviour {

		public CurrencyType currencyType;
		public TextMeshProUGUI consumableAmountText;
		public GameObject gemsCounterBanner;
		public GameObject openShopButton;
		public GameObject gemsIcon;
		public GameObject addedGemsIcon;
		public TextMeshProUGUI addedGemsAmount;

		private bool rvCanAppear;
		private Vector3 menuPosition = new Vector3 (0f, -21f, 0f);
		private Vector3 popupPosition = new Vector3 (0f, -71f, 0f);
		private Vector3 levelsPosition = new Vector3 (0f, -181f , 0f);
		private CanvasController.Popups currentOpenPopup;
		private float lastKnownTimeScale = 1f;
		private float gameEndedPositionTop = 0;
		private float gameEndedPositionBottom = -215f;

		private bool isGameEnded = false;
		private bool defaultPositionsAvailable = false;
		private float topPositionYDefault = 0;
		private float bottomPositionYDefault = 0;


		void OnEnable()
		{
			if (!ShouldShowPresenter()) {
				HideFeature ();
				return;
			}
			SubscribeToRelevantEvents ();

			InitializeConsumablePresenter();
		}

		void HideFeature()
		{
			rvCanAppear = false;
			gameObject.SetActive (false);
		}
		bool ShouldShowPresenter()
		{
			return true;
		}

		void OnDisable()
		{
			UnsubscribeToRelevantEvents ();
		}

		public void ToggleShowPresenter(bool toggle, int positionIndex, bool canShowRV, CanvasController.Popups currentPopup)
		{
			isGameEnded = false;
			rvCanAppear = canShowRV;
			Vector2 targetPosition = Vector2.zero;
			switch (positionIndex) {
			case 1: 
				targetPosition = menuPosition;
				break;
			case 2: 
				targetPosition = popupPosition;
				break;
			case 3:
				if (toggle)
				{
					PositionPresenterGameEnded();
					isGameEnded = true;
				}
				else
				{
					ResetPresenterGameEnded();
				}
				break;

			case 4: 
				targetPosition = levelsPosition;
				break;
			default:
				targetPosition = menuPosition;
				break;
			}
			currentOpenPopup = currentPopup;
			
			if (isGameEnded)
			{
				PositionPresenterGameEnded();
			}
			else
			{
				SetPresenterPosition (targetPosition);
			}
			gameObject.SetActive (toggle);
			addedGemsAmount.gameObject.SetActive (false);
			addedGemsIcon.SetActive (false);
			CheckOpenShopButton ();

		}

		void PositionPresenterGameEnded()
		{
//			Vector3 gemsCounter = rvIcon.GetComponent<RectTransform>().localPosition;
//			gemsCounterBanner.GetComponent<RectTransform>().localPosition = new Vector3(gemsCounter.x, gameEndedPositionTop, gemsCounter.z);
//			
//			Vector3 rvIconRect = rvIcon.GetComponent<RectTransform>().localPosition;
//			rvIcon.GetComponent<RectTransform>().localPosition = new Vector3(rvIconRect.x, gameEndedPositionBottom, rvIconRect.z);
//			
		}
		void ResetPresenterGameEnded()
		{
//			Vector3 gemsCounter = rvIcon.GetComponent<RectTransform>().localPosition;
//			gemsCounterBanner.GetComponent<RectTransform>().localPosition = new Vector3(gemsCounter.x, topPositionYDefault, gemsCounter.z);
//			
//			Vector3 rvIconRect = rvIcon.GetComponent<RectTransform>().localPosition;
//			rvIcon.GetComponent<RectTransform>().localPosition = new Vector3(rvIconRect.x, bottomPositionYDefault, rvIconRect.z);
//			
		}
		void CheckOpenShopButton()
		{
			openShopButton.SetActive (!CanvasController.GetPopup<UI_BoosterShopPopup> ().isActiveAndEnabled);
		}

		void SetPresenterPosition(Vector3 targetPosition)
		{
			gameObject.GetComponent<RectTransform>().localPosition=targetPosition;
		}

		void SubscribeToRelevantEvents ()
		{
			CurrencyService.CurrencyAmountChanged += OnAmountChanged;
		}
		void UnsubscribeToRelevantEvents ()
		{
			CurrencyService.CurrencyAmountChanged -= OnAmountChanged;
		}
		void OnAmountChanged(CurrencyType currencyType, int currentValue, int changedAmount)
		{
			if (this.currencyType != currencyType) {
				return;
			}

			if (changedAmount > 0) 
			{
				ShowAddedConsumableAnimation (currentValue, changedAmount);
			} 
			else 
			{
				ShowRemovedConsumableAnimation (currentValue, changedAmount);
			}
		}

		void ShowRemovedConsumableAnimation(int currentValue, int changedAmount)
		{
			float yPosGem =gemsIcon.GetComponent<RectTransform>().localPosition.y;
			float xPosGem =gemsIcon.GetComponent<RectTransform>().localPosition.x;
			addedGemsIcon.GetComponent<RectTransform> ().localPosition = new Vector3 (xPosGem,yPosGem, 1);
			addedGemsIcon.SetActive (true);
			LeanTween.delayedCall (0.2f, 
				()=>{
					LeanTween.moveLocalY (addedGemsIcon, yPosGem + 100f, 0.4f).setIgnoreTimeScale(true).setOnComplete(()=>{
						addedGemsIcon.SetActive (false);
					} );
				}

			).setIgnoreTimeScale(true);



			float yPosAmount =consumableAmountText.gameObject.GetComponent<RectTransform>().localPosition.y;
			float xPosAmount =consumableAmountText.gameObject.GetComponent<RectTransform>().localPosition.x;

			addedGemsAmount.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (xPosAmount,yPosAmount, 1);
			addedGemsAmount.gameObject.SetActive (true);
			addedGemsAmount.text = changedAmount.ToString ();
			LeanTween.delayedCall (0.2f, 
				()=>{
					LeanTween.moveLocalY (addedGemsAmount.gameObject, yPosAmount + 100f, 0.4f).setIgnoreTimeScale(true).setOnComplete(()=>{
						addedGemsAmount.gameObject.SetActive (false);
					} );}
			).setIgnoreTimeScale(true);

			UpdateFinalAmount();

		}


		void ShowAddedConsumableAnimation(int currentValue, int changedAmount)
		{
			
//			lastKnownTimeScale = Time.timeScale;
//			Time.timeScale = 1f;
			float yPosGem =gemsIcon.GetComponent<RectTransform>().localPosition.y;
			float xPosGem =gemsIcon.GetComponent<RectTransform>().localPosition.x;
			addedGemsIcon.GetComponent<RectTransform> ().localPosition = new Vector3 (xPosGem,yPosGem - 100f, 1);
			addedGemsIcon.SetActive (true);
			LeanTween.delayedCall (0.2f, 
				()=>{
					LeanTween.moveLocalY (addedGemsIcon, yPosGem, 0.4f).setIgnoreTimeScale(true).setOnComplete(()=>{
						addedGemsIcon.SetActive (false);
					} );
				}
			
			).setIgnoreTimeScale(true);

		

			float yPosAmount =consumableAmountText.gameObject.GetComponent<RectTransform>().localPosition.y;
			float xPosAmount =consumableAmountText.gameObject.GetComponent<RectTransform>().localPosition.x;

			addedGemsAmount.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (xPosAmount,yPosAmount - 100f, 1);
			addedGemsAmount.gameObject.SetActive (true);
			addedGemsAmount.text = "+"+ changedAmount.ToString ();
			LeanTween.delayedCall (0.2f, 
				()=>{
					LeanTween.moveLocalY (addedGemsAmount.gameObject, yPosAmount, 0.4f).setIgnoreTimeScale(true).setOnComplete(()=>{
					UpdateFinalAmount();
					addedGemsAmount.gameObject.SetActive (false);
				} );}
			).setIgnoreTimeScale(true);


		}

		void UpdateFinalAmount()
		{
			LeanTween.scale (consumableAmountText.gameObject, Vector3.one*1.2f, 0.1f).setEaseInOutBounce ().setOnComplete( ()=>{
				LeanTween.scale (consumableAmountText.gameObject, Vector3.one, 0.1f);
			});
			InitializeConsumablePresenter ();
//			Time.timeScale = lastKnownTimeScale;
		}

		void InitializeConsumablePresenter()
		{
			consumableAmountText.text = CurrencyService.Instance.GetCurrentAmount (currencyType).ToString();

			if (!defaultPositionsAvailable)
			{
				topPositionYDefault = gemsCounterBanner.GetComponent<RectTransform>().localPosition.y;
				defaultPositionsAvailable = true;
			}
		}

		public void Show()
		{
			gameObject.SetActive (true);
			InitializeConsumablePresenter ();

			addedGemsAmount.gameObject.SetActive (false);
			addedGemsIcon.SetActive (false);
		}


		public void Hide()
		{
			gameObject.SetActive (false);
			addedGemsAmount.gameObject.SetActive (false);
			addedGemsIcon.SetActive (false);
			
		}


		public void OpenStoreTapped()
		{
			bool isPopUpOnHold = false;
			
			if (currentOpenPopup != CanvasController.Popups.Empty) 
			{
				UI_PopupRelayer.Instance.PutPopupOnHold (currentOpenPopup);
				CanvasController.Instance.location = currentOpenPopup.ToString();
				currentOpenPopup = CanvasController.Popups.Empty;
				isPopUpOnHold = true;
			}
			
			CanvasController.Open (CanvasController.Popups.BoosterShop);
			CanvasController.GetPopup<UI_BoosterShopPopup> ().ShowConsumableProducts ();
		}
	}
}
