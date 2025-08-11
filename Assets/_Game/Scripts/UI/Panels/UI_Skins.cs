using UnityEngine;

namespace LightItUp.UI
{
    public class UI_Skins : MonoBehaviour {

        private void OnEnable()
        {
            AdController.SetBannerAdArea(GetComponent<RectTransform>());
            CanvasController.TogglePresenter (false, 1);
        }
        public void GoBack()
        {
            CanvasController.Open(CanvasController.Panels.Menu);
        }
    }
}
