using _Game.Scripts.SeekingMissiles.Models;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor
{
    [CustomEditor(typeof(SeekingMissilesModel))]
    public class SeekingMissilesModelEditor : UnityEditor.Editor
    {
        #region Private Members

        private SeekingMissilesModel _target;
        private SerializedProperty _missilesAmountProp;

        #endregion

        #region Methods

        private void OnEnable()
        {
            _target = (SeekingMissilesModel)target;
            _missilesAmountProp = serializedObject.FindProperty("_missilesAmount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (prop.name == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(prop, true);
                    continue;
                }

                if (prop.name == "_missilesAmount")
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(prop, true);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(25), GUILayout.Height(18)))
                    {
                        _target.IncreaseMissilesAmount();
                        SaveChanges();
                    }

                    // Small "-" button
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(25), GUILayout.Height(18)))
                    {
                        _target.DecreaseMissilesAmount();
                        SaveChanges();
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                //Sanity check
                if (_target.MissilesAmount < 0)
                {
                    while (_target.MissilesAmount < 0)
                        _target.IncreaseMissilesAmount();
                    
                    SaveChanges();
                }
            }
        }
        
        private void SaveChanges()
        {
            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}