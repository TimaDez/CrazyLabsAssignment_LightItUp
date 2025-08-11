using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HyperCasual.Skins
{
    [CreateAssetMenu(fileName = "SkinsData", menuName = "[HyperCasual]/SkinsData")]
    public class SkinsData : ScriptableObject
    {
        private static SkinsData _instance;
        
        public static SkinsData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SkinsData>("Data/SkinsData");
                }

                return _instance;
            }
            
        }
        
		public List<SkinConfig> list;

		private int currentSkinListIndex;


        public List<SkinConfig> SkinsWithConditions(IEnumerable<ConditionData> conditions)
        {
            var conditionIds = conditions.Select(cond => cond.id);

            return list.Where(s => s.condition != null && conditionIds.Contains(s.condition.id)).ToList();
        }
        

		public void SelectSkinList(int skinIndex)
		{
			currentSkinListIndex = skinIndex;
		}
		public List<SkinConfig> GetCurrentSkin()
		{
			switch (currentSkinListIndex) {
			case 1:
				return list;
			default:
				return list;
			
			}
		}
        public int Count
        {
			get { return list.Count; }
        }
        
        public int UnlockedCount
        {
            get { return list.Count(s => s.Unlocked); }            
        }

        public SkinConfig CurrentSkin
        {
            get
            {
                SkinConfig result;
                
                if (string.IsNullOrEmpty(CurrentSkinId))
                {
                    result = list.FirstOrDefault(config => config.condition == null);

                    if (result == null)
                    {
                        result = list[0];
                    }
                    
                    CurrentSkinId = result.id;
                    return result;
                }

                var id = CurrentSkinId;
                
                result = list.FirstOrDefault(config => config.id == id);
                
                return result;

            }
            
        }

        public string GetCurrentSkinResourcePath()
        {
            return CurrentSkin.modelResourcePath;
        }
        
        public static string CurrentSkinId
        {
            get { return PlayerPrefs.GetString("current_skin_id", ""); }
            set
            {
                PlayerPrefs.SetString("current_skin_id", value);
                PlayerPrefs.Save();               
            }
        }

        public int CurrentSkinIndex
        {
            get { return list.IndexOf(CurrentSkin); }
        }
    }
}