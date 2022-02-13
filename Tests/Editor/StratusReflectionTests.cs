using NUnit.Framework;

using Stratus.Utilities;

using System;
using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusReflectionTests
	{
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
			Type[] actual = StratusReflection.TypesDefinedFromGeneric(baseType);
			Assert.AreEqual(expected.Length, actual.Length, "No clsoed generic types were found"); ;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetsTypeDefinitionParameterMap()
		{
			Type baseType = typeof(MockObject<>);
			Dictionary<Type, Type[]> map = StratusReflection.TypeDefinitionParameterMap(baseType);
			Assert.True(map.ContainsKey(typeof(int)));
			Assert.True(map[typeof(int)].Length == 1 &&
				map[typeof(int)][0] == typeof(IntMockObject));
			Assert.True(map[typeof(string)].Length == 1 &&
				map[typeof(string)][0] == typeof(StringMockObject));
		}
	}
}