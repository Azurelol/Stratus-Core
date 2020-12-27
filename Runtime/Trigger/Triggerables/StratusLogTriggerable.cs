using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Simple event that logs a message to the console when triggered.
	/// </summary>
	public class StratusLogTriggerable : StratusTriggerable
	{
		public LogType type = LogType.Log;

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{
			descriptionMode = DescriptionMode.Manual;
		}

		protected override void OnTrigger()
		{
			StratusDebug.Log(type, description, this); 
		}

	}
}
