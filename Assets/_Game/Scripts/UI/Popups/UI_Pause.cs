using HyperCasual;
using UnityEngine.UI;
using LightItUp.Data;

namespace LightItUp.UI
{
    public class UI_Pause : UI_Popup
    {

        bool _interactable;
    
        public enum EndCloseAction
        {
            Resume, Restart, Home, Settings
        }

        private EndCloseAction _action = EndCloseAction.Resume;

        public Button btnClose, btnReplay, btnHome, btnResume;

        void Start()
        {
            btnClose.onClick.AddListener(OnClickClose);
            btnReplay.onClick.AddListener(OnClickReplay);
            btnHome.onClick.AddListener(OnClickHome);
            btnResume.onClick.AddListener(OnClickResume);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _action = EndCloseAction.Resume;
            ToggleInteraction(true);
        }

        void ToggleInteraction(bool toggle)
        {
            btnClose.interactable = toggle;
            btnReplay.interactable = toggle;
            btnHome.interactable = toggle;
            btnResume.interactable = toggle;
            _interactable = toggle;
        }

        public override void CloseAnimationComplete()
        {
            base.CloseAnimationComplete();
        
            switch (_action)
            {
                case EndCloseAction.Resume:
                    GameManager.Instance.TogglePause(false);
                    break;
			case EndCloseAction.Restart:     
				LeanTween.cancelAll ();
				GameManager.Instance.TogglePause (false);
				GameManager.Instance.CleanupScene ();
				GameManager.Instance.isReplay = true;
				StatisticsService.EndRun ();
                GameManager.Instance.StartGame();
                    break;
                case EndCloseAction.Home:
                    LeanTween.cancelAll();
                    GameManager.Instance.TogglePause(false);
                    GameManager.Instance.CleanupScene();
                    CanvasController.Open(CanvasController.Panels.Menu);
                    StatisticsService.EndRun();
                    break;
            }       
        }

        public void OnClickResume()
        {
            _action = EndCloseAction.Resume;
            ToggleInteraction(false);
            ClosePopup();
        }

        public void OnClickReplay()
        {
            ToggleInteraction(false);
            _action = EndCloseAction.Restart;
            ClosePopup();
        }

        public void OnClickHome()
        {
            ToggleInteraction(false);
            _action = EndCloseAction.Home;
            ClosePopup();  
        }

        public void OnClickClose()
        {
            OnClickResume();
        }

        protected override bool HandleBackPressed()
        {
            if (!_interactable)
                return true;
        
            return base.HandleBackPressed();
        }
    }
}