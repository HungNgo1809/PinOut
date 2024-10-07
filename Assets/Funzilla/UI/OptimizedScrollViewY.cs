using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class OptimizedScrollViewY : MonoBehaviour
	{
		[SerializeField] private ScrollRect scroll;
		[SerializeField] private OptimizedScrollItem itemPrefab;
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

		// Use this for initialization
		protected void Init(int itemCount)
		{
			scroll.onValueChanged.AddListener((position) => { OnScrolled(); });
			_viewportSize = scroll.viewport.rect.height;
			_itemSize = itemPrefab.RectTransform.sizeDelta.y;
			_last = null;
			MakePool();
			_maxVisible = Mathf.FloorToInt(_viewportSize / _itemSize) + 1;
			SetItemCount(itemCount);
			Refresh();
		}

		internal void MoveTo(int index)
		{
			var p = scroll.content.anchoredPosition;
			p.y = _itemSize * index;
			scroll.content.anchoredPosition = p;
			OnScrolled();
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
			scroll.content.sizeDelta = new Vector2(0, _nItems * _itemSize + padding);
		}

		private void OnScrolled()
		{
			if (_nItems <= 0)
			{
				return;
			}

			var y = scroll.content.anchoredPosition.y;
			var iMax = _nItems - 1;
			var iNewFirst = (int)(y / _itemSize);
			var iNewLast = (int)((y + _viewportSize) / _itemSize);
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
				y = iNewFirst * _itemSize;
				_last = _items.First;
				for (var i = iNewFirst; _last != null && i <= iNewLast; i++, _last = _last.Next)
				{
					ShowItem(_last.Value, y, i);
					y -= _itemSize;
				}

				_last = _last?.Previous;
			}
			else
			{
				if (_last != null)
				{
					y = _last.Value.RectTransform.anchoredPosition.y;
					for (var i = iOldLast + 1; i <= iNewLast; i++)
					{
						_last = _last.Next;
						if (_last == null) break;
						y -= _itemSize;
						ShowItem(_last.Value, y, i);
					}
				}

				y = _items.First.Value.RectTransform.anchoredPosition.y;
				for (var i = iOldFirst - 1; i >= iNewFirst; i--)
				{
					y += _itemSize;
					var item = _items.Last;
					_items.RemoveLast();
					_items.AddFirst(item);
					ShowItem(item.Value, y, i);
				}
			}
		}

		private static void ShowItem(OptimizedScrollItem item, float pos, int index)
		{
			item.RectTransform.anchoredPosition = new Vector2(0, pos);
			item.gameObject.SetActive(true);
			item.OnVisible(index);
		}

		[ContextMenu("Test")]
		private void MakePool()
		{
			_items.Clear();
			var h = itemPrefab.RectTransform.sizeDelta.y;
			var vh = scroll.viewport.rect.height;
			var n = Mathf.RoundToInt(vh / h) + 2;
			for (var i = scroll.content.childCount; i < n; i++)
			{
				Instantiate(itemPrefab, scroll.content);
			}

			for (var i = 0; i < n; i++)
			{
				var item = scroll.content.GetChild(i).GetComponent<OptimizedScrollItem>();
				item.RectTransform.anchoredPosition = new Vector2(0, -h * i);
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