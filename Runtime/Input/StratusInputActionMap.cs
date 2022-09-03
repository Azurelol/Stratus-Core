using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Linq;

namespace Stratus
{
	/// <summary>
	/// Base class for input action maps
	/// </summary>
	public abstract class StratusInputActionMap
	{
		/// <summary>
		/// The name of the action map these inputs are for
		/// </summary>
		public abstract string map { get; }

		/// <summary>
		/// An abstract function that maps the context to the provided actions in the derived map
		/// </summary>
		/// <param name="context"></param>
		public abstract bool HandleInput(InputAction.CallbackContext context);

		/// <summary>
		/// Converts an input action phase enumerated value from Unity's to this system
		/// </summary>
		/// <param name="phase"></param>
		/// <returns></returns>
		public StratusInputActionPhase Convert(InputActionPhase phase) => StratusInputUtility.Convert(phase);
	}

	public abstract class StratusInputActionMap<T> : StratusInputActionMap
		where T : Enum
	{
		private static Lazy<T[]> enumeratedValues = new Lazy<T[]>(() => StratusEnum.Values<T>());
		private static Lazy<Dictionary<string, T>> enumeratedValuesByName =>
			new Lazy<Dictionary<string, T>>(() => enumeratedValues.Value.ToDictionary(v => v.ToString().ToLowerInvariant()));

		private Dictionary<string, Action<InputAction>> _actions
			= new Dictionary<string, Action<InputAction>>(StringComparer.InvariantCultureIgnoreCase);
		private bool _initialized;

		public IReadOnlyDictionary<string, Action<InputAction>> actions => _actions;
		public bool lowercase { get; private set; }
		public int boundActions => _actions.Count;

		protected virtual void OnInitialize()
		{
		}

		public StratusInputActionMap(bool lowercase = false) : this()
		{
			this.lowercase = lowercase;
		}

		protected StratusInputActionMap()
		{
		}

		private void Initialize()
		{
			OnInitialize();
			_initialized = true;
		}

		public bool Contains(string name) => actions.ContainsKey(name);

		protected void TryBindActions()
		{
			var members = Utilities.StratusReflection.GetAllFieldsOrProperties(this)
				.Where(m => typeof(Delegate).IsAssignableFrom(m.type));

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
			}
		}

		public void Bind(T action, Action<InputAction> onAction)
		{
			string name = action.ToString();
			_actions.AddOrUpdate(lowercase ? name.ToLowerInvariant() : name, onAction);
		}

		public void Bind<ValueType>(T action, Action<ValueType> onAction)
			where ValueType : struct
		{
			Bind(action, a => onAction(a.ReadValue<ValueType>()));
		}

		public void Bind<ValueType>(T action, Action<InputActionPhase, ValueType> onAction)
			where ValueType : struct
		{
			Bind(action, a => onAction(a.phase, a.ReadValue<ValueType>()));
		}

		public void Bind(T action, Action onAction)
		{
			Bind(action, a => onAction());
		}

		public void Bind(T action, Action onAction, InputActionPhase phase)
		{
			Bind(action, a =>
			{
				if (a.phase == phase)
				{
					onAction();
				}
			});
		}

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			bool handled = false;
			if (!_initialized)
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
					Debug.LogWarning($"No action bound for {context.action.name} ({_actions.Count})");
				}
			}
			return handled;
		}
	}

	public class StratusDefaultInputActionMap : StratusInputActionMap
	{
		private string _name;

		public StratusDefaultInputActionMap(string name)
		{

		}
		public override string map => _name;

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			throw new NotImplementedException();
		}
	}

	public interface IStratusInputUIActionHandler
	{
		void Navigate(Vector2 dir);
	}


}