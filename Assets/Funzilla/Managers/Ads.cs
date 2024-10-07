
using System;
using System.Collections;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using Facebook.Unity;
#endif

namespace Funzilla
{
	internal enum RewardedVideoState
	{
#if !UNITY_EDITOR
		Closed, NotReady,
#endif
		Failed, Watched
	}

	internal class Ads : Singleton<Ads>
	{
		internal const string SdkName = "IronSource";

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")] private static extern bool isIos14();
		[DllImport("__Internal")] private static extern bool advertiserTrackingPrompted();
		[DllImport("__Internal")] private static extern void promptAdvertiserTracking();
		[DllImport("__Internal")] private static extern bool advertiserTrackingEnabled();
#endif
		private const float InterstitialLoadDelayTime = 1.0f;
		private bool _interstitialShown;

		private float _lastInterstitialShowTime;
		private float _lastRewardedVideoShowTime;

		private bool InterstitialAllowed { get; set; } = true;
		private bool BannerAllowed { get; set; } = false;

		private enum State { NotInitialized, Initializing, Initialized }
		private State _state = State.NotInitialized;

		internal void Init()
		{
			if (_state != State.NotInitialized)
			{
				return;
			}
			_state = State.Initializing;
			_lastInterstitialShowTime = Time.realtimeSinceStartup;

			IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailed;
			IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedVideoAdOpened;
			IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoAdClosed;
			IronSourceEvents.onRewardedVideoAdStartedEvent += OnRewardedVideoAdStarted;
			IronSourceEvents.onRewardedVideoAdEndedEvent += OnRewardedVideoAdEnded;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewarded;
			IronSourceEvents.onRewardedVideoAdClickedEvent += OnRewardedVideoAdClicked;
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChanged;

			IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialAdReady;
			IronSourceEvents.onInterstitialAdLoadFailedEvent += OnInterstitialAdLoadFailed;
			IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialAdOpened;
			IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialAdClosed;
			IronSourceEvents.onInterstitialAdShowSucceededEvent += OnInterstitialAdShowSucceeded;
			IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialAdShowFailed;
			IronSourceEvents.onInterstitialAdClickedEvent += OnInterstitialAdClicked;
			#region Banner
			IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
			IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
			IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
			#endregion

#if !UNITY_EDITOR && UNITY_IOS
			if (isIos14())
			{
				promptAdvertiserTracking();
			}
			else
			{
				InitSDK();
			}
#else
			InitSDK();
#endif
		}

#if !UNITY_EDITOR && UNITY_IOS
		private void Update()
		{
			if (!advertiserTrackingPrompted())
			{
				return;
			}
			InitSDK();
		}
#endif

		private void InitSDK()
		{
			enabled = false;
			var userId = IronSource.Agent.getAdvertiserId();
			IronSource.Agent.setUserId(userId);
#if !UNITY_EDITOR
			Firebase.Analytics.FirebaseAnalytics.SetUserId(userId);
#if UNITY_IOS
			FB.Mobile.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled());
#endif
#endif

			IronSource.Agent.init(Config.IronSourceId);
			StartCoroutine(LoadInterstitialWithDelay(InterstitialLoadDelayTime));
			_state = State.Initialized;
#if DEBUG_ENABLED
			IronSource.Agent.validateIntegration();
#endif
		}

		private static void LoadInterstitial()
		{
			IronSource.Agent.loadInterstitial();
		}

		private static IEnumerator LoadInterstitialWithDelay(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			LoadInterstitial();
		}

		private Action<RewardedVideoState> _rewardedVideoCallback;
		private RewardedVideoState _rewardedVideoState;
		private string _interstitialPlace;
		private string _rewardedVideoPlace;

#if UNITY_EDITOR
		internal bool RewardedVideoReady => true;
#else
		internal bool RewardedVideoReady => _state == State.Initialized && IronSource.Agent.isRewardedVideoAvailable();
#endif

		private bool InterstitialValid => _state == State.Initialized;

		private void ShowReadyInterstitial(Action onFinished)
		{
			if (!InterstitialAllowed)
			{
				onFinished();
				return;
			}

			try
			{
				_onIntersitialRequestProcessed = onFinished;
				IronSource.Agent.showInterstitial("FS" + Config.InterstitialCappingTime);
			}
			catch
			{
				onFinished?.Invoke();
			}
		}

		private bool CanShowInterstitial
		{
			get
			{
				if (Profile.Vip)
				{
#if DEBUG_ENABLED
					Debug.LogError("Cannot show interstitial to VIP");
#endif
					return false;
				}

				// Check availability
				if (!InterstitialValid)
				{ // Not ready yet
#if DEBUG_ENABLED
					Debug.LogError("Interstitial is not either initialized or loaded");
#endif
					return false;
				}

				// Check capping
				if (Time.realtimeSinceStartup - _lastRewardedVideoShowTime < Config.InterstitialRewardedVideoCappingTime)
				{
#if DEBUG_ENABLED
					var t = Time.realtimeSinceStartup - _lastRewardedVideoShowTime;
					Debug.LogError("Rewarded video opened " + t + " seconds ago. Need to wait " +
						(Config.InterstitialRewardedVideoCappingTime - t) + " seconds to show interstitial");
#endif
					return false;
				}
				if (!_interstitialShown)
				{
					if (Time.realtimeSinceStartup - _lastInterstitialShowTime < Config.FirstInterstitialCappingTime)
					{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - _lastInterstitialShowTime;
							Debug.LogError("Need wait " +
								(Config.FirstInterstitialCappingTime - t) + " seconds to show interstitial");
#endif
						return false;
					}
				}
				else
				{
					if (Time.realtimeSinceStartup - _lastInterstitialShowTime < Config.InterstitialCappingTime)
					{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - _lastInterstitialShowTime;
							Debug.LogError("Interstitial opened " + t + " seconds ago. Need to wait " +
								(Config.InterstitialRewardedVideoCappingTime - t) + " seconds to show interstitial");
#endif
						return false;
					}
				}
				return true;
			}
		}

		internal static bool ShowInterstitial(string place, Action onFinished = null)
		{
			if (!Instance.CanShowInterstitial)
			{
				onFinished?.Invoke();
				return false;
			}

			Instance._interstitialPlace = place;
			if (IronSource.Agent.isInterstitialReady())
			{
				Instance.ShowReadyInterstitial(onFinished);
				return true;
			}
			onFinished?.Invoke();
			return false;
		}

		private static void ShowMessage(string msg)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject activity =
			new AndroidJavaClass("com.unity3d.player.UnityPlayer").
			GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject toastClass = new AndroidJavaClass("android.widget.Toast");
			toastClass.CallStatic<AndroidJavaObject>("makeText", activity, msg, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
#else
			Debug.LogWarning(msg);
#endif
		}

		private static void ShowRewardedVideoFailMessage()
		{
			ShowMessage(Application.internetReachability == NetworkReachability.NotReachable
				? "No internet connection. Try again"
				: "No video available at the moment. Try again later");
		}

		internal static void ShowRewardedVideo(string place, Action<RewardedVideoState> callback)
		{
			Instance._rewardedVideoPlace = place;
#if UNITY_EDITOR
			callback(RewardedVideoState.Watched);
#else
			if (Instance._rewardedVideoCallback != null)
			{ // Previous rewarded video request is not finished yet
				callback(RewardedVideoState.Closed);
				return;
			}
			if (!Instance.RewardedVideoReady)
			{
				Analytics.LogRewardedVideoFailedEvent(place);
				callback(RewardedVideoState.NotReady);
				ShowRewardedVideoFailMessage();
				return;
			}
			Analytics.LogRewardVideoClickedEvent(place);
			Adjust.TrackEvent(Adjust.RwClicked);
			Instance._rewardedVideoState = RewardedVideoState.Closed;
			Instance._rewardedVideoCallback = callback;
			SceneManager.ShowLoading();
			if (IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
			}
#endif
		}

		private void OnApplicationPause(bool isPaused)
		{
			IronSource.Agent.onApplicationPause(isPaused);
		}

		private void OnRewardedVideoFailed()
		{
			ShowRewardedVideoFailMessage();
			SceneManager.HideLoading();
			_rewardedVideoCallback?.Invoke(RewardedVideoState.Failed);
			_rewardedVideoCallback = null;
			ShowMessage("Video failed to show. Please retry");
		}

		private void OnRewardedVideoAdShowFailed(IronSourceError error)
		{
			Analytics.LogRewardedVideoFailedEvent(_rewardedVideoPlace);
			OnRewardedVideoFailed();
			ShowMessage("Video failed to show. Please retry");
		}

		private void OnRewardedVideoAdOpened()
		{
			Analytics.LogRewardedVideoShownEvent(_rewardedVideoPlace);
			Adjust.TrackEvent(Adjust.RwShown);
			SceneManager.HideLoading();
			SoundManager.Pause();
		}

		private void OnRewardedVideoAdClosed()
		{
			if (_rewardedVideoState == RewardedVideoState.Watched)
			{
				_lastRewardedVideoShowTime = Time.realtimeSinceStartup;
				Analytics.LogRewardVideoWatchedEvent(_rewardedVideoPlace);
				Adjust.TrackEvent(Adjust.RwWatched);
			}
			SceneManager.HideLoading();
			SoundManager.Resume();
			_rewardedVideoCallback?.Invoke(_rewardedVideoState);
			_rewardedVideoCallback = null;
		}

		private static void OnRewardedVideoAdStarted()
		{

		}

		private void OnRewardedVideoAdEnded()
		{
			_rewardedVideoState = RewardedVideoState.Watched;
		}

		private void OnRewardedVideoAdRewarded(IronSourcePlacement placement)
		{
			_rewardedVideoState = RewardedVideoState.Watched;
		}

		private static void OnRewardedVideoAdClicked(IronSourcePlacement placement)
		{

		}

		private static void OnRewardedVideoAvailabilityChanged(bool available)
		{

		}

		private Action _onIntersitialRequestProcessed;

		private void OnInterstitialAdReady()
		{
		}

		private void OnInterstitialAdLoadFailed(IronSourceError error)
		{
			LoadInterstitial();
		}

		private static void OnInterstitialAdOpened()
		{

		}

		private void OnInterstitialAdClosed()
		{
			SoundManager.Resume();
			LoadInterstitial();
			_lastInterstitialShowTime = Time.realtimeSinceStartup;
			_onIntersitialRequestProcessed?.Invoke();
			_onIntersitialRequestProcessed = null;
		}

		private void OnInterstitialAdShowSucceeded()
		{
			SoundManager.Pause();
			Analytics.LogInterstitialShownEvent(_interstitialPlace);
			_interstitialShown = true;
		}

		private void OnInterstitialAdShowFailed(IronSourceError error)
		{
			Analytics.LogInterstitialFailedEvent(_interstitialPlace);
			_onIntersitialRequestProcessed?.Invoke();
			_onIntersitialRequestProcessed = null;
		}

		private static void OnInterstitialAdClicked()
		{
		}

		#region Banner

		private bool _isBannerReady;

		private bool CanShowBanner =>
			!Profile.Vip &&
			Config.BannerEnabled &&
			_state == State.Initialized &&
			_isBannerReady;

		private void LoadBanner()
		{
			if (!Config.BannerEnabled)
			{
				return;
			}
			if (_isBannerReady)
			{
				return;
			}
			IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
		}

		private void ShowBanner()
		{
			if(!BannerAllowed)
			{
				return;
			}
			if (!CanShowBanner)
			{
				return;
			}
			if (_isBannerReady)
			{
				try
				{
					// TODO: Show Banner shield
					IronSource.Agent.displayBanner();
				}
				catch
				{
					// ignored
				}

				Analytics.LogBannerShownedEvent();
			}
			else
			{
				LoadBanner();
			}
		}

		internal void HideBanner()
		{
			try
			{
				// TODO: Hide Banner shield
				IronSource.Agent.hideBanner();
			}
			catch
			{
				// ignored
			}
		}

		private static void BannerAdClickedEvent()
		{
			Analytics.LogBannerClickedEvent();
		}

		private void BannerAdLoadFailedEvent(IronSourceError obj)
		{
			Analytics.LogBannerFailedEvent();
			_isBannerReady = false;
		}

		private void BannerAdLoadedEvent()
		{
			_isBannerReady = true;
			ShowBanner();
		}

		#endregion
	}
}