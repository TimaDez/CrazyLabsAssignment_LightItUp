using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightItUp.Data
{     
	[CreateAssetMenu(fileName = "BoosterData", menuName = "[HyperCasual]/BoosterData")]
	public class BoosterConfiguration : ScriptableObject {

		public BoosterType boosterType;
		public int BoosterUnlockLevel;
		public string BoosterName;
		public string BoosterDescription;
	}
}
