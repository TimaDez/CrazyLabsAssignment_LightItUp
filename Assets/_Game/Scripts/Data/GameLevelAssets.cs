using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightItUp.Game;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using LightItUp.Singletons;

namespace LightItUp.Data
{
    public class GameLevelAssets : SingletonAsset<GameLevelAssets>
    {
    

        public static string LevelsFolderDefault = "GameLevels/";    
        public static string LevelsFolder = LevelsFolderDefault;
    
#if UNITY_EDITOR
        public override void Init()
        {
            if (allLevels == null) 
                allLevels = new List<TextAsset>();
        }
#endif
	
        public List<TextAsset> allLevels;
        
        public void UpdateAllLevels() {
            allLevels.RemoveAll(x => x == null);
            List<TextAsset> t = new List<TextAsset>();
            t.AddRange(Resources.LoadAll<TextAsset>(LevelsFolder));
            allLevels.RemoveAll(x => !t.Contains(x));
            t.RemoveAll(x => allLevels.Contains(x));
            allLevels.AddRange(t);
            for (int i = allLevels.Count - 1; i >= 1; i--)
            {
                for (int u = i-1; u >= 0; u--)
                {
                    if (allLevels[i] == allLevels[u]) {
                        allLevels.RemoveAt(i);
                    }
                }
            }
        }

        public TextAsset GetByUid(string uid)
        {
            return allLevels.FirstOrDefault(x =>
            {
                var loadedLevelData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameLevel.GameLevelData>(x.text);

                return loadedLevelData.uid == uid;
            });
        }
    
#if UNITY_EDITOR
        private void OnEnable()
        {
            UpdateAllLevels();
        }
#endif
    
    
#if UNITY_EDITOR
        public void OnValidate() {
            UpdateAllLevels();
        }
    
        public void InsertLevel(int idx, TextAsset level)
        {
            allLevels.Insert(idx, level);
            UpdateAllLevels();
        }
    
#endif
    
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/GameLevelAssets")]
        public static void CreatePrefabAssets()
        {
            CreateAsset();
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GameLevelAssets))]
    public class GameLevelAssetsEditor : Editor
    {
        ReorderableList list;
        private void OnEnable()
        {
            list = GetList("allLevels", "AllLevels");
        }
        public override void OnInspectorGUI()
        {
            var myTarget = (GameLevelAssets)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Update List"))
            {
                myTarget.OnValidate();
            }
            //serializedObject.Update();
            //list.DoLayoutList();
            //serializedObject.ApplyModifiedProperties();

        }


        public ReorderableList GetList(string propertyName, string header)
        {
            var l = new ReorderableList(serializedObject,
                serializedObject.FindProperty(propertyName),
                true, true, true, true);
            l.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, header);
            };
            l.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var gui = GUI.enabled;
                GUI.enabled = false;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(rect, element, GUIContent.none);
                GUI.enabled = gui;

            };
            l.onCanRemoveCallback = (a) => {
                return false;
            };
            l.onCanAddCallback = (a) => {
                return false;
            };
            return l;
        }
    }
#endif
}