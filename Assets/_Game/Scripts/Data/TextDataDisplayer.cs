using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;

namespace LightItUp.Data
{
    public class TextDataDisplayer : TextDataDisplayerBase<TextDataDisplayer.Values>
    {

        public enum Values
        {
            Game_BlocksLitUp = 0,
            Game_BlocksTotal = 1,
            Game_CurrentLevel = 2,
            Game_HighestHighscore = 3,

            Ingame_Points = 100,



            Application_VersionNumber = 800,
        }
        public override string GetValue(Values value)
        {
            var gameLevel = GameManager.Instance.currentLevel;
            switch (value)
            {
                // Game
                case Values.Game_BlocksLitUp:
                    if (gameLevel != null)
                    {
                        return "" + gameLevel.LitBlockCount;
                    }
                    return "0";
                case Values.Game_BlocksTotal:
                    if (gameLevel != null)
                    {
                        return "" + gameLevel.blocks.Count;
                    }
                    return "0";

                case Values.Game_CurrentLevel:
                    return "" + (GameData.PlayerData.selectedLevelIdx + 1);

                case Values.Game_HighestHighscore:
                    return "" + GameData.PlayerData.GetHighestHighscore();

                case Values.Ingame_Points:
                    return ""+GameData.PlayerData.ingamePoints;


                //Application
                case Values.Application_VersionNumber:
                    return "" + Application.version;

                default:
                    Debug.LogError("No value with type: " + value.ToString(), gameObject);
                    return "";
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TextDataDisplayer))]
    public class TextDataDisplayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            TextDataDisplayer myTarget = (TextDataDisplayer)target;
            DrawDefaultInspector();
            myTarget.DrawLocalizerDropdown(serializedObject);
        }
    }
#endif
}