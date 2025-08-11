using System;
using System.Collections.Generic;
using HyperCasual;
using HyperCasual.PsdkSupport;
using HyperCasual.Skins;
using UnityEngine;
using LightItUp.Currency;
using LightItUp.Data;
using LightItUp.Game;
using LightItUp.Sound;
using UnityEngine.UI;

namespace LightItUp.UI
{
    public class UI_GameEnded : UI_Popup
    {

        [SerializeField] private UI_GemsEarned gemsEarned;
        
        public TMPro.TextMeshProUGUI winText;
        public GameObject anyLevelWinPanel;
        public GameObject lastLevelWinPanel;
        public SkinUnlockedButton newSkinsButton;
		public UI_BoosterRVOffer boosterOfferButton;
        public List<Animator> anims = new List<Animator>();
        public List<GameObject> wonElements, lostElements;

        public ProgressUIDisplay progressDisplay;

		private bool isFirstTimeOpen = true;

		public void SetFirstTimeOpen(bool value)
		{
			isFirstTimeOpen = value;
		}

        private int menuDisplayCount
        {
            get { return PlayerPrefs.GetInt("GameEndedDisplayCount", 0); }
            set
            {
                PlayerPrefs.SetInt("GameEndedDisplayCount", value);
                PlayerPrefs.Save();
            }
        }
        
        int nextDisplayOfNative;
        
        int levelIdx;

		private void OnDisable()
		{
			CanvasController.TogglePresenter (false,3, false);
		}


        protected override void OnEnable()
        {
            base.OnEnable();

            buttonLock = false;
            
            gemsEarned.gameObject.SetActive(false);
            
            menuDisplayCount++;
    
            Time.timeScale = 0;
            
            foreach (var wonElement in wonElements)
            {
                wonElement.SetActive(GameData.PlayerData.wonLastGame);
            }
            
            foreach (var lostElement in lostElements)
            {
                lostElement.SetActive(!GameData.PlayerData.wonLastGame);
            }
            
            winText.text = GameData.PlayerData.wonLastGame ? string.Format("LEVEL {0} COMPLETE", GameManager.Instance.currentLevel.levelIdx) : "GAME OVER";
            levelIdx = GameDataBase.PlayerData.selectedLevelIdx;
            bool isLastLevel = levelIdx == GameLevelAssets.Instance.allLevels.Count - 1;

            SetupPlayButton(isLastLevel);

            if (GameData.PlayerData.wonLastGame)
            {
                SoundManager.PlaySound("WinnerJingle");

				if (isFirstTimeOpen) {

					bool newScore, newStar;
					GameData.PlayerData.SetHighscore(levelIdx, GameData.PlayerData.ingamePoints, GameData.PlayerData.starsCollectedInLevel, out newScore, out newStar);
					StatisticsService.SetStat(GameStats.complete_level, levelIdx+1);

					gemsEarned.ShowGemsEarned(GameData.PlayerData.starsCollectedInLevel);
					
					if (GameData.PlayerData.starsCollectedInLevel == 3 && newStar)
					{                    
						StatisticsService.CountStat(GameStats.complete_levels_max_stars, 1);
					}

					GameDataBase.PlayerData.selectedLevelIdx++;
					GameDataBase.PlayerData.SaveQueued();            
				}                 
            }
            else
            {
				
                progressDisplay.Set(GameManager.Instance.currentLevel.LitBlockCount, GameManager.Instance.currentLevel.blocks.Count);
				GameData.PlayerData.ingamePoints = 0;
                GameData.PlayerData.jumpCount = int.MaxValue;
            }
            
            StatisticsService.EndRun();
            
            var newSkins = SkinsState.CheckConditionsForNewSkins();
            
            if (newSkins != null && newSkins.Count > 0)
            {
                newSkinsButton.Show(newSkins[0]);
				boosterOfferButton.HideButton ();

            }
            else
            {
                newSkinsButton.Hide();
				if (BoosterService.Instance.AreBoostersActive () && !GameData.PlayerData.wonLastGame) {
					boosterOfferButton.ShowButton ();
				} else {
					boosterOfferButton.HideButton ();
				}

			}   
            CanvasController.TogglePresenter (true, 3, true,CanvasController.Popups.GameEnded);
        }

        private void SetupPlayButton(bool isLastLevel)
        {
            if (isLastLevel)
            {
                anyLevelWinPanel.GetComponent<Button>().interactable = false;
            }
            else
            {
                anyLevelWinPanel.GetComponent<Button>().interactable = true;
            }
                        
        }

         public override void OpenAnimationComplete()
         {
             base.OpenAnimationComplete();
             CanvasController.Instance.CloseAllPanels();                
         }
         
        public void GoBack()
        {
            ClosePopup(() =>
            {
                GameManager.Instance.CleanupScene();
                CanvasController.Open(CanvasController.Panels.Menu);
            });
        }

        public void OpenSkinsMenu()
        {
            ClosePopup(() =>
            {
                GameManager.Instance.CleanupScene();
                CanvasController.Open(CanvasController.Panels.Skins);
            });
        }

        public void OpenLevelsMenu()
        {
            ClosePopup(() =>
            {
                GameManager.Instance.CleanupScene();
                CanvasController.Open(CanvasController.Panels.Levels);
            });
        }

        public void OpenSettings()
        {
            ClosePopup(() =>
            {
                GameManager.Instance.CleanupScene();
                CanvasController.Open(CanvasController.Popups.Settings);
            });
        }

        public void Retry()
        {
            if (GameData.PlayerData.wonLastGame)
            {
                if (levelIdx < GameLevelAssets.Instance.allLevels.Count - 1)
                {
                    CanvasController.Instance.location = "replay";
                    GameDataBase.PlayerData.selectedLevelIdx--;
                }
            }
            else
            {
                CanvasController.Instance.location = "retry";
            }
            GameManager.Instance.isReplay = true;

            ClosePopup(() =>
            {
                Time.timeScale = 1;
                //UI_PopupRelayer.Instance.PutPopupOnHold(CanvasController.Popups.GameEnded);
                BoosterService.Instance.ConsumeSelectedBoosters();
                GameManager.Instance.StartSelectedLevel();
            });
        }

        protected override bool HandleBackPressed()
        {
            GoBack();
            return true;
        }

        private bool buttonLock = false;
        
        public void GoNext()
        {
            if (buttonLock) return;
            
            buttonLock = true;
            
            foreach (Animator a in anims)
            {
                if (a != null)
                    a.Play("ScaleOut");
            }
            if (GameData.PlayerData.wonLastGame)
            {
                ClosePopup(() =>
                {
                    CanvasController.Instance.location = "nextLevel";
                    GameManager.Instance.StartGame();
                        
                });
            }
            else
            {
                Retry();
            }

            
        }
    
        
        public void OpenBoosterStore()
        {
            UI_PopupRelayer.Instance.PutPopupOnHold(CanvasController.Popups.GameEnded);
            CanvasController.Open (CanvasController.Popups.BoosterShop);
        }
    }
}
