using System.Collections;
using System.Collections.Generic;
using LightItUp;
using LightItUp.UI;
using UnityEngine;

public enum TextType
{
    PrivacyPolicy,
    TermsOfUse
}
public class UI_PrivacyPopup : UI_Popup
{
    [SerializeField] private GameObject privacyTitle;
    [SerializeField] private GameObject termsTitle;
    [SerializeField] private GameObject privacyText;
    [SerializeField] private GameObject termsText;
    [SerializeField] private GameObject loading;

    private TextType textType;
    
    
    private void OnEnable()
    {
        ToggleElements(loading, true);
        StartCoroutine(ShowTextWithDelay());
    }

    private void OnDisable()
    {
        ToggleElements(loading, true);
        ToggleElements(privacyText, false);
        ToggleElements(termsText, false);
    }

    IEnumerator ShowTextWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.35f);
        ToggleElements(loading, false);
        
        if (textType == TextType.PrivacyPolicy)
        {
            ToggleElements(privacyText, true);
            ToggleElements(termsText, false);
        }
        else
        {
            ToggleElements(privacyText, false);
            ToggleElements(termsText, true);
        }
    }
    
    public void ShowPrivacyPopup()
    {
        ToggleElements(privacyTitle, true);
        ToggleElements(termsTitle, false);
        textType = TextType.PrivacyPolicy;
    }

    public void ShowTermsPopup()
    {
        ToggleElements(privacyTitle, false);
        ToggleElements(termsTitle, true);
        textType = TextType.TermsOfUse;
    }

    private void ToggleElements(GameObject element, bool toggle)
    {
        element.SetActive(toggle);
    }

    public void Close()
    {
        ClosePopup(() =>
        {
            UI_PopupRelayer.Instance.OpenOnHoldPopup();
        });
    }
    protected override bool HandleBackPressed()
    {
        ClosePopup(() =>
        {
            UI_PopupRelayer.Instance.OpenOnHoldPopup();
        });
        return true;
    }
}
