using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class WinUI : Scene
	{
		[SerializeField] private Button watchAdButton;
		[SerializeField] private Button noThanksButton;

		private void Awake()
		{
			watchAdButton.transform.HeartBeat(1.1f);
			watchAdButton.onClick.AddListener(() =>
			{
				Ads.ShowRewardedVideo("WinX3Gems", result =>
				{
					if (result != RewardedVideoState.Watched) return;
					Profile.CoinAmount += Gameplay.CoinsEarned * 3;
					Gameplay.CoinsEarned = 0;
					Close();
				});
			});
			noThanksButton.onClick.AddListener(() =>
			{
				Profile.CoinAmount += Gameplay.CoinsEarned;
				Gameplay.CoinsEarned = 0;
				Close();
			});
		}

		private void Close()
		{
			SceneManager.ShowLoading( () =>
			{
				SceneManager.ReloadScene(SceneID.Gameplay);
				SceneManager.CloseScene(SceneID.WinUI);
			});
		}
	}
}