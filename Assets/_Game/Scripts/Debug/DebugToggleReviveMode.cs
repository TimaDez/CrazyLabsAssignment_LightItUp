using System.Collections;
using System.Collections.Generic;
using HyperCasual;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Game;


[RequireComponent(typeof(Button))]
public class DebugToggleReviveMode : MonoBehaviour {	
	
	private Text text;
	
	// Use this for initialization
	void Awake () {
		GetComponent<Button>().onClick.AddListener(ToggleReviveMode);
		text = GetComponentInChildren<Text>();
		
		RefreshReviveText();
	}

	private void ToggleReviveMode()
	{
		ReviveConfig.reviveType = ReviveConfig.reviveType == ReviveConfig.ReviveType.ClosestBlock
			? ReviveConfig.ReviveType.LastBlock
			: ReviveConfig.ReviveType.ClosestBlock;
		RefreshReviveText();
	}

	void RefreshReviveText()
	{
		text.text = "Revive: " + ReviveConfig.reviveType;
	}
}
