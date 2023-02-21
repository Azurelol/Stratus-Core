using Stratus.Logging;
using Stratus.OdinSerializer;

using System;

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Base class for scriptable objects within the Stratus Framework
	/// </summary>
	public abstract class StratusScriptable : SerializedScriptableObject, IStratusLogger
	{
		public const string scriptablesMenu = StratusCore.rootMenu + "Scriptables/";
	}

	/// <summary>
	/// Scriptable object for one data member
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusScriptable<T> : StratusScriptable
	{
		[SerializeField]
		private T _data;
		public T data => _data;
	}
}
