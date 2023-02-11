﻿using System;
using UnityEngine;

namespace Stratus
{
	//public static class IStratusLoggerExtensions
 //   {
	//	/// <summary>
	//	/// Prints the given message to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void Log(this IStratusLogger logger, object value) => StratusDebug.Log(value, logger, 2);

	//	/// <summary>
	//	/// Prints the given message to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void LogIf(this IStratusLogger logger, bool condition, object value) => StratusDebug.LogIf(condition, value, logger, 2);

	//	/// <summary>
	//	/// Prints the given warning message to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void LogWarning(this IStratusLogger logger, object value) => StratusDebug.LogWarning(value, logger, 2);

	//	/// <summary>
	//	/// Prints the given error message to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void LogError(this IStratusLogger logger, object value) => StratusDebug.LogError(value, logger, 2);

	//	/// <summary>
	//	/// Prints the given exception to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void LogException(this IStratusLogger logger, Exception e) => StratusDebug.LogException(e);

	//	/// <summary>
	//	/// Prints the given operation result to the console
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public static void Log(this IStratusLogger logger, StratusOperationResult result) => StratusDebug.Log(result, logger, 2);
	//}

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