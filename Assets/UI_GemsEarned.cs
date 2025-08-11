using System.Collections;
using System.Collections.Generic;
using LightItUp.Currency;
using TMPro;
using UnityEngine;
// using UnityEngine.Experimental.UIElements;
using UnityEngine.UIElements;

public class UI_GemsEarned : MonoBehaviour
{
    [SerializeField] private Button storeButton;
    [SerializeField] private TextMeshProUGUI earnedText;

    private string originalText;
    
    private void Awake()
    {
        originalText = earnedText.text;
    }

    public void ShowGemsEarned(int starsCount)
    {
        if (starsCount > 0)
        {
            gameObject.SetActive(true);
            int earnedAmount = starsCount * 20;
            SetText(earnedAmount);
            StartCoroutine(AddEarnedCurrency(earnedAmount));
         }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

    private IEnumerator AddEarnedCurrency(int earnedAmount)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        CurrencyService.Instance.AddCurrency(CurrencyType.Gems, earnedAmount, "", 0);
    }

    private void SetText(int earnedAmount)
    {
        earnedText.text = originalText.Replace("#", earnedAmount.ToString());
    }
}
