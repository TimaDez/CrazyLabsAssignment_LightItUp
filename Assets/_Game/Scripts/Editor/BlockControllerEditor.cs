using UnityEditor;
using UnityEngine;

namespace LightItUp.Game
{
    [CustomEditor(typeof(BlockController))]
    public class BlockControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            BlockController myTarget = (BlockController)target;
            if (GUILayout.Button("Validate"))
            {
                myTarget.OnValidate();
            }
            if (GUILayout.Button("Save"))
            {
                myTarget.SaveBlock();
            }
        }
    }
}