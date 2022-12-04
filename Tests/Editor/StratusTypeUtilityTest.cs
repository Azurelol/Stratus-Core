﻿using NUnit.Framework;

using System;
using Stratus.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusTypeUtilityTest : StratusTest
	{
		private class MockA
		{
		}

		private class MockB : MockA
		{
		}

		[Test]
		public void GetsSubclassNameByTypeParameter()
		{
			var actual = StratusTypeUtility.SubclassNames<MockA>().ToHashSet();
			Assert.True(actual.Contains(nameof(MockB)));
		}

		[Test]
		public void GetsSubclassNameByType()
		{
			var actual = StratusTypeUtility.SubclassNames(typeof(MockA)).ToHashSet();
			Assert.True(actual.Contains(nameof(MockB)));
		}

		[TestCase(typeof(MockA), typeof(MockB))]
		public void SubclassesOf(Type baseType, params Type[] expected)
		{
			var actual = StratusTypeUtility.SubclassesOf(baseType);
			AssertEquality(expected, actual);
		}

		[AttributeUsage(AttributeTargets.Class)]
		private class MockAttribute : Attribute
		{
		}

		public interface MockInterface
		{
		}

		private class MockObject
		{
		}

		private class MockObject<T> : MockObject
		{
		}

		private class IntMockObject : MockObject<int>, MockInterface
		{
		}

		[Mock]
		private class StringMockObject : MockObject<string>, MockInterface
		{
		}

		[Test]
		public void FindsTypesDefinedFromGeneric()
		{
			Type baseType = typeof(MockObject<>);
			Type[] expected = new Type[]
			{
				typeof(IntMockObject),
				typeof(StringMockObject)
			};
			Type[] actual = StratusTypeUtility.TypesDefinedFromGeneric(baseType);
			Assert.AreEqual(expected.Length, actual.Length, "No clsoed generic types were found"); ;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetsTypeDefinitionParameterMap()
		{
			Type baseType = typeof(MockObject<>);
			Dictionary<Type, Type[]> map = StratusTypeUtility.TypeDefinitionParameterMap(baseType);
			Assert.True(map.ContainsKey(typeof(int)));
			Assert.True(map[typeof(int)].Length == 1 &&
				map[typeof(int)][0] == typeof(IntMockObject));
			Assert.True(map[typeof(string)].Length == 1 &&
				map[typeof(string)][0] == typeof(StringMockObject));
		}

		[TestCase(typeof(List<string>), typeof(string))]
		[TestCase(typeof(List<int>), typeof(int))]
		public void FindsCollectionElementType(Type collectionType, Type expected)
		{
			var collection = (ICollection)StratusObjectUtility.Instantiate(collectionType);
			var actual = StratusTypeUtility.GetElementType(collection);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetsTypesWithAttributes()
		{
			Type attrType = typeof(MockAttribute);
			var types = StratusTypeUtility.GetAllTypesWithAttribute(attrType).ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof(StringMockObject), types[0]);
		}

		[TestCase("System.Int32", typeof(int))]
		public void ResolvesTypeFromString(string typeName, Type expected)
		{
			Assert.AreEqual(expected, StratusTypeUtility.ResolveType(typeName));
		}

		[Test]
		public void InterfaceImplementations()
		{
			Type baseType = typeof(MockObject);
			Type interfaceType = typeof(MockInterface);
			var implementationTypes = StratusTypeUtility.InterfaceImplementations(baseType, interfaceType);
			var expected = new Type[] { typeof(IntMockObject), typeof(StringMockObject) };
			Assert.AreEqual(expected.Length, implementationTypes.Length);
			AssertEquality(expected, implementationTypes);
		}
	}
}