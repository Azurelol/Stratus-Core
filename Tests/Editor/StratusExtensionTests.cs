using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	[ClassDescription(classDescription)]
	internal class TestDataObject
	{
		[MemberDescription(nameDescription)]
		public string name;
		[HideInInspector, SerializeField]
		public int value;
		[MemberDescription(inverseValueDescription)]
		public int inverseValue => -this.value;

		public const string classDescription = "A test class used for the unit tests";
		public const string nameDescription = "The name of the object";
		public const string inverseValueDescription = "The inverse value";

		public TestDataObject(string name, int value)
		{
			this.name = name;
			this.value = value;
		}

		public override string ToString()
		{
			return this.name;
		}

		public void Boop(int n, int b)
		{
			n.Iterate(() => Console.WriteLine(this.value + b));
		}
	}

	public partial class StratusExtensionsTests
	{
		[Test]
		public void TestIntegerExtensions()
		{
			// Iterate
			{
				int n = 3;

				int result = 0;
				n.Iterate(() => result += 1);
				Assert.AreEqual(n, result);

				result = 0;
				n.Iterate((i) => result += i * 2);
				Assert.AreEqual(0 * 2 + 1 * 2 + 2 * 2, result);

				result = 3;
				n.IterateReverse((i) =>
				{
					Assert.AreEqual(result - 1, i);
					result--;
				});
			}
		}

		[Test]
		public static void TestVectorExtensions()
		{
			float min = 0f, max = 1f;
			Vector2 value1 = new Vector2(min, max);

			// Inclusive
			for (float i = min; i < max; i += 0.1f)
			{
				Assert.True(value1.ContainsInclusive(i));
			}
			Assert.False(value1.ContainsInclusive(-0.01f));
			Assert.False(value1.ContainsInclusive(1.25f));

			// Exclusive
			Assert.False(value1.ContainsExclusive(max));
			Assert.False(value1.ContainsExclusive(min));

			// Average
			float average = min + max / 2f;
			Assert.AreEqual(average, value1.Average());

			// Trimming
			float x = 1f, y = 2f, z = 3f;
			Vector3 value2 = new Vector3(x, y, z);
			Assert.AreEqual(new Vector2(x, y), value2.XY());
			Assert.AreEqual(new Vector2(x, z), value2.XZ());
			Assert.AreEqual(new Vector2(y, z), value2.YZ());
		}

		[Test]
		public static void TestColorExtensions()
		{
			// To Alpha
			{
				Assert.AreEqual(new Color(1, 0, 0, 0.5f), Color.red.ScaleAlpha(0.5f));
			}

			// To Hex
			{
				Assert.AreEqual(ColorUtility.ToHtmlStringRGBA(Color.blue), Color.blue.ToHex());
			}

			// HSV
			{

			}
		}

		//------------------------------------------------------------------------/
		// Member Info
		//------------------------------------------------------------------------/
		[Test]
		public static void TestMemberInfoExtensions()
		{
			Type testType = typeof(TestDataObject);

			// Get Field Exhaustive 
			FieldInfo nameField = testType.GetFieldExhaustive(nameof(TestDataObject.name));
			FieldInfo valueField = testType.GetFieldExhaustive(nameof(TestDataObject.value));
			PropertyInfo inverseValueProperty = testType.GetProperty(nameof(TestDataObject.inverseValue));

			// Get Description
			{
				Assert.AreEqual(testType.GetDescription(), TestDataObject.classDescription);
				Assert.AreEqual(nameField.GetDescription(), TestDataObject.nameDescription);
				Assert.AreEqual(inverseValueProperty.GetDescription(), TestDataObject.inverseValueDescription);
			}

			// Get Value <T>			
			TestDataObject a = new TestDataObject("A", 7);
			Assert.AreEqual(7, valueField.GetValue<int>(a));

			// Attribute Has/Get
			{
				Assert.True(nameField.HasAttribute<MemberDescriptionAttribute>());
				Assert.True(nameField.HasAttribute(typeof(MemberDescriptionAttribute)));
				Assert.False(valueField.HasAttribute<MemberDescriptionAttribute>());
				Assert.NotNull(inverseValueProperty.GetAttribute<MemberDescriptionAttribute>());
			}

			// Map Attribute
			{
				Dictionary<Type, Attribute> map = valueField.MapAttributes();
				Assert.True(map.ContainsKey(typeof(HideInInspector)));
				Assert.True(map.ContainsKey(typeof(SerializeField)));
			}

			// Get Full Name
			{
				MethodBase boopMethod = testType.GetMethod(nameof(TestDataObject.Boop));
				Assert.AreEqual("Boop(int n, int b)", boopMethod.GetFullName());
				Assert.AreEqual("int n, int b", boopMethod.GetParameterNames());
			}
		}



	}
}
