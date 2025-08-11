using HyperCasual;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;

public enum LevelsMode
{
    Levels_Default, Levels_New
}

[RequireComponent(typeof(Button))]
public class DebugToggleLevelsMode : MonoBehaviour {	
	
    // debug revive
    public static bool shouldOverrideLevelsMode = false;
    public static LevelsMode overrideLevelsMode = LevelsMode.Levels_Default;

    private Text text;
	
    // Use this for initialization
    void Awake () {
        GetComponent<Button>().onClick.AddListener(ToggleLevelsMode);
        text = GetComponentInChildren<Text>();

        //overrideReviveType = TestGroupService.Config;
		
        RefreshReviveText();
    }

    private void ToggleLevelsMode()
    {
        shouldOverrideLevelsMode = true;
        overrideLevelsMode = overrideLevelsMode == LevelsMode.Levels_Default
            ? LevelsMode.Levels_New
            : LevelsMode.Levels_Default;
        RefreshReviveText();
        
        GameManager.Instance.RefreshAllLevels();
    }

    void RefreshReviveText()
    {
        text.text = "Levels: " + overrideLevelsMode.ToString();
    }
}