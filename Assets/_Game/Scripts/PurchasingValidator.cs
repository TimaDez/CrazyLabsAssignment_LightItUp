using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;
using Cinemachine;
using HyperCasual.PsdkSupport;
using LightItUp.UI;

namespace LightItUp
{
	
	public class PurchasingValidator : SingletonCreate<PurchasingValidator> 
	{

		private bool isPurchaseActive = false;
		private InappProductInfo currentInappInfo;
		private UI_BoosterInappButton boosterInappButton;
		private bool isRegularProduct;


		private string nonConsumableProductName = "specialOffer";
		private string noAdsProductName = "noads";

		void OnEnable()
		{
			isRegularProduct = false;
			isPurchaseActive = false;
		}

		public void ProductAttempted(InappProductInfo attemptedInapp,UI_BoosterInappButton boosterInappButton)
		{
			if (!isPurchaseActive) 
			{
				this.currentInappInfo = attemptedInapp;
				this.boosterInappButton = boosterInappButton;
				isPurchaseActive = true;
				isRegularProduct = true;
			}
		}



		public bool CanAttemptPurchase()
		{
			return !isPurchaseActive;
		}


	}


}