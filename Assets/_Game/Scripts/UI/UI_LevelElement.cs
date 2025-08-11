using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LightItUp.Data;
using System;

namespace LightItUp.UI
{
    public class UI_LevelElement : MonoBehaviour {
        public Sprite starFilled;
        public Sprite starEmpty;
        public Button button;
        public TMPro.TextMeshProUGUI text;
        public GameObject lockObject;
        public List<Image> stars;
        public GameObject imageLevelPassed;
        
		private RectTransform rectTrans;

        [SerializeField] private GameObject leftDots;
        [SerializeField] private GameObject rightDots;

        public int level;

        private bool levelUnlocked = false;
        private StarGateInfo sgInfo;

        private Action<float, System.Object> Filler;
        private bool gatePrevUnlocked = false;

		void Awake()
		{
			rectTrans = GetComponent<RectTransform> ();
		}

//        private void OnEnable()
//        {
//            DeactivateLevel();
//        }

        public void Setup(int levelIdx, UnityAction onClicked) 
        {
            level = levelIdx;

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(onClicked);

            text.text = "" + (1 + levelIdx);

            // Level unlocked by gameplay
            levelUnlocked = levelIdx <= GameData.PlayerData.unlockedLevelIdx;

            // Check if level is locked by Star Gate
            bool sgLock = StarGatesManager.Instance.StarGateLockSetup(levelIdx+1);

            if (sgLock && levelUnlocked) levelUnlocked = false;

            button.interactable = levelUnlocked;
            button.gameObject.SetActive(levelUnlocked);
            lockObject.SetActive(!levelUnlocked);
            
            
//            ActivateLevel();
            SetupDots();
            
            var highscore = GameData.PlayerData.GetLevelHighscore(levelIdx);

            for (int i = 0; i < stars.Count; i++)
            {
                if (levelIdx <= GameData.PlayerData.unlockedLevelIdx)
                {
                    stars[i].sprite = (highscore != null && i < highscore.stars) ? starFilled : starEmpty;
                }
                else
                {
                    stars[i].sprite = starEmpty;
                }
            
            }
            
//            ActivateLevel();
        }

        public void SetupDots()
        {
            // Is next level locked with Star Gate
            bool checkNextGate = StarGatesManager.Instance.IsStarGateActive(level+2);
            bool gateUnlocked = false;

            // Check if the dots animation has already played on unlock
            if (checkNextGate)
            {
                sgInfo = StarGatesManager.Instance.FindStarGateInfoByLevel(level + 2);
                gateUnlocked = StarGatesManager.Instance.IsSGUnlocked(sgInfo);

                if(PlayerPrefs.HasKey("GatePrevUnlocked" + sgInfo.levelLock))
                {
                    if (PlayerPrefs.GetInt("GatePrevUnlocked" + sgInfo.levelLock) == 1)
                    {
                        gatePrevUnlocked = true;
                    }
                    else gatePrevUnlocked = false;
                }
                else gatePrevUnlocked = false;
            }
            
            // If Gates are disabled by subscription but Gate exists and its first time opened
            if(StarGatesManager.Instance.disableGatesAB && checkNextGate && !gatePrevUnlocked)
            {
                SavePreviouslyUnlockedGate(sgInfo.levelLock);

                if (GameData.PlayerData.unlockedLevelIdx + 1 >= sgInfo.levelLock)
                {
                    StarGatesManager.Instance.SaveUnlockedGate(sgInfo); 
                }
                ActivateDotsAnimation(true);
            }
            // If gate is getting unlocked first time without subscription
            else if (checkNextGate && gateUnlocked && !gatePrevUnlocked)
            {
                SavePreviouslyUnlockedGate(sgInfo.levelLock);
                ActivateDotsAnimation(true);
            }
            // If gate exists but is not unlocked
            else if (checkNextGate && !gateUnlocked)
            {
                leftDots.SetActive(false);
                rightDots.SetActive(false);
                
            }
            //If dots animation was previously played
            else if(checkNextGate && gateUnlocked && gatePrevUnlocked)
            {
                ActivateDotsAnimation(false);
            }
            //If there is no gate
            else if(!checkNextGate)
            {
                ActivateDotsAnimation(false);
            }
        }
        private void SavePreviouslyUnlockedGate(int level)
        {
            PlayerPrefs.SetInt("GatePrevUnlocked" + level, 1);
        }
        private void ActivateDotsAnimation(bool animate)
        {
            if (level % 2 != 0)
            {
                DotsAnimation(leftDots, animate, true);
                DotsAnimation(rightDots, animate, false);
            }
            else
            {
                DotsAnimation(leftDots, animate, false);
                DotsAnimation(rightDots, animate, true);
            }
        }
        private void DotsAnimation(GameObject dots, bool isAnimate, bool isActive)
        {
            if(isAnimate && isActive)
            {
                Filler += FillDots;
                dots.SetActive(isActive);
                LeanTween.value(dots, Filler, 0, 1, 1f);
            }
            else
            {
                dots.GetComponent<Image>().fillAmount = 1;
                dots.SetActive(isActive);
            }
        }
        private void FillDots(float value, System.Object dots)
        {
            GameObject d = (GameObject)dots;
            d.GetComponent<Image>().fillAmount = value;

            if(d.GetComponent<Image>().fillAmount == 1)
            {
                Filler -= FillDots;
            }
        }
        
        
        
        [SerializeField] private GameObject dots;
        [SerializeField] private GameObject image;
        [SerializeField] private GameObject starsGroup;

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            ActivateLevel();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            DeactivateLevel();
        }

        private void ActivateLevel()
        {
            if (!lockObject.activeSelf)
            {
                dots.SetActive(true);
                if (level < GameData.PlayerData.unlockedLevelIdx || level == GameLevelAssets.Instance.allLevels.Count-1)
                {
                    imageLevelPassed.SetActive(true);
                    text.color = Color.black;
                }
                image.SetActive(true);
                starsGroup.SetActive(true);
            }
            else
            {
                DeactivateLevel();
                starsGroup.SetActive(true);
            }
        }
        private void DeactivateLevel()
        {
            dots.SetActive(false);
            image.SetActive(false);
            starsGroup.SetActive(false);
        }
    }
}
