using UnityEngine;

namespace LightItUp.UI
{
    public class ButtonUrl : ButtonBase {
        public enum UrlLinks {
            Terms,
            PrivacyPolicy
        }
        public UrlLinks urlToOpen;

        public override void OnClick()
        {
            Application.OpenURL(GetUrl());
        }

        string GetUrl() {
            switch (urlToOpen)
            {
                case UrlLinks.Terms:
                    return "";

                case UrlLinks.PrivacyPolicy:
                    return "";

                default:
                    return "";
            }

        }
    }
}
