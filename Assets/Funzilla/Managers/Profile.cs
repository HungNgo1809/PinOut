using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Funzilla
{
	internal class Profile : Singleton<Profile>
	{
		private const string Passphase = "Nama1234";
		private const string SaveFile = "/save.dat";

		[Serializable]
		private class UserData
		{
			// Soft currency
			[SerializeField] internal int nCoins;

			// Level
			[SerializeField] internal int level = 1;
			[SerializeField] internal int playCount;

			// First open time
			[SerializeField] internal string firstTime;

			[SerializeField] internal List<string> skins = new List<string>();
			[SerializeField] internal int currentSkin;
		}

		private UserData _data;
		private bool _vip;

		internal static bool Vip
		{
			get => Instance._vip;
			set
			{
				if (Instance._vip == value)
				{
					return;
				}

				Instance._vip = value;
				EventManager.Annouce(EventType.VipChanged);
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			LoadLocal();
		}

		internal static int CurrentSkinIndex
		{
			get => Instance._data?.currentSkin ?? 0;
			set
			{
				if (Instance._data == null || value < 0 || value >= Instance._data.skins.Count) return;
				Instance._data.currentSkin = value;
				RequestSave();
			}
		}

		internal string CurrentSkin
		{
			get
			{
				if (_data?.skins == null ||
					_data.currentSkin < 0 ||
					_data.currentSkin >= _data.skins.Count)
					return string.Empty;

				return _data.skins[_data.currentSkin];
			}
			
			set
			{
				if (_data?.skins == null)
					return;

				var index = _data.skins.IndexOf(value);
				if (index < 0 || index >= _data.skins.Count)
					return;

				_data.currentSkin = index;
				RequestSave();
			}
		}

		internal static List<string> Skins => Instance._data?.skins;

		internal static void UnlockSkin(string skin)
		{
			if (Instance._data == null) return;
			if (string.IsNullOrEmpty(skin)) return;
			if (Instance._data.skins.Contains(skin)) return;
			Instance._data.currentSkin = Instance._data.skins.Count;
			Instance._data.skins.Add(skin);
			RequestSave();
		}

		internal static int CoinAmount
		{
			get => Instance._data?.nCoins ?? 0;
			set
			{
				if (Instance._data == null)
				{
					return;
				}

				Instance._data.nCoins = value;
				EventManager.Annouce(EventType.CoinAmountChanged);
				RequestSave();
			}
		}
		
		internal static int Level
		{
			get => Instance._data?.level ?? 1;
			set
			{
				if (Instance._data == null) return;
				Instance._data.level = value < 1 ? 1 : value;
				RequestSave();
			}
		}

		internal static int PlayCount
		{
			get => Instance._data?.playCount ?? 0;
			set
			{
				if (Instance._data == null) return;
				Instance._data.playCount = value;
				RequestSave();
			}
		}

		internal static DateTime FirstOpenTime
		{
			get
			{
				if (Instance._data == null || string.IsNullOrEmpty(Instance._data.firstTime)) return DateTime.Now;
				try
				{
					return DateTime.Parse(Instance._data.firstTime);
				}
				catch
				{
					// ignored
				}

				return DateTime.Now;
			}
		}

		private void LoadLocal()
		{
			try
			{
				TextReader tr = new StreamReader(Application.persistentDataPath + SaveFile);
				var encryptedJson = tr.ReadToEnd();
				tr.Close();

				var json = Security.Decrypt(encryptedJson, Passphase);
				_data = JsonUtility.FromJson<UserData>(json);
			}
			catch
			{
				// ignored
			}

			if (_data == null)
			{
				_data = new UserData {firstTime = DateTime.Now.ToString(CultureInfo.InvariantCulture)};
				RequestSave();
			}

			_data.skins ??= new List<string>();
			if (_data.skins.Count <= 0)
			{
				_data.skins.Add("Boy");
				_data.currentSkin = 0;
				RequestSave();
			}

			if (_data.level < 1)
			{
				_data.level = 1;
				RequestSave();
			}
		}

		private bool _modifed;

		private static void RequestSave()
		{
			Instance._modifed = true;
		}

		private void Update()
		{
			if (!_modifed) return;
			_modifed = false;
			SaveLocal();
		}

		private void SaveLocal()
		{
			try
			{
				var json = JsonUtility.ToJson(_data);
				var encryptedJson = Security.Encrypt(json, Passphase);

				TextWriter tw = new StreamWriter(Application.persistentDataPath + SaveFile);
				tw.Write(encryptedJson);
				tw.Close();
			}
			catch
			{
				// ignored
			}
		}
	}
}