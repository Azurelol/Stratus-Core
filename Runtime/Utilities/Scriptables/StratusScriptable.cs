using Stratus.OdinSerializer;

namespace Stratus
{
	/// <summary>
	/// Base class for scriptable objects within the Stratus Framework
	/// </summary>
	public abstract class StratusScriptable : SerializedScriptableObject, IStratusLogger
	{
	}

	/// <summary>
	/// Scriptable object for one data member
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusScriptable<T> : StratusScriptable
	{
		public T data;
	}
}
