using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Stratus
{
	public enum StratusInputActionPhase
	{
		Disabled,
		Waiting,
		Started,
		Performed,
		Canceled,
	}

	public delegate void StratusInputActionCallback(StratusInputActionPhase phase);
	public delegate void StratusInputActionCallback<T>(StratusInputActionPhase phase, T value);

	public static class StratusInputUtility
	{
		public static StratusInputActionPhase Convert(this InputActionPhase phase)
		{
			switch (phase)
			{
				case InputActionPhase.Disabled:
					return StratusInputActionPhase.Disabled;
				case InputActionPhase.Waiting:
					return StratusInputActionPhase.Waiting;
				case InputActionPhase.Started:
					return StratusInputActionPhase.Started;
				case InputActionPhase.Performed:
					return StratusInputActionPhase.Performed;
				case InputActionPhase.Canceled:
					return StratusInputActionPhase.Canceled;
			}
			throw new NotImplementedException(phase.ToString());
		}

		public static InputActionPhase Convert(this StratusInputActionPhase phase)
		{
			switch (phase)
			{
				case StratusInputActionPhase.Disabled:
					return InputActionPhase.Disabled;
				case StratusInputActionPhase.Waiting:
					return InputActionPhase.Waiting;
				case StratusInputActionPhase.Started:
					return InputActionPhase.Started;
				case StratusInputActionPhase.Performed:
					return InputActionPhase.Performed;
				case StratusInputActionPhase.Canceled:
					return InputActionPhase.Canceled;
			}
			throw new NotImplementedException(phase.ToString());
		}
	}

	public class StratusPersistentInputAction<T>
	{
		public T currentValue { get; private set; }
		public InputActionPhase currentPhase { get; private set; }
		public bool active { get; private set; }
		public Action<T> callback { get; private set; }

		public StratusPersistentInputAction(Action<T> callback)
		{
			this.callback = callback;
		}

		public void Set(InputActionPhase phase, T value)
		{
			currentValue = value;
			currentPhase = phase;
			switch (phase)
			{
				case InputActionPhase.Started:
				case InputActionPhase.Performed:
					active = true;
					break;
				case InputActionPhase.Canceled:
				case InputActionPhase.Disabled:
				case InputActionPhase.Waiting:
					active = false;
					break;
			}
		}

		public void Update()
		{
			if (active)
			{
				callback(currentValue);
			}
		}
	}

}