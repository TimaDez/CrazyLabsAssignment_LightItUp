using UnityEngine;
using UnityEngine.UI;

namespace LightItUp.UI
{
    [RequireComponent(typeof(Toggle))]
    public abstract class ToggleBase : MonoBehaviour {

        Toggle _toggle;
        protected Toggle toggle
        {
            get
            {
                if (_toggle == null)
                    _toggle = GetComponent<Toggle>();
                return _toggle;
            }
        }

        protected virtual void Awake()
        {
            toggle.onValueChanged = new Toggle.ToggleEvent();
            toggle.onValueChanged.AddListener(ToggleValue);
        }

        public abstract void ToggleValue(bool t);
    }
}
