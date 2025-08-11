using HyperCasual.Components.DebugComponents;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DebugToggleFPSDisplay : MonoBehaviour
{
    private Text text;

    private FPSCounter _fpsCounter;

    public static bool IsOn
    {
        get { return PlayerPrefs.GetInt("debug_display_fps", 0) == 1; }
        set { PlayerPrefs.SetInt("debug_display_fps", value?1:0);}
    }
	
    void Awake () {
        GetComponent<Button>().onClick.AddListener(OnClick);
        text = GetComponentInChildren<Text>();

        _fpsCounter = FindObjectOfType<FPSCounter>();

        RefreshText();
    }

    private void OnClick()
    {
        IsOn = !IsOn;

        if (_fpsCounter != null)
        {
            _fpsCounter.GetComponent<Text>().enabled = IsOn;
        }
        
        RefreshText();        
    }

    void RefreshText()
    {
        text.text = "Show FPS: " + IsOn;
    }
}