using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using TMPro;
using UnityEngine;

public class StarCounterController : MonoBehaviour 
{
//	public GameObject counter;
	
	private void OnEnable()
	{
		SetupStarsCounter();
	}

	// Set stars counter
	private void SetupStarsCounter()
	{
		transform.GetComponentInChildren<TextMeshProUGUI>().text = GetTotalStars().ToString();
	}
	public int GetTotalStars()
	{
		return GameData.PlayerData.TotalStarsAllLevels;
	}
}
