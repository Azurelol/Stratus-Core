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