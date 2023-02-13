using Stratus.Interpolation;
using Stratus.Utilities;

using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

namespace Stratus.Unity.Interpolation
{
	/// <summary>
	/// Handles the updating of all actions.
	/// </summary>
	[StratusSingleton("Stratus Action System", true, true)]
	public class ActionSpace : StratusSingletonBehaviour<ActionSpace>
	{
		#region Properties
		public ActionScheduler<GameObject> scheduler { get; } = new ActionScheduler<GameObject>();
		#endregion

		#region Messages
		protected override void OnAwake()
		{
			scheduler.onConnect += this.OnConnect;
			scheduler.onDisconnect += this.OnDisconnect;
		}
		
		private void OnConnect(GameObject obj)
		{
			gameObject.GetOrAddComponent<StratusActionsRegistration>();
		}

		private void OnDisconnect(GameObject obj)
		{
			gameObject.RemoveComponent<StratusActionsRegistration>();
		}

		private void FixedUpdate()
		{
			scheduler.Update(Time.deltaTime);
		}
		#endregion
	}
}