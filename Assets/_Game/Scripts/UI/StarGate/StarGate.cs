using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.PsdkSupport;
using LightItUp.Data;
using LightItUp.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Currency;

namespace LightItUp
{
    public class StarGate : MonoBehaviour
    {
        private const string UNLOCK_TRIGGER = "CloseBorder";

        public StarGateInfo starGateInfo;
        private Action OnUnlockListener;

        TextMeshProUGUI starsAmountText;
        Button unlockButton;
		UI_Levels levelsCtrl;

        public event Action StarGateUnlocked = () => { };

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            unlockButton = transform.GetComponentInChildren<Button>();
        }
        private void OnEnable()
        {
            //StarGateUnlocked += OnUnlockListener;
            //StarGateUnlocked += StarGatesManager.Instance.SendStarGateDEvent();
            if (unlockButton != null) unlockButton.onClick.AddListener(AnimateGate);

        }
        private void OnDisable()
        {
           // StarGateUnlocked -= OnUnlockListener;
            if(unlockButton != null) unlockButton.onClick.RemoveListener(AnimateGate);
        }

		public void SetupStarGate(StarGateInfo starGateInfo, UI_Levels levelsCtrl)
        {
            this.starGateInfo = starGateInfo;

			this.levelsCtrl = levelsCtrl;

            starsAmountText = transform.GetComponentInChildren<TextMeshProUGUI>();
            starsAmountText.text = GetStarsAmountText();
            
        }
		
        //Called on button click
        private void AnimateGate()
        {
            // Stars unlock
			if (GetTotalStars () >= starGateInfo.starsRequired) 
            {
				animator.SetTrigger (UNLOCK_TRIGGER);
			}
           
        }
        // Call on Animation ended event
        private void UnlockStarGate()
        {
            StarGatesManager.Instance.SaveUnlockedGate(starGateInfo);
            StarGatesManager.Instance.SendStarGateDEvent(starGateInfo.levelLock, true);   //Send Delta Event
            levelsCtrl.StargateUnlocked(this);
        }

        public string GetStarsAmountText()
        {
            return GetTotalStars() + "/" + starGateInfo.starsRequired;
        }
        public static int GetTotalStars()
        {
            return GameData.PlayerData.TotalStarsAllLevels;
        }
        
    }

}
