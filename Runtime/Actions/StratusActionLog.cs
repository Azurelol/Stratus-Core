using Stratus.Interpolation;

using UnityEngine;

namespace Stratus.Unity.Interpolation
{
	/// <summary>
	/// An action that logs to the console
	/// </summary>
	public class StratusActionLog : ActionBase
	{
		MonoBehaviour target;
		object message;

		public StratusActionLog(object message, MonoBehaviour obj = null)
		{
			this.message = message;
			target = obj;
		}

		public override float Update(float dt)
		{
			StratusDebug.Log(message, this.target);
			this.isFinished = true;

			if (ActionSpace.debug)
			{
				StratusLog.Info("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}

	}

}