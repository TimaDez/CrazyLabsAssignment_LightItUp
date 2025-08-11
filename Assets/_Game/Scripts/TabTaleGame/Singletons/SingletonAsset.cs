using UnityEngine;

namespace LightItUp.Singletons
{
    public abstract class SingletonAsset<T> : ScriptableObject where T : SingletonAsset<T>
    {
        static bool Verbose { get { return Debug.isDebugBuild; } }
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    if (Application.isPlaying && Verbose)
                        Debug.Log("SingletonAsset:Resources.Load(" + classnameWithoutNamespace + ")");
                    instance = Resources.Load<T>(classnameWithoutNamespace);
                    if (instance == null)
                    {
#if UNITY_EDITOR
                        Debug.Log("SingletonAsset:Create " + classnameWithoutNamespace + ".asset");
                        instance = CreateAsset();
                        if (instance == null)
                            Debug.LogError("Could not find singleton asset " + classnameWithoutNamespace);
#else
                Debug.LogError("Could not find singleton asset " + classnameWithoutNamespace);
#endif
                    }
#if UNITY_EDITOR
                    instance.Init();
#endif
                }
                return instance;
            }
        }
#if UNITY_EDITOR
        public abstract void Init();
#endif
        static string classnameWithoutNamespace
        {
            get
            {
                string[] l = typeof(T).ToString().Split(new char[] { '.' });
                return l[l.Length - 1];
            }
        }

#if UNITY_EDITOR
        protected static T CreateAsset()
        {
            T asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/_Game/Resources/" + classnameWithoutNamespace + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
            return asset;
        }
#endif

    }
}