using System;

#if !UNITY_EDITOR
using System.Globalization;
using GameAnalyticsSDK;
#endif

namespace Funzilla
{
	internal class Config : Singleton<Config>
	{
#if UNITY_ANDROID
		private const string IronSrcID = "106faafd1";
#else
		public const string IronSrcID = "106faafd1";
#endif
		internal static float InterstitialCappingTime { get; private set; } = 45f;
		internal static float FirstInterstitialCappingTime { get; private set; } = 45f;
		internal static float InterstitialRewardedVideoCappingTime { get; private set; } = 45f;

		internal static string IronSourceId { get; private set; } = IronSrcID;

		internal static bool CheatEnabled { get; private set; } = true;
		internal static bool BannerEnabled { get; private set; } = true;

		private enum State
		{
			None,
			Initializing,
			Initialized
		}

		private State _state = State.None;
		internal static bool Initialized => Instance._state == State.Initialized;

		internal static void Init()
		{
			if (Instance._state == State.None)
			{
				Instance._state = State.Initializing;
			}
		}

		private void Update()
		{
			switch (_state)
			{
				case State.Initializing:
#if !UNITY_EDITOR && REMOTE_ENABLED
					if (!GameAnalytics.IsRemoteConfigsReady()) return;
					LoadConfigs();
#else
					Ads.Instance.Init();
					enabled = false;
					_state = State.Initialized;
#endif
					break;
				case State.Initialized:
					break;
				case State.None:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

#if !UNITY_EDITOR && REMOTE_ENABLED
		private static string GetConfigValue(string name, string defaultValue)
		{
			return GameAnalytics.GetRemoteConfigsValueAsString("name", defaultValue);
		}

		private static float GetConfigValue(string name, float defaultValue)
		{
			var value = GameAnalytics.GetRemoteConfigsValueAsString(name, defaultValue.ToString(CultureInfo.InvariantCulture));
			return float.TryParse(value, out var result) ? result : defaultValue;
		}
		
		private static bool GetConfigValue(string name, bool defaultValue)
		{
			var value = GameAnalytics.GetRemoteConfigsValueAsString(name, defaultValue.ToString(CultureInfo.InvariantCulture));
			return bool.TryParse(value, out var result) ? result : defaultValue;
		}

		private void LoadConfigs()
		{
			enabled = false;
			_state = State.Initialized;

			IronSourceId = GetConfigValue("ironsource_id", IronSourceId);
			InterstitialCappingTime = GetConfigValue("interstitial_capping_time", InterstitialCappingTime);
			InterstitialRewardedVideoCappingTime = GetConfigValue("interstitial_reward_capping_time", InterstitialRewardedVideoCappingTime);
			FirstInterstitialCappingTime = GetConfigValue("first_interstitial_capping_time", FirstInterstitialCappingTime);
			BannerEnabled = GetConfigValue("banner_enabled", BannerEnabled);
			CheatEnabled = GetConfigValue("cheat_enabled", CheatEnabled);
			EventManager.Annouce(EventType.ConfigsLoaded);
			Ads.Instance.Init();
		}
#endif
	}
}