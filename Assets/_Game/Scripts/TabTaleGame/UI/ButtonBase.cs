using UnityEngine;
using UnityEngine.UI;

namespace LightItUp.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonBase : MonoBehaviour
    {
        Button _button;
        protected Button button
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();
                return _button;
            }
        }
        protected virtual void Awake()
        {
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(OnClick);
        }

        public abstract void OnClick();
    }
}
