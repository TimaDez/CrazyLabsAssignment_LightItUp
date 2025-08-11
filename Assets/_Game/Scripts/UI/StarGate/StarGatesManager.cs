using System;
using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using LightItUp.Singletons;
using LightItUp.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Game;
using LightItUp.Currency;

namespace LightItUp
{
    public class StarGatesManager : SingletonCreate<StarGatesManager>
    {

        private static string STATUS_OPEN = "open";
        private static string STATUS_CLOSED = "closed";
        private static string STATUS_ALREADY_OPEN = "alreadyOpen";

        public const string LEVEL_UNLOCK_KEY = "star_gate_";

        public bool disableGatesAB 
		{
			get
			{
				return GameManager.Instance.debugDisableGates;
			}
				
		}

        public List<StarGateInfo> starGatesInfo;



		public List<StarGateInfo> GetLockedStargates()
		{
			List<StarGateInfo> sgInfos = new List<StarGateInfo> ();

            if (disableGatesAB) return sgInfos;

            int currentLevel = GameData.PlayerData.unlockedLevelIdx;

            foreach (StarGateInfo sg in starGatesInfo)
			{
				if (!IsSGUnlocked(sg) && !IsSGEarned(sg))
                {
                    if(currentLevel >= sg.levelLock)
                    {
                        SaveUnlockedGate(sg);
                    }
					else sgInfos.Add (sg);
				}
			}
			return sgInfos;
		}
        

        public List<StarGateInfo> GetUnlockedStargates()
		{
			List<StarGateInfo> sgInfos = new List<StarGateInfo> ();

            if (disableGatesAB) return sgInfos;

            foreach (StarGateInfo sg in starGatesInfo)
			{
				if (!IsSGUnlocked(sg) && IsSGEarned(sg))
                {
                    if(sg.levelLock > GameData.PlayerData.unlockedLevelIdx)
                    {
                        sgInfos.Add(sg);
                    }
					
				}
			}
			return sgInfos;
		}

        /// <summary>
        /// Is star gate available for unlock
        /// </summary>
        /// <param name="sg"> ScriptableObject with Info for given stargate </param>
        /// <returns>true: Gate is unlockable with stars ( no subscription needed ) </returns>
		bool IsSGEarned(StarGateInfo sg)
		{
			if (GameData.PlayerData.TotalStarsAllLevels >= sg.starsRequired)
            {
				return true;
			}
            else
            {
				return false;
			}
		}

        public StarGateInfo FindStarGateInfoByLevel(int level)
        {
            foreach(StarGateInfo sg in starGatesInfo)
            {
                if (sg.levelLock == level) return sg;
            }
            return null;
        }

        public bool IsStarGateActive(int level)
        {
	        foreach(StarGateInfo sg in starGatesInfo)
	        {
		        if (sg.levelLock == level) return true;
	        }
	        return false;
        }

        public bool CheckForLevelLock(int level)
        {
            if (disableGatesAB) return false;

            foreach (StarGateInfo sg in starGatesInfo)
            {
                // If Gate exists and is not unlocked
				if (sg.levelLock == level && level > GameData.PlayerData.unlockedLevelIdx)
                {
	                string closeKey = LEVEL_UNLOCK_KEY + sg.levelLock + STATUS_CLOSED;
	                string alreadyOpenKey = LEVEL_UNLOCK_KEY + sg.levelLock + STATUS_ALREADY_OPEN;
	                string openKey = LEVEL_UNLOCK_KEY + sg.levelLock + STATUS_OPEN;
	                
	                // Player reached star gate
	                if (level == GameData.PlayerData.unlockedLevelIdx + 1)
	                {
		                if (!IsSGUnlocked(sg)) // STATUS: CLOSED
		                {
			                
			                if (!PlayerPrefs.HasKey(closeKey))
							{
								PlayerPrefs.SetString(closeKey, STATUS_CLOSED);
							}
			                return true;
		                }
		                else if(!PlayerPrefs.HasKey(alreadyOpenKey) )// STATUS: ALREADY OPENED
		                {
			                
			                if (PlayerPrefs.HasKey(openKey) && !PlayerPrefs.HasKey(closeKey))
			                {
				                PlayerPrefs.SetString(alreadyOpenKey, STATUS_ALREADY_OPEN);
			                }
			                
		                }
	                }
	                
                }
            }
            return false;
        }
        
        // Used for setting gates in levels panel
        public bool StarGateLockSetup(int level)
        {
	        if (disableGatesAB) return false;

	        foreach (StarGateInfo sg in starGatesInfo)
	        {
		        // If Gate exists and is not unlocked
		        if (sg.levelLock == level && level > GameData.PlayerData.unlockedLevelIdx && !IsSGUnlocked(sg))
		        {
			        return true;
		        }
	        }
	        return false;
        }

        public void SaveUnlockedGate(StarGateInfo sgInfo)
        {
            PlayerPrefs.SetString(LEVEL_UNLOCK_KEY + sgInfo.levelLock, "unlocked");
        }
        // Check PlayerPrefs if Level Lock is Unlocked
        public bool IsSGUnlocked(StarGateInfo sg)
        {
            if (PlayerPrefs.HasKey(LEVEL_UNLOCK_KEY + sg.levelLock))
            {
                if (PlayerPrefs.GetString(LEVEL_UNLOCK_KEY + sg.levelLock) == "unlocked")
                {
                    return true;
                }
            }
            return false;
        }

        public void SendStarGateDEvent(int level, bool onOpen = false)
        {
            // status: open - user opens star gate after reaching closed gate
            // status: close - user reached closed star gate
            // status: alreadyOpen - user reached opened star gate
            StarGateInfo sgInfo = FindStarGateInfoByLevel(level);


            if (sgInfo == null)
            {
                return;
            }
            
            // STATUS: OPEN
            string closeKey = LEVEL_UNLOCK_KEY + sgInfo.levelLock + STATUS_CLOSED;
            string openKey = LEVEL_UNLOCK_KEY + sgInfo.levelLock + STATUS_OPEN;
            
            if (PlayerPrefs.HasKey(closeKey) && !PlayerPrefs.HasKey(openKey))
            {
	            PlayerPrefs.SetString(openKey, STATUS_OPEN);
            }
            else if (!PlayerPrefs.HasKey(closeKey))
            {
	            PlayerPrefs.SetString(openKey, STATUS_OPEN);
            }
        }

    }
}