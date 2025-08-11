using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HyperCasual.PsdkSupport;

namespace LightItUp.Currency
{

	public interface IInappProduct  {
		
		void OnPurchaseResolved (bool val);
		void RefreshProduct ();

}

}