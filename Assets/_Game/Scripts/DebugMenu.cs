using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Game;

namespace LightItUp
{
    public class DebugMenu : MonoBehaviour {
        public Button toggleDebug;
        public TMPro.TMP_InputField orthoMin;
        public TMPro.TMP_InputField orthoMax;
        public TMPro.TMP_InputField orthoZoomSpeed;
        public TMPro.TMP_InputField orthoZoomChangeSpeed;
        public TMPro.TMP_InputField followDampeningX;
        public TMPro.TMP_InputField followDampeningY;
        public Toggle toggleAutoZoomToFit;
        public Toggle linearJumping;
        public Toggle playerCenterJumping;
        public Toggle jumpThroughWalls;
        public GameObject[] gos;
        // Use this for initialization
        void OnEnable () {
            orthoMin.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            orthoMax.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            orthoZoomSpeed.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            orthoZoomChangeSpeed.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            followDampeningX.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            followDampeningY.onValueChanged = new TMPro.TMP_InputField.OnChangeEvent();
            toggleAutoZoomToFit.onValueChanged = new Toggle.ToggleEvent();
            linearJumping.onValueChanged = new Toggle.ToggleEvent();
            playerCenterJumping.onValueChanged = new Toggle.ToggleEvent();
            jumpThroughWalls.onValueChanged = new Toggle.ToggleEvent();

            orthoMin.onValueChanged.AddListener(OrthoMinChanged);
            orthoMax.onValueChanged.AddListener(OrthoMaxChanged);
            orthoZoomSpeed.onValueChanged.AddListener(OrthoZoomSpeedChanged);
            orthoZoomChangeSpeed.onValueChanged.AddListener(OrthoZoomChangeSpeedChanged);
            followDampeningX.onValueChanged.AddListener(DampeningXChanged);
            followDampeningY.onValueChanged.AddListener(DampeningYChanged);
            toggleAutoZoomToFit.onValueChanged.AddListener(ToggledAutoZoomToFit);
            linearJumping.onValueChanged.AddListener(ToggledLinearJump);
            playerCenterJumping.onValueChanged.AddListener(ToggledPlayerCenterJump);
            jumpThroughWalls.onValueChanged.AddListener(ToggledJumpThroughWalls);


            orthoMin.text = "" + GameData.PlayerData.game_OrthographicMin;
            orthoMax.text = "" + GameData.PlayerData.game_OrthographicMax;
            orthoZoomSpeed.text = "" + GameData.PlayerData.game_OrthographicZoomSpeed;
            orthoZoomChangeSpeed.text = "" + GameData.PlayerData.game_OrthographicZoomChangeDirectionSpeed;
            followDampeningX.text = "" + GameData.PlayerData.game_DampeningX;
            followDampeningY.text = "" + GameData.PlayerData.game_DampeningY;
            toggleAutoZoomToFit.isOn = GameData.PlayerData.autoZoomToShow;
            linearJumping.isOn = GameData.PlayerData.useStraightJumping;
            playerCenterJumping.isOn = GameData.PlayerData.usePlayerCenterJumping;
            jumpThroughWalls.isOn = GameData.PlayerData.useJumpThroughWalls;

            toggleDebug.onClick = new Button.ButtonClickedEvent();
            toggleDebug.onClick.AddListener(ToggleGos);

            gos[0].SetActive(true);
            ToggleGos();
        }
        public void ToggleGos() {
            var a = !gos[0].activeSelf;
            foreach (var f in gos) {
                f.SetActive(a);
            }
        }

        void OrthoMinChanged(string s)
        {
            GameData.PlayerData.game_OrthographicMin = float.Parse(s);
        }
        void OrthoMaxChanged(string s)
        {
            GameData.PlayerData.game_OrthographicMax = float.Parse(s);
        }
        void OrthoZoomSpeedChanged(string s)
        {
            GameData.PlayerData.game_OrthographicZoomSpeed = float.Parse(s);
        }
        void OrthoZoomChangeSpeedChanged(string s)
        {
            GameData.PlayerData.game_OrthographicZoomChangeDirectionSpeed = float.Parse(s);
        }


        void DampeningXChanged(string s)
        {
            GameData.PlayerData.game_DampeningX = float.Parse(s);
        }
        void DampeningYChanged(string s)
        {
            GameData.PlayerData.game_DampeningY = float.Parse(s);
        }
        void ToggledAutoZoomToFit(bool val) {
            GameData.PlayerData.autoZoomToShow = val;
        }
        void ToggledLinearJump(bool val)
        {
            GameData.PlayerData.useStraightJumping = val;
        }
        void ToggledPlayerCenterJump(bool val)
        {
            GameData.PlayerData.usePlayerCenterJumping = val;
        }
        void ToggledJumpThroughWalls(bool val)
        {
            GameData.PlayerData.useJumpThroughWalls = val;
        }
        public void LoadNextLevel() {
            WinConditionChecker.LoadNextLevel();
        }
    }
}
