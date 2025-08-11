using HyperCasual.Skins;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class DebugToggleLevelIdDisplay : MonoBehaviour
{
    private Text text;

    public static bool IsOn
    {
        get { return PlayerPrefs.GetInt("debug_display_uid", 0) == 1; }
        set { PlayerPrefs.SetInt("debug_display_uid", value?1:0);}
    }
	
    // Use this for initialization
    void Awake () {
        GetComponent<Button>().onClick.AddListener(OnClick);
        text = GetComponentInChildren<Text>();

        //overrideReviveType = TestGroupService.Config;
		
        RefreshText();
    }

    private void OnClick()
    {
        IsOn = !IsOn;
        RefreshText();        
    }

    void RefreshText()
    {
        text.text = "Display Uid: " + IsOn;
    }
}
