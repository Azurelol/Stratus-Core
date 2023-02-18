using Stratus.Interpolation;
using Stratus.Unity.Interpolation;

using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
	namespace Examples
	{
		public class ActionsSample : StratusBehaviour
		{
			[Header("Common Settings")]
			public float duration = 1.5f;
			public Ease ease = Ease.Linear;

			[Header("Values")]
			public Color colorValue = Color.red;
			public int integerValue = 7;
			public float floatValue = 7f;
			public Image[] imageGroup;

			public StratusRuntimeMethodField testingMethods;

			// Property: Value type POD
			private float floatProperty { set; get; } = 1.0f;
			// Property: Value type STRUCT
			private Vector2 vector2Property { set; get; } = new Vector2();

			void Awake()
			{
				testingMethods = new StratusRuntimeMethodField(TestActionSequence, TestActionCall, TestActionGroup);
			}

			void TestActionSequence()
			{
				float delay = 2.0f;
				int finalValue = 16;

				var seq = ActionSpace.Sequence(this);
				ActionSpace.Trace(seq, $"Waiting {delay} seconds");
				ActionSpace.Delay(seq, delay);
				ActionSpace.Trace(seq, $"Now interpolating the initial value of {nameof(integerValue)} " +
				  $"from {integerValue} to {finalValue} over {duration} seconds");
				ActionSpace.Property(seq, () => this.integerValue, finalValue, duration, Ease.Linear);
				ActionSpace.Trace(seq, $"The final value of {nameof(integerValue)} is:");
				ActionSpace.Call(seq, () => this.PrintValue(this.integerValue));
			}

			void TestActionGroup()
			{
				StratusDebug.Log($"Action Group Test: Interpolating the color of {imageGroup.Length} images at the same time", this);
				var group = ActionSpace.Group(this);
				foreach (var image in imageGroup)
					ActionSpace.Property(group, () => image.color, colorValue, duration, ease);
			}

			void TestActionCall()
			{
				float boops = 6f;
				float delay = 1.5f;
				string first = null;
				string second = null;

				var seq = ActionSpace.Sequence(this);
				// First, boop
				ActionSpace.Call(seq, () => Boop(boops));
				// Second, wait
				ActionSpace.Trace(seq, $"Waiting {delay} seconds");
				ActionSpace.Delay(seq, delay);
				// Third, set the values to be used
				ActionSpace.Call(seq, () =>
				{
					delay = 5f;
					first = "Hello";
					second = "Mundo";
				});
				ActionSpace.Call(seq, () => Beep(first, second, this.gameObject));
			}

			void Boop(float boopValue)
			{
				StratusDebug.Log("Booped for '" + boopValue + "' points!", this);
			}

			void Beep(string first, string second, GameObject obj)
			{
				StratusDebug.Log(obj + " says = " + first + " " + second);
			}

			void PrintValue(object obj)
			{
				StratusDebug.Log(obj);
			}
		}
	}
}
