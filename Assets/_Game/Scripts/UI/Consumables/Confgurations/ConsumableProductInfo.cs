using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LightItUp.Currency
{
	[CreateAssetMenu(fileName = "ConsumableProductInfo", menuName = "[Tabtale]/ConsumableProductInfo", order = 1)]
	public class ConsumableProductInfo : ScriptableObject {

		public string inappName;
		public CurrencyType currencyType;
		public int currencyAmount;
		public List<BoosterType> boosters;
		public int boosterAmount;

	
}

}