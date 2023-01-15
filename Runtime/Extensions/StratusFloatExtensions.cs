using UnityEngine;
using System;
using System.ComponentModel.DataAnnotations;

namespace Stratus.Extensions
{
	public static class StratusFloatExtensions
	{

		/// <summary>
		/// Returns a linearly interpolated value from a (other) to itself (b) at a given t (0-1)
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float LerpFrom(this float b, float a, [ParameterRange(0, 1f)] float t)
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

		public static int RoundToInt(this float value, StratusRoundingMethod method)
			=> (int)value.Round(method);

		/// <summary>
		/// Rounds according to the given method
		/// </summary>
		public static float Round(this float value, StratusRoundingMethod method)
		{
			if (method == StratusRoundingMethod.Default)
			{
				return Mathf.RoundToInt(value);
			}

			const float operand = 1f;
			const float cutoff = 0.5f;

			switch (method)
			{
				case StratusRoundingMethod.Symmetrical:
					{
						float modulo = value % operand;
						bool negative = value < 0;

						float result = negative
							? value + modulo
							: value - modulo;

						if (modulo >= cutoff)
						{
							if (negative)
							{
								result--;
							}
							else
							{
								result++;
							}
						}

						return result;
					}
			}
			throw new NotImplementedException($"Rounding with method `{method}` not implemented");
		}


	}
}

