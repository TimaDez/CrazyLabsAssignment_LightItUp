using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;

namespace LightItUp.Data
{
    [RequireComponent(typeof(Image))]
    public class ImageDataDisplayer : ImageDataDisplayerBase<ImageDataDisplayer.ImageData> {
        public enum ImageData
        {
            Game_BlockProgress  = 0,

            Ingame_Star1 = 100,
            Ingame_Star2,
            Ingame_Star3,
            Ingame_StarProgress
        }
        bool isFilling = false;
        Coroutine fillRoutine;

        protected override void OnDataChanged()
        {
            var oldSprite = image.sprite;
            base.OnDataChanged();

            if (image.sprite != oldSprite)
            {
                switch (data)
                {
                    case ImageData.Ingame_Star1:
                    case ImageData.Ingame_Star2:
                    case ImageData.Ingame_Star3:
                        image.transform.localScale = Vector3.one;
                        LeanTween.scale(image.GetComponent<RectTransform>(), Vector3.one * 1.3f, 0.15f).setEaseInOutSine().setLoopPingPong(1);
                        break;
                }
            }
            
        }

        protected override Sprite GetSprite()
        {
            switch (data)
            {
                //          case ImageData.Current_Block_Level:
                //              return SpriteAssets.Instance.GetBlockLevelUI(GameData.PlayerData.buildingLevel - 1);


                case ImageData.Ingame_Star1:
                    return GameData.PlayerData.starsCollectedInLevel > 0 ? SpriteAssets.Instance.starFilled : SpriteAssets.Instance.starEmpty;

                case ImageData.Ingame_Star2:
                    // return GameData.PlayerData.jumpCount <= 150 ? SpriteAssets.Instance.starFilled : SpriteAssets.Instance.starEmpty; //TEMP HACK 
                    return GameData.PlayerData.starsCollectedInLevel > 1 ? SpriteAssets.Instance.starFilled : SpriteAssets.Instance.starEmpty;

                case ImageData.Ingame_Star3:
                    // return GameData.PlayerData.jumpCount <= 100? SpriteAssets.Instance.starFilled : SpriteAssets.Instance.starEmpty; //TEMP HACK 
                    return GameData.PlayerData.starsCollectedInLevel > 2 ? SpriteAssets.Instance.starFilled : SpriteAssets.Instance.starEmpty;

                default:
                    return image.sprite;

            }
        }

        protected override void SetupImageSettings()
        {
            var gameLevel = GameManager.Instance.currentLevel;
            // int star1 = 150;//  TEMP HACK 
            int star1 = gameLevel != null ? gameLevel.jumps1Star : 150;
            // int star2 = 100;// TEMP HACK
            int star2 = gameLevel != null ? gameLevel.jumps2Star : 100;
        
            switch (data)
            {
                case ImageData.Game_BlockProgress:
                    float f = 0;
                    if (gameLevel != null)
                    {
                        f = gameLevel.LitBlockCount;
                        f /= gameLevel.blocks.Count;
                    }
                    //image.fillAmount = f;
                    if (isFilling)
                        ActionRunner.StopSpecificCoroutine(fillRoutine);
                    fillRoutine = ActionRunner.Run(FillProgress(image.fillAmount, f));
                    break;

                case ImageData.Ingame_StarProgress:
                    if (gameLevel == null) return;
                    int count = GameData.PlayerData.jumpCount;
                    float fill = 0;
                    if (count >= star1)
                    {
                        fill = 1f / 3;
                    }
                    else if (count >= star2)
                    {
                        float a = count - star2;
                        a /= star1 - star2;
                        fill = Mathf.SmoothStep(1f / 3, 2f / 3, 1-a);
                    }
                    else
                    {
                        float a = count;
                        a /= star2;
                        fill = Mathf.SmoothStep(2f / 3, 1, 1-a);
                    }
                   
                    break;



                default:
                    return;

            }
        }
        public IEnumerator FillProgress(float preFill, float fill)
        {
            float t = 0;
            isFilling = true;
            while (t  < 1)
            {
                image.fillAmount = Mathf.Lerp(preFill, fill, t);
                t += Time.deltaTime*2;
                yield return null;
            }
            isFilling = false;
        }
    }
}

