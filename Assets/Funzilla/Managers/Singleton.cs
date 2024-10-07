
using UnityEditor;

namespace Funzilla
{
	using UnityEngine;
	internal class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool _shuttingDown;
		private static object _lock = new object();
		private static T _instance;
		
		protected static T Instance
		{
			get
			{
				if (_shuttingDown)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
						"' already destroyed. Returning null.");
					return null;
				}

				lock (_lock)
				{
					if (_instance != null) return _instance;
					// Search for existing instance.
					_instance = (T)FindObjectOfType(typeof(T));

					// Create new instance if one doesn't already exist.
					if (_instance != null) return _instance;
					
#if UNITY_EDITOR
					try
					{
						var assetName = typeof(T).Name;
						var path = $"Assets/Funzilla/Managers/{assetName}.prefab";
						var prefab = AssetDatabase.LoadAssetAtPath<T>(path);
						_instance = Instantiate(prefab);
					}
					catch
					{
						// ignored
					}

					if (_instance == null)
#endif
					{
						// Need to create a new GameObject to attach the singleton to.
						var singletonObject = new GameObject();
						_instance = singletonObject.AddComponent<T>();
						singletonObject.name = typeof(T) + " (Singleton)";
					}

					// Make instance persistent.
					DontDestroyOnLoad(_instance.gameObject);
					return _instance;
				}
			}
		}


		private void OnApplicationQuit()
		{
			_shuttingDown = true;
		}


		private void OnDestroy()
		{
			_shuttingDown = true;
		}
	}
}