using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LightItUp.Currency
{
	[CreateAssetMenu(fileName = "ConsumableLocalProductInfo", menuName = "[Tabtale]/ConsumableLocalProductInfo", order = 1)]
	public class ConsumableLocalProductInfo : ScriptableObject {


		public CurrencyType currencyType;
		public int currencyCostAmount;
		public List<BoosterType> boosters;
		public int boosterAmount;

	
}

}