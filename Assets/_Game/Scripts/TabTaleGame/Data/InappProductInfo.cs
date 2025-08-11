using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LightItUp
{
[CreateAssetMenu(fileName = "InappProductInfo", menuName = "[Tabtale]/InappProductInfo", order = 1)]
	public class InappProductInfo : ScriptableObject {

		public string inappName;
		public List<BoosterReward> boosterRewards;
		public string analyticsString;
		public int priceLevel;
	}

	[Serializable]
	public struct BoosterReward
	{
		[SerializeField]
		public BoosterType boosterType;
		[SerializeField]
		public int boosterAmmount;
	}

}
