using System.Linq;
using UnityEngine;
using LightItUp.Singletons;
using System;
using System.Collections.Generic;
using LightItUp.Currency;

namespace LightItUp.UI
{
    public abstract class CanvasControllerBase<T,Y,U> : SingletonCreate<T> 
        where T : CanvasControllerBase<T, Y, U> 
        where Y : struct, System.IConvertible
        where U : struct, System.IConvertible
    {


        public GameObject[] panels;
        public GameObject[] popups;
        public UI_GenericAlert alert;
        public GameObject loading;
        public GameObject interactionBlocker;
        Camera CanvasCamera;
        public Canvas canvas;
        public RectTransform narrowViewScaler;
		public ConsumablePresenter consumablePresenter;

//        public string previousPopUpName = "";
        public string location = "";
        
        public bool NarrowAspectLayout {
            get
            {
                if (CanvasCamera == null) CanvasCamera = Camera.main;
                return CanvasCamera.aspect < 0.54f;
            }
        }

        static int _animationCount = 0;
        static int _loadingCount = 0;
        public static int AnimationCount
        {
            get
            {
                return _animationCount;
            }
            set
            {
                
                _animationCount = Mathf.Max(0, value);
                Instance.interactionBlocker.SetActive(value != 0);
				if (value < 0)
				{
					Instance.interactionBlocker.SetActive(false);
					Debug.LogError("Negative animationCount!");
				}
            }
        }
        public static int LoadingCount
        {
            get
            {
                return _loadingCount;
            }
            set
            {
                if (value < 0)
                {
                    Debug.LogError("Negative loadingCount!");
                }
                _loadingCount = Mathf.Max(0, value);
                Instance.loading.SetActive(value != 0);
            }
        }
        public static bool HasOpenPopups
        {
            get
            {
                if (CanvasController.AnimationCount > 0)
                    return true;
            
                var popups = Instance.popups.ToList();

                foreach (var popup in popups)
                {
                    if (popup.gameObject.activeInHierarchy)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    
        public static R GetPanel<R>() where R : MonoBehaviour {
            foreach (var p in Instance.panels)
            {
                var r = p.GetComponent<R>();
                if (r != null)
                    return r;
            }
            return null;
        }
        public static R GetPopup<R>() where R : MonoBehaviour
        {
            foreach (var p in Instance.popups)
            {
                var r = p.GetComponent<R>();
                if (r != null)
                    return r;
            }
            return null;
        }
        public static UI_GenericAlert GenericAlert
        {
            get
            {
                return Instance.alert;
            }
        }


        public override void Awake()
        {
            base.Awake();
        
            if (Instance != this) return;
            CloseAllPopups();
            GenericAlert.gameObject.SetActive(false);
            AnimationCount = 0;
            //LoadingCount = 0;        
        }

		public static void TogglePresenter(bool toggle, int positionIndex = 1, bool canShowRV = false, CanvasController.Popups currentPopup = CanvasController.Popups.Empty)
		{
			if (Instance == null) {
				return;
			}
			Instance.consumablePresenter.ToggleShowPresenter (toggle, positionIndex,canShowRV, currentPopup);
		}

        public static void Open(Y panel)
        {
            foreach (var p in Instance.panels)
            {
                if (p.activeSelf) p.SetActive(false);
            }
            var openedPanel = Instance.panels.FirstOrDefault(x => x.name.ToLower().Equals(panel.ToString().ToLower()));
            if (openedPanel != null)
            {
                if (openedPanel.activeSelf)
                {
                    Debug.LogError("Panel already opened! " + panel.ToString());
                }
                Instance.CloseAllPanels();
                openedPanel.SetActive(true);    

            }
            else
            {
                Debug.LogError("No panel with name: " + panel.ToString());
            }
        }
        public void CloseAllPanels()
        {
            foreach (var p in Instance.panels)
            {
                if (p.activeSelf) p.SetActive(false);
            }
        }

        public static void Open(U popup)
        {
            
//            foreach (var p in Instance.popups)
//            {
//                if (p.activeSelf)
//                {
//                    p.SetActive(false);
//                }
//            }
            var openedPopup = Instance.popups.FirstOrDefault(x => x.name.ToLower().Equals(popup.ToString().ToLower()));
            if (openedPopup != null)
            {
                if (openedPopup.activeSelf)
                {
                    Debug.LogError("Popup already opened! " + popup.ToString());
                }
                Instance.CloseAllPopups();
                openedPopup.SetActive(true);
            }
            else
            {
                Debug.LogError("No Popup with name: " + popup.ToString());
            }
        }

        protected void ClosePopup(U popup)
        {
            var openedPopup = Instance.popups.FirstOrDefault(x => x.name.ToLower().Equals(popup.ToString().ToLower()));
            if (openedPopup != null)
            {
				if (openedPopup.gameObject.activeSelf) {
					openedPopup.gameObject.SetActive (false);
				}
            }
        }
        public void CloseAllPopups()
        {
            foreach (var p in Instance.popups)
            {
                if (p.activeSelf)
                {
                    p.SetActive(false);
                }
            }

        }

        public string GetLocation()
        {
            if (location.Length > 0)
            {
                string temp = location;
                location = "";
                return temp;
            }
            else
            {
				foreach (GameObject popup in CanvasController.Instance.popups)
				{
					if (popup.activeSelf)
					{
						return popup.name;
					}
				}

                foreach (GameObject panel in CanvasController.Instance.panels)
                {
                    if (panel.activeSelf)
                    {
                        return panel.name;
                    }
                }
            }

            return "";
        }
        public string GetActivePanel()
        {
            foreach (GameObject panel in CanvasController.Instance.panels)
            {
                if (panel.activeSelf)
                {
                    return panel.name;
                }
            }
            return "";
        }
        public string GetOpenedPopUpName()
        {
            foreach (GameObject popup in popups)
            {
                if (popup.activeInHierarchy)
                {
                    return popup.name;
                }
            }

            return "";
        }

        public void EndLoading()
        {
            LoadingCount = 0;
        }

    }
}
