
using System;
using UnityEngine;

namespace Funzilla
{
	public enum BallState
	{
		GoingUp,
		Falling,
		Bouncing
	}

	internal class Player : MonoBehaviour
	{
		private const float Gravity = -20.0f;
		private const float BouncePositionY = -0.5f;
		private float _bounceY;
		private const float MinStretch = 0.8f;
		private const float MaxStretch = 1.2f;
		private float _r, _rMin, _rMax;
		private float _v0; // Velocity at bounce position
		private float _y0; // Bounce position
		private float _t; // Time
		private Vector3 _oldPos;
		private Vector3 _scale;

		private BallState _state = BallState.GoingUp;
		private void Awake()
		{
			var t = transform;
			_scale = t.localScale;
			_r = _scale.x * 0.5f;
			_rMin = _r * MinStretch;
			_rMax = _r * MaxStretch;
			_bounceY = BouncePositionY;
			Jump();
			_oldPos = transform.localPosition;
			_oldPos.y = _bounceY + _rMin;
			_state = BallState.GoingUp;
		}

		private float Step()
		{
			_t += Time.smoothDeltaTime;
			var t = transform;
			var p = _oldPos = t.localPosition;
			p.y = _y0 + _v0 * _t + Gravity * _t * _t * 0.5f;
			t.localPosition = p;
			return p.y - _oldPos.y;
		}

		private void UpdateStretch()
		{
			var v = _v0 + Gravity * _t;
			var k = Mathf.Abs(v / _v0);
			_scale.y = 2 * (_r + (_rMax - _r) * k);
			_scale.x = _scale.z = _r * 4 - _scale.y;
			transform.localScale = _scale;
		}

		// Update is called once per frame
		private void Update()
		{
			switch (_state)
			{
				case BallState.GoingUp:
					if (Step() < 0)
					{
						_state = BallState.Falling;
					}
					UpdateStretch();
					break;

				case BallState.Falling:
					if (Step() < 0)
					{
						Fall();
					}
					UpdateStretch();
					break;

				case BallState.Bouncing:
					var dy = Step();
					if (transform.localPosition.y > _bounceY + _rMax)
					{
						UpdateStretch();
						if (dy > 0)
						{
							_state = BallState.GoingUp;
						}
					}
					else
					{
						var t = transform;
						if (t.localPosition.y < _bounceY + _rMin)
						{
							var p = t.localPosition;
							p.y = _bounceY + _rMin;
							t.localPosition = p;
							Jump();
						}

						_scale.y = 2 * (transform.localPosition.y - _bounceY);
						_scale.x = _scale.z = _r * 4 - _scale.y;
						t.localScale = _scale;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Jump()
		{
			_y0 = _bounceY + _rMin;
			const float y = 3.0f;
			var T = Mathf.Sqrt(2 * y / Mathf.Abs(-Gravity));
			_v0 = -Gravity * T;
			_t = 0;
		}

		private void Fall()
		{
			if (transform.localPosition.y - _rMax > BouncePositionY)
			{
				return;
			}
			_state = BallState.Bouncing;
		}
	}

}

