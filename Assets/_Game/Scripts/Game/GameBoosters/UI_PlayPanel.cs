using LightItUp;
using LightItUp.Data;
using LightItUp.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayPanel : MonoBehaviour
{

    public UI_GameEnded gameEndedPopUp;
    
    public List<UI_BoosterButton> boosterButtons;
    [SerializeField] Button play;

    private void Awake()
    {
        BoosterService.Instance.OnBoosterAmountChange += InitButtons;
    }

    private void OnEnable()
    {
        InitButtons();
        
    }


    void InitButtons()
    {
        for (int i = 0; i < boosterButtons.Count; i++)
        {
            boosterButtons[i].InitButton(
                BoosterService.Instance.IsBoosterUnlocked(boosterButtons[i].boosterType),
                BoosterService.Instance.GetMandatoryBoosterAmmount(boosterButtons[i].boosterType) > 0,
                BoosterService.Instance.GetTotalBoosterAmmount(boosterButtons[i].boosterType),
                gameEndedPopUp
            );
        }

    }

    
    public void NextLevel()
    {
        GameManager.Instance.StartGame(true);
    }
    
    public void RetryLevel()
    {
        
    }
    List<BoosterType>  GetSelectedBoosters ()
    {
        List<BoosterType> selectedBoosters = new List<BoosterType> ();
        for (int i = 0; i < boosterButtons.Count; i++) {
				
            if (boosterButtons[i].isSelected) {
                selectedBoosters.Add (boosterButtons [i].boosterType);
            }
        }
        return selectedBoosters;
    }
}
