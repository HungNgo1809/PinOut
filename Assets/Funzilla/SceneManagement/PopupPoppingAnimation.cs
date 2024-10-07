
using DG.Tweening;
using UnityEngine;

namespace Funzilla
{
	internal class PopupPoppingAnimation : PopupAnimation
	{
		[SerializeField] private float duration = 0.3f;

		public override void AnimateIn()
		{
			transform.localScale = Vector3.zero;
			transform.DOScale(1, duration).SetEase(Ease.OutBack).OnComplete(()=> {
				SceneManager.OnSceneAnimatedIn(popup);
			});
		}

		public override void AnimateOut()
		{
			transform.localScale = Vector3.one;
			transform.DOScale(0, duration).SetEase(Ease.InBack).OnComplete(() => {
				SceneManager.OnSceneAnimatedOut(popup);
			});
		}
	}
}