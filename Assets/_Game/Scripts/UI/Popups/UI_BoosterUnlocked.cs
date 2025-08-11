using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.UI;
using UnityEngine.UI;
using TMPro;
using System;
using LightItUp.Data;

namespace LightItUp.UI
{
	public class UI_BoosterUnlocked : UI_Popup {


		public TextMeshProUGUI boosterNameText;
		public Image boosterImage;
		public Button tryItNowButton;
		public Button backHomeButton;
		public Image boosterSmallIcon;
		public TextMeshProUGUI boosterDescription;
		private BoosterType boosterType;

		private Action OnBoosterUnlockClosed = () => { };
		
		public enum EndCloseAction
		{
			Home, TryItNow
		}

		private EndCloseAction _action = EndCloseAction.Home;

		[HideInInspector]
		public static Action <BoosterType,bool> UI_BoosterUnlockedPopupClosed;

		protected override void OnEnable()
		{
			base.OnEnable();
			ToggleInteraction(true);
			
			
		}
		void Start()
		{
			Debug.Log ("Start");
		}
        protected override bool HandleBackPressed()
        {
            OnBackSelected();
            return true;
        }
        public void InitBoosterUnlockedPopup (BoosterType boosterType, Action StartLevel)
		{
			this.boosterType = boosterType;
			boosterNameText.text = BoosterService.Instance.GetBoosterNameByType(boosterType);
			boosterImage.sprite = GetSpriteByType ();
			boosterSmallIcon.sprite = GetSmallSpriteByType ();
			boosterDescription.text = BoosterService.Instance.GetBoosterDescriptionByType(boosterType);
			OnBoosterUnlockClosed += StartLevel;
		}

		Sprite GetSmallSpriteByType ()
		{
			switch (this.boosterType) {
			case BoosterType.MagicHat:
				return SpriteAssets.Instance.boosterIconSmallAura;
			case BoosterType.MetalBoots:
				return SpriteAssets.Instance.boosterIconSmallBoots;
			case BoosterType.SpringShoes:
				return SpriteAssets.Instance.boosterIconSmallShoes;
			default:
				return null;
			}
		}

		Sprite GetSpriteByType ()
		{
			switch (this.boosterType) {
			case BoosterType.MagicHat:
				return SpriteAssets.Instance.boosterIconBigAura;
			case BoosterType.MetalBoots:
				return SpriteAssets.Instance.boosterIconBigBoots;
			case BoosterType.SpringShoes:
				return SpriteAssets.Instance.boosterIconBigShoes;
			default:
				return null;
			}
		}

		public void OnBackSelected()
		{
			ToggleInteraction(false);
			
			_action = EndCloseAction.Home;
            
            ClosePopup(() =>
            {
	            GameManager.Instance.CleanupScene();
	            CanvasController.Open(CanvasController.Panels.Menu);
            });
			

		}

		public void OnTryItNowSelected()
		{
            ToggleInteraction(false);
			_action = EndCloseAction.TryItNow;
			ClosePopup(() =>
			{
				
				OnBoosterUnlockClosed();
				OnBoosterUnlockClosed = null;
			});
		}

		void ToggleInteraction(bool toggle)
		{
			tryItNowButton.interactable = toggle;
			backHomeButton.interactable = toggle;
		}

		public override void OpenAnimationComplete ()
		{
			base.OpenAnimationComplete ();
			CanvasController.AnimationCount = CanvasController.AnimationCount > 1?0:0;
			GameManager.Instance.CleanupScene();
			BoosterUnlockSetup();
		}

		public override void CloseAnimationComplete()
		{

			base.CloseAnimationComplete();

			

//			if (_action == EndCloseAction.TryItNow)
//			{
//				OnBoosterUnlockClosed();
//				OnBoosterUnlockClosed = null;
//			}

		}

		private void BoosterUnlockSetup()
		{
			string itemSpent = "play";
			int amountSpent = 0;

//            LeanTween.cancelAll();
			BoosterService.Instance.UnlockBooster (boosterType);
			
			if (!GameData.PlayerData.IsBoosterPurchased(boosterType))
			{
				BoosterService.Instance.AddMandatoryBoosterCharges (boosterType,1);
			}
			else
			{
				BoosterService.Instance.AddRegularBoosterCharges(boosterType, 1);
			}
			BoosterService.Instance.AddRegularBoosterCharges (boosterType,BoosterService.Instance.boosterConfig.initialBoosterUnlockCharges-1);
			CanvasController.Instance.location = "tryBooster";
		}
	}
	
	
}
