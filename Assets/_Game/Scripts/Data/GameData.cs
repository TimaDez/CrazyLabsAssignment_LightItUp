using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Data
{
    public class GameData : GameDataBase
    {
        public static string savedBuildingsFilePath = Application.persistentDataPath + "/gameData.dat";
        static bool saveQueued = false;

        public static void TryLoadData()
        {
            var s = FileManager.LoadFile<SaveData>(savedBuildingsFilePath, true);
            if (s == null)
                s = new SaveData();
        }
        public static void SaveQueued()
        {
            if (saveQueued)
            {
                saveQueued = false;
                SaveGameData();
            }

        }
        public static void SaveGameData()
        {
            SaveData d = new SaveData();
            //d.buildingsOnDisplay = buildingsOnDisplay;
            //d.savedBuildingsDict = savedBuildingsDict;
            FileManager.SaveFile(savedBuildingsFilePath, d, true);
        }
        [System.Serializable]
        public class SaveData
        {
            //public List<int> buildingsOnDisplay;
            //public Dictionary<int, SavedBlockBuilding> savedBuildingsDict;
        }
    }
}

