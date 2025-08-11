using UnityEngine;

namespace LightItUp.Singletons
{
    public class SingletonLoad<T> : MonoBehaviour where T : Component
    {
        static bool Verbose { get { return Debug.isDebugBuild; } }
        static string errorMessage = "More than 1 singleton exists: ";

        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] t = FindObjectsOfType<T>();
                    if (t.Length > 1)
                    {
                        instance = t[0];
                        for (int i = 1; i < t.Length; i++)
                            Destroy(t[i].gameObject);
                        Debug.LogError(errorMessage + classnameWithoutNamespace);
                    }
                    else if (t.Length == 1)
                        instance = t[0];
                    else
                    {
                        string path = "Singletons/" + classnameWithoutNamespace;
                        if (Verbose)
                            Debug.Log("Singleton:Resources.Load(" + path + ")");
                        T g = Resources.Load<T>(path);

                        if (g != null)
                        {
                            instance = Instantiate<T>(g);
                            if (instance != null)
                                instance.name = classnameWithoutNamespace;
                        }
                        if (instance == null)
                            Debug.LogError("Could not find singleton prefab " + path);
                    }
                }
                return instance;
            }
        }
        static string classnameWithoutNamespace
        {
            get
            {
                string[] l = typeof(T).ToString().Split(new char[] { '.' });
                return l[l.Length - 1];
            }
        }

        public virtual void Awake()
        {
            T t = GetComponent<T>();
            if (instance != null && instance != t)
            {
                Debug.LogError(errorMessage + (typeof(T)).ToString());
                Destroy(gameObject);
            }
            else
            {
                instance = t;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}