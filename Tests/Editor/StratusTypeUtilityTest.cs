using NUnit.Framework;

using System;
using Stratus.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

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
		public void SubclassNames()
		{
			var actual = StratusTypeUtility.SubclassNames(typeof(MockA)).ToHashSet(); ;
			Assert.True(actual.Contains(nameof(MockB)));
		}

		[TestCase(typeof(MockA), typeof(MockB))]
		public void SubclassesOf(Type baseType, params Type[] expected)
		{
			var actual = StratusTypeUtility.SubclassesOf(baseType);
			AssertEquality(expected, actual);
		}

		private class MockObject
		{
		}

		private class MockObject<T> : MockObject
		{
		}

		private class IntMockObject : MockObject<int>
		{
		}

		private class StringMockObject : MockObject<string>
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
		public void FindsCollectionElementType(Type collectionType, Type expected)
		{
			var collection = (ICollection)StratusObjectUtility.Instantiate(collectionType);
			var actual = StratusTypeUtility.GetElementType(collection);
			Assert.AreEqual(expected, actual);
		}
	}
}