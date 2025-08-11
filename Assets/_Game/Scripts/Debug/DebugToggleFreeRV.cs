using HyperCasual.Components.DebugComponents;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class DebugToggleFreeRV : MonoBehaviour
{
    private Text text;

    public static bool IsOn
    {
        get { return PlayerPrefs.GetInt("debug_free_rv", 0) == 1; }
		set { PlayerPrefs.SetInt("debug_free_rv", value?1:0);}
    }
	
    void Awake () {
        GetComponent<Button>().onClick.AddListener(OnClick);
        text = GetComponentInChildren<Text>();
        RefreshText();
    }

    private void OnClick()
    {
        IsOn = !IsOn;


        RefreshText();        
    }

    void RefreshText()
    {
        text.text = "Free RV: " + IsOn;
    }
}