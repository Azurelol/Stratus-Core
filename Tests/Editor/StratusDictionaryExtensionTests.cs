using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using Stratus.Editor.Tests;

namespace Stratus.Editor.Tests
{
	public partial class StratusDictionaryExtensionsTests
	{
		private static readonly TestDataObject a = new TestDataObject("A", 3);
		private static readonly TestDataObject b = new TestDataObject("B", 5);
		private static readonly TestDataObject c = new TestDataObject("C", 7);

		private static Func<TestDataObject, string> keyFunction = (x) => x.name;

		[Test]
		public void TestAddRange()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();

			// Add Range 
			{
				TestDataObject[] range = new TestDataObject[] { a, b, c };
				values.AddRange(keyFunction, range);
				Assert.AreEqual(3, values.Count);

				values.Clear();
				values.AddRange(keyFunction, a, b, c);
				Assert.AreEqual(3, values.Count);

				// Unique
				Assert.False(values.AddUnique(keyFunction(a), a));
				values.AddRangeUnique(keyFunction, range);
				Assert.AreEqual(3, values.Count);

				// Where
				values.Clear();
				values.AddRangeWhere(keyFunction, (x) => x.value < 6, range);
				Assert.AreEqual(2, values.Count);
				Assert.True(values.ContainsKey(a.name));
				Assert.True(values.ContainsKey(b.name));
			}
		}

		[Test]
		public void TestTryInvoke()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();
			values.AddRange(keyFunction, a, b, c);

			// Try Invoke
			{
				int result;

				result = values.TryInvoke(a.name, (x) => x.value);
				Assert.AreEqual(result, a.value);

				result = -1;
				values.TryInvoke("NULL", (x) => { result = x.value; });
				Assert.AreNotEqual(result, b.value);
			}
		}
	}
}