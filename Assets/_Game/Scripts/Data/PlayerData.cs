using System.Collections.Generic;
using HyperCasual.Skins;
using Newtonsoft.Json;
using UnityEngine;
using LightItUp.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Data
{
    [System.Serializable]
    public class PlayerData : PlayerDataBase<PlayerData.PlayerDataIntValues, PlayerData>
    {
    
        public void RefreshUnlockedLevel()
        {
            var levelCount = GameLevelAssets.Instance.allLevels.Count;

            int highestHighscoreIndex = GetHighestLevelIndexWithHighscore();

            if (highestHighscoreIndex == -1) // first level
                return;

            int nextUnlockedIndex = Mathf.Min(highestHighscoreIndex + 1, levelCount - 1);
        
            unlockedLevelIdx = nextUnlockedIndex;
            selectedLevelIdx = nextUnlockedIndex;
        
            SaveQueued();
        }

        public int GetHighestLevelIndexWithHighscore()
        {
            var levelCount = GameLevelAssets.Instance.allLevels.Count;

            int highestLevelIndex = -1;
        
            for (int i = 0; i < levelCount; i++)
            {
                var levelHighscore = GetLevelHighscore(i);
            
                if (levelHighscore != null)
                {
                    highestLevelIndex = i;
                }
                else
                {
                    break;
                }            
            }

            return highestLevelIndex;

        }
    
        protected override PlayerData GetDefault()
        {
            return new PlayerData();
        }

        public enum PlayerDataIntValues
        {
            SoundEnabled,
            MusicEnabled,
            VibrateEnabled,
            Game_OrhtographicMax,
            Game_OrhtographicMin,
            Ingame_Points,
            Ingame_JumpCount,
            UnlockedLevelIdx,
            SelectedLevelIdx,
            ShowControlsTutorial,
            ShowLevel10Popup,
            NoAds
        }

        [JsonIgnore]
        public bool noAds
        {
            get
            {
                return GetValue(PlayerDataIntValues.NoAds, 0) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.NoAds, value ? 1 : 0);
            }
        }

        [JsonIgnore]
        public bool soundEnabled
        {
            get
            {
                return GetValue(PlayerDataIntValues.SoundEnabled, 1) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.SoundEnabled, value ? 1 : 0);
            }
        }
        [JsonIgnore]
        public bool musicEnabled
        {
            get
            {
                return GetValue(PlayerDataIntValues.MusicEnabled, 1) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.MusicEnabled, value ? 1 : 0);
            }
        }
        [JsonIgnore]
        public bool vibrateEnabled
        {
            get
            {
                return GetValue(PlayerDataIntValues.VibrateEnabled, 0) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.VibrateEnabled, value ? 1 : 0);
            }
        }
        [JsonIgnore]
        public int ingamePoints
        {
            get
            {
                return GetValue(PlayerDataIntValues.Ingame_Points, 0);
            }
            set
            {
                SetValue(PlayerDataIntValues.Ingame_Points, value);
            }
        }
        [JsonIgnore]
        public int jumpCount
        {
            get
            {
                return GetValue(PlayerDataIntValues.Ingame_JumpCount, 0);
            }
            set
            {
                SetValue(PlayerDataIntValues.Ingame_JumpCount, value);
            }
        }
        [JsonIgnore]
        public int selectedLevelIdx
        {
            get
            {
                return GetValue(PlayerDataIntValues.SelectedLevelIdx, 0);
            }
            set
            {

                // Debug.Log("Selected level: "+ selectedLevelIdx+" -> "+value);
                //Debug.Log("Jumps: " + GameData.PlayerData.jumpCount.ToString());
                var val = Mathf.Clamp(value, 0, GameLevelAssets.Instance.allLevels.Count-1);
                SetValue(PlayerDataIntValues.SelectedLevelIdx, val);
                if (val >= unlockedLevelIdx)
                {
                    unlockedLevelIdx = val;
                }
            }
        }
        [JsonIgnore]
        public int unlockedLevelIdx
        {
            get
            {
                return GetValue(PlayerDataIntValues.UnlockedLevelIdx, selectedLevelIdx);
            }
            set
            {

                // Debug.Log("unlockedLevelIdx: " + unlockedLevelIdx + " -> " + value);
				if (GameLevelAssets.Instance.allLevels != null) {
					if (GameLevelAssets.Instance.allLevels.Count > 1) {
						var val = Mathf.Clamp(value, 0, GameLevelAssets.Instance.allLevels.Count-1);
						SetValue(PlayerDataIntValues.UnlockedLevelIdx, val);
					}
				}
            }
        }
        [JsonIgnore]
        public bool showControlsTutorial
        {
            get
            {
                return GetValue(PlayerDataIntValues.ShowControlsTutorial, 1) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.ShowControlsTutorial, value ? 1 : 0);
            }
        }
        [JsonIgnore]
        public bool showLevel10Popup
        {
            get
            {
                return GetValue(PlayerDataIntValues.ShowLevel10Popup, 1) == 1;
            }
            set
            {
                SetValue(PlayerDataIntValues.ShowLevel10Popup, value ? 1 : 0);
            }
        }
        public class HighscoreData {
            public int score;
            public int stars;
        }

		public class BoosterInfo {
			public int mandatoryCharges;
			public int regularCharges;
			public bool isUnlocked;
			public bool isPurchased;
		}

        [JsonIgnore]
        public int TotalStarsAllLevels
        {
            get
            {
                var highscores = LevelHighscores;

                int total = 0;
            
                foreach (var keyValuePair in highscores)
                {
                    total += keyValuePair.Value.stars;
                }

                return total;

            }        
        }
    
    

        [JsonIgnore]
        public Dictionary<int, HighscoreData> LevelHighscores {
            get {
                if (levelHighscores == null)
                {
                    levelHighscores = new Dictionary<int, HighscoreData>();
                }
                return levelHighscores;
            }
        }

		[JsonIgnore]
		public Dictionary<int, int> CompletionPercentages {
			get {
				if (completionPercentages == null)
				{
					completionPercentages = new Dictionary<int, int>();
				}
				return completionPercentages;
			}
		}
		[JsonIgnore]
		public Dictionary<BoosterType, BoosterInfo> BoosterData2 {
			get {
				if (boosterScores == null)
				{
					boosterScores = new Dictionary<BoosterType, BoosterInfo>();
				}
				return boosterScores;
			}
		}


        [JsonProperty]
		Dictionary<int, HighscoreData> levelHighscores;

		[JsonProperty]
		Dictionary<int, int> completionPercentages;

		[JsonProperty]
		Dictionary<BoosterType, BoosterInfo> boosterScores;

        [JsonIgnore]
        public Dictionary<string, bool> Skins {
            get {
                if (skins == null)
                {
                    skins = new Dictionary<string, bool>();
                }
                return skins;
            }
        }
        
        [JsonProperty] 
		Dictionary<string, bool> skins;


        public float game_OrthographicMax = 15;
        public float game_OrthographicMin = 25;
        public float game_OrthographicZoomSpeed = 4f;
        public float game_OrthographicZoomChangeDirectionSpeed = 20f;
        public float game_DampeningX = 0.5f;
        public float game_DampeningY = 0.5f;
        public bool autoZoomToShow = true;
    
        public bool wonLastGame = false;
        public bool useStraightJumping = false;
        public bool usePlayerCenterJumping = false;
        public bool useJumpThroughWalls = false;

        [JsonIgnore]
        public int starsCollectedInLevel;
        [JsonIgnore]
        public Vector3 revivePosition;

        // more player data here!
		public bool IsBoosterUnlocked(BoosterType boosterType){
			if (BoosterData2.ContainsKey(boosterType)) {
				return BoosterData2 [boosterType].isUnlocked;
			}
			return false;
		}

		public bool IsBoosterAllowed(BoosterType boosterType){
			if (BoosterData2.ContainsKey(boosterType)) {
				return BoosterData2 [boosterType].isUnlocked || BoosterData2 [boosterType].isPurchased;
			}
			return false;
		}

		public bool IsBoosterPurchased(BoosterType boosterType){
			if (BoosterData2.ContainsKey(boosterType)) {
				return BoosterData2 [boosterType].isPurchased;
			}
			return false;
		}

		public void SetBoosterCharges(BoosterType booster, int val)
		{
			if (BoosterData2.ContainsKey(booster)) {
				BoosterData2 [booster].regularCharges = val;
				QueueSaveData();
				SaveQueued();
			}

		}

		public void AddMandatoryBoosterCharges(BoosterType booster, int val)
		{
			if (BoosterData2.ContainsKey (booster)) {
				BoosterData2 [booster].mandatoryCharges += val;
				QueueSaveData ();
				SaveQueued ();
			} else {

				BoosterData2 [booster] = new BoosterInfo ();
				BoosterData2 [booster].isUnlocked = false;
				BoosterData2 [booster].regularCharges = 0;
				BoosterData2 [booster].mandatoryCharges = val;
				QueueSaveData ();
				SaveQueued ();
			}

		}

		public void AddRegularBoosterCharges(BoosterType booster, int val)
		{
			if (BoosterData2.ContainsKey(booster)) {
				BoosterData2 [booster].regularCharges += val;
				QueueSaveData();
				SaveQueued();
			}

		}

		public void SetBoosterPurchase(BoosterType booster, bool val)
		{
			if (BoosterData2.ContainsKey (booster)) {
				BoosterData2 [booster].isPurchased = val;
			} else {
				BoosterData2 [booster] = new BoosterInfo ();
				BoosterData2 [booster].isUnlocked = false;
				BoosterData2 [booster].isPurchased = val;
				BoosterData2 [booster].regularCharges = 0;
				BoosterData2 [booster].mandatoryCharges = 0;
			}

			QueueSaveData();
			SaveQueued();
		}

		public void SetBoosterUnlock(BoosterType booster, bool val)
		{
			if (BoosterData2.ContainsKey (booster)) {
				BoosterData2 [booster].isUnlocked = val;
			} else {
				BoosterData2 [booster] = new BoosterInfo ();
				BoosterData2 [booster].isUnlocked = val;
				BoosterData2 [booster].isPurchased = false;
				BoosterData2 [booster].regularCharges = 0;
				BoosterData2 [booster].mandatoryCharges = 0;
			}

			QueueSaveData();
			SaveQueued();
		}

		public int GetTotalBoosterAmmount(BoosterType booster)
		{
			if (BoosterData2.ContainsKey (booster)) {
				return BoosterData2 [booster].regularCharges + BoosterData2 [booster].mandatoryCharges;
			}
			return 0;
		}
		public int GetMandatoryBoosterAmmount(BoosterType booster)
		{
			if (BoosterData2.ContainsKey (booster)) {
				return BoosterData2 [booster].mandatoryCharges;
			}
			return 0;
		}

		public int GetRegularBoosterAmmount(BoosterType booster)
		{
			if (BoosterData2.ContainsKey (booster)) {
				return BoosterData2 [booster].regularCharges;
			}
			return 0;
		}

		public bool ConsumeBooster(BoosterType booster, int ammount = 1)
		{
			if (IsBoosterUnlocked(booster) || IsBoosterPurchased(booster)) {
				if (BoosterData2.ContainsKey(booster) && BoosterData2[booster].regularCharges+BoosterData2[booster].mandatoryCharges >= ammount) {
					if (BoosterData2 [booster].mandatoryCharges >= ammount) {
						BoosterData2 [booster].mandatoryCharges -= ammount;
					} else {
						BoosterData2 [booster].regularCharges -= ammount;
					}
					QueueSaveData();
					SaveQueued();
					return true;
				}
			}
			return false;

		}
        public int GetHighestHighscore() {
            int i = 0;
            foreach (var p in LevelHighscores)
            {

                i = Mathf.Max(p.Value.score, i);
            }
            return i;
        }
        public HighscoreData GetLevelHighscore(int level) {
            if (LevelHighscores.ContainsKey(level))
            {
                return LevelHighscores[level];
            }
            return null;
        }
		public int GetCompletionPercentage(int level) {
			if (CompletionPercentages.ContainsKey(level))
			{
				return CompletionPercentages[level];
			}
			return 0;
		}

		public void SetCompletionPercentage(int completionPercentage, int level)
		{
			if (CompletionPercentages.ContainsKey(level))
			{
				if (CompletionPercentages[level] < completionPercentage)
				{
					CompletionPercentages[level] = completionPercentage;
				}
			} 
			else
			{
				CompletionPercentages.Add (level, completionPercentage);
			}
			QueueSaveData();
		}

        public void SetHighscore(int level, int score, int stars, out bool newScore, out bool newStars)
        {
            newScore = false;
            newStars = false;
            if (LevelHighscores.ContainsKey(level))
            {
                if (LevelHighscores[level].score < score)
                {
                    LevelHighscores[level].score = score;
                    newScore = true;
                }
                if (LevelHighscores[level].stars < stars)
                {
                    LevelHighscores[level].stars = stars;
                    newStars = true;
                }
            }
            else
            {
                LevelHighscores.Add(level, new HighscoreData
                {
                    score = score,
                    stars = stars
                });
                newScore = true;
                newStars = true;
            }
            QueueSaveData();
        }

        public void UnlockAllLevels()
        {
            var levelCount = GameLevelAssets.Instance.allLevels.Count;

            bool newScore, newStars;

            for (int i = 0; i < levelCount; i++)
            {
                SetHighscore(i, 10, 1, out newScore, out newStars);
            }

            selectedLevelIdx = levelCount - 1;
            unlockedLevelIdx = levelCount - 1;

            SaveQueued();
        }
    }
}
