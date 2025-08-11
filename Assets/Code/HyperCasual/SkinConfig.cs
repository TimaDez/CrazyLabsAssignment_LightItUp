using HyperCasual.Skins;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
[CustomEditor(typeof(SkinConfig))]
public class SkinInspector : Editor
{
    private static string lastPath;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkinConfig config = (SkinConfig)target;
        
        EditorGUILayout.LabelField("Model Path",config.modelResourcePath);

        if (GUILayout.Button("Select Path"))
        {
            SelectPath();
        }

    }

    void SelectPath()
    {
        string path = EditorUtility.OpenFilePanel("Select Model", lastPath, "prefab");
        if (path.Length != 0)
        {
            lastPath = path;
            SkinConfig config = (SkinConfig)target;

            var fileName = Path.GetFileNameWithoutExtension(path);
            config.modelResourcePath = fileName;
            AssetDatabase.Refresh();
        }
    }
    
    
}








#endif


 


namespace HyperCasual.Skins
{
    [CreateAssetMenu(fileName = "Skin", menuName = "[Jetpack VS. Colors]/Skin")]
    public class SkinConfig : ScriptableObject
    {
        public string id;        
        public string description;
        
        public ConditionData condition;
        
        [Header("View data")]
        public Sprite icon;
        public Sprite iconHighlighted;
        public Sprite iconCircle;
        
        
        [SerializeField]
        public string modelResourcePath;
        
        public bool Unlocked
        {
            get
            {                
                if (PlayerPrefs.GetInt("skin_" + id, 0) == 1)
                {
                    return true;
                }
                
                if (condition == null)
                    return true;

                if (SkinUnlockToggle.IsOn)
                    return true;

                return condition.DidComplete;
            }

            set
            {
                PlayerPrefs.SetInt("skin_" + id, value ? 1 : 0);
            }
        }
    }
}