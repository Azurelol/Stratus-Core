using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Linq;
using Stratus.Extensions;
using Stratus.Reflection;

namespace Stratus
{
	public interface IStratusInputActionMap
	{
		/// <summary>
		/// The name of the action map these inputs are for
		/// </summary>
		string name { get; }
		/// <summary>
		/// An abstract function that maps the context to the provided actions in the derived map
		/// </summary>
		/// <param name="context"></param>
		bool HandleInput(InputAction.CallbackContext context);
	}

	/// <summary>
	/// Base class for input action maps
	/// </summary>
	public abstract class StratusInputActionMap : IStratusInputActionMap
	{
		protected Dictionary<string, Action<InputAction>> _actions
			= new Dictionary<string, Action<InputAction>>(StringComparer.InvariantCultureIgnoreCase);


		public abstract string name { get; }
		public bool initialized { get; private set; }
		public IReadOnlyDictionary<string, Action<InputAction>> actions
		{
			get
			{
				Initialize();
				return _actions;
			}
		}
		public int count => actions.Count;
		public bool lowercase { get; protected set; }

		public StratusInputActionMap()
		{
		}

		protected virtual void OnInitialize()
		{
		}

		private void Initialize()
		{
			if (initialized)
			{
				return;
			}

			OnInitialize();
			initialized = true;
		}

		public bool Contains(string name) => actions.ContainsKey(name);

		public void Bind(string action, Action<InputAction> onAction)
		{
			_actions.AddOrUpdate(lowercase ? action.ToLowerInvariant() : action, onAction);
		}

		public void Bind<ValueType>(string action, Action<ValueType> onAction)
			where ValueType : struct
		{
			Bind(action, a => onAction(a.ReadValue<ValueType>()));
		}

		public void Bind<ValueType>(string action, Action<InputActionPhase, ValueType> onAction)
			where ValueType : struct
		{
			Bind(action, a => onAction(a.phase, a.ReadValue<ValueType>()));
		}

		public StratusInputActionMap Bind(string action, Action onAction)
		{
			Bind(action, a => onAction());
			return this;
		}

		public void Bind(string action, Action onAction, InputActionPhase phase)
		{
			Bind(action, a =>
			{
				if (a.phase == phase)
				{
					onAction();
				}
			});
		}

		public bool HandleInput(InputAction.CallbackContext context)
		{
			bool handled = false;
			if (!initialized)
			{
				Initialize();
			}
			if (context.phase != InputActionPhase.Waiting)
			{
				if (_actions.ContainsKey(context.action.name))
				{
					_actions[context.action.name].Invoke(context.action);
					handled = true;
				}
				else
				{
					//Debug.LogWarning($"No action bound for {context.action.name} ({_actions.Count})");
				}
			}
			return handled;
		}

		/// <summary>
		/// Converts an input action phase enumerated value from Unity's to this system
		/// </summary>
		/// <param name="phase"></param>
		/// <returns></returns>
		public StratusInputActionPhase Convert(InputActionPhase phase) => StratusInputUtility.Convert(phase);

	}

	public class StratusDefaultInputActionMap : StratusInputActionMap
	{
		private string _name;

		public StratusDefaultInputActionMap(string name)
		{
			_name = name;
		}

		public override string name => _name;
	}

	public abstract class StratusInputActionMap<T> : StratusInputActionMap
		where T : Enum
	{
		private static Lazy<T[]> enumeratedValues = new Lazy<T[]>(() => StratusEnum.Values<T>());
		private static Lazy<Dictionary<string, T>> enumeratedValuesByName =>
			new Lazy<Dictionary<string, T>>(() => enumeratedValues.Value.ToDictionary(v => v.ToString().ToLowerInvariant()));


		public StratusInputActionMap(bool lowercase = false) : this()
		{
			this.lowercase = lowercase;
		}

		protected StratusInputActionMap() : base()
		{
		}

		protected void TryBindActions()
		{
			var members = StratusReflection.GetAllFieldsOrProperties(this)
				.Where(m => typeof(Delegate).IsAssignableFrom(m.type)).ToArray();

			if (members.IsNullOrEmpty())
			{
				StratusDebug.LogWarning($"Found no action members in the map class {GetType().Name}");
			}

			int count = 0;
			foreach (var member in members)
			{
				if (!enumeratedValuesByName.Value.ContainsKey(member.name.ToLowerInvariant()))
				{
					StratusDebug.LogWarning($"No enumeration found for {member.name}");
					continue;
				}

				T value = enumeratedValuesByName.Value[member.name.ToLowerInvariant()];

				if (member.type == typeof(Action))
				{
					Bind(value, (Action)member.value, InputActionPhase.Started);
				}

				if (member.type == typeof(Action<Vector2>))
				{
					Bind(value, (Action<Vector2>)member.value);
				}

				count++;
			}

			if (count == 0)
			{
				StratusDebug.LogWarning($"Found no actions to bind to in {GetType().Name}");
			}
		}

		public void Bind(T action, Action<InputAction> onAction)
		{
			Bind(action.ToString(), onAction);
		}

		public void Bind<ValueType>(T action, Action<ValueType> onAction)
			where ValueType : struct
		{
			Bind(action.ToString(), onAction);
		}

		public void Bind<ValueType>(T action, Action<InputActionPhase, ValueType> onAction)
			where ValueType : struct
		{
			Bind(action.ToString(), onAction);
		}

		public void Bind(T action, Action onAction)
		{
			Bind(action.ToString(), onAction);
		}

		public void Bind(T action, Action onAction, InputActionPhase phase)
		{
			Bind(action.ToString(), onAction, phase);
		}
	}

	public interface IStratusInputUIActionHandler
	{
		void Navigate(Vector2 dir);
	}
}