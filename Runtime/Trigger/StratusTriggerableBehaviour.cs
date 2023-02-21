using Stratus.Logging;
using Stratus.Unity.Interpolation;

using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// A component that when triggered will perform a specific action.
	/// </summary>
	public abstract class StratusTriggerableBehaviour : StratusTriggerBase
	{
		#region Fields
		/// <summary>
		/// Whether this event dispatcher will respond to trigger events
		/// </summary>
		[Tooltip("How long after activation before the event is fired")]
		public float delay; 
		#endregion

		#region Properties
		protected StratusTriggerBehaviour.TriggerEvent lastTriggerEvent { get; private set; } 
		#endregion

		#region Events
		/// <summary>
		/// Subscribe to be notified when this trigger has been activated
		/// </summary>
		public UnityAction<StratusTriggerableBehaviour> onTriggered { get; set; }
		#endregion

		#region Virtual
		protected abstract void OnAwake();
		protected abstract void OnTrigger(object data);
		#endregion

		#region Messages
		void Awake()
		{
			awoke = true;
			this.gameObject.Connect<StratusTriggerBehaviour.TriggerEvent>(this.OnTriggerEvent);
			this.OnAwake();
			onTriggered = (StratusTriggerableBehaviour trigger) => { };
		}
		#endregion

		#region Event Handlers
		protected void OnTriggerEvent(StratusTriggerBehaviour.TriggerEvent e)
		{
			lastTriggerEvent = e;
			this.RunTriggerSequence(e.data);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Executes this triggerable, after a set <see cref="delay"/>
		/// </summary>
		public void Trigger(object data = null)
		{
			if (!enabled)
			{
				this.LogWarning($"Cannot trigger while not enabled");
				return;
			}

			if (debug)
			{
				StratusDebug.Log($"<i>{description}</i> has been triggered!", this);
			}

			this.RunTriggerSequence(data);
			activated = true;
		}
		#endregion

		#region Procedures
		protected void RunTriggerSequence(object data)
		{
			var seq = ActionSpace.Sequence(this);
			ActionSpace.Delay(seq, this.delay);
			ActionSpace.Call(seq, () => this.OnTrigger(data));
			ActionSpace.Call(seq, () => onTriggered(this));
		} 
		#endregion
	}

}