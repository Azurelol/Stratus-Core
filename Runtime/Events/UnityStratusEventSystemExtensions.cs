using UnityEngine;
using System;

namespace Stratus
{
	public static class UnityStratusEventSystemExtensions
	{
		/// <summary>
		/// Connects to the specified event on this given object.
		/// </summary>
		/// <typeparam name="TEvent">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="func">The member function callback. </param>
		public static void Connect<TEvent>(this GameObject gameObj, Action<TEvent> func)
			where TEvent : StratusEvent
		{
			UnityStratusEventSystem.Connect(gameObj, func);
		}

		/// <summary>
		/// Connects to all events on the given gameobject
		/// </summary>
		/// <param name="gameObj"></param>
		/// <param name="func"></param>
		/// <param name="type"></param>
		public static void Connect(this GameObject gameObj, Action<StratusEvent> func, Type type)
		{
			UnityStratusEventSystem.Connect(gameObj, type, func);
		}

		/// <summary>
		/// Disconnects this component from all events.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		public static void Disconnect(this MonoBehaviour component)
		{
			UnityStratusEventSystem.Disconnect(component.gameObject);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto this object.
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		/// <param name="nextFrame">Whether the event should be sent next frame.</param>
		public static void Dispatch<TEvent>(this GameObject gameObj, TEvent eventObj) 
			where TEvent : StratusEvent
		{
			UnityStratusEventSystem.Dispatch<TEvent>(gameObj, eventObj);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		/// <param name="nextFrame">Whether the event should be sent next frame.</param>
		public static void Dispatch(this GameObject gameObj, StratusEvent eventObj, Type type)
		{
			UnityStratusEventSystem.Dispatch(gameObj, eventObj);
		}

		/// <summary>
		/// Dispatches the given event of the specified type to this object and all its children.
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchDown<TEvent>(this GameObject gameObj, TEvent eventObj) where TEvent : StratusEvent
		{
			UnityStratusEventSystem.DispatchDown<TEvent>(gameObj, eventObj);
		}

		/// <summary>
		/// Dispatches an event up the tree on each parent recursively.
		/// </summary>
		/// <typeparam name="TEvent">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchUp<TEvent>(this GameObject gameObj, TEvent eventObj) where TEvent : StratusEvent
		{
			UnityStratusEventSystem.DispatchUp<TEvent>(gameObj, eventObj);
		}
	}
}