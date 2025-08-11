using UnityEngine;
using UnityEngine.UI;

namespace HyperCasual.Skins
{
    public class SkinUnlockToggle : MonoBehaviour
    {
        public static bool IsOn { get; set; }
        
        public Button Button;

        public void Toggle()
        {
            IsOn = !IsOn;
            Button.GetComponentInChildren<Text>().text = IsOn ? "On" : "Off";
        }

        public void OnEnable()
        {
            Button.GetComponentInChildren<Text>().text = IsOn ? "On" : "Off";
        }
        
    }
}