using UnityEngine;

namespace Funzilla
{
	internal class Preference : Singleton<Preference>
	{
		private const string OptionSfx = "Sfx";
		private const string OptionMusic = "Music";
		private const string OptionVibration = "Vibration";

		private void Awake()
		{
			_sfxOn = PlayerPrefs.GetInt(OptionSfx, 1) > 0;
			_vibrationOn = PlayerPrefs.GetInt(OptionVibration, 1) > 0;
		}

		private bool _sfxOn = true;
		public static bool SfxOn
		{
			get => Instance._sfxOn;
			set
			{
				Instance._sfxOn = value;
				PlayerPrefs.SetInt(OptionSfx, Instance._sfxOn ? 1 : 0);
			}
		}

		private bool _musicOn = true;
		public static bool MusicOn
		{
			get => Instance._musicOn;
			set
			{
				Instance._musicOn = value;
				PlayerPrefs.SetInt(OptionMusic, Instance._musicOn ? 1 : 0);
				if (value)
				{
					SoundManager.ResumeMusic();
				}
				else
				{
					SoundManager.StopMusic();
				}
			}
		}

		private bool _vibrationOn = true;
		public static bool VibrationOn
		{
			get => Instance._vibrationOn;
			set
			{
				Instance._vibrationOn = value;
				PlayerPrefs.SetInt(OptionVibration, Instance._vibrationOn ? 1 : 0);
			}
		}
	}
}