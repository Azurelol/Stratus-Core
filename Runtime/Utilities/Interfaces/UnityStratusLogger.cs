using Stratus.Logging;

using System;

using UnityEngine;

namespace Stratus
{
	public class UnityStratusLogger : StratusLogger
	{
		public override void LogError(string message)
		{
			StratusDebug.LogError(message);
		}

		public override void LogInfo(string message)
		{
			StratusDebug.Log(message, 2);
		}

		public override void LogWarning(string message)
		{
			StratusDebug.LogWarning(message);
		}

		public override void LogException(Exception ex)
		{
			StratusDebug.LogError(ex);
		}
	}
}