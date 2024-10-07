using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class LoadingUI : MonoBehaviour
	{
		[SerializeField] private Image background;
		[SerializeField] private Text progressText;
		[SerializeField] private float fakeLoadTime = 1.0f;

		private void Awake()
		{
			DOVirtual
				.Float(0, 100, fakeLoadTime, t =>
				{
					progressText.text = Mathf.FloorToInt(t) + "%";
				})
				.OnComplete(() =>
				{
					progressText.text = "100%";
					background.DOFade(0, 0.3f).OnComplete(() =>
					{
						Destroy(gameObject);
					});
				});
		}
	}
}