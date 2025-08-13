using System;
using _Game.Scripts.SeekingMissiles;
using HyperCasual;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Sound;
using LightItUp.UI;
using UnityEngine.Serialization;

namespace LightItUp.Data
{
    public class GameManager : GameManagerBase<GameManager>
    {
        public bool debugDisableGates = false;
        
        public GameLevel currentLevel;
        public Action playerStart;
        
        public static int currentLevelIndex;
        public static bool IsApplicationQuitting;
        public bool isReplay;
        
        [SerializeField] private bool _activeSeekingMissilesFeature = true;
        
        public Action OnAutoScroll = () => { };
        private bool _paused;    
        private bool hasPlayedLastBlockEffect;
        private SeekingMissilesController _seekingMissilesController = null;

        private void OnApplicationQuit()
        {
            IsApplicationQuitting = true;
        }

        public override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;

            ObjectPool.Instance.Init();

            CanvasController.Instance.loadingView.SetActive(true);
            OnGameReady();
        }

        bool CheckForTestLevel()
        {
            var l = FindObjectOfType<GameLevel>();
            if (l != null)
            {
#if UNITY_EDITOR
                if (l.saveLevelOnPlay)
                {
                    l.Save();
                    int lIdx = l.levelIdx;
                    Destroy(l.gameObject);
                    StartLevel(lIdx - 1);
                }
                else
#endif
                {
                    currentLevel = l;
                    l.LoadPlayer();
                    l.FinalizeStartLevel();
                }
            }

            return l != null;
        }


        private void OnGameReady()
        {
            CanvasController.Instance.loadingView.SetActive(false);
            
            RefreshAllLevels();

            if (GameData.PlayerData.unlockedLevelIdx > 0)
            {
                StatisticsService.SetStat(GameStats.complete_level, GameData.PlayerData.unlockedLevelIdx);
            }

            CanvasController.AnimationCount = 0;

            bool usingTestLevel = CheckForTestLevel();

            if (usingTestLevel)
                return;

            SoundManager.PlayMusic(MusicNames.BGMusic);
            CanvasController.Open(CanvasController.Panels.Menu);
            
        }

        public void RefreshAllLevels()
        {
            GameLevelAssets.LevelsFolder = LevelOptimizationConfig2.LevelsFolder;
            GameLevelAssets.Instance.UpdateAllLevels();

            var playerData = GameData.PlayerData;

            playerData.RefreshUnlockedLevel();
        }


        protected override void LateUpdate()
        {
            var p = CanvasController.GetPopup<UI_GameEnded>();
            if (currentLevel == null || p.gameObject.activeSelf) {
                base.LateUpdatePopUpCheck(p);
            }
        }

        

        public void StartGame(bool isMainMenuCall = false)
        {
            int nextLevel = GameData.PlayerData.selectedLevelIdx;
            
            Debug.Log($"[GameManager] StartGame() nextLevel: {nextLevel}");
            if (isMainMenuCall)
            {
                nextLevel = GameData.PlayerData.unlockedLevelIdx;
            }
            
            if (!StarGatesManager.Instance.CheckForLevelLock(nextLevel + 1))
            {
                if (BoosterService.Instance.ShouldShowBoosterUnlockedPopup(nextLevel + 1))
                {
                    CanvasController.Open(CanvasController.Popups.BoosterUnlocked);
                    var popup = CanvasController.GetPopup<UI_BoosterUnlocked>();

                    BoosterType boosterType = BoosterService.Instance.GetCurrentUnlockedBooster(nextLevel + 1);
                    popup.InitBoosterUnlockedPopup(boosterType, OnBoosterUnlocked);
                }
                else
                {
                    BoosterService.Instance.ConsumeSelectedBoosters();
                    StartLevel(nextLevel);
                }
            }
            else
            {
                if(!isMainMenuCall)
                {
                    CleanupScene();
                    CanvasController.Open(CanvasController.Panels.Menu);
                }
                else
                {
                    OnAutoScroll(); // Event scrolls levels
                }
            }
        }
        public void OnBoosterUnlocked()
        {
            BoosterService.Instance.ConsumeSelectedBoosters();
            StartLevel(GameData.PlayerData.unlockedLevelIdx);
        }
        
        public void OpenPreGamePopup()
        {
            if (BoosterService.Instance.AreBoostersActive()) 
            {
                if (BoosterService.Instance.ShouldShowBoosterUnlockedPopup(GameData.PlayerData.selectedLevelIdx)) 
                {
                    CanvasController.Open(CanvasController.Popups.BoosterUnlocked);
                 } 
                else 
                {
                    OpenPreLevelPopup();
                }

            } else 
            {

                StartSelectedLevel();
            }
        }
        
        public void StartSelectedLevel()
        {
            StartLevel(GameData.PlayerData.selectedLevelIdx);

        }


        public void OpenPreLevelPopup()
        {
            int selectedLevel = GameData.PlayerData.selectedLevelIdx;

            if (StarGatesManager.Instance.CheckForLevelLock(selectedLevel + 1))
            {
                CleanupScene();
                CanvasController.Instance.CloseAllPopups();
                CanvasController.Open(CanvasController.Panels.Levels);
            }
            else
            {
                CanvasController.Open (CanvasController.Popups.PreLevelPopup);
            }
        }
        
        public void RevivePlayer()
        {                  
            currentLevel.RevivePlayer();
            WinConditionChecker.Revive();
        }

        
    
		public void StartDifferentLevel(int otherLevelIndex)
		{
			StartLevel (otherLevelIndex, true);
		}
		public void StartLevel(int levelIdx, bool isRestartingThroughBoosterSelection = false)
        {
            Debug.Log($"[GameManager] StartLevel() levelIdx: {levelIdx}");
            StatisticsService.StartRun();
            StatisticsService.SetStat(GameStats.play_level, levelIdx+1);
            currentLevelIndex = levelIdx;
            hasPlayedLastBlockEffect = false;
            CleanupScene();
            var gameLevel = Instantiate(PrefabAssets.Instance.gameLevelClean);
            currentLevel = gameLevel;
            currentLevel.OnLevelLoaded += OnLevelLoaded;
            gameLevel.Load(levelIdx + 1);
            CanvasController.Open(CanvasController.Panels.Game);
            CanvasController.GetPanel<UI_Game>().SetSeekingMissilesActive(_activeSeekingMissilesFeature);
            
            GameData.PlayerData.selectedLevelIdx = levelIdx;
            var cpfx = FindObjectsOfType<CelebrationFX>();
            foreach (CelebrationFX c in cpfx)
            {
                ObjectPool.ReturnCelebrationFX(c);
            }        
     
            bool areBoostersAB = BoosterService.Instance.AreBoostersActive ();
			if (!areBoostersAB) {
				currentLevel.ConfirmGameLoadFinalized ();
				return;
			}

			if (isRestartingThroughBoosterSelection) {
				currentLevel.ConfirmGameLoadFinalized ();
				return;
			}

            if (UI_PopupRelayer.Instance.GetPopUpsCount()>0)
            {
                UI_PopupRelayer.Instance.RemovePreviousHeldPopup();
            }
            
            currentLevel.ConfirmGameLoadFinalized ();
        }

        private void OnLevelLoaded()
        {
            currentLevel.OnLevelLoaded -= OnLevelLoaded;

            CreateSeekingMissilesController();
        }
        
        private void CreateSeekingMissilesController()
        {
            Debug.Log($"[GameManager] CreateSeekingMissiles()");
            if(currentLevel == null || !_activeSeekingMissilesFeature )
            {
                return;
            }

            if (_seekingMissilesController != null)
            {
                _seekingMissilesController.Init(currentLevel.player, currentLevel.blocks);
                return;
            }
            
            _seekingMissilesController = Instantiate(PrefabAssets.Instance.SeekingMissilesControllerPrefab);
            if (_seekingMissilesController == null)
            {
                Debug.LogWarning("[GameLevel] InitSeekingMissiles() SeekingMissilesController is not initialized.");
                return;
            }
			
            _seekingMissilesController.Init(currentLevel.player, currentLevel.blocks);
        }
        
        void ShowBoostersPurchaseFeature()
		{
			
            CanvasController.Open (CanvasController.Popups.PreLevelPopup);
			UI_PreLevelPopup.UI_BoosterPopupClosed += OnBoosterPopupClosed;
			UI_PreLevelPopup.UI_BoosterPopupMinimized += OnBoosterPopupMinimized;
		
		}

		void OnBoosterPopupMinimized()
		{
			CanvasController.Open (CanvasController.Popups.BoosterShop);
			UI_PreLevelPopup.UI_BoosterPopupMinimized -= OnBoosterPopupMinimized;
			UI_PreLevelPopup.UI_BoosterPopupClosed -= OnBoosterPopupClosed;
			UI_BoosterShopPopup.UI_BoosterShopPopupClosed += OnBoosterShopClosed;
		}

		void OnBoosterShopClosed()
		{
			UI_BoosterShopPopup.UI_BoosterShopPopupClosed -= OnBoosterShopClosed;
			ShowBoostersPurchaseFeature ();
		}

		void OnBoosterPopupClosed(int selectedLevel, bool isSuccesfull)
		{
			UI_PreLevelPopup.UI_BoosterPopupClosed -= OnBoosterPopupClosed;
			UI_BoosterShopPopup.UI_BoosterShopPopupClosed -= OnBoosterShopClosed;
			UI_PreLevelPopup.UI_BoosterPopupMinimized -= OnBoosterPopupMinimized;

			if (currentLevel != null && currentLevelIndex == selectedLevel) {

				currentLevel.ConfirmGameLoadFinalized ();
			} else {

				//SendDeltaEvent.MissionAbandoned();
				StartDifferentLevel (selectedLevel);
			}
		}

        public void CleanupScene()
        {
            GameData.PlayerData.wonLastGame = false;
            WinConditionChecker.Reset();
            if (currentLevel != null)
            {
                currentLevel.CleanTutorials();
                Destroy(currentLevel.gameObject);
            }
            Resources.UnloadUnusedAssets();
			CinemachineController.Instance.Deactivate ();

        }
        public bool IsLastBlock()
        {
            if (hasPlayedLastBlockEffect) return false;
            var unlitBlock = currentLevel.blocks.Find(x => {
                bool hasUnlitPart = false;
                if (x.explodeController != null) {
                    var unlitPart = x.explodeController.parts.Find(y => !y.IsLit);
                    if (unlitPart != null) hasUnlitPart = true;
                }
                return !x.IsLit || hasUnlitPart;
            });
            if (unlitBlock == null)
            {
                hasPlayedLastBlockEffect = true;
            }
            return unlitBlock == null;

        }

        public void OpenRevive()
        {
            bool canRevive = currentLevel.player.CanRevive(out GameData.PlayerData.revivePosition, ReviveConfig.reviveType);
        
            // check for revive option            
            if (canRevive)
            {
                CanvasController.Open(CanvasController.Popups.ReviveOffer);
            }
            else
            {
                OpenGameEnded(false);
            }
        }

    

        public void OpenGameEnded(bool didWin)
        {
            if (UI_PopupRelayer.Instance.GetPopUpOnHoldName() == CanvasController.Popups.GameEnded.ToString())
            {
                UI_PopupRelayer.Instance.OpenOnHoldPopup();
            }
            else
            {
                CanvasController.GetPopup<UI_GameEnded> ().SetFirstTimeOpen(true);
                CanvasController.Open(CanvasController.Popups.GameEnded);
            }
            
            
        }
    
        
    
        public void TogglePause(bool b)
        {
            if (b == _paused)
                return;
        
            _paused = b;

            Time.timeScale = _paused ? 0 : 1;
        }

        private void OnDestroy()
        {
            currentLevel.OnLevelLoaded -= OnLevelLoaded;
        }
    }
    
}