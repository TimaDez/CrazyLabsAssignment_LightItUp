using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.PsdkSupport;
using UnityEngine.UI;
using System;

namespace LightItUp.UI
{
	public class UI_BoosterInappButton : MonoBehaviour {
		
		public List<InappProductInfo> inappInfos;
		public InappProductInfo inappInfo;
		public Text priceTag;

		private string productValueString;
		public static Action ProductPurchasedSuccesfuly;

		void OnEnable ()
		{
			ConfigureInappInfo ();
			GetComponent<Button> ().interactable = true;
			InitButton ();
		}


		void ConfigureInappInfo()
		{
			int priceLevel = 1;
			for (int i = 0; i < inappInfos.Count; i++) 
			{
				if (inappInfos[i].priceLevel == priceLevel) {
					inappInfo = inappInfos [i];
				}
			}
		}
		private void PsdkWrapperOnOnBillingInit(bool success)
		{
			if (success)
			{				
				GetComponent<Button> ().interactable = true;
				priceTag.text = "Unavailable";
			}
			else
			{
				GetComponent<Button> ().interactable = false;
				priceTag.text = "";
			}			
		}
		void InitButton()
		{

			GetComponent<Button> ().interactable = true;
			priceTag.text = "Unavailable";
		}

		public void RewardPurchase()
		{
			for (int i = 0; i < inappInfo.boosterRewards.Count; i++) 
            {
				BoosterService.Instance.SetBoosterPurchased (inappInfo.boosterRewards [i].boosterType);
				//BoosterService.Instance.AddRegularBoosterCharges (inappInfo.boosterRewards [i].boosterType, inappInfo.boosterRewards [i].boosterAmmount);
			}
			if (ProductPurchasedSuccesfuly!=null) 
			{
				ProductPurchasedSuccesfuly.Invoke ();
			}
		}

		public void ButtonTapped()
		{
			if (PurchasingValidator.Instance.CanAttemptPurchase()) 
			{
				PurchasingValidator.Instance.ProductAttempted (inappInfo, this);
			}


		}


	}

}