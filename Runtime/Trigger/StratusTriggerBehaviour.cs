using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Stratus.Events;

namespace Stratus
{
	/// <summary>
	/// A component that triggers a selected triggerable when the specified condition is met.
	/// </summary>
	public abstract class StratusTriggerBehaviour : StratusTriggerBase
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// When received by a triggerable component, it will activate it
		/// </summary>
		public class TriggerEvent : Events.Event
		{
			public object data { get; private set; }

			public TriggerEvent()
			{
			}

			public TriggerEvent(object data)
			{
				Update(data);
			}

			internal void Update(object data)
			{
				this.data = data;
			}
		}

		/// <summary>
		/// How the trigger is delivered to the target triggerable
		/// </summary>
		public enum Scope
		{
			[Tooltip("Only the target triggerable will be triggered")]
			Component,
			[Tooltip("All triggerable components in the GameObject will be triggered")]
			GameObject
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// What component to send the trigger event to
		/// </summary>
		[Header("Targeting")]
		[Tooltip("What component to send the trigger event to")]
		public List<StratusTriggerableBehaviour> targets = new List<StratusTriggerableBehaviour>();
		/// <summary>
		/// Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component
		/// </summary>
		[Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
		public Scope scope = Scope.Component;
		/// <summary>
		/// Whether it should also trigger all of the target's children
		/// </summary>
		[Tooltip("Whether it should also trigger all of the target's children")]
		public bool recursive = false;

		[Header("Lifetime")]
		/// <summary>
		/// Whether the trigger should persist after being activated
		/// </summary>
		[Tooltip("")]
		public bool persistent = true;
		/// <summary>
		/// Subscribe to be notified when this trigger has been activated
		/// </summary>
		public UnityAction<StratusTriggerBehaviour> onActivate { get; set; } = (StratusTriggerBehaviour trigger) => { };

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The trigger event being dispatched
		/// </summary>
		private TriggerEvent triggerEvent => new TriggerEvent();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// On awake, thee trigger first initializes the subclass before connecting to enabled events
		/// </summary>
		void Awake()
		{
			awoke = true;
			this.OnAwake();
		}
		/// <summary>
		/// Invoked the first time this trigger is initialized
		/// </summary>
		protected abstract void OnAwake();

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Triggers the event
		/// </summary>
		protected void Activate(object data = null)
		{
			if (!enabled)
			{
				return;
			}

			if (debug)
			{
				StratusDebug.Log($"<i>{description}</i> has been activated!", this);
			}

			// Dispatch the trigger event onto a given target if one is provided
			foreach (var target in targets)
			{
				if (target == null)
					continue;

				if (scope == Scope.GameObject)
				{
					target.gameObject.Dispatch<TriggerEvent>(triggerEvent);
					if (this.recursive)
					{
						foreach (var child in target.gameObject.Children())
						{
							child.Dispatch<TriggerEvent>(triggerEvent);
						}
					}
				}

				else if (scope == Scope.Component)
				{
					target.Trigger();
				}
			}

			// If not persistent, disable
			if (!persistent)
			{
				this.enabled = false;
			}

			// Announce this trigger was activated
			activated = true;
			this.onActivate(this);

		}








	}

}