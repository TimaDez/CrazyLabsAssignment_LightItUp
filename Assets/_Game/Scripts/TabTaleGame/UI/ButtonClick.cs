using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LightItUp.Sound;

namespace LightItUp.UI
{
    [DisallowMultipleComponent]
    public class ButtonClick : MonoBehaviour, IPointerClickHandler {
        public string clipName = "";
        string defaultClip = "MenuClick";
        Button _button;
        Button button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }
                return _button;
            }

        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (button != null && !button.IsInteractable())
            {
                return;
            }
            if (string.IsNullOrEmpty(clipName))
            {
                SoundManager.PlaySound(defaultClip);
            }
            else
            {
                SoundManager.PlaySound(clipName);
            }
        }
    }
}
