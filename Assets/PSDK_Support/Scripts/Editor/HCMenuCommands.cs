using UnityEditor;
using UnityEngine;
using LightItUp.Data;
using LightItUp.Data;

namespace HyperCasual
{
	
	public static class HCMenuCommands{

		[MenuItem("TabTale/Clear Player Prefs", false, 0)]
		public static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}
		
		[MenuItem("TabTale/Clear Player Data", false, 0)]
		public static void ClearPlayerData()
		{
			ClearPlayerPrefs();
			
			FileManager.DeleteFile(PlayerData.filePath);			
		}
		
	}

}

