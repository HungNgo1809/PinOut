
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && REMOTE_ENABLED
using GameAnalyticsSDK;
using System;
#endif

namespace Funzilla
{
	internal enum ExperimentType
	{
		Origin,
		NewFeature
	}

	internal class ExperimentManager
	{
		internal static List<ExperimentType> Experiments { get; } = new List<ExperimentType>
		{
			ExperimentType.Origin,
			ExperimentType.NewFeature,
		};

		private ExperimentType _activeExperiment;
		internal static ExperimentType ActiveExperiment => Instance._activeExperiment;

		private static ExperimentManager _instance;

		private static ExperimentManager Instance
		{
			get
			{
				_instance ??= new ExperimentManager();
				return _instance;
			}
		}

		private ExperimentManager()
		{
			var configName = "AB_" + Application.version;
#if !UNITY_EDITOR && REMOTE_ENABLED
			var s = GameAnalytics.GetRemoteConfigsValueAsString(
				configName, ExperimentType.Origin.ToString());
			if (!Enum.TryParse(s, out _activeExperiment))
			{
				_activeExperiment = ExperimentType.Origin;
			}
#else
			var experiment = PlayerPrefs.GetInt(configName, -1);
			if (experiment < 0 || experiment >= Experiments.Count)
			{
				experiment = Random.Range(0, Experiments.Count);
				PlayerPrefs.SetInt(configName, experiment);
			}
			_activeExperiment = Experiments[experiment];
#endif
			var logEvent = $"{configName}_{_activeExperiment.ToString()}";
			Analytics.LogEvent(logEvent);
		}

		internal static void ChangeTest(ExperimentType experiment)
		{
			Instance._activeExperiment = experiment;
			LevelManager.LoadLevels();
			SceneManager.ReloadScene(SceneID.Gameplay);
		}
	}
}