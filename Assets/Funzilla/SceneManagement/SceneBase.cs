
using UnityEngine;

namespace Funzilla
{
	internal class SceneBase : MonoBehaviour
	{
		internal SceneID ID { get; set; }

		internal virtual void OnBackButtonPressed()
		{

		}

		internal virtual void AnimateIn()
		{
			SceneManager.OnSceneAnimatedIn(this);
		}

		internal virtual void AnimateOut()
		{
			SceneManager.OnSceneAnimatedOut(this);
		}
	}
}