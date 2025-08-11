using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LightItUp.Currency
{
	[CreateAssetMenu(fileName = "RVConsumableConfig", menuName = "[Tabtale]/RVConsumableConfig", order = 1)]
	public class RVConsumableConfig : ScriptableObject {

		public float rvReplenishTime;
		public CurrencyType currency;
		public int currencyReward;

	}

}