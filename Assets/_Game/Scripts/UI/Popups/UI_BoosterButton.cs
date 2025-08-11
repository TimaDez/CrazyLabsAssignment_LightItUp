using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LightItUp;
using UnityEngine.UI;
using TMPro;
using LightItUp.Data;
using LightItUp.Currency;

namespace LightItUp.UI
	{
	public class UI_BoosterButton : MonoBehaviour {


		public static Action<int> UI_BoosterButtonLockedAtLevel;
		public TextMeshProUGUI boosterCharges;
		public BoosterType boosterType;
		public Image boosterFrame;
		public Image boosterIcon;
		public Image statusIcon;

		[HideInInspector]
		public bool isSelected; 
		private bool isActive;
		private bool isForceSelected;
		private int remainingCharges;
		private UI_GameEnded gameEndedPopup;

	

		public void OnButtonSelected()
		{
			ButtonTapped();
		}


		public void ButtonTapped()
		{
			if (isForceSelected) {
				return;
			}

			if (!isActive) {
//				ShowUnlockAtLvlText ();
				return;
			}
			if (remainingCharges == 0) {
				OpenBoosterStore ();
				return;
			}
			if (!isSelected)
            {
                BoosterService.Instance.AddSelectedBooster(boosterType);
                ShowSelectedState ();
			}
            else
            {
                BoosterService.Instance.RemoveSelectedBooster(boosterType);
                ShowDeselectedState ();
			}
		}

		void ShowSelectedState()
		{
			isSelected = true;
			
			
			boosterCharges.gameObject.SetActive (false);
			statusIcon.sprite = SpriteAssets.Instance.boosterSelectedCheckmark;
			statusIcon.gameObject.SetActive (true);
			boosterIcon.color = Color.white;
		}

		void ShowDeselectedState()
		{
			isSelected = false;
			
			statusIcon.gameObject.SetActive (false);
			boosterCharges.text = remainingCharges.ToString();
			boosterCharges.gameObject.SetActive (true);
		}

		public void InitButton(bool isActive, bool isForceSelected, int numberOfRemainingCharges, UI_GameEnded popup=null )
		{
			this.isActive = isActive;
			this.isForceSelected = isForceSelected;
			this.isSelected = isForceSelected;
			this.remainingCharges = numberOfRemainingCharges;
			gameEndedPopup = popup;
			
			if (!isActive) {
				ShowDisabledButtonState ();
				return;
			}
		 	else {
				if (isForceSelected)
                {
                    BoosterService.Instance.AddSelectedBooster(boosterType);
                    ShowSelectedState ();
					return;
				}
                else
                {
					if (numberOfRemainingCharges > 0)
                    {
						ShowEnabledButtonState ();
					}
                    else
                    {
						ShowAddMoreButtonState ();
					}
				}

			}
		}

		void ShowAddMoreButtonState()
		{
			statusIcon.gameObject.SetActive (true);
			statusIcon.sprite = SpriteAssets.Instance.boosterAddMore;
			boosterCharges.gameObject.SetActive(false);
			boosterIcon.color = Color.white;
			boosterIcon.color = Color.gray;
		}
		void ShowDisabledButtonState()
		{
			statusIcon.gameObject.SetActive (true);
			statusIcon.sprite = SpriteAssets.Instance.boosterLock;
			boosterIcon.color = Color.gray;
			boosterCharges.gameObject.SetActive (false);
		}

		void ShowEnabledButtonState()
		{
			statusIcon.gameObject.SetActive(false);
			boosterCharges.gameObject.SetActive (true);
			boosterCharges.text = remainingCharges.ToString();
			boosterIcon.color = Color.white;
		}

		void ShowUnlockAtLvlText()
		{
			UI_BoosterButtonLockedAtLevel.Invoke (BoosterService.Instance.GetBoosterUnlockLevel (boosterType));
		}

		void OpenBoosterStore()
		{
			if (gameEndedPopup != null)
			{
				gameEndedPopup.OpenBoosterStore ();
			}
			else
			{
				CanvasController.Open (CanvasController.Popups.BoosterShop);
			}
		}
	}
}
