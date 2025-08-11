using UnityEngine;

namespace LightItUp.Singletons
{
    public class SingletonScene<T> : MonoBehaviour where T : Component {
        static bool Verbose { get { return Debug.isDebugBuild; } }
        static string errorMessage = "More than 1 singleton exists: ";
        static T instance;
        public static T Instance {
            get {
                if (instance == null) {
                    T[] t = FindObjectsOfType<T>();
                    if (t.Length > 1) {
                        instance = t[0];
                        for (int i = 1; i < t.Length; i++) Destroy(t[i].gameObject);
                        Debug.LogError(errorMessage + (typeof(T)).ToString());
                    }
                    else if (t.Length == 1) {
                        if (Verbose) Debug.Log("SingletonScene: Found " + typeof(T).ToString() + " in the scene");
                        instance = t[0];
                    }
                    else Debug.LogError("Could not find singleton of type: " + typeof(T).ToString());
                }
                return instance;
            }
        }
        protected virtual void Awake() {
            T t = GetComponent<T>();
            if (instance != null && instance != t) {
                Debug.LogError(errorMessage + (typeof(T)).ToString());
                Destroy(gameObject);
            }
            else {
                instance = t;
            }
        }
    }
}
