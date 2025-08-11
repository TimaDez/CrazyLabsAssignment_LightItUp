using System.Collections.Generic;
using System.Linq;
using LightItUp;
using UnityEngine;

namespace HyperCasual.Skins
{
    public static class SkinsState
    {
        public static bool HasNewSkins
        {
            get { return PlayerPrefs.GetInt("has_new_skins", 0) == 1; }
            set { PlayerPrefs.SetInt("has_new_skins", value?1:0);}
        }

        public static List<SkinConfig> CheckConditionsForNewSkins()
        {
            var newlyUpdatedConditions = ConditionsManager.Instance.UpdateConditions();

            if (newlyUpdatedConditions.Count == 0)
                return null;

            var completeConditions = newlyUpdatedConditions.Where(c => c.DidComplete);

            var numCompleteConditions = completeConditions.Count();
        
            if (numCompleteConditions > 0)		
            {
                StatisticsService.CountStat(GameStats.unlock_skin, numCompleteConditions);
                newlyUpdatedConditions.AddRange(ConditionsManager.Instance.UpdateConditions());
                completeConditions = newlyUpdatedConditions.Where(c => c.DidComplete);
                var unlockedSkins = SkinsData.Instance.SkinsWithConditions(completeConditions);
        
                foreach (var skin in unlockedSkins)
                {
                    skin.Unlocked = true;
                }	
                
                HasNewSkins = true;

                return unlockedSkins;
            }

            return null;
        }

    }
}