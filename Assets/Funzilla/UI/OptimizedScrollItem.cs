using UnityEngine;

namespace Funzilla
{
	internal class OptimizedScrollItem : MonoBehaviour
	{
		[SerializeField] protected RectTransform rectTransform;
		internal RectTransform RectTransform => rectTransform;

		internal virtual void OnVisible(int index)
		{
			
		}
	}
}