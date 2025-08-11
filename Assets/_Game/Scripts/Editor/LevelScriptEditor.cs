using System;
using System.Collections.Generic;
using LightItUp.Data;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace LightItUp.Game
{
    [CustomEditor(typeof(GameLevel))]
    public class LevelScriptEditor : UnityEditor.Editor
    {
        #region menu_items
        [MenuItem("GameObject/GameLevel", false, -2)]
        static void CreateGameLevel()
        {
            var gl = Resources.Load<GameLevel>("GameLevel");
            var p = Instantiate(gl);
            p.name = gl.name;
            Selection.activeGameObject = p.gameObject;
            Undo.RegisterCreatedObjectUndo(p.gameObject, "Created GameLevel");
        }
        
        [MenuItem("Custom/Models/Set Anim Unscaled", false, 1)]
        static void SetAnimUnscaled()
        {
            var models = Resources.LoadAll<PlayerController>("SkinModels");
            foreach (var playerController in models)
            {
                foreach (var ps in playerController.GetComponentsInChildren<ParticleSystem>())
                {
                    var m = ps.main;
                    m.useUnscaledTime = true;
                }
            }
        }

        [MenuItem("Custom/Level/Create levels ID")]
        static void CreateLevelsID()
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar("Create levels ids", "Preparing", 0);
            var levels = GameLevelAssets.Instance.allLevels;
            CreateGameLevel();

            var gl = FindObjectOfType<GameLevel>();

            for (int i = 1; i < levels.Count; i++)
            {
                bool cancel = EditorUtility.DisplayCancelableProgressBar("Create levels id", 
                    "generating id for level: " + (i+1) +  "/" + levels.Count, (i+1) * 1.0f / levels.Count);
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }

                try
                {
                    gl.Load(i);
                    if (string.IsNullOrEmpty(gl.uid))
                        gl.Save();
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.LogException(e);
                    break;
                }
                
                
            }
            
            EditorUtility.ClearProgressBar();
        }

        #endregion
        
        private ReorderableList blockList;
        private ReorderableList wallList;
        private ReorderableList starsList;
        private ReorderableList reviveZonesList;
        private ReorderableList safeZonesList;
        
        int page
        {
            get
            {
                return EditorPrefs.GetInt("LevelScriptEditor:currentPage", 0);
            }
            set
            {
                EditorPrefs.SetInt("LevelScriptEditor:currentPage", value);
            }
        }
        
        bool expandedSaveLoadList
        {
            get
            {
                return EditorPrefs.GetBool("LevelScriptEditor:expandedSaveLoadList", true);
            }
            set
            {
                EditorPrefs.SetBool("LevelScriptEditor:expandedSaveLoadList", value);
            }
        }
        
        bool expandedGameList
        {
            get
            {
                return EditorPrefs.GetBool("LevelScriptEditor:expandedGameList", true);
            }
            set
            {
                EditorPrefs.SetBool("LevelScriptEditor:expandedGameList", value);
            }
        }
        
        bool expandedReviveZonesList
        {
            get
            {
                return EditorPrefs.GetBool("LevelScriptEditor:expandedReviveZonesList", true);
            }
            set
            {
                EditorPrefs.SetBool("LevelScriptEditor:expandedReviveZonesList", value);
            }
        }
        
        bool moveLevels;
        
        void GetAllLevels()
        {
            GameLevelAssets.Instance.UpdateAllLevels();
        }
       
        private void OnEnable()
        {
            //GameLevel myTarget = (GameLevel)target;
            
            blockList = GetList("blocks", "All blocks", RemoveBlock, PrefabAssets.Instance.blocks[0]);
            wallList = GetList("walls", "All Walls", RemoveWall, PrefabAssets.Instance.wall);
            starsList = GetList("stars", "All Stars", RemoveStar, PrefabAssets.Instance.starDummy, 3);
            reviveZonesList = GetList("reviveZones", "Revive Zones", RemoveReviveZone, PrefabAssets.Instance.reviveZone);
            GetAllLevels();
        }
        
        public override void OnInspectorGUI()
        {
           DrawDefaultInspector();
        
            GameLevel myTarget = (GameLevel)target;
            if (myTarget.gameObject.scene.name == null)
                return;
            
            DrawBlockAndWallsInspector();
            DrawReviveZonesInspector();
            DrawSaveLoadInspector(myTarget);
            serializedObject.ApplyModifiedProperties();
        }
       
        private void DrawSaveLoadInspector(GameLevel myTarget)
        {
            
            EditorGUILayout.BeginVertical("Box");
            expandedSaveLoadList = EditorGUILayout.Foldout(expandedSaveLoadList, "Save / Load Level", true);
            if (expandedSaveLoadList)
            {
                var allLevels = GameLevelAssets.Instance.allLevels;
                myTarget.saveLevelOnPlay = EditorGUILayout.Toggle("Save level on play", myTarget.saveLevelOnPlay);
                EditorGUILayout.LabelField("LevelIdx: " + myTarget.levelIdx, EditorStyles.boldLabel);
                page = EditorGUILayout.IntField("Page", page);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous"))
                {
                    page--;
                }

                if (GUILayout.Button("Next"))
                {
                    page++;
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Overwrite/Insert/Delete");
                moveLevels = EditorGUILayout.Toggle("Move levels", moveLevels);
                EditorGUILayout.EndHorizontal();
                int itemsPrPage = GameSettings.InGame.levelsPrPage;
                page = Mathf.Clamp(page, 0, allLevels.Count / itemsPrPage);
                int startingIdx = page * itemsPrPage;
                int levelCount = 0;
                for (int i = startingIdx; i < Mathf.Min(allLevels.Count, startingIdx + itemsPrPage); i++)
                {
                    levelCount++;
                    int levelIdx = i + 1;
                    string levelStr = "" + levelIdx;
                    while (levelStr.Length < 3)
                        levelStr = "0" + levelStr;
                    EditorGUILayout.BeginHorizontal();

                    //Save/Insert/Delete
                    if (GUILayout.Button("S", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Save level",
                            "Save level: " + levelIdx + " - " + myTarget.name + "?\nWill overwrite level: " + levelIdx + " - " +
                            GameLevelAssets.Instance.allLevels[levelIdx - 1].name + "!",
                            "Save", "Cancel"))
                        {
                            myTarget.levelIdx = levelIdx;
                            myTarget.Save();
                            GetAllLevels();
                            EditorGUIUtility.ExitGUI();
                        }
                    }

                    if (GUILayout.Button("I", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Save level",
                            "Save & insert level: " + levelIdx + " - " + myTarget.name + "?\nWill insert " + levelIdx + " - " +
                            GameLevelAssets.Instance.allLevels[levelIdx - 1].name + "!",
                            "Save", "Cancel"))
                        {
                            myTarget.levelIdx = levelIdx;
                            myTarget.Save(true);
                            GetAllLevels();
                            EditorGUIUtility.ExitGUI();
                        }
                    }

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Delete level",
                            "Delete level: " + levelIdx + " - " + GameLevelAssets.Instance.allLevels[levelIdx - 1].name + "?",
                            "Delete", "Cancel"))
                        {
                            var path = Application.dataPath + "/_Game/Resources/GameLevels/" +
                                       GameLevelAssets.Instance.allLevels[levelIdx - 1].name + ".txt";
                            FileManager.DeleteFile(path);
                            GetAllLevels();
                            EditorGUIUtility.ExitGUI();
                        }
                    }

                    //Load level
                    if (levelIdx == myTarget.levelIdx &&
                        GameLevelAssets.Instance.allLevels[myTarget.levelIdx - 1].name == myTarget.name)
                        GUI.color = new Color(0.8f, 1, 0.8f, 1);
                    ;
                    if (GUILayout.Button("(" + levelIdx + ") " + allLevels[i].name))
                    {
                        if (EditorUtility.DisplayDialog("Load level",
                            "Load level: " + levelIdx + " - " + GameLevelAssets.Instance.allLevels[levelIdx - 1].name +
                            "?\nWill overwrite current level!",
                            "Load", "Cancel"))
                        {
                            myTarget.levelIdx = levelIdx;
                            myTarget.Load(levelIdx);
                            GetAllLevels();
                            EditorGUIUtility.ExitGUI();
                        }
                    }

                    GUI.color = Color.white;
                    if (moveLevels)
                    {
                        //Move level up/down
                        if (GUILayout.Button("/\\", GUILayout.Width(20)) && i > 0)
                        {
                            var tmp = allLevels[i];
                            allLevels[i] = allLevels[i - 1];
                            allLevels[i - 1] = tmp;
                        }

                        if (GUILayout.Button("\\/", GUILayout.Width(20)) && i < allLevels.Count - 1)
                        {
                            var tmp = allLevels[i];
                            allLevels[i] = allLevels[i + 1];
                            allLevels[i + 1] = tmp;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                if (levelCount < itemsPrPage)
                {
                    if (GUILayout.Button("Save as last"))
                    {
                        myTarget.levelIdx = int.MaxValue;
                        myTarget.Save(true);
                        GetAllLevels();
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBlockAndWallsInspector()
        {
            EditorGUILayout.BeginVertical("Box");
            expandedGameList = EditorGUILayout.Foldout(expandedGameList, "Blocks & Walls", true);
            if (expandedGameList)
            {
                serializedObject.Update();
                blockList.DoLayoutList();
                wallList.DoLayoutList();
                starsList.DoLayoutList();

				serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }
        
        private void DrawReviveZonesInspector()
        {
            EditorGUILayout.BeginVertical("Box");
            expandedReviveZonesList = EditorGUILayout.Foldout(expandedReviveZonesList, "Revive Zones", true);
            if (expandedReviveZonesList)
            {
                serializedObject.Update();
                reviveZonesList.DoLayoutList();
                //safeZonesList.DoLayoutList();
            }

            EditorGUILayout.EndVertical();
        }

        T AddObject<T>(SerializedProperty prop, T prefab) where T : Component
        {
            GameLevel myTarget = (GameLevel)target;
            var idx = prop.arraySize;
            prop.InsertArrayElementAtIndex(idx);
            var arrayElement = prop.GetArrayElementAtIndex(idx);
            var bc = Instantiate(prefab, myTarget.transform);
            arrayElement.objectReferenceValue = bc;
            arrayElement.objectReferenceValue.name = prefab.name;
            var c = bc.GetComponent<BlockController>();
            if (c != null)
                c.OnValidate();
            return bc;
        }
        
        ReorderableList GetList<T>(string propertyName, string header, ReorderableList.RemoveCallbackDelegate onRemove, T prefab, int maxCount = -1) where T : Component
        {
            var l = new ReorderableList(serializedObject,
                serializedObject.FindProperty(propertyName),
                true, true, true, true);
            
            l.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, header);
            };
            
            l.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var gui = GUI.enabled;
                GUI.enabled = false;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element, GUIContent.none);
                GUI.enabled = gui;
            };
            
            l.onCanAddCallback = (a) =>
            {
                return !Application.isPlaying && (maxCount < 0 || a.count < maxCount) ;
            };
            
            l.onAddCallback = (a) =>
            {
                AddObject(a.serializedProperty, prefab);
            };
            
            l.onCanRemoveCallback = (a) =>
            {
                return onRemove != null;
            };
            
            l.onRemoveCallback = onRemove;
            
            return l;
        }
        
        void RemoveBlock(ReorderableList a)
        {
            var mytarget = target as GameLevel;
            var b = mytarget.blocks[a.index];
            mytarget.blocks.RemoveAt(a.index);
            if (b != null) DestroyImmediate(b.gameObject);
        }
        
        void RemoveWall(ReorderableList a)
        {
            var mytarget = target as GameLevel;
            var b = mytarget.walls[a.index];
            mytarget.walls.RemoveAt(a.index);
            if (b != null) DestroyImmediate(b.gameObject);
        }
        
        void RemoveStar(ReorderableList a)
        {
            var mytarget = target as GameLevel;
            var b = mytarget.stars[a.index];
            mytarget.stars.RemoveAt(a.index);
            if (b != null)
                DestroyImmediate(b.gameObject);
        }
        
        void RemoveReviveZone(ReorderableList a)
        {
            var mytarget = target as GameLevel;
            var b = mytarget.reviveZones[a.index];
            mytarget.reviveZones.RemoveAt(a.index);
            if (b != null) DestroyImmediate(b.gameObject);
        }
    }
}