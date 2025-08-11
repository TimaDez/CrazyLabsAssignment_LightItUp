using System;
using System.Linq;
using HyperCasual.PsdkSupport;
using HyperCasual.Skins;
using UnityEngine;
using LightItUp.Data;
using LightItUp.Game;
using LightItUp.Currency;
using System.Collections;

namespace LightItUp.UI
{
    public class UI_Menu : MonoBehaviour
    {
        public RectTransform rt;
        public bool usingNativeCampaign = false;
        //public NativeCampaignDisplay nativeCampaignDisplay;
		public GameObject tapToStart;
		public GameObject title;
		public GameObject subscriptionButton;

        [SerializeField] private GameObject levels;


        private void OnEnable()
        {
            Time.timeScale = 1;
            
            levels.SetActive(true);
            
            Debug.LogWarning("No Ads: Initialize noAds button state");
                
            SkinsState.CheckConditionsForNewSkins();
			CanvasController.TogglePresenter (true, 1, true);


        }

		public void ToggleVisuals(bool toggle)
		{
//			tapToStart.SetActive (toggle);
//			title.SetActive (toggle);			
		}
		private void OnDisable()
		{
			//SubscriptionService.AnnounceSubscriptionStatus -= OnAnnounceSubscriptionInitialized;
            //CanvasController.TogglePresenter (false, 1, false);
            levels.SetActive(false);
		}

    
        public void StartSelectedLevel()
        {
            GameManager.Instance.isReplay = false;
			if (BoosterService.Instance.AreBoostersActive()) 
            {
				UI_PopupRelayer.Instance.RemovePreviousHeldPopup ();
                GameManager.Instance.OpenPreLevelPopup();
			}else
			{
            	GameManager.Instance.StartSelectedLevel();
			}
        }

        public void OpenSettings() 
        {
            CanvasController.Open(CanvasController.Popups.Settings);
        }
        public void OpenLevels()
        {        
            GameManager.Instance.CleanupScene();
            CanvasController.Instance.CloseAllPopups();
            CanvasController.Open(CanvasController.Panels.Levels);        
        }
    
        public void OpenSkins()
        {
            GameManager.Instance.CleanupScene();
            CanvasController.Instance.CloseAllPopups();
            CanvasController.Open(CanvasController.Panels.Skins);
        }

        public void OpenShop()
        {
            GameManager.Instance.CleanupScene();
            CanvasController.Instance.CloseAllPopups();
            CanvasController.Open (CanvasController.Popups.BoosterShop);
        }
        private void Update()
        {                        
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CanvasController.AnimationCount == 0)
                {
                    var popups = CanvasController.Instance.popups.ToList();
            
                    foreach (var popup in popups)
                    {
                        if (popup.gameObject.activeInHierarchy)
                        {
                            return;
                        }
                    }
            
                    CanvasController.Instance.CloseAllPopups();
                    CanvasController.Open(CanvasController.Popups.Quit);
                }            
            }
        }
    }
}
