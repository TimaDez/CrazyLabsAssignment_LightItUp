using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.UI;
using UnityEngine.UI;
using HyperCasual.Skins;

public class UI_GameStarted : UI_Popup {


	public Text levelNumberText;
	public Text completionText;
	public Text completionPercentage;
	public List<GameObject> completionStars;
	public ProgressUIDisplay progressDisplay;
	public UI_BoosterButton boosterMagicHat;
	public UI_BoosterButton boosterSpringShoes;
	public UI_BoosterButton boosterMetalBoots;

	public GameObject getMoreButton;
	public GameObject startButton;

}
