using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Sound;

namespace LightItUp.UI
{
    public class UI_Settings : UI_Popup
    {
        [SerializeField] private Button privacyButton;
        [SerializeField] private Button termsOfUseButton;

        bool off;
        public Image controlButton;
        public Image soundBtnImage;
        public Image musicBtnImage;
        public Image vibrateBtnImage;
        public float fadedButtonAlpha = 0.5f;

        private void Awake()
        {
            privacyButton.onClick.AddListener(ShowPrivacyPopup);
            termsOfUseButton.onClick.AddListener(ShowTermsOfUsePopup);
        }

        private void OnDestroy()
        {
            privacyButton.onClick.RemoveListener(ShowPrivacyPopup);
            termsOfUseButton.onClick.RemoveListener(ShowTermsOfUsePopup);
        }

        private void ShowTermsOfUsePopup()
        {
            UI_PopupRelayer.Instance.PutPopupOnHold (CanvasController.Popups.Settings);
            CanvasController.Open (CanvasController.Popups.PrivacyPopup);
            CanvasController.GetPopup<UI_PrivacyPopup> ().ShowTermsPopup();
        }

        private void ShowPrivacyPopup()
        {
            UI_PopupRelayer.Instance.PutPopupOnHold (CanvasController.Popups.Settings);
            CanvasController.Open (CanvasController.Popups.PrivacyPopup);
            CanvasController.GetPopup<UI_PrivacyPopup> ().ShowPrivacyPopup();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ControlBtnColor();
            SetSoundImages();
            SetVibrateColor();
        }
        public override void CloseAnimationComplete()
        {
            base.CloseAnimationComplete();
            GameManager.Instance.CleanupScene();
        }
        private void OnDisable()
        {
            var uiMenu = CanvasController.GetPanel<UI_Menu>();
        
            if (!uiMenu.gameObject.activeSelf)
            {
                CanvasController.Open(CanvasController.Panels.Menu);
            }
        }
        public void ToggleSound()
        {
            GameData.PlayerData.soundEnabled = !GameData.PlayerData.soundEnabled;
            SetSoundImages();
        }
        public void ToggleMusic()
        {
            GameData.PlayerData.musicEnabled = !GameData.PlayerData.musicEnabled;
            if (!GameData.PlayerData.musicEnabled)
            {
                SoundManager.StopMusic();
            }
            else
            {
                SoundManager.PlayMusic("BGMusic");
            }
            SetSoundImages();
        }

        public void ToggleControlTutorial()
        {
            GameData.PlayerData.showControlsTutorial = !GameData.PlayerData.showControlsTutorial;
            ControlBtnColor();
        }
        public void ToggleVibrate() {
            GameData.PlayerData.vibrateEnabled = !GameData.PlayerData.vibrateEnabled;
            HapticFeedback.Generate(GameSettings.InGame.hapticFeedbackChangedSetting);
            SetVibrateColor();
        }

        void ControlBtnColor()
        {
            var t = controlButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.color = new Color(t.color.r, t.color.g, t.color.b, GameData.PlayerData.showControlsTutorial ? 1 : 0.2f);
            controlButton.color = new Color(controlButton.color.r, controlButton.color.g, controlButton.color.b, GameData.PlayerData.showControlsTutorial ? 1 : fadedButtonAlpha);
        }
        void SetSoundImages()
        {
            soundBtnImage.color = new Color(soundBtnImage.color.r, soundBtnImage.color.g, soundBtnImage.color.b, GameData.PlayerData.soundEnabled ? 1 : fadedButtonAlpha);
            musicBtnImage.color = new Color(musicBtnImage.color.r, musicBtnImage.color.g, musicBtnImage.color.b, GameData.PlayerData.musicEnabled ? 1 : fadedButtonAlpha);
        }
        void SetVibrateColor() {
            vibrateBtnImage.color = new Color(vibrateBtnImage.color.r, vibrateBtnImage.color.g, vibrateBtnImage.color.b, GameData.PlayerData.vibrateEnabled ? 1 : fadedButtonAlpha);
        }


//        protected override bool HandleBackPressed()
//        {
////            return PSDKWrapper.OnBackPressed();        
//        }
    }
}
