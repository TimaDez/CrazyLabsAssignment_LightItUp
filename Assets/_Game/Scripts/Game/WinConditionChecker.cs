using HyperCasual.PsdkSupport;
using LightItUp.Currency;
using UnityEngine;
using LightItUp.Data;
using LightItUp.UI;

namespace LightItUp.Game
{
    public static class WinConditionChecker {

        public static event System.Action OnWinGame;
        public static event System.Action OnLitChanged;
        public static bool GameOver {
            get {
                return gameOver;
            }
        }
        static bool gameOver = false;
        private static void OnLitStateChanged() {
            if (gameOver || GameManager.Instance.currentLevel.player.PlayerDead)
                return;
        
            if (OnLitChanged != null) 
                OnLitChanged();
        
            GameData.PlayerData.ValueWasUpdated();

            CheckWinCondition();
        }

        public static void CheckWinCondition()
        {
            foreach (var bl in GameManager.Instance.currentLevel.blocks)
            {
                if (!bl.IsLit && bl.mustBeListToWin)
                    return;
            }
        
            foreach (var bl in GameManager.Instance.currentLevel.explodeParts)
            {
                if (!bl.IsLit && bl.mustBeListToWin)
                    return;
            }
        
            Win();
        }

        private static void Win()
        {        
            gameOver = true;
            GameManager.Instance.currentLevel.player.ChangeState(PlayerController.State.Win);
			CinemachineController.Instance.AnnounceGameState (GameState.EndLevel);
            HapticFeedback.Generate(GameSettings.InGame.hapticFeedbackCompleteLevel);
            GameManager.Instance.currentLevel.ZoomOutCamera();
            GameData.PlayerData.wonLastGame = true;
            
            ActionRunner.WaitAndRun(GameSettings.CameraFocus.zoomOutGameOverDuration + 1,  OpenGameEndedPopup);
        }


        private static void OpenGameEndedPopup()
        {
            GameManager.Instance.currentLevel.PlayCelebration();
            var game = CanvasController.GetPanel<UI_Game>();
            game.Hide();
            GameManager.Instance.OpenGameEnded(true);
        }
    

        public static void Revive()
        {
            gameOver = false;
        }
        public static void LoadNextLevel() {
            //All blocks lit, win game
            Debug.Log("Won game!");
            if (OnWinGame != null)
            {
                OnWinGame();
            }
        }

        public static void Add(BlockController block) {
//        blocks.Add(block);
            block.OnLitStateChanged += OnLitStateChanged;
        }

        public static void Reset() {
            OnWinGame = null;
            //_blocks = new List<BlockController>();
            //blocks.Clear();
            gameOver = false;
        }
    }
}
