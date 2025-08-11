using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Singletons;
using System;

namespace LightItUp.Currency
{

	public enum CurrencyType 
	{
		Gems
	}

	public static class CurrencyConstants
	{
		public static string currencyPersistencyString = "curPersist";
	}

	public class CurrencyService : SingletonCreate<CurrencyService>  {
	

		public static Action<CurrencyType, int, int> CurrencyAmountChanged; 

		public void OnEnable()
		{
			//ConsumeCurrency (CurrencyType.Gems, GetCurrentAmount (CurrencyType.Gems));
			//AddCurrency(CurrencyType.Gems,40);

		}
	
		public void AddCurrency(CurrencyType curType, int currencyAmount, string itemSpent, int amountSpent)
		{
			string persistencyKey = GetPersistenyKey (curType);
			if (!PlayerPrefs.HasKey(persistencyKey)) {
				PlayerPrefs.SetInt (persistencyKey, 0);
			}
			int previousAmount = PlayerPrefs.GetInt (persistencyKey);

			PlayerPrefs.SetInt (persistencyKey, previousAmount + currencyAmount);
			AnnounceCurrencyChange (curType,previousAmount + currencyAmount, currencyAmount);


		}
		 
		public bool ConsumeCurrency(CurrencyType curType, int requestedAmount)
		{
			string persistencyKey = GetPersistenyKey (curType);
			if (!PlayerPrefs.HasKey(persistencyKey)) 
            {
				PlayerPrefs.SetInt (persistencyKey, 0);
			}
			int currentAmount = PlayerPrefs.GetInt (persistencyKey);

			if (currentAmount >= requestedAmount) 
            {

				PlayerPrefs.SetInt (persistencyKey, currentAmount - requestedAmount);
				AnnounceCurrencyChange (curType, currentAmount - requestedAmount, -requestedAmount);
				return true;
			} 
			return false;

		}

		public int GetCurrentAmount(CurrencyType curType)
		{
			string persistencyKey = GetPersistenyKey (curType);

			if (PlayerPrefs.HasKey (persistencyKey)) {
				return PlayerPrefs.GetInt (persistencyKey);
			} else {
				PlayerPrefs.SetInt (persistencyKey, 0);
			}
			return 0;
		}


		void AnnounceCurrencyChange (CurrencyType curType, int totalAmount, int changedAmount)
		{
			if (CurrencyAmountChanged!=null) {
				CurrencyAmountChanged (curType, totalAmount, changedAmount);
			}
		}


		string GetPersistenyKey(CurrencyType curType)
		{

			return CurrencyConstants.currencyPersistencyString + curType.ToString ();
		}

}

}
