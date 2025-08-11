using UnityEngine;
using LightItUp.Data;
using RSG;
using System;
using UnityEngine.UI;
using TMPro;
using LightItUp.Currency;

namespace LightItUp.UI
{

	public class UI_BoosterRVOffer : MonoBehaviour {

		public BoosterType boosterType;
		public bool isRandomBooster;
		public Image boosterIcon;
		public TextMeshProUGUI boosterDescription;
		public TextMeshProUGUI boosterName;
		public bool isStoreRedirect;
		private Promise<bool> _rvSuccessPromise;

		public void InitBoosterRVOffer(BoosterType boosterType)
		{
			this.boosterType = boosterType;
		}
	
		public void OnGoToShop()
		{
			UI_PopupRelayer.Instance.PutPopupOnHold (CanvasController.Popups.GameEnded);

			CanvasController.Instance.location = UI_PopupRelayer.Instance.popUpOnHold.ToString();
			
			CanvasController.Open (CanvasController.Popups.BoosterShop);
		}

		public void OnRvAttempted()
		{
			ResolveResult(true);   
		}

		void ResolveResult(bool val)
		{
			UI_BoosterShopPopup.OnRVButtonResolved ();
			if (val) 
			{
				BoosterService.Instance.AddMandatoryBoosterCharges (boosterType,1);
			}
			ShowButton ();
		}
		public void HideButton()
		{
			gameObject.SetActive (false);
		}


		public void ShowButton()
		{
			gameObject.SetActive (true);
			if (isRandomBooster) 
			{
				RandomizeBoosterButton ();
			}
		}

		void RandomizeBoosterButton()
		{
			int myEnumMemberCount = Enum.GetNames(typeof(BoosterType)).Length;
			int randomBoosterIndex = UnityEngine.Random.Range (0, myEnumMemberCount);
			var boosters =Enum.GetValues (typeof(BoosterType));
			boosterType = (BoosterType)boosters.GetValue (randomBoosterIndex);
			ApplyVisuals ();
		
		}

		void ApplyVisuals()
		{	
			BoosterConfiguration boosterConfig = BoosterService.Instance.boosterConfig.GetSpecificBoosterConfigByType (boosterType);
			boosterDescription.text = boosterConfig.BoosterDescription;
			boosterName.text = boosterConfig.BoosterName;
			switch (boosterType) {
			case BoosterType.MagicHat:
				boosterIcon.sprite = SpriteAssets.Instance.boosterButtonAuraDefault;
				break;

			case BoosterType.MetalBoots:
				boosterIcon.sprite = SpriteAssets.Instance.boosterButtonBootsDefault;
				break;

			case BoosterType.SpringShoes:
				boosterIcon.sprite = SpriteAssets.Instance.boosterButtonShoesDefault;
				break;
			default:
				break;
			}
		}
	}

}