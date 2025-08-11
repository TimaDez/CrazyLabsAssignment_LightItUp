using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;

namespace LightItUp
{
	public class LevelNoTxt : MonoBehaviour {

		private Text _lvlTxt;

		private void Awake(){
			_lvlTxt = GetComponent<Text>();
		}
	
		private void OnEnable(){
			_lvlTxt.text = "Lvl: " + (GameManager.Instance.currentLevel.levelIdx).ToString();
		}
	}
}