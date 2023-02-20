using Stratus.Timers;

using UnityEngine;

namespace Stratus.Extensions
{
	public static class StratusTimerExtensions
	{
		/// <summary>
		/// Updates the timer by the default delta time (Time.deltaTime)
		/// </summary>
		/// <returns>True if is done, false otherwise</returns>
		public static bool Update(this Timer timer)
		{
			if (timer.isFinished)
			{
				return true;

			}
			return timer.Update(Time.deltaTime);
		}
	}
}
