using UnityEngine;

namespace LightItUp.Singletons
{
    public class SingletonCreate<T> : MonoBehaviour where T : Component
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
                        Debug.LogError(errorMessage + (typeof(T)).ToString());
                    }
                    else if (t.Length == 1)
                        instance = t[0];
                    else
                    {
                        if (Verbose)
                            Debug.Log("SingletonCreate: Create " + classnameWithoutNamespace);
                        GameObject g = new GameObject(typeof(T).ToString());
                        g.name = classnameWithoutNamespace;
                        instance = g.AddComponent<T>();
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
