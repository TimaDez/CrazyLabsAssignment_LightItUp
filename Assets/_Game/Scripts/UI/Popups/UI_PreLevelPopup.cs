using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using LightItUp.Data;
using HyperCasual.Skins;
using HyperCasual;
using LightItUp.Currency;

namespace LightItUp.UI
{
	public class UI_PreLevelPopup : UI_Popup {
		
		public List<UI_BoosterButton> boosterButtons;
		public GameObject selectLevelNext;
		public GameObject selectLevelPrevious;
		public GameObject progressBar;
		public GameObject starsBar;

		public ProgressUIDisplay progressDisplay;
		public TextMeshProUGUI progressText;
		public TextMeshProUGUI newLevelText;
		public List<GameObject> stars;
		public TextMeshProUGUI levelNumberText;
		public UI_Menu uiMenu;
		[HideInInspector]
		public static Action<int, bool> UI_BoosterPopupClosed;
		[HideInInspector]
		public static Action UI_BoosterPopupMinimized;
		private int currentSelectLevel;

		private string location;
		private string popUpName;
		public enum EndCloseAction
		{
			Home, StartLevel
		}

		private EndCloseAction _action = EndCloseAction.StartLevel;

		protected override void OnEnable()
		{
			base.OnEnable ();
			
			location = CanvasController.Instance.GetLocation();
			popUpName = CanvasController.Instance.GetOpenedPopUpName();
			
			currentSelectLevel = GameData.PlayerData.selectedLevelIdx;
			InitButtons ();
			InitLevelProgress ();
			InitLevelNavigation ();
			RefreshTitle ();
			
			//todo fix bug elseway
		}

//		private void OnDisable()
//		{
//			CanvasController.Instance.SetPreviousPopUp(this.name);
//		}

		public override void OpenAnimationComplete ()
		{
			base.OpenAnimationComplete ();
			CanvasController.AnimationCount = CanvasController.AnimationCount > 1?0:0;
			
			
		}
        void InitLevelNavigation()
        {


            if (currentSelectLevel == GameData.PlayerData.unlockedLevelIdx) 
            {
                selectLevelNext.GetComponent<Button>().interactable = false;
                selectLevelNext.GetComponent<Image>().color = Color.gray;
            } 
            else 
            {

                selectLevelNext.GetComponent<Button>().interactable = true;
                selectLevelNext.GetComponent<Image>().color = Color.white;
			}

			if (currentSelectLevel == 0) {
				selectLevelPrevious.GetComponent<Button> ().interactable = false;
				selectLevelPrevious.GetComponent<Image> ().color = Color.gray;
			} else {
				selectLevelPrevious.GetComponent<Button> ().interactable = true;
				selectLevelPrevious.GetComponent<Image> ().color = Color.white;
			}
		}

		public void SelectLevel(int levelIncrement)
		{
            currentSelectLevel += levelIncrement;

            if(StarGatesManager.Instance.CheckForLevelLock(currentSelectLevel + 1))
            {
                GameManager.Instance.CleanupScene();
                CanvasController.Instance.CloseAllPopups();
                CanvasController.Open(CanvasController.Panels.Levels);
            }
            else
            {
                RefreshTitle();
                InitLevelNavigation();
                InitLevelProgress();
            }

		}

		void InitLevelProgress()
		{
			LightItUp.Data.PlayerData.HighscoreData levelHighscore = GameData.PlayerData.GetLevelHighscore (currentSelectLevel);
//			if () {
//				
//			}
			if (levelHighscore != null) {

				progressBar.SetActive (false);
				starsBar.SetActive (false);
				starsBar.SetActive (true);
				newLevelText.gameObject.SetActive (false);
				for (int i = 0; i < stars.Count; i++) {
					stars [i].SetActive (false);
					stars [i].SetActive (true);
					if (levelHighscore.stars >= i + 1) {
						stars [i].GetComponent<Image> ().sprite = SpriteAssets.Instance.starFilled;
					} else {
						stars [i].GetComponent<Image> ().sprite = SpriteAssets.Instance.starEmpty;
					}
				}
				return;
			}

			if (levelHighscore == null) {
				float currentPercentage = GameData.PlayerData.GetCompletionPercentage (currentSelectLevel + 1);
				if (currentPercentage > 0) {
					starsBar.SetActive (false);
					progressBar.SetActive (true);
					newLevelText.gameObject.SetActive (false);
					progressDisplay.gameObject.SetActive (true);
					progressDisplay.Set (GameData.PlayerData.GetCompletionPercentage (currentSelectLevel + 1), 100);
				} else {

					starsBar.SetActive (false);
					progressBar.SetActive (true);
					progressDisplay.gameObject.SetActive (false);
					newLevelText.gameObject.SetActive (true);
				}
			}

		
		}

		void RefreshTitle()
		{
			levelNumberText.text = "LEVEL " + (currentSelectLevel+1).ToString ();
		}

		void InitButtons()
		{
			for (int i = 0; i < boosterButtons.Count; i++) {
				boosterButtons [i].InitButton (
					BoosterService.Instance.IsBoosterUnlocked (boosterButtons [i].boosterType),
					BoosterService.Instance.GetMandatoryBoosterAmmount (boosterButtons [i].boosterType) > 0,
					BoosterService.Instance.GetTotalBoosterAmmount (boosterButtons [i].boosterType)
				
				);
			}

		}

		void ConsumeSelectedBoosters()
		{
			for (int i = 0; i < boosterButtons.Count; i++) {
				if (boosterButtons [i].isSelected) {
					BoosterService.Instance.ConsumeBooster (boosterButtons [i].boosterType, 1);
				}
			}
		}

		public void OnBackSelected()
		{
			ToggleInteraction(false);
			_action = EndCloseAction.Home;
//			PSDKWrapper.Instance.ShowInterstitialBackToMenu().Done(result =>
//				{
					ClosePopup();    
//				});
		}
		public void OnConfirmLevelStart()
		{
			ToggleInteraction(false);
			_action = EndCloseAction.StartLevel;
			List<BoosterType> selectedBoosters = GetSelectedBoosters ();
			for (int i = 0; i < selectedBoosters.Count; i++) {
				BoosterService.Instance.PurchaseBooster (selectedBoosters[i]);
			}
			ClosePopup();    
		}

		List<BoosterType>  GetSelectedBoosters ()
		{
			List<BoosterType> selectedBoosters = new List<BoosterType> ();
			for (int i = 0; i < boosterButtons.Count; i++) {
				
				if (boosterButtons[i].isSelected) {
					selectedBoosters.Add (boosterButtons [i].boosterType);
				}
			}
			return selectedBoosters;
		}
	
		void ToggleInteraction(bool val)
		{
			
		}
		public override void CloseAnimationComplete()
		{
			base.CloseAnimationComplete();

			string buttonName = "";
			
			GameData.PlayerData.selectedLevelIdx = currentSelectLevel;
			switch (_action)
			{
			case EndCloseAction.StartLevel:     
				//LeanTween.cancelAll();
				//ConsumeSelectedBoosters ();
				//UI_BoosterPopupClosed.Invoke (currentSelectLevel - 1, true); 
				GameManager.Instance.CleanupScene();
				GameManager.Instance.StartSelectedLevel();
				buttonName = "StartLevel";
				break;
			case EndCloseAction.Home:
				LeanTween.cancelAll();
				//UI_BoosterPopupClosed.Invoke (currentSelectLevel-1,false);
				//SendDeltaEvent.MissionAbandoned();
				GameManager.Instance.CleanupScene();

				CanvasController.Open(CanvasController.Panels.Menu);
				StatisticsService.EndRun();
//				if (UI_PopupRelayer.Instance.IsPopupOnHold()) {
//					CanvasController.Open (UI_PopupRelayer.Instance.PullPopupOnHold());
//
//				}
				buttonName = "Home";
				break;
			}

		}

		public void RequestGetMore()
		{
			UI_PopupRelayer.Instance.PutPopupOnHold (CanvasController.Popups.PreLevelPopup);
			CanvasController.Open (CanvasController.Popups.BoosterShop);
			CanvasController.GetPopup<UI_BoosterShopPopup> ().ShowBoosterProducts ();
		}
		public void RequestBoosterStore(string buttonName)
		{
			UI_PopupRelayer.Instance.PutPopupOnHold (CanvasController.Popups.PreLevelPopup);
			CanvasController.Open (CanvasController.Popups.BoosterShop);
		}

	}
}
