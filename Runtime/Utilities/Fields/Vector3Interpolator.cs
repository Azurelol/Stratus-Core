using Stratus.Interpolation;

using UnityEngine;

namespace Stratus
{
	namespace Utilities
	{
		/// <summary>
		/// Interpolates a Vector3
		/// </summary>
		public class Vector3Interpolator : StratusInterpolator<Vector3>
		{
			protected override void Interpolate(float t)
			{
				_currentValue = Vector3.Slerp(_startingValue, _EndingValue, t);
			}
		}

		/// <summary>
		/// Interpolates a Vector2
		/// </summary>
		public class Vector2Interpolator : StratusInterpolator<Vector2>
		{
			protected override void Interpolate(float t)
			{
				_currentValue = Vector2.Lerp(_startingValue, _EndingValue, t);
			}
		}
	}
}
