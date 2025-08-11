using UnityEngine;
using LightItUp.UI;

namespace LightItUp
{
    public static class AdController
    {
        public static bool isBannerVisible;

        

        public static void SetBannerAdArea(RectTransform rt)
        {
            float bannerHeight = isBannerVisible ? (Screen.height / 11) : 0;
            var min = rt.anchorMin;
            float f = bannerHeight / Screen.height;
            min.y = f;
            rt.anchorMin = min;
        }
    }
}
