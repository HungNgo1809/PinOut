
using UnityEngine;

namespace Funzilla
{
	internal class PopupFlyingAnimation : PopupAnimation
	{
		[SerializeField] private Transform target;
		[SerializeField] private Vector2 from;
		[SerializeField] private Vector2 to;
		[SerializeField] private float duration = 0.5f;

		private float _time;
		private float _direction;
		private float _targetTime;

		public override void AnimateIn()
		{
			if (target == null || duration <= 0)
			{
				SceneManager.OnSceneAnimatedIn(popup);
			}
			else
			{
				transform.localPosition = from;
				_time = 0;
				_direction = 1;
				_targetTime = duration;
				enabled = true;
			}
		}

		public override void AnimateOut()
		{
			if (target == null || duration <= 0)
			{
				SceneManager.OnSceneAnimatedOut(popup);
			}
			else
			{
				transform.localPosition = to;
				_time = duration;
				_direction = -1;
				_targetTime = 0;
				enabled = true;
			}
		}

		private void Update()
		{
			_time += Time.smoothDeltaTime * _direction;
			var done = _time * _direction > _targetTime;
			if (done)
			{
				_time = _targetTime;
			}

			var t = 1 - _time / duration;
			t *= t;

			target.localPosition = Vector2.Lerp(to, from, t);

			if (!done) return;
			if (_direction > 0)
			{
				SceneManager.OnSceneAnimatedIn(popup);
			}
			else
			{
				SceneManager.OnSceneAnimatedOut(popup);
			}
			enabled = false;
		}
	}
}