using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	public static partial class StratusRoutines
	{
		public static IEnumerator Lerp<T>(T initialValue, T finalValue, float duration, System.Action<T> setter, System.Func<T, T, float, T> lerpFunction, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			System.Action<float> lerp = (float t) =>
			{
				T currentValue = lerpFunction(initialValue, finalValue, t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(float initialValue, float finalValue, float duration, System.Action<float> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			float diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				float currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(int initialValue, int finalValue, float duration, System.Action<int> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			float diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				float currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(Mathf.CeilToInt(currentValue));
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(bool initialValue, bool finalValue, float duration, System.Action<bool> setter, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			yield return new WaitForSeconds(duration);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector2 initialValue, Vector2 finalValue, float duration, System.Action<Vector2> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector2 diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				Vector2 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector3 initialValue, Vector3 finalValue, float duration, System.Action<Vector3> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector3 diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				Vector3 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector4 initialValue, Vector4 finalValue, float duration, System.Action<Vector4> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector4 diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				Vector4 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Color initialValue, Color finalValue, float duration, System.Action<Color> setter, StratusEase ease = StratusEase.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Color diff = finalValue - initialValue;
			StratusEasing.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (float t) =>
			{
				Color currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}
	}
}