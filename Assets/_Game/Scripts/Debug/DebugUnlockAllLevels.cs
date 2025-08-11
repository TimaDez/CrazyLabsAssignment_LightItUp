using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Data;
using LightItUp.UI;


public class DebugUnlockAllLevels : MonoBehaviour {

	public void UnlockAllLevels()
	{
		GameData.PlayerData.UnlockAllLevels();

		GameManager.Instance.debugDisableGates = true;
		
		var levelsPanel = FindObjectOfType<UI_Levels>();

		if (levelsPanel != null && levelsPanel.gameObject.activeInHierarchy)
		{
			levelsPanel.forceUpdate = true;
		}
	}
}
