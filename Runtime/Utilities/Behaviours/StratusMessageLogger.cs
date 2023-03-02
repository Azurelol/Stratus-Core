using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Stratus.Collections;
using Stratus.Extensions;

namespace Stratus
{
	/// <summary>
	/// Records JSON-compatible messages
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusMessageLogger<T, EventType> : StratusBehaviour
		where T : class
		where EventType : UnityEvent<T>
	{
		[Serializable]
		public class MessageAddedEvent : UnityEvent<T>
		{
		}

		[SerializeField]
		private int capacity = 500;
		private CircularBuffer<T> _messages;		
		[SerializeField]
		private EventType _onMessageAdded;

		protected virtual void OnAwake()
		{
		}

		private void Awake()
		{
			ResetMessages();
			OnAwake();
		}

		public void ResetMessages()
		{
			_messages = new CircularBuffer<T>(capacity);
		}

		public void DeserializeAndLoad(string json, bool additive = false)
		{
			if (!additive && !_messages.IsEmpty)
			{
				ResetMessages();
			}

			T[] messages = JsonUtility.FromJson<T[]>(json);
			messages.ForEach(l => Add(l));
		}

		public string Serialize()
		{
			T[] messages = _messages.ToArray();
			string serialization = JsonUtility.ToJson(messages);
			return serialization;
		}

		public void Add(T message)
		{
			_messages.PushFront(message);
			_onMessageAdded?.Invoke(message);
		}
	}
}