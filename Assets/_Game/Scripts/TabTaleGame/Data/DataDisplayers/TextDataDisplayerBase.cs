using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace LightItUp.Data
{
    public abstract class TextDataDisplayerBase<T> : MonoBehaviour where T : struct, IConvertible
    {

        public bool updateOnPlayerDataChanges = true;
        public List<T> values;
        [TextArea]
        public string formattingText;
        public string localizerId;

        public void OnValidate()
        {
            if (Application.isPlaying)
                return;
            if (!string.IsNullOrEmpty(localizerId))
            {
                formattingText = Localization.GetString(localizerId);
            }
            textComponent.text = formattingText;
        }
        TMPro.TextMeshProUGUI _textComponent;
        TMPro.TextMeshProUGUI textComponent
        {
            get
            {
                if (_textComponent == null)
                    _textComponent = GetComponent<TMPro.TextMeshProUGUI>();
                return _textComponent;
            }
        }
        public void SetText(string text) {
            textComponent.text = text;
        }

        public abstract string GetValue(T value);

        public void DataChanged()
        {
            if (!updateOnPlayerDataChanges)
            {
                return;
            }
            if (!string.IsNullOrEmpty(localizerId))
            {
                formattingText = Localization.GetString(localizerId);
            }

            if (!string.IsNullOrEmpty(formattingText))
            {
                if (values.Count == 0)
                {
                    textComponent.text = formattingText;
                }
                else
                {
                    string[] s = new string[values.Count];
                    for (int i = 0; i < values.Count; i++)
                    {
                        s[i] = GetValue(values[i]);
                    }
                    textComponent.text = string.Format(formattingText, s);
                }
            }
            else if (values.Count > 0)
            {
                textComponent.text = GetValue(values[0]);
            }
            else
            {
                textComponent.text = "";
            }

        }

        private void OnEnable()
        {
            if (values.Count == 0)
                updateOnPlayerDataChanges = false;
            GameData.PlayerData.OnValueChanged += DataChanged;
            DataChanged();
        }

        private void OnDisable()
        {
            GameData.PlayerData.OnValueChanged -= DataChanged;
        }
#if UNITY_EDITOR
        public void DrawLocalizerDropdown(SerializedObject serializedObject) {
            GUI.changed = false;
            serializedObject.Update();
            var prop = serializedObject.FindProperty("localizerId");
            var localizerId = prop.stringValue;
            int idx = TextIds.allIds.IndexOf(localizerId);
            int newIdx = EditorGUILayout.Popup(idx, TextIds.allIds.ToArray());
            if (newIdx != idx)
            {
                prop.stringValue = TextIds.allIds[newIdx];
            }
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                OnValidate();
            }
        }
#endif
    }
}
/* Example editor!!

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

*/
