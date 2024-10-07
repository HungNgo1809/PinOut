
using UnityEngine;

namespace Funzilla
{
	internal class PopupAnimation : MonoBehaviour
	{
		[SerializeField] protected Popup popup;
		public virtual void AnimateIn()
		{
			SceneManager.OnSceneAnimatedIn(popup);
		}

		public virtual void AnimateOut()
		{
			SceneManager.OnSceneAnimatedOut(popup);
		}
	}
}