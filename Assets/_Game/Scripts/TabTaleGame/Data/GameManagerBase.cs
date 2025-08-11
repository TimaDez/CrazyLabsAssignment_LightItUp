using LightItUp.Data;
using LightItUp.Singletons;
using LightItUp.UI;
using LightItUp.UI;

namespace LightItUp.Data
{
    public class GameManagerBase<T> : SingletonCreate<T> where T : GameManagerBase<T>
    {
        protected virtual void Start()
        { 
        }

		protected virtual void LateUpdate()
		{
		}

		protected virtual void LateUpdatePopUpCheck(UI_GameEnded gameEndPopup)
        {
            var game = CanvasController.GetPanel<UI_Game>();
			if (CanvasController.AnimationCount <= 0 && !game.gameObject.activeSelf && !gameEndPopup.gameObject.activeSelf)
            {
                GameData.PlayerData.SaveQueued();
                GameData.SaveQueued();
            }        
        }

        private void OnApplicationQuit()
        {
            GameData.PlayerData.SaveData();
            GameData.SaveGameData();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                GameData.PlayerData.SaveData();
                GameData.SaveGameData();
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameData.PlayerData.SaveData();
                GameData.SaveGameData();
            }
        }
    }
}
