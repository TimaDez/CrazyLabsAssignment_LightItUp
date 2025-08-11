using System.Collections;
using System.Collections.Generic;
using LightItUp.Currency;
using LightItUp.Data;
using UnityEngine;
using UnityEngine.UI;

public class DebugAddStars : MonoBehaviour {

    private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(AddStars);
	}

	private void AddStars()
	{
        bool newScore, newStar;
        int stars = GameData.PlayerData.GetLevelHighscore(0).stars;
        GameData.PlayerData.SetHighscore(0, stars + 10, stars + 10, out newScore, out newStar);
    }
}
