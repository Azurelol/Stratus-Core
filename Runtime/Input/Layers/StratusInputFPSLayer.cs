using Stratus.Inputs;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus
{
	public class StratusInputFPSActionMap : StratusInputActionMap
	{
		public override string name { get; }
	}

	public class StratusFPSInputLayer<ActionMap> : StratusInputLayer<ActionMap>
		where ActionMap : StratusInputActionMap, new()
	{
		public bool lockCursor;

		public StratusFPSInputLayer(string label) : base(label)
		{
		}

		public StratusFPSInputLayer(string label, ActionMap actions) : base(label, actions)
		{
		}

		protected override void OnActive(bool active)
		{
			if (lockCursor)
			{
				if (active)
				{
					StratusCursorLock.LockCursor(CursorLockMode.Locked, false);
				}
				else
				{
					StratusCursorLock.LockCursor(CursorLockMode.None, true);
				}
			}
		}
	}

}