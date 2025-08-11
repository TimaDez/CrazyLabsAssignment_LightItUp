using UnityEngine;
using LightItUp.Data;
using LightItUp.Data;

namespace LightItUp.Editor {
    public class PPrefsHelper {
        [UnityEditor.MenuItem("Custom/PlayerPrefs/Delete PlayerPrefs")]
        public static void DeletePlayerPrefs() {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.LogWarning("Deleted all player prefs!");
        }
        [UnityEditor.MenuItem("Custom/PlayerPrefs/Delete PlayerData")]
        public static void DeleteUserData()
        {
            FileManager.DeleteFile(PlayerData.filePath);
        }
       /* [UnityEditor.MenuItem("Custom/PlayerPrefs/Delete SavedBuildings")]
        public static void DeleteSavedBuildings()
        {
            FileManager.DeleteFile(GameData.savedBuildingsFilePath);
        }*/
        [UnityEditor.MenuItem("Custom/PlayerPrefs/Delete All")]
        public static void DeleteAll()
        {
            DeletePlayerPrefs();
            DeleteUserData();
           // DeleteSavedBuildings();
        }
    }
}
