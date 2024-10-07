using System;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class LevelButton : OptimizedScrollItem
	{
		[SerializeField] private Text text;
		[SerializeField] private Button button;

		private int _index;
		private void Start()
		{
			button.onClick.AddListener(() =>
			{
				Profile.Level = _index + 1;
				SceneManager.ReloadScene(SceneID.Gameplay);
				GetComponentInParent<LevelSelector>().gameObject.SetActive(false);
			});
		}

		internal override void OnVisible(int index)
		{
			_index = index;
			button.interactable = index != Profile.Level - 1;
			text.text = index >= 0 && index < LevelManager.Levels.Count ? $"{index + 1} - {LevelManager.Levels[index]}" : "";
		}
	}
}