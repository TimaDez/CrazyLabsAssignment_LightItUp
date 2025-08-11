using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HyperCasual.PsdkSupport;
using LightItUp.Currency;
using UnityEngine.UI;

namespace LightItUp.UI
{
	public class UI_BoosterShopPopup : UI_Popup {


		public static Action UI_BoosterShopPopupClosed;
		public static Action UI_RVButtonResolved;
		public ScrollRect scrollRect;
		public UI_Menu uiMenu;
		public NoInternetHUD noInternetHUD;
		public List<GameObject> subscriptionActiveObjects;
		public List<GameObject> subscriptionInactiveObjects;
		
		protected override void OnEnable()
		{
			base.OnEnable ();

			if (uiMenu!=null) 
			{
//				uiMenu.ShopPopupOpen ();
			}

			scrollRect.verticalNormalizedPosition = 1;
			CanvasController.TogglePresenter (true, 1, false);
			CheckMenuPanelVisuals (false);

		}

		void CheckMenuPanelVisuals(bool toggle)
		{
			UI_Menu uiMenu = CanvasController.GetPanel<UI_Menu> ();
			if (uiMenu.isActiveAndEnabled) {
				uiMenu.ToggleVisuals (toggle);
//				uiMenu.OnShopClosed ();
			}
		}
		void OnDisable()
		{
			CanvasController.TogglePresenter (true, 1, true);
			CheckMenuPanelVisuals (true);
		}

		public void ShowConsumableProducts()
		{
			scrollRect.verticalNormalizedPosition = 0.32f;
		}
		public void ShowBoosterProducts()
		{
			scrollRect.verticalNormalizedPosition = 0f;
		}

		public void OnBackButton()
		{
			if (UI_BoosterShopPopupClosed!=null) {
				UI_BoosterShopPopupClosed.Invoke ();

			}

			if (!IsNoInternetActive ()) {
				ClosePopup ();
				
			}
		}

		public override void OpenAnimationComplete ()
		{
			base.OpenAnimationComplete ();
			CanvasController.AnimationCount = CanvasController.AnimationCount > 1?0:0;

		}

		public static void OnRVButtonResolved()
		{
			if (UI_RVButtonResolved!=null) {
				UI_RVButtonResolved.Invoke();
				
			}
		}	
		public override void CloseAnimationComplete()
		{
			base.CloseAnimationComplete();
		
			if (UI_PopupRelayer.Instance.IsPopupOnHold ()) {
				UI_PopupRelayer.Instance.OpenOnHoldPopup ();
			}

		}

		bool IsNoInternetActive()
		{

			GameObject noInternetHud = GameObject.FindGameObjectWithTag ("noInternetHUD");
			if (noInternetHud!=null && noInternetHud.activeSelf) {
				return true;
			}
			return false;
		}

		protected override void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (CanvasController.AnimationCount > 0)
					return;

				if (!HandleBackPressed() && !IsNoInternetActive ())
					ClosePopup();
			}
		}

	}
}
