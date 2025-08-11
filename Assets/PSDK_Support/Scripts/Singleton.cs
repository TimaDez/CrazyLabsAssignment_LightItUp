using UnityEngine;
using System.Collections;

namespace HyperCasual.PsdkSupport
{

	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		private static object _lock = new object();

		protected static string pathToResource;

		public static bool IsLoaded
		{
			get { return _instance != null; }
		}

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					return null;
				}

				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[PsdkSingleton] Something went really wrong " +
							               " - there should never be more than 1 singleton!" +
							               " Reopenning the scene might fix it.");
							return _instance;
						}

						if (_instance == null)
						{
							if (pathToResource != null)
							{
								return CreateFromResources();
							}

							GameObject singleton = new GameObject();

							var ty = typeof(T);

							var component = singleton.AddComponent(ty);

							_instance = (T) component;
							string[] delimited = typeof(T).ToString().Split(new char[] {'.'});
							singleton.name = delimited[delimited.Length - 1];

							DontDestroyOnLoad(singleton);
						}
					}

					return _instance;
				}
			}
		}

		static T CreateFromResources()
		{
			var prefab = Resources.Load(pathToResource) as GameObject;
			var go = Instantiate(prefab) as GameObject;
			DontDestroyOnLoad(go);
			_instance = go.GetComponent<T>();
			return _instance;
		}

		private static bool applicationIsQuitting = false;

		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// <para>
		/// IMPORTANT: <br/>
		/// When you override OnDestroy, you must call base.OnDestroy()<br/>
		/// at the end of youe inhertued derived OnDestroy.<br/>
		/// </para>
		/// </summary>
		protected virtual void OnDestroy()
		{
			applicationIsQuitting = true;
		}

		protected virtual void Awake()
		{
			if (_instance)
			{
				DestroyImmediate(gameObject);
				applicationIsQuitting = false;
			}
			else
			{
				DontDestroyOnLoad(gameObject);
				_instance = gameObject.GetComponent<T>();
			}
		}
	}
}