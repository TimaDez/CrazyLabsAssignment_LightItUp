using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LightItUp.UI
{
	public class UI_BoosterLockedText : MonoBehaviour {

		public TextMeshProUGUI unlockText;

		public void ShowBoosterLocked (int level)
		{
			
			unlockText.text = "Unlocks at level " + level;
			StartAnimation();
		}
		public void ShowNotEnoughGold ()
		{

			unlockText.text = "Not enough gold";
			StartAnimation();
		}

		private void StartAnimation()
		{
			GetComponent<Animator> ().SetTrigger("ShowExpl");
		}

	}

}