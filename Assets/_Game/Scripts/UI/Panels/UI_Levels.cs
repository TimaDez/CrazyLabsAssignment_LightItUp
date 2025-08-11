using System.Collections.Generic;
using UnityEngine;
using LightItUp.Data;
using System;
using TMPro;
// using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using System.Collections;
using LightItUp.Currency;

namespace LightItUp.UI
{
    public class UI_Levels : MonoBehaviour {

        private readonly string LAST_LEVEL_SEEN_KEY = "LastLevelSeen";

        public UI_LevelElement levelElementTemplate;
        [SerializeField]private RectTransform content; 

        StarGatesManager starGatesManager;

        public GameObject starGateElement;

        private GameObject subscriptionStarGate;        // Subscription SG Instance
        public GameObject subscribtionStarGatePrefab;   // Subscription SG Prefab

        List<GameObject> starGatesList = new List<GameObject>();
        List<GameObject> subscriptionSGList = new List<GameObject>();
		List<StarGate> starGates = new List<StarGate> ();

		List<UI_LevelElement> elements;
      
        [HideInInspector]
        public bool forceUpdate;
    
		private float levelSize;
		private float gateSize;
		
		private float stargatePaddingSize = 10f;
		private float borderTop = 120f;
		private float borderBot = 120f;
		private float lastScrollValue;
        private float totalGateSize = 0f;
        private float stargateSize = 0f;

        public float topPadding = 30f;
        public float sidesPadding = 0f;
        public float bottomPadding = 0f;

//        public ScrollRect scrollRect;
        private float totalContentSize;
        private bool scrollPositioned;

        private bool currentSubscriptionState = false;

        public void Awake()
        {
            GameManager.Instance.OnAutoScroll += AutoScroll;
            
			elements = new List<UI_LevelElement>();
			InstantiateLevelObjects();
        }

        void InstantiateLevelObjects()
        {
            elements = new List<UI_LevelElement>();

            for (int i = 0; i < GameLevelAssets.Instance.allLevels.Count; i++)
            {
                elements.Add(Instantiate(levelElementTemplate, levelElementTemplate.transform.parent));
            }

        }
        public void ScrollBarChange()
        {
//            Debug.Log("------------------- ScrollBarChange:: " + content.position.y);
        }
        private void OnEnable()
        {
            
            Time.timeScale = 1;
            
            starGatesList.Clear();
            subscriptionSGList.Clear(); 
            
			CanvasController.TogglePresenter(true, 1, false, CanvasController.Popups.Empty);
            
            
            totalGateSize = 0f;
            
            if(!StarGatesManager.Instance.disableGatesAB)
            {
                InstantiateGateObjects();
            }
            
            CollectItemsInfo ();
            SetScrollSize ();
           
            PositionLevelElements ();
            PositionScrollView(totalContentSize);

        }

        private void OnDisable()
        {
            CanvasController.TogglePresenter(false, 1);
            UnloadStargates();
            SetLastLevelSeen();
        }

        private void OnSubscriptionStatusChange(bool subscriptionState)
        {
            if (subscriptionState != currentSubscriptionState)
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
                
                currentSubscriptionState = subscriptionState;
            }
            
//            if (subscriptionState)
//            {
//                gameObject.SetActive(false);
//                gameObject.SetActive(true);
//            }
        }
        
        void CollectItemsInfo()
        {
            RectTransform elementRt = (RectTransform)levelElementTemplate.transform;
            levelSize = elementRt.rect.size.y;
            
            RectTransform gateRt = (RectTransform)starGateElement.transform;
            gateSize = gateRt.rect.size.y + topPadding/2;
            
            starGatesManager = StarGatesManager.Instance;
        }
        
        void SetScrollSize()
        {
            int levelsCount = GameLevelAssets.Instance.allLevels.Count;
            totalContentSize =  (levelsCount - 1) * (levelSize + topPadding) + ActiveStarGatesSize() + bottomPadding;
            content.sizeDelta = new Vector2(0f, totalContentSize);
        }

        private void PositionScrollView(float totalContentSize)
        {
            content.anchoredPosition = new Vector2(0, GetItemPosition(GetLastLevelSeen()));
            AutoScroll();
        }

        private float ActiveStarGatesSize()
        {
            float starGatesSize = 0;

            foreach (GameObject starGate in starGatesList)
            {
                if (starGate.activeSelf)
                {
                    starGatesSize += gateSize;
                }
            }

            foreach (GameObject starGate in subscriptionSGList)
            {
                if (starGate.activeSelf)
                {
                    starGatesSize += gateSize * 1.25f;
                }
                
            }

            return starGatesSize;
        }
        
        void PositionLevelElements()
        {
            float starGateOffset = 0f;
            float targetPositionY = -(levelSize/2 + topPadding);

            for (int i = elements.Count-1; i >= 0 ; i--) 
            {
                int currentLevel = i;

                float targetPositionX = (currentLevel) % 2 == 0 ? content.rect.width / 2 - sidesPadding : content.rect.width / 2 + sidesPadding;

                
                
                elements [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (targetPositionX, targetPositionY);

                SetLevel (elements [i], i);
                elements [i].gameObject.SetActive (true);

                
                
                if (!StarGatesManager.Instance.disableGatesAB && starGatesManager.StarGateLockSetup(currentLevel + 1))
                {
                    targetPositionY -= levelSize / 2;
                    starGateOffset = PositionStarGate(currentLevel+ 1, targetPositionY + stargatePaddingSize, -1);

                    targetPositionY -= topPadding + starGateOffset / 2;
                }
                
                targetPositionY += (levelSize + topPadding) * (-1);
                
            }
        }

        private float GetItemPosition(int positionOfLevel)
        {

            Time.timeScale = 1;
            float itemPosition = 0;


            //int lastPlayedLevel = GameData.PlayerData.unlockedLevelIdx;
            int lastPlayedLevel = positionOfLevel;
            float contentPositionY = totalContentSize - topPadding;
            List<StarGateInfo> sgInfoList = StarGatesManager.Instance.GetUnlockedStargates();


            // Scroll to unlockable star gate
            if (sgInfoList != null && sgInfoList.Count >= 1)
            {
                int level = sgInfoList[0].levelLock;
                itemPosition = totalContentSize - (level * (levelSize + topPadding)) + bottomPadding * 1.3f;
            }
            // Scroll to next playable level
            else
            {
                int level = lastPlayedLevel + 1;
                float offset = 1f;
                
                if (lastPlayedLevel % 2 == 0)
                {
                    level = lastPlayedLevel;
                    offset = 0.5f;
                }

                itemPosition = totalContentSize - (level * (levelSize + topPadding)) + bottomPadding * offset;
            }

            StarGatesManager.Instance.CheckForLevelLock(lastPlayedLevel + 1);
            
            return itemPosition;

        }
        private void AutoScroll()
        {
            int unlockedLevel = GameData.PlayerData.unlockedLevelIdx;
            float pos = GetItemPosition(unlockedLevel);
            LeanTween.moveY(content, pos, 2f);
        }
        
        void InstantiateGateObjects()
        {
            stargateSize = 0f;
            
            starGatesList = new List<GameObject>();
            subscriptionSGList = new List<GameObject>();
            
            List<StarGateInfo> sgUInfos = StarGatesManager.Instance.GetUnlockedStargates();
            
            //Init Star Gate List
            for (int i = 0; i < sgUInfos.Count; i++)
            {
                GameObject currentStarGate = Instantiate(starGateElement, levelElementTemplate.transform.parent);
                currentStarGate.GetComponent<StarGate>().SetupStarGate(sgUInfos[i], this);
                starGatesList.Add(currentStarGate);
                starGates.Add(currentStarGate.GetComponent<StarGate>());
            }
            List<StarGateInfo> sgLInfos = StarGatesManager.Instance.GetLockedStargates();

            //Init Subscription Star Gate List
            for (int i = 0; i < sgLInfos.Count; i++)
            {
                
                GameObject currentStarGate = Instantiate(subscribtionStarGatePrefab, levelElementTemplate.transform.parent);
				currentStarGate.GetComponent<StarGate>().SetupStarGate(sgLInfos[i], this);
                subscriptionSGList.Add(currentStarGate);
                starGates.Add(currentStarGate.GetComponent<StarGate>());
            }
            
        }
        
        private void SetLastLevelSeen()
        {
            int level = GameData.PlayerData.unlockedLevelIdx;
            PlayerPrefs.SetInt(LAST_LEVEL_SEEN_KEY, level);
        }
        private int GetLastLevelSeen()
        {
            int level = PlayerPrefs.GetInt(LAST_LEVEL_SEEN_KEY);
            return level;
        }
        private void SetLevel(UI_LevelElement level, int levelIdx)
        {
            level.name = "Level_" + (levelIdx + 1 );
            level.Setup(levelIdx, () =>
            {
                ClickedLevel(levelIdx);
            });
        }

        // Rearange levels when Gate is unlocked
        private void RearrangeLevels(StarGate sg)
		{

            int startingLevel = sg.starGateInfo.levelLock - 1;

            // Unlock locked level
            SetLevel(elements[startingLevel], startingLevel);

            // Activate Dots on Level after new Unlocked level
            SetLevel(elements[startingLevel], sg.starGateInfo.levelLock - 1);
            SetLevel(elements[startingLevel - 1], sg.starGateInfo.levelLock - 2);
            

            // Scale height of Content
            LeanTween.size(content, new Vector2(content.sizeDelta.x, content.sizeDelta.y - (gateSize)), 0.3f);
            
            // Move levels
            for (int i = startingLevel - 1; i >= 0; i--) 
            {

                Vector2 prevPos = elements[i].GetComponent<RectTransform>().localPosition;
                LeanTween.moveLocal(elements[i].gameObject, new Vector3(prevPos.x, prevPos.y + gateSize, 1), 0.3f);
            }
            
            // Move star gates
            for (int i = 0; i < starGatesList.Count; i++)
            {
                if (starGatesList[i].activeSelf && starGatesList[i].GetComponent<StarGate>().starGateInfo.levelLock != startingLevel)
                {
                    Vector2 prevPos = starGatesList[i].GetComponent<RectTransform>().localPosition;
                    LeanTween.moveLocal(starGatesList[i].gameObject, new Vector3(prevPos.x, prevPos.y + gateSize, 1), 0.3f);
                }
            }
            
            // Move subscription star gates
            for (int i = 0; i < subscriptionSGList.Count; i++)
            {
                if (subscriptionSGList[i].activeSelf && subscriptionSGList[i].GetComponent<StarGate>().starGateInfo.levelLock != startingLevel)
                {
                    Vector2 prevPos = subscriptionSGList[i].GetComponent<RectTransform>().localPosition;
                    LeanTween.moveLocal(subscriptionSGList[i].gameObject, new Vector3(prevPos.x, prevPos.y, 1), 0.3f);
                }
            }
            
            // Disable unlocked gate
            LeanTween.delayedCall( 0.4f, () => 
            {
                sg.gameObject.SetActive(false);
            });
        }

        private float PositionStarGate(GameObject newStarGate, float targetPosition, int direction) 
        {
            RectTransform rtStarGate = (RectTransform)newStarGate.transform;
            float starGateSize = rtStarGate.rect.size.y;
            float sgPosY = targetPosition;

            if (newStarGate.CompareTag("SubscriptionStarGate"))
            {
                sgPosY = targetPosition + (starGateSize) * direction + topPadding / 2;
                starGateSize *= 1.3f;
            }
            else
            {
                sgPosY = targetPosition + (starGateSize) * direction + topPadding / 2;
            }


            Vector2 center = new Vector2(0, sgPosY);

            rtStarGate.localPosition = center;

            // Set star gate width
            rtStarGate.sizeDelta = new Vector2(content.rect.width, rtStarGate.rect.height);

            return starGateSize;
        }

        private float PositionStarGate(int currentLevelIdx, float targetPosition, int sign)
        {
            float sgOffset = 0f;
            GameObject targetStarGate = null;
            for (int i = 0; i < starGates.Count; i++) 
            {
                if (starGates[i].starGateInfo.levelLock == currentLevelIdx) 
                {
                    targetStarGate = starGates [i].gameObject;
                }
            }


            sgOffset = PositionStarGate(targetStarGate, targetPosition, sign);
            targetStarGate.SetActive(true);
            
            return sgOffset;
        }
        
        void UnloadStargates()
        {
            for (int i = 0; i < starGates.Count; i++) 
            {
                Destroy (starGates [i].gameObject);
            }
            starGates = new List<StarGate> ();
        }
        

        // Callback from Gate Unlock ( click )
        public void StargateUnlocked(StarGate sg)
        {
            RearrangeLevels(sg);
//            StartCoroutine(StartLevelAfterDelay(sg.starGateInfo.levelLock - 1));
        }
        
        
        // Start Level after StarGate unlock delayed
        private IEnumerator StartLevelAfterDelay(int level)
        {
            yield return new WaitForSeconds(1f);

            if(GameData.PlayerData.unlockedLevelIdx >= level)
            {
                ClickedLevel(level);
            }
        }
        
        public void ClickedLevel(int levelIdx)
        {
            GameManager.Instance.isReplay = false;

//            if (BoosterService.Instance.AreBoostersActive()) 
//            {
//				GameData.PlayerData.selectedLevelIdx = levelIdx;
//
//                GameManager.Instance.OpenPreLevelPopup();
//
//            }
//            else
//			{
//            	GameManager.Instance.StartLevel(levelIdx);
//			}
//            BoosterService.Instance.ConsumeSelectedBoosters();
            GameData.PlayerData.selectedLevelIdx = levelIdx;
            GameManager.Instance.StartGame();
        }
        
        public void GoBack() 
        {
            CanvasController.Open(CanvasController.Panels.Menu);
        }
        void Update()
        {
            if (CanvasController.AnimationCount == 0)
            {
                //if (Input.GetKeyDown(KeyCode.Escape))
                //    GoBack();
            }
        }
        
        // Find Star Gate in list
        private GameObject FindStarGate(int level)
        {
            foreach(StarGate starGate in starGates)
            {
                if(starGate.GetComponent<StarGate>().starGateInfo.levelLock == level)
                {
                    return starGate.gameObject;
                }
            }
            return null;
        }
    }
}
