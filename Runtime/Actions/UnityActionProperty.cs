using Stratus.Interpolation;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Stratus.Unity.Interpolation
{
	/// <summary>
	/// Used for interpolating a Color value
	/// </summary>
	public class ActionPropertyColor : ActionProperty<Color>
	{
		public ActionPropertyColor(object target, PropertyInfo property, Color endValue, float duration, Ease ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyColor(object target, FieldInfo field, Color endValue, float duration, Ease ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference()
		{
			this.difference = this.endValue - this.initialValue;
		}

		public override Color ComputeCurrentValue(float easeVal)
		{
			return this.initialValue + this.difference * easeVal;
		}
	}

	/// <summary>
	/// Used for interpolating a Quaternion value
	/// </summary>
	public class ActionPropertyQuaternion : ActionProperty<Quaternion>
	{
		public ActionPropertyQuaternion(object target, PropertyInfo property, Quaternion endValue, float duration, Ease ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyQuaternion(object target, FieldInfo field, Quaternion endValue, float duration, Ease ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { }
		public override Quaternion ComputeCurrentValue(float easeVal) { return new Quaternion(); }
		public override void SetCurrent()
		{
			//Debug.Log("Setting!");
			Set(Quaternion.Lerp(initialValue, endValue, Time.time * Time.deltaTime));
		}

		public override void SetLast()
		{
			Set(endValue);
		}
	}
}