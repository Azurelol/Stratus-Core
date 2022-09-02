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
		private Lazy<T[]> availableActions = new Lazy<T[]>(() => StratusEnum.Values<T>());
		private Dictionary<string, Action<InputAction>> actions
			= new Dictionary<string, Action<InputAction>>(StringComparer.InvariantCultureIgnoreCase);
		public bool lowercase { get; private set; }

		public StratusInputActionMap(bool lowercase = false) : this()
		{
			this.lowercase = lowercase;
		}

		protected StratusInputActionMap()
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
			// Try binding all actions based by their type
			var fieldsOrProperties = Utilities.StratusReflection.GetAllFieldsOrProperties(this);				
			//foreach (var field in Utilities.)
		}

		public void Bind(T action, Action<InputAction> onAction)
		{
			string name = action.ToString();
			actions.AddOrUpdate(lowercase ? name.ToLowerInvariant() : name, onAction);
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
			if (context.phase != InputActionPhase.Waiting)
			{
				if (actions.ContainsKey(context.action.name))
				{
					actions[context.action.name].Invoke(context.action);
					handled = true;
				}
				else
				{
					Debug.LogWarning($"No action {context.action.name} ({actions.Count})");
				}
			}
			return handled;
		}
	}

	public interface IStratusInputUIActionHandler
	{
		void Navigate(Vector2 dir);
	}


}