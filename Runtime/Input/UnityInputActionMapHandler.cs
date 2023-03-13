using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Inputs
{
	/// <summary>
	/// Base class for input action maps
	/// </summary>
	public class UnityInputActionMapHandler : ActionMapHandler<InputAction.CallbackContext>
	{
		private string _name;
		public override string name => _name;

		public UnityInputActionMapHandler(string name = null)
		{
			_name = name;
		}

		public void Bind(string action, Action<InputAction> onAction)
		{
			Bind(action, c => onAction(c.action));
		}

		public void Bind<TValue>(string action, Action<TValue> onAction)
			where TValue : struct
		{
			Bind(action, a => onAction(a.ReadValue<TValue>()));
		}

		public void Bind<TValue>(string action, Action<InputActionPhase, TValue> onAction)
			where TValue : struct
		{
			Bind(action, a => onAction(a.phase, a.ReadValue<TValue>()));
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

		public override bool TryBind(string name, object deleg)
		{
			if (deleg is Action action)
			{
				Bind(name, action, InputActionPhase.Started);
				return true;
			}
			else if (deleg is Action<Vector2> vec2Action)
			{
				Bind(name, vec2Action);
				return true;
			}
			return false;
		}

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			bool handled = false;
			if (context.phase != InputActionPhase.Waiting)
			{
				if (actions.ContainsKey(context.action.name))
				{
					actions[context.action.name].Invoke(context);
					handled = true;
				}
			}
			return handled;
		}

		public StratusInputActionPhase Convert(InputActionPhase phase) => phase.Convert();
	}

	public interface IStratusInputUIActionHandler
	{
		void Navigate(Vector2 dir);
	}
}