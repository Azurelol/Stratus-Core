using UnityEngine;
using Stratus;
using System;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// Invokes a method when triggered.
	/// </summary>
	public class StratusInvokeTriggerable : StratusTriggerableBehaviour
	{
		public UnityEvent callbacks = new UnityEvent();

		/// <summary>
		/// The number of methods to invoke
		/// </summary>
		public int count => callbacks.GetPersistentEventCount();

		public override string automaticDescription
		{
			get
			{
				if (count > 0)
				{
					string description = $"Invoke {callbacks.GetPersistentTarget(0)}.{callbacks.GetPersistentMethodName(0)}";
					for (int i = 1; i < count; ++i)
						description += $", {callbacks.GetPersistentTarget(i)}.{callbacks.GetPersistentMethodName(i)}";

					return description;
				}
				return string.Empty;

			}
		}

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger(object data = null)
		{
			callbacks.Invoke();
		}
	}

}