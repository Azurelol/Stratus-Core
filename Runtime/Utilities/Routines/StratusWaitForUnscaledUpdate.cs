using UnityEngine;
using Stratus;

namespace Stratus
{
	public class StratusWaitForUnscaledUpdate : CustomYieldInstruction
	{
		private float WaitTIme;

		public override bool keepWaiting
		{
			get
			{
				return Time.realtimeSinceStartup < WaitTIme;
			}
		}

		public StratusWaitForUnscaledUpdate()
		{
			WaitTIme = Time.realtimeSinceStartup + Time.fixedDeltaTime;
		}
	}
}
