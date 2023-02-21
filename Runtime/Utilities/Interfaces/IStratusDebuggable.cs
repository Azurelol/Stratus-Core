using Stratus.Logging;

namespace Stratus
{
	/// <summary>
	/// Interface for globally toggling debug on and off
	/// </summary>
	public interface IStratusDebuggable
	{
		bool debug { get; set; }
	}

	public interface IStratusDebugLogger : IStratusLogger, IStratusDebuggable
	{
	}

	public static class IStratusDebugLoggerExtensions
	{
		public static void Log(this IStratusDebugLogger logger, object value)
		{
			if (logger.debug)
			{
				StratusDebug.Log(value, logger, 2);
			}
		}
	}
}