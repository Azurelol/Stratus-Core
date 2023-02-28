using Stratus.Events;

using System;

namespace Stratus
{
	/// <summary>
	/// Pauses the simulation
	/// </summary>
	public class StratusPauseEvent : Event
	{
	}

	/// <summary>
	/// Resumes the simulation
	/// </summary>
	public class StratusResumeEvent : Event
	{
	}

	/// <summary>
	/// Default options for pausing
	/// </summary>
	[Serializable]
	public class StratusPauseOptions
	{
		public bool timeScale = true;
	}
}