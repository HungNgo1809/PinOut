using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class OptimizedScrollViewX : MonoBehaviour
	{
		[SerializeField] private ScrollRect scroll;
		[SerializeField] private OptimizedScrollItem itemPrefab;
		[SerializeField] private RectTransform viewport;
		[SerializeField] private float padding = 20;
		private float _itemSize;

		// Characteristics
		private int _nItems;
		private float _viewportSize;

		// Visible items
		private int _iFirst;
		private int _iLast;
		private readonly LinkedList<OptimizedScrollItem> _items = new LinkedList<OptimizedScrollItem>();
		private LinkedListNode<OptimizedScrollItem> _last;
		private int _maxVisible;

		private void Start()
		{
			scroll.onValueChanged.AddListener((position) => { OnScrolled(); });
		}

		// Use this for initialization
		protected void Init(int itemCount)
		{
			_viewportSize = viewport.rect.width;
			_itemSize = itemPrefab.RectTransform.sizeDelta.x;
			_last = null;
			MakePool();
			_maxVisible = Mathf.FloorToInt(_viewportSize / _itemSize) + 1;
			SetItemCount(itemCount);
			Refresh();
		}

		private void Refresh()
		{
			_iFirst = _iLast = _maxVisible * 3; // Causing refresh
			scroll.content.anchoredPosition = new Vector2(0, 0);
			OnScrolled();
		}

		private void SetItemCount(int itemCount)
		{
			_nItems = itemCount;
			scroll.content.sizeDelta = new Vector2(_nItems * _itemSize + padding, 0);
		}

		private void OnScrolled()
		{
			if (_nItems <= 0)
			{
				return;
			}

			var x = -scroll.content.anchoredPosition.x;
			var iMax = _nItems - 1;
			var iNewFirst = (int)(x / _itemSize);
			var iNewLast = (int)((x + _viewportSize) / _itemSize);
			if (iNewFirst == _iFirst && iNewLast == _iLast)
			{
				return;
			}

			iNewFirst = Mathf.Clamp(iNewFirst, 0, iMax);
			iNewLast = Mathf.Clamp(iNewLast, 0, iMax);
			var iOldFirst = _iFirst;
			var iOldLast = _iLast;
			_iFirst = iNewFirst;
			_iLast = iNewLast;

			var refreshing =
				iNewFirst - iOldFirst >= _maxVisible ||
				iOldLast - iNewLast >= _maxVisible;

			if (refreshing)
			{
				var it = _items.First;
				for (var i = iOldFirst; it != null && i <= iOldLast; i++, it = it.Next)
				{
					it.Value.gameObject.SetActive(false);
				}
			}
			else
			{
				for (var i = iOldFirst; i < iNewFirst; i++)
				{
					var item = _items.First;
					_items.RemoveFirst();
					_items.AddLast(item);
					item.Value.gameObject.SetActive(false);
				}

				for (var i = iNewLast; i < iOldLast; i++)
				{
					if (_last == null) break;
					_last.Value.gameObject.SetActive(false);
					_last = _last.Previous;
				}
			}

			if (refreshing)
			{
				x = iNewFirst * _itemSize;
				_last = _items.First;
				for (var i = iNewFirst; _last != null && i <= iNewLast; i++, _last = _last.Next)
				{
					ShowItem(_last.Value, x, i);
					x += _itemSize;
				}

				_last = _last?.Previous;
			}
			else
			{
				if (_last != null)
				{
					x = _last.Value.RectTransform.anchoredPosition.x;
					for (var i = iOldLast + 1; i <= iNewLast; i++)
					{
						_last = _last.Next;
						if (_last == null) break;
						x += _itemSize;
						ShowItem(_last.Value, x, i);
					}
				}

				x = _items.First.Value.RectTransform.anchoredPosition.x;
				for (var i = iOldFirst - 1; i >= iNewFirst; i--)
				{
					x -= _itemSize;
					var item = _items.Last;
					_items.RemoveLast();
					_items.AddFirst(item);
					ShowItem(item.Value, x, i);
				}
			}
		}

		private static void ShowItem(OptimizedScrollItem item, float pos, int index)
		{
			item.RectTransform.anchoredPosition = new Vector2(pos, 0);
			item.gameObject.SetActive(true);
			item.OnVisible(index);
		}

		[ContextMenu("Test")]
		private void MakePool()
		{
			_items.Clear();
			var w = itemPrefab.RectTransform.sizeDelta.x;
			var n = Mathf.RoundToInt(viewport.rect.width / w) + 2;
			for (var i = scroll.content.childCount; i < n; i++)
			{
				Instantiate(itemPrefab, scroll.content);
			}

			for (var i = 0; i < n; i++)
			{
				var item = scroll.content.GetChild(i).GetComponent<OptimizedScrollItem>();
				item.RectTransform.anchoredPosition = new Vector2(w * i, 0);
#if UNITY_EDITOR
				if (EditorApplication.isPlayingOrWillChangePlaymode)
				{
					item.gameObject.SetActive(false);
				}
				else
				{
					item.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
#else
				item.gameObject.SetActive(false);
#endif
				_items.AddLast(item);
			}
		}
	}
}