
using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using GameAnalyticsSDK;

namespace Funzilla
{
	internal class GameManager : Singleton<GameManager>
	{
		internal static bool FirebaseOk { get; private set; }

		private enum State
		{
			None,
			InitializingFirebase,
			InitializingConfig,
			Initialized
		}

		private State _state = State.None;
		private readonly Queue<Action> _queue = new Queue<Action>();

		private void Start()
		{
			if (_state != State.Initialized && _queue.Count <= 0)
			{
				Init(() =>
				{
					SceneManager.OpenScene(SceneID.Gameplay);
				});
			}
		}

		internal static void Init(Action onComplete)
		{
			switch (Instance._state)
			{
				case State.None:
					Instance._state = State.InitializingFirebase;
					Application.targetFrameRate = 60;
					GameAnalytics.Initialize();
					FB.Init();
#if UNITY_EDITOR
					FirebaseOk = true;
#else
					FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
					{
						if (task.Result != DependencyStatus.Available) return;
						FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
						FirebaseOk = true;
					});
#endif
					if (onComplete != null) Instance._queue.Enqueue(onComplete);
					break;
				case State.InitializingFirebase:
				case State.InitializingConfig:
					if (onComplete != null) Instance._queue.Enqueue(onComplete);
					break;
				case State.Initialized:
					onComplete?.Invoke();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Update()
		{
			switch (_state)
			{
				case State.None:
					break;
				case State.InitializingFirebase:
					if (FirebaseOk)
					{
						_state = State.InitializingConfig;
						Config.Init();
						Adjust.Init();
					}
					break;
				case State.InitializingConfig:
					if (Config.Initialized)
					{
						_state = State.Initialized;
						enabled = false;
						while (_queue.Count > 0)
						{
							var onComplete = _queue.Dequeue();
							onComplete?.Invoke();
						}
					}
					break;
				case State.Initialized:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}