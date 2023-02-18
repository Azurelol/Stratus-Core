using UnityEngine;
using System;
using System.Collections;
using Stratus.Interpolation;
using Stratus.Unity.Interpolation;

namespace Stratus
{
	/// <summary>
	/// Provides common operations on a <seealso cref="Transform"/>
	/// </summary>
	public class StratusTransformTriggerable : StratusTriggerableBehaviour, StratusTriggerBase.Restartable
	{
		public enum ValueType
		{
			[Tooltip("Use a static value")]
			Static,
			[Tooltip("Use the values of another transform")]
			Mirror
		}

		public enum EventType
		{
			Translate,
			Rotate,
			RotateAround,
			Scale,
			Parent,
			Reset
		}

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		[Tooltip("The transform whos is being operated on")]
		public Transform target;
		[Tooltip("The type of the event")]
		public EventType eventType;
		[Tooltip("The duration of the event")]
		public float duration = 1.0f;
		[Tooltip("The interpolation algorithm to use")]
		public Ease ease;

		// Values
		[Tooltip("How the value to use is decided")]
		public ValueType valueType;
		[Tooltip("The value to set")]
		[DrawIf("valueType", ValueType.Static, ComparisonType.Equals)]
		public Vector3 value;
		[Tooltip("The transform whose values we are using for the operation")]
		[DrawIf("valueType", ValueType.Mirror, ComparisonType.Equals)]
		public Transform source;

		// Options
		[DrawIf("eventType", EventType.RotateAround, ComparisonType.Equals)]
		public float angleAroundTarget = 0f;
		[DrawIf("eventType", EventType.RotateAround, ComparisonType.Equals)]
		public Vector3 axis = Vector3.up;

		[Tooltip("Whether to use local coordinates for the translation (respective to the parent)")]
		[DrawIf("eventType", EventType.Translate, ComparisonType.Equals)]
		public bool local = false;
		[Tooltip("Whether the value is an offset from the current value")]
		[DrawIf("valueType", ValueType.Mirror, ComparisonType.NotEqual)]
		public bool offset = false;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public Vector3 previousValue { get; private set; }
		private ActionSet currentAction { get; set; }
		private IEnumerator currentCoroutine { get; set; }
		private bool isMirror => valueType == ValueType.Mirror;
		private TransformationType transformationType;

		public override string automaticDescription
		{
			get
			{
				if (target)
					return $"{eventType} {target.name} over {duration}s";
				return string.Empty;
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{
		}

		protected override void OnTrigger(object data = null)
		{
			Apply(value, true);
		}

		public void OnRestart()
		{
			Cancel();
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Interpolates to the specified transformation.
		/// </summary>
		public void Apply(Vector3 value, bool applyOffset)
		{
			currentAction = ActionSpace.Sequence(this);

			switch (eventType)
			{
				case EventType.Translate:
					transformationType = TransformationType.Translate;
					if (valueType == ValueType.Static)
					{
						if (local)
						{
							previousValue = target.localPosition;
							if (applyOffset) value += previousValue;
							currentCoroutine = StratusRoutines.Interpolate(previousValue, value, duration, (Vector3 val) => { target.localPosition = val; }, ease);
						}
						else
						{
							previousValue = target.position;
							if (applyOffset) value += previousValue;
							currentCoroutine = StratusRoutines.Interpolate(previousValue, value, duration, (Vector3 val) => { target.position = val; }, ease);
						}
					}
					else if (valueType == ValueType.Mirror)
					{
						previousValue = target.position;
						currentCoroutine = StratusRoutines.Interpolate(previousValue, source.position, duration, (Vector3 val) => { target.position = val; }, ease);
					}
					target.StartCoroutine(currentCoroutine, transformationType);
					break;

				case EventType.Rotate:
					transformationType = TransformationType.Rotate;
					previousValue = target.rotation.eulerAngles;
					if (applyOffset) value += previousValue;
					currentCoroutine = StratusRoutines.Rotate(target, isMirror ? source.rotation.eulerAngles : value, duration);
					target.StartCoroutine(currentCoroutine, transformationType);
					break;

				case EventType.RotateAround:
					transformationType = TransformationType.Translate | TransformationType.Rotate;
					previousValue = target.rotation.eulerAngles;
					if (applyOffset) value += previousValue;
					currentCoroutine = StratusRoutines.RotateAround(target, isMirror ? source.position : value, axis, angleAroundTarget, duration);
					target.StartCoroutine(currentCoroutine, transformationType);
					break;

				case EventType.Scale:
					transformationType = TransformationType.Scale;
					previousValue = target.localScale;
					if (applyOffset) value += previousValue;
					currentCoroutine = StratusRoutines.Interpolate(previousValue, isMirror ? source.localScale : value, duration, (Vector3 val) => { target.localScale = val; }, ease);
					target.StartCoroutine(currentCoroutine, transformationType);
					break;

				case EventType.Parent:
					ActionSpace.Delay(currentAction, duration);
					ActionSpace.Call(currentAction, () => { target.SetParent(source); });
					break;

				case EventType.Reset:
					ActionSpace.Delay(currentAction, duration);
					ActionSpace.Call(currentAction, () => { target.Reset(); });
					break;
			}
		}

		/// <summary>
		/// Cancels the current transformation event
		/// </summary>
		public void Cancel()
		{
			currentAction?.Cancel();
			if (currentCoroutine != null)
			{
				target.StopCoroutine(transformationType);
				currentCoroutine = null;
			}
		}

		/// <summary>
		/// Reverts to the previous transformation.
		/// </summary>
		public void Revert()
		{
			this.Apply(this.previousValue, false);
		}


	}

}