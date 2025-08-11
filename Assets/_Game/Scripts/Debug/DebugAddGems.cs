using System.Collections;
using System.Collections.Generic;
using LightItUp.Currency;
using UnityEngine;
using UnityEngine.UI;

public class DebugAddGems : MonoBehaviour {
	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(AddGems);
	}

	private void AddGems()
	{
		CurrencyService.Instance.AddCurrency(CurrencyType.Gems, 2000, "0", 0); 
	}
}
