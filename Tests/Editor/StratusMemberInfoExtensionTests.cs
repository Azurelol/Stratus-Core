using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusMemberInfoExtensionTests
	{
		[ClassDescription(classDescription)]
		internal class MockDataObject
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

			public MockDataObject(string name, int value)
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

			public int Boop1(int c, int d)
			{
				return c + d;
			}
		}

		private static Type testType = typeof(MockDataObject);

		[Test]
		public void GetDescription()
		{
			PropertyInfo inverseValueProperty = testType.GetProperty(nameof(MockDataObject.inverseValue));
			Assert.AreEqual(inverseValueProperty.GetDescription(), MockDataObject.inverseValueDescription);
			Assert.NotNull(inverseValueProperty.GetAttribute<MemberDescriptionAttribute>());
			Assert.AreEqual(testType.GetDescription(), MockDataObject.classDescription);

			FieldInfo nameField = testType.GetFieldIncludePrivate(nameof(MockDataObject.name));
			Assert.AreEqual(nameField.GetDescription(), MockDataObject.nameDescription);
		}

		[Test]
		public void GetValue()
		{
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			const int value = 7;
			MockDataObject a = new MockDataObject("A", value);
			Assert.AreEqual(value, valueField.GetValue<int>(a));
		}

		[Test]
		public void HasAttribute()
		{
			FieldInfo nameField = testType.GetFieldIncludePrivate(nameof(MockDataObject.name));
			Assert.True(nameField.HasAttribute<MemberDescriptionAttribute>());
			Assert.True(nameField.HasAttribute(typeof(MemberDescriptionAttribute)));
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			Assert.False(valueField.HasAttribute<MemberDescriptionAttribute>());
		}

		[Test]
		public void MapAttributes()
		{
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			Dictionary<Type, Attribute> map = valueField.MapAttributes();
			Assert.True(map.ContainsKey(typeof(HideInInspector)));
			Assert.True(map.ContainsKey(typeof(SerializeField)));
		}

		[Test]
		public void GetFullName()
		{
			Assert.AreEqual("Boop(int n, int b)", testType.GetMethod(nameof(MockDataObject.Boop)).GetFullName());
			Assert.AreEqual("Boop1(int c, int d)", testType.GetMethod(nameof(MockDataObject.Boop1)).GetFullName());
		}

		[Test]
		public void GetParameterNames()
		{
			Assert.AreEqual("int n, int b", testType.GetMethod(nameof(MockDataObject.Boop)).GetParameterNames());
			Assert.AreEqual("int c, int d", testType.GetMethod(nameof(MockDataObject.Boop1)).GetParameterNames());
		}
	}

}