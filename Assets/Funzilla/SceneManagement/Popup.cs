
using UnityEngine;

namespace Funzilla
{
	internal class Popup : SceneBase
	{
		[SerializeField] private new PopupAnimation animation;

		internal override void AnimateIn()
		{
			if (animation)
			{
				animation.AnimateIn();
			}
			else
			{
				SceneManager.OnSceneAnimatedIn(this);
			}
		}

		internal override void AnimateOut()
		{
			if (animation)
			{
				animation.AnimateOut();
			}
			else
			{
				SceneManager.OnSceneAnimatedOut(this);
			}
		}
	}
}