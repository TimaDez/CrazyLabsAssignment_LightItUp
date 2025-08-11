using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.UI;

namespace LightItUp.UI
{
	public class UI_Level10Info : UI_Popup {

	
	
	
		public Image controlButton;
		protected override void OnEnable()
		{
			base.OnEnable();
			GameData.PlayerData.showControlsTutorial = false;
			AdController.SetBannerAdArea(GetComponent<RectTransform>());
		}
		public override void ClosePopup()
		{
			ClosePopup(()=> {
				CanvasController.GetPopup<UI_GameEnded> ().SetFirstTimeOpen(true);
				CanvasController.Open(CanvasController.Popups.GameEnded);
			});
		}
	}
}
