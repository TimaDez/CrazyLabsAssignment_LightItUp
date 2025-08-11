using UnityEngine;
using LightItUp.UI;
using System.Collections.Generic;
using System;

namespace LightItUp
{
    public class CanvasController : CanvasControllerBase<CanvasController, CanvasController.Panels, CanvasController.Popups>
    {
        public GameObject loadingView;

        public enum Panels
        {
            Menu,
            Game,
            Skins,
            Levels
        }
        public enum Popups
        {
            Settings,
            ReviveOffer,
            GameEnded,
            Level10Info,
            Quit,
            RateUs,
            Pause,
			PreLevelPopup,
			BoosterUnlocked,
			BoosterShop,
			Subscription,
			Empty,
			PrivacyPopup

        }
        public Popups GetOpenedPopup()
        {
            string activePopUpName = GetOpenedPopUpName();

            foreach (Popups name in Enum.GetValues(typeof(Popups)))
            {
                if(activePopUpName.Equals(name.ToString()))
                {
                    return name;
                }
            }
            return Popups.Empty;
        }
        public override void Awake()
        {
            base.Awake();
        }

        [ContextMenu("unscale animators")]
        public void DebugMakeAnimatorsUnscaled()
        {
            var componentsInChildren = GetComponentsInChildren<Animator>(true);
        
            foreach (var componentsInChild in componentsInChildren)
            {
                componentsInChild.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }
    
    
    
        private void Start()
        {

            //MakeCanvasFitIPhoneX();
        }
        private void MakeCanvasFitIPhoneX()
        {
            var device = SystemInfo.deviceModel;

            float ratio = Screen.height;
            ratio /= 812;

            if (NarrowAspectLayout)
            {
                var sa = Screen.safeArea;


                // Hacky fix for iphone xs
                switch (device)
                {
                    case "iPhone11,2":
                    case "iPhone11,4":
                    case "iPhone11,6":
                    case "iPhone11,8":
                        sa = new Rect(0, 0, Screen.width, Screen.height);
                        sa.yMax -= 44 * ratio;
                        sa.yMin += 34 * ratio;
                        sa.xMax -= 16 * ratio;
                        sa.xMin += 16 * ratio;
                        break;
                }

                //var sa = new Rect(10, 20, Screen.width - 20, Screen.height - 30);
                sa.x /= Screen.width;
                sa.width /= Screen.width;
                sa.y /= Screen.height;
                sa.height /= Screen.height;

                float w = 1152;
                float h = 2048;
                var r = narrowViewScaler.rect;
                var scale = h / r.height;

                //var w = r.width;
                // 9:16 with 2048 height requires 1152 width!
                scale = r.width / w;
                r.width = w;

                //Get full screen height
                float f = (w / Screen.width) * Screen.height;

                Vector2 size = new Vector2(w * sa.width, f * sa.height);
                Vector2 center = new Vector2(-w / 2 + w * sa.center.x, -f / 2 + f * sa.center.y);

                narrowViewScaler.anchorMin = new Vector2(0.5f, 0.5f);
                narrowViewScaler.anchorMax = new Vector2(0.5f, 0.5f);
                narrowViewScaler.pivot = new Vector2(0.5f, 0.5f);
                narrowViewScaler.sizeDelta = size;
                narrowViewScaler.localScale = Vector3.one * scale;
                narrowViewScaler.localPosition = center;
            }

        }
    }
}
