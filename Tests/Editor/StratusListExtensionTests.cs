using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace Stratus.Tests
{
	public partial class StratusListExtensionsTests
	{
		[Test]
		public void TestRemoveNull()
		{
			List<string> values = new List<string>
				{
					null
				};
			Assert.AreEqual(1, values.RemoveNull());
		}

		[Test]
		public void TestAddRange()
		{
			string a = "12", b = "34", c = "56";
			List<string> values = new List<string>();
			values.AddRange(a, b, c);
			Assert.AreEqual(new string[] { a, b, c }, values.ToArray());
		}

		[Test]
		public void TestForEachRemoveInvalid()
		{
			List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
			values.ForEachRemoveInvalid(
				(tdo) => tdo.value += 1,
				(tdo) => tdo.value < 5);
			Assert.True(values.Count == 1);
			Assert.True(values.First().name == "A" && values.First().value == 4);
		}

		[Test]
		public void TestRemoveInvalid()
		{
			List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
			values.RemoveInvalid((tdo) => tdo.value < 5);
			Assert.True(values.Count == 1);
			Assert.True(values.First().name == "A");
		}

		[Test]
		public void TestClone()
		{
			List<int> original = new List<int>() { 1, 2, 3, 4, 5 };
			List<int> copy = original.Clone();
			Assert.AreNotSame(original, copy);
		}

		[Test]
		public void TestAddRangeWhere()
		{
			TestDataObject a = new TestDataObject("A", 1);
			TestDataObject b = new TestDataObject("B", 2);

			List<TestDataObject> values = new List<TestDataObject>();
			values.AddRangeWhere((x) => x.value > 1, a, b);
			Assert.AreEqual(1, values.Count);
			Assert.AreEqual(b, values.First());
		}

		[Test]
		public void TestAddRangeUnique()
		{
			TestDataObject a = new TestDataObject("A", 1);

			List<TestDataObject> values = new List<TestDataObject>
				{
					a
				};
			values.AddRangeUnique(a, a);
			Assert.AreEqual(a, values.First());
			Assert.AreEqual(1, values.Count);
		}
	}
}