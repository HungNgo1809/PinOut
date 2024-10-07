
using UnityEngine;

#if !UNITY_EDITOR && !DEBUG_ENABLED
using System.Collections.Generic;
using Firebase.Analytics;
using System;
using GameAnalyticsSDK;
#endif

namespace Funzilla
{
	internal class Analytics : Singleton<Analytics>
	{

#if !UNITY_EDITOR && !DEBUG_ENABLED
		private class FirebaseEvent
		{
			internal FirebaseEvent(string name, Parameter[] parameters)
			{
				Name = name;
				Parameters = parameters;
			}
			internal readonly string Name;
			internal readonly Parameter[] Parameters;
		}

		private static readonly Queue<FirebaseEvent> FirebaseEvents = new Queue<FirebaseEvent>(4);

		private static void LogFirebaseEvent(string eventName)
		{
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				FirebaseEvents.Enqueue(new FirebaseEvent(eventName, null));
			}
		}

		private static void LogFirebaseEvent(string eventName, string paramName, string paramValue)
		{
			var firebaseParameters = new Parameter[] {
				new Parameter(paramName, paramValue),
			};
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				FirebaseEvents.Enqueue(new FirebaseEvent(eventName, firebaseParameters));
			}
		}

		private static void LogFirebaseEvent(string eventName, string param1Name, string param1Value, string param2Name, string param2Value)
		{
			var firebaseParameters = new Parameter[] {
				new Parameter(param1Name, param1Value),
				new Parameter(param2Name, param2Value),
			};
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				FirebaseEvents.Enqueue(new FirebaseEvent(eventName, firebaseParameters));
			}
		}

		private void Update()
		{
			if (!GameManager.FirebaseOk) return;
			while (FirebaseEvents.Count > 0)
			{
				var evt = FirebaseEvents.Dequeue();
				try
				{
					if (evt.Parameters == null)
					{
						FirebaseAnalytics.LogEvent(evt.Name);
					}
					else
					{
						FirebaseAnalytics.LogEvent(evt.Name, evt.Parameters);
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
		}
#endif

		internal static void LogEvent(string eventName)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.Log("Event: " + eventName);
#else
			LogFirebaseEvent(eventName);
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		internal static void LogEvent(string eventName, string paramName, string paramValue)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.Log($"Event: {eventName}, {paramName}={paramValue}");
#else
			LogFirebaseEvent(eventName, paramName, paramValue);
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		internal static void LogEvent(string eventName, string param1Name, string param1Value, string param2Name, string param2Value)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.Log($"Event: {eventName}, {param1Name}={param1Value}, {param2Name}={param2Value}");
#else
			LogFirebaseEvent(eventName, param1Name, param1Value, param2Name, param2Value);
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		internal static void LogInterstitialFailedEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Interstitial failed at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, Ads.SdkName, placement);
			LogFirebaseEvent("ad_fs_failed");
#endif
		}
		
		internal static void LogInterstitialShownEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Interstitial showed at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, Ads.SdkName, placement);
			LogFirebaseEvent("ad_fs_shown", "place", placement);
#endif
			Adjust.TrackEvent(Adjust.FsShown);
		}
		
		internal static void LogInterstitialClickedEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Interstitial clicked at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, Ads.SdkName, placement);
			LogFirebaseEvent("ad_fs_clicked", "place", placement);
#endif
		}
		
		internal static void LogRewardedVideoFailedEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Rewarded video failed at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, Ads.SdkName, placement);
			LogFirebaseEvent("ad_rw_shown", "place", placement);
#endif
		}
		
		internal static void LogRewardedVideoShownEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Rewarded video showed at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, Ads.SdkName, placement);
			LogFirebaseEvent("ad_rw_shown", "place", placement);
#endif
			Adjust.TrackEvent(Adjust.RwShown);
		}
		
		internal static void LogRewardVideoClickedEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Rewarded video watched at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, Ads.SdkName, placement);
			LogFirebaseEvent("ad_rw_clicked", "place", placement);
#endif
			Adjust.TrackEvent(Adjust.RwClicked);
		}
		
		internal static void LogRewardVideoWatchedEvent(string placement)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"Rewarded video watched at {placement}");
#else
			GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, Ads.SdkName, placement);
			LogFirebaseEvent("ad_rw_watched", "place", placement);
#endif
			Adjust.TrackEvent(Adjust.RwWatched);
		}

		internal static void LogBannerFailedEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError("Banner failed");
#else
			GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Banner, Ads.SdkName, string.Empty);
			LogFirebaseEvent("ad_bn_failed");
#endif
		}
		
		internal static void LogBannerClickedEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError("Banner clicked");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Banner, Ads.SdkName, string.Empty);
			LogFirebaseEvent("ad_bn_clicked");
#endif
		}
		
		internal static void LogBannerShownedEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError("Banner shown");
#else
			GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, Ads.SdkName, string.Empty);
			LogFirebaseEvent("ad_bn_shown");
#endif
		}

		internal static void LogLevelStartEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"level_{Profile.Level}_start");
#else
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Profile.Level.ToString());
			LogFirebaseEvent($"level_{Profile.Level}_start");
#endif
			Adjust.TrackEvent(Adjust.LevelStart);
		}
		
		internal static void LogLevelCompleteEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"level_{Profile.Level}_start");
#else
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Profile.Level.ToString());
			LogFirebaseEvent($"level_{Profile.Level}_complete");
#endif
		}
		
		internal static void LogLevelFailEvent()
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError($"level_{Profile.Level}_start");
#else
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Profile.Level.ToString());
			LogFirebaseEvent($"level_{Profile.Level}_fail");
#endif
		}
	}
}