using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using LightItUp.Data;

namespace LightItUp.Data
{
    public class GameSettings : GameSettingsBase<GameSettings>
    {
        public static BlockSettings Block
        {
            get
            {
                return Instance.block;
            }
        }

        public static InGameSettings InGame
        {
            get
            {
                return Instance.ingame;
            }
        }
        public static PlayerSettings Player
        {
            get
            {
                return Instance.player;
            }
        }
        public static CameraFocusSettings CameraFocus
        {
            get
            {
                return Instance.cameraFocus;
            }
        }
        
        public static LevelSettings Level
        {
            get
            {
                return Instance.level;
            }
        }


        public BlockSettings block;
        public InGameSettings ingame;
        public PlayerSettings player;
        public CameraFocusSettings cameraFocus;
        public LevelSettings level;

        [System.Serializable]
        public class BlockSettings
        {
            public float lineThickness = 0.8f;
            public float spikeThickness = 0.8f;
            public float spikeColliderThickness = 0.2f;
            public float uvScale = 0.25f;
            public int circlePartitions = 32;
            public float polySpikesGraceArea = 0.2f;
            public float blockExplodeDelay = 1;

            public int pointsPlayerLightsUp = 30;
            public int pointsBlockLightsUp = 15;
            public int pointsExplodingPlayerLightsUp = 30;
            public int pointsExplodingBlockLightsUp = 15;
            public int pointsExplodingExplosion = 15;
        }

        [System.Serializable]
        public class InGameSettings
        {
            public LayerMask reviveCheckMask;
            public float reviveYDistance = 2;
            public int levelsPrPage = 10;
            public Vector2 tutorialDoubleJumpTextOffset = new Vector2(2, 2);
            public Vector2 tutorialDoubleJumpTextPivot = new Vector2(0, 0);
            [TextArea]
            public string tutorialDoubleJumpText = "";
            public float tutorialDoubleJumpFontSize = 5;
            public UIFeedbackType hapticFeedbackPlayerDead = UIFeedbackType.ImpactLight;
            public UIFeedbackType hapticFeedbackCompleteLevel = UIFeedbackType.ImpactLight;
            public UIFeedbackType hapticFeedbackLightUpBlock = UIFeedbackType.ImpactLight;
            public UIFeedbackType hapticFeedbackChangedSetting = UIFeedbackType.ImpactLight;

        }
        [System.Serializable]
        public class PlayerSettings
        {
            public Vector2 jumpPower = new Vector2(7.2f, 18);
            public float gravityScale = 2.5f;
            public float killPlayerY = 10;
            public bool reduceGravityWhenHoldingJump = false;
            public float reduceGravityScale = 0.5f;
			public float maxReviveDistance = 10;
			public float reviveInvulnerabilityDuration = 5f;
			public float blinkSpeed = 0.2f;
			public float slowDownTarget = 0.5f;
        }
        [System.Serializable]
        public class CameraFocusSettings
        {
            public float playerBorder = 5;
            public float playerZoomBorder = 1;
            public float blockBorder = 0;
            public float blockZoomBorder = 0;
            public float blockExplodePartsBorder = 1;
            public float blockExplodePartsFocusDuration = 3;
            public float blockExplodePartsFocusFadeDuration = 0.5f;
            public float wallBorder = 0;
            public float wallZoomBorder = 0;
            public float blendTime = 1.8f;
            public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public float zPosition = -10;
            public float minOrthoSize = 10;
            [Range(0f, 0.9999f)]
            public float dampeningX = 0.8f;
            [Range(0f, 0.9999f)]
            public float dampeningY = 0.9f;
            [Range(0f, 0.9999f)]
            public float dampeningOrtho = 0.95f;
            public float zoomDirectionChangeSpeed = 20f;
            public bool sortBlocksByPriority = false;
            public float zoomOutGameOverDuration = 1;
            public float zoomOutWinDuration = 2;
        }

        [System.Serializable]
        public class LevelSettings
        {
            public Color safeColor = Color.green;
            public Color unsafeColor = Color.red;
            public bool drawZonesHighlights = true;
        }

        public class SavedSettings
        {
            public BlockSettings block;
            public InGameSettings ingame;
            public PlayerSettings player;
            public CameraFocusSettings cameraFocus;
            public LevelSettings levelSettings;
        }

        protected override string SaveJson()
        {
            SavedSettings ss = new SavedSettings
            {
                block = block,
                ingame = ingame,
                player = player,
                cameraFocus = cameraFocus
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(ss, Newtonsoft.Json.Formatting.Indented);
        }
        protected override void LoadJson(string json)
        {
            SavedSettings ss = Newtonsoft.Json.JsonConvert.DeserializeObject<SavedSettings>(json);
            block = ss.block;
            ingame = ss.ingame;
            player = ss.player;
            cameraFocus = ss.cameraFocus;
        }



#if UNITY_EDITOR
        [MenuItem("Assets/Create/GameSettings")]
        public static void CreateGameSettings()
        {
            CreateAsset();
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GameSettings))]
    public class GameSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameSettings myTarget = (GameSettings)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Load from json"))
            {
                myTarget.Load();
            }
            if (GUILayout.Button("Save to json"))
            {
                myTarget.Save();
            }
        }
    }
#endif
}