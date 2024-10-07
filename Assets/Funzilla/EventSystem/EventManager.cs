
using System;
using System.Collections.Generic;
using System.Linq;

namespace Funzilla
{
	internal class EventManager : Singleton<EventManager>
	{
		private class Event
		{
			public Event(EventType type, object data)
			{
				Type = type;
				Data = data;
			}
			internal readonly EventType Type;
			internal readonly object Data;
		}

		private readonly List<Action<object>> _actions = new List<Action<object>>(Enum.GetNames(typeof(EventType)).Length);
		private readonly Queue<Event> _events = new Queue<Event>();

		private void Awake()
		{
			for (var i = 0; i < Enum.GetNames(typeof(EventType)).Length; i++)
			{
				_actions.Add(null);
			}
		}

		internal static void Subscribe(EventType type, Action<object> action)
		{
			if (Instance._actions[(int)type] == null)
			{
				Instance._actions[(int)type] = action;
			}
			else if (!Instance._actions[(int)type].GetInvocationList().Contains(action))
			{
				Instance._actions[(int)type] += action;
			}
		}

		internal static void Unsubscribe(EventType type, Action<object> action)
		{
			if (Instance._actions[(int)type] != null)
			{
				Instance._actions[(int)type] -= action;
			}
		}

		internal static void Annouce(EventType type, object data = null)
		{
			Instance._events.Enqueue(new Event(type,data));
		}

		private void Dispatch()
		{
			if (_events == null || _events.Count <= 0)
			{
				return;
			}
			var e = _events.Dequeue();
			_actions[(int)e.Type]?.Invoke(e.Data);
		}

		private void Update()
		{
			Dispatch();
		}
	}
}