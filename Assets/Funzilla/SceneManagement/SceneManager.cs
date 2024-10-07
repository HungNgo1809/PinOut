
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Funzilla
{
	internal class SceneManager : Singleton<SceneManager>
	{
		[SerializeField] private SceneShield popupShield;
		[SerializeField] private LoadingShield loadingShield;

		private Transform _sceneNode;
		private Transform _popupNode;

		private enum ActionType
		{
			OpenScene,
			CloseScene,
			CloseScenes,
			OpenPopup,
			ClosePopup,
			ClosePopups,
		}

		private class Action
		{
			internal Action(ActionType type) { Type = type; }
			internal readonly ActionType Type;
		}

		// Scene action that's taking place
		private Action _action;

		private class SceneAction : Action
		{
			internal SceneAction(ActionType type, SceneID sceneId) : base(type)
			{
				SceneId = sceneId;
			}
			internal readonly SceneID SceneId;
		}

		private class PopupOpenAction : Action
		{
			internal PopupOpenAction(SceneID sceneId) : base(ActionType.OpenPopup)
			{
				SceneId = sceneId;
			}
			internal readonly SceneID SceneId;
		}

		// Node that stores inactive loaded scenes
		private Transform _pool;

		// Currently active scene
		private readonly List<Scene> _visibleScenes = new List<Scene>(4);

		// Currently active popups
		private readonly List<Popup> _visiblePopups = new List<Popup>(4);

		// Queued actions
		private readonly Queue<Action> _actions = new Queue<Action>(4);

		// Loaded scenes
		readonly Dictionary<string, SceneBase> _scenes = new Dictionary<string, SceneBase>(10);

		private static SceneID GetSceneID(string sceneId)
		{
			for (var i = 0; i < (int)SceneID.END; i++)
			{
				var sceneName = SceneNames.ScenesNameArray[i];
				if (sceneName.Equals(sceneId))
				{
					return (SceneID)i;
				}
			}
			return SceneID.END;
		}

		private void Awake()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
			_pool = new GameObject("pool").transform;
			_pool.SetParent(transform, false);
			_sceneNode = new GameObject("scenes").transform;
			_sceneNode.SetParent(transform, false);
			_popupNode = new GameObject("popups").transform;
			_popupNode.SetParent(transform, false);
			for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var activeScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				var scene = GetScene(activeScene);
				if (scene == null) continue;

				scene.ID = GetSceneID(activeScene.name);
				scene.name = activeScene.name;
				_scenes.Add(scene.name, scene);
				if (scene as Scene)
				{
					_visibleScenes.Add(scene as Scene);
					scene.gameObject.transform.SetParent(_sceneNode, false);
				}
				else
				{
					_visiblePopups.Add(scene as Popup);
					scene.gameObject.transform.SetParent(_popupNode, false);
				}

				UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(activeScene);
				popupShield.transform.SetSiblingIndex(_sceneNode.GetSiblingIndex());
			}
		}

		internal static void OpenScene(SceneID sceneId)
		{
			Instance._actions.Enqueue(new SceneAction(ActionType.OpenScene, sceneId));
		}

		internal static void CloseScene()
		{
			Instance._actions.Enqueue(new SceneAction(ActionType.CloseScene, SceneID.END));
		}

		internal static void CloseScene(SceneID sceneId)
		{
			Instance._actions.Enqueue(new SceneAction(ActionType.CloseScene, sceneId));
		}

		internal static void CloseScenes()
		{
			Instance._actions.Enqueue(new Action(ActionType.CloseScenes));
		}

		internal static void OpenPopup(SceneID sceneId)
		{
			Instance._actions.Enqueue(new PopupOpenAction(sceneId));
		}

		internal static void ClosePopup()
		{
			Instance._actions.Enqueue(new Action(ActionType.ClosePopup));
		}

		internal static void ClosePopups()
		{
			Instance._actions.Enqueue(new Action(ActionType.ClosePopups));
		}
		
		internal static void ReloadScene(SceneID sceneId)
		{
			Instance._actions.Enqueue(new SceneAction(ActionType.CloseScene, sceneId));
			Instance._actions.Enqueue(new SceneAction(ActionType.OpenScene, sceneId));
		}

		internal static void ShowLoading(
			bool loadingAnimationEnabled = true,
			float opacity = 0.7f,
			System.Action onComplete = null)
		{
			Instance.loadingShield.Show(loadingAnimationEnabled, opacity, onComplete);
		}

		internal static void ShowLoading(System.Action onComplete)
		{
			Instance.loadingShield.Show(false, 1.0f, onComplete);
		}

		internal static void HideLoading()
		{
			Instance.loadingShield.Hide();
		}

		private SceneBase GetLoadedScene(SceneID sceneId)
		{
			var sceneName = SceneNames.GetSceneName(sceneId);
			return _scenes.TryGetValue(sceneName, out var scene) ? scene : null;
		}

		private void LoadScene(SceneID sceneId)
		{
			var sceneName = SceneNames.GetSceneName(sceneId);

			if (_scenes.TryGetValue(sceneName, out var scene))
			{
				if (!scene.gameObject.activeSelf)
				{
					scene.gameObject.SetActive(true);
					OnSceneLoaded(scene);
				}
				else
				{
					_action = null;
				}
			}
			else
			{
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
					sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			}
		}

		private void OnSceneLoaded(SceneBase scene)
		{
			if (_action == null)
			{
				return;
			}

			switch (_action.Type)
			{
				case ActionType.OpenScene:
					var sceneOpenAction = (SceneAction)_action;
					scene.transform.SetParent(_sceneNode, false);
					scene.ID = sceneOpenAction.SceneId;
					_visibleScenes.Add((Scene)scene);
					scene.AnimateIn();
					break;

				case ActionType.OpenPopup:
					scene.transform.SetParent(_popupNode, false);
					var popupOpenAction = (PopupOpenAction)_action;
					scene.ID = popupOpenAction.SceneId;
					_visiblePopups.Add((Popup)scene);
					scene.AnimateIn();
					break;
				case ActionType.CloseScene:
					break;
				case ActionType.CloseScenes:
					break;
				case ActionType.ClosePopup:
					break;
				case ActionType.ClosePopups:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnSceneLoaded(
			UnityEngine.SceneManagement.Scene loadedScene,
			UnityEngine.SceneManagement.LoadSceneMode mode)
		{
			OnSceneLoaded(loadedScene);
		}

		private void ConsumeScene(Component scene)
		{
			_scenes.Remove(scene.name);
			Destroy(scene.gameObject);
		}

		internal static void OnSceneAnimatedOut(SceneBase scene)
		{
			var instance = Instance;
			switch (Instance._action.Type)
			{
				case ActionType.OpenScene:
					break;
				case ActionType.CloseScene:
					instance.ConsumeScene(scene);
					instance._action = null;
					break;
				case ActionType.OpenPopup:
					{
						scene.gameObject.SetActive(false);
						var sceneId = ((PopupOpenAction)instance._action).SceneId;
						instance.LoadScene(sceneId);
					}
					break;
				case ActionType.ClosePopup:
					instance.ConsumeScene(scene);
					if (instance._visiblePopups.Count > 0)
					{ // Let the last popup appear
						var popup = instance._visiblePopups.Last();
						popup.gameObject.SetActive(true);
						popup.AnimateIn();
					}
					else
					{ // No popup active, hide the shield now
						instance._action = null;
					}
					break;
				case ActionType.ClosePopups:
					instance.ConsumeScene(scene);
					foreach (var popup in instance._visiblePopups)
					{
						instance.ConsumeScene(popup);
					}
					instance._visiblePopups.Clear();
					instance.popupShield.Hide();
					instance._action = null;
					break;
				case ActionType.CloseScenes:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal static void OnSceneAnimatedIn(SceneBase scene)
		{
			Instance._action = null;
		}

		private static SceneBase GetScene(UnityEngine.SceneManagement.Scene loadedScene)
		{
			SceneBase scene = null;
			foreach (var obj in loadedScene.GetRootGameObjects())
			{
				scene = obj.GetComponent<SceneBase>();
				if (scene == null)
				{
					scene = obj.GetComponentInChildren<SceneBase>();
				}
				if (scene != null)
				{
					break;
				}
			}
			return scene;
		}

		private void OnSceneLoaded(UnityEngine.SceneManagement.Scene loadedScene)
		{
			var scene = GetScene(loadedScene);
			foreach (var obj in loadedScene.GetRootGameObjects())
			{
				scene = obj.GetComponent<SceneBase>();
				if (scene == null)
				{
					scene = obj.GetComponentInChildren<SceneBase>();
				}
				if (scene != null)
				{
					break;
				}
			}

			if (scene == null) return;
			scene.name = loadedScene.name;
			_scenes.Add(loadedScene.name, scene);

			OnSceneLoaded(scene);
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(loadedScene);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (_visiblePopups.Count > 0)
				{
					_visiblePopups.Last().OnBackButtonPressed();
				}
				else if (_visibleScenes.Count > 0)
				{
					_visibleScenes.Last().OnBackButtonPressed();
				}
			}
			if (_action != null || _actions.Count <= 0)
			{
				return;
			}

			_action = _actions.Dequeue();
			switch (_action.Type)
			{
				case ActionType.OpenScene:
					{
						var sceneOpenAction = (SceneAction)_action;
						var sceneId = sceneOpenAction.SceneId;
						LoadScene(sceneId);
					}
					break;
				case ActionType.CloseScene:
					{
						var sceneAction = (SceneAction)_action;
						if (sceneAction.SceneId == SceneID.END)
						{
							if (_visibleScenes.Count <= 0)
							{
								return;
							}
							var scene = _visibleScenes.Last();
							_visibleScenes.RemoveAt(_visibleScenes.Count - 1);
							scene.AnimateOut();
						}
						else
						{
							var sceneBase = GetLoadedScene(sceneAction.SceneId);
							if (sceneBase)
							{
								var scene = sceneBase.gameObject.GetComponent<Scene>();
								if (scene)
								{
									_visibleScenes.Remove(scene);
									scene.AnimateOut();
									return;
								}
							}
							_action = null;
						}
					}
					break;
				case ActionType.CloseScenes:
					foreach (var scene in _visibleScenes)
					{
						ConsumeScene(scene);
					}
					_visibleScenes.Clear();
					_action = null;
					break;
				case ActionType.OpenPopup:
					{
						var sceneId = ((PopupOpenAction)_action).SceneId;
						var scene = GetLoadedScene(sceneId);
						if (scene && scene.gameObject.activeSelf)
						{ // Should not open a popup already opened
							_action = null;
							return;
						}
						if (_visiblePopups.Count > 0)
						{ // Hide the current active popup first
							_visiblePopups.Last().AnimateOut();
						}
						else
						{
							popupShield.Show();
							LoadScene(sceneId);
						}
					}
					break;
				case ActionType.ClosePopups:
				case ActionType.ClosePopup:
					{
						if (_visiblePopups.Count <= 0)
						{
							_action = null;
							return;
						}
						var popup = _visiblePopups.Last();
						_visiblePopups.RemoveAt(_visiblePopups.Count - 1);
						popup.AnimateOut();
						if (_visiblePopups.Count <= 0)
						{
							popupShield.Hide();
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}