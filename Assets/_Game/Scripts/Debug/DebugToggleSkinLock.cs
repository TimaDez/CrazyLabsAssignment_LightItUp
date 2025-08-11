using HyperCasual.Skins;
using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(Button))]
public class DebugToggleSkinLock : MonoBehaviour
{
    private Text text;
	
    // Use this for initialization
    void Awake () {
        GetComponent<Button>().onClick.AddListener(OnClick);
        text = GetComponentInChildren<Text>();

        //overrideReviveType = TestGroupService.Config;
		
        RefreshText();
    }

    private void OnClick()
    {
        SkinUnlockToggle.IsOn = !SkinUnlockToggle.IsOn;
        RefreshText();        
    }

    void RefreshText()
    {
        text.text = "Skin Unlock: " + SkinUnlockToggle.IsOn;
    }
}