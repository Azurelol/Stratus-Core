using UnityEngine;
using System;
using System.ComponentModel.DataAnnotations;

namespace Stratus
{
	public static partial class Extensions
	{

		/// <summary>
		/// Returns a linearly interpolated value from a (other) to itself (b) at a given t (0-1)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float LerpFrom(this float b, float a, [ParameterRange(0,1f)] float t)
		{
			return a.LerpTo(b, t);
		}

		/// <summary>
		/// Returns a linearly interpolated value from a (itself) to the target (b) at a given t (0-1)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float LerpTo(this float a, float b, [ParameterRange(0, 1f)] float t)
		{
			return (1f - t) * a + t * b;
		}

		/// <summary>
		/// Rounds the given float value by N decimal places
		/// </summary>
		public static float Round(this float f, int digits = 2) => (float)Math.Round(f, digits);

		/// <summary>
		/// Converts this value to its percentage (dividing by 100)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static float ToPercent(this float f) => (f * 0.01f).Round();

		/// <summary>
		/// Converts this value from a percentage (multiplying by 100)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static float FromPercent(this float f) => (f * 100f).Round();

		/// <summary>
		/// Given a percentage float value, returns the string.
		/// (e.g: 1.5f -> 150.00%)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string FromPercentString(this float f)
		{
			return $"{(f.FromPercent()):0.00}%";
		}

		/// <summary>
		/// Given a percentage float value rounded, returns the string.
		/// (e.g: 1.5f -> 150%)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string FromPercentRoundedString(this float f)
		{
			return $"{(f.FromPercent()):0}%";
		}
	}
}