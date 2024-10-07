using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class FPS : MonoBehaviour
	{
		[SerializeField] private Text fpsText;

		private float _time;
		private float _seconds;
		private int _frames;

		private void Update()
		{
			var now = Time.realtimeSinceStartup;
			var delta = now - _time;
			_time = now;
			_seconds += delta;
			_frames++;
			if (!(_seconds >= 1.0f)) return;
			var s = Mathf.CeilToInt(_frames / _seconds).ToString("00") + "FPS";
			if (fpsText != null)
			{
				fpsText.text = s;
			}
			
			_seconds = 0;
			_frames = 0;
		}
	}
}
