using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;

using UnityEngine;

namespace Stratus.Tests
{
	public class StratusEnumerableExtensionTests
	{
		[Test]
		public void TestToString()
		{
			string a = "ABCD", b = "EDFG";
			TestDataObject[] values = new TestDataObject[]
			{
					new TestDataObject(a, 1),
					new TestDataObject(b, 2),
			};
			Assert.AreEqual(new string[] { a, b, }, values.ToStringArray());
			Assert.AreEqual(new string[] { a, b, }, values.ToStringArray(x => x.name));
		}

		[Test]
		public void TestJoin()
		{
			string a = "a", b = "b", c = "c", d = "d";
			string[] sequence = new string[] { a, b, c, d};
			string separator = ",";
			Assert.AreEqual($"{a}{separator}{b}{separator}{c}{separator}{d}", sequence.ToStringJoin());
		}

		[Test]
		public void TestTypeNames()
		{
			// Typenames
			{
				object[] values1 = new object[]
				{
					"Hello",
					1,
					2.5f
				};
				Type[] values1Types = new Type[]
				{
					typeof(string),
					typeof(int),
					typeof(float),
				};

				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1.TypeNames());

				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1Types.TypeNames());
			}
		}

		[Test]
		public void TestDuplicateKeys()
		{
			int[] input;
			Func<int, string> keyFunction = x => x.ToString();

			input = new int[] { 2, 3, 4, 2};
			Assert.True(input.HasDuplicateKeys(keyFunction));
			Assert.True(input.HasDuplicateKeys());
			input = new int[] { 2, 3, 4, 5, 6, 7 };
			Assert.False(input.HasDuplicateKeys(keyFunction));
			Assert.False(input.HasDuplicateKeys());
		}

		[Test]
		public void TestFindFirst()
		{
			{
				int[] values = new int[] { 1, 2, 3, 3, 4 };
				Assert.True(values.HasDuplicateKeys());
				Assert.AreEqual(3, values.FindFirstDuplicate());
			}
			{
				TestDataObject a = new TestDataObject("a", 3);
				TestDataObject b = new TestDataObject("b", 7);
				TestDataObject c = new TestDataObject("c", 5);
				var values = new TestDataObject[] { a, b, c, a, b };
				Assert.AreEqual(a, values.FindFirstDuplicate(x => x.name));
			}
			{
				string a = "12", b = "34", c = "56";
				string[] values = new string[] { a, b, c };
				Assert.AreEqual(b, values.FindFirst(x => x.Contains("3")));
			}
		}

		[Test]
		public void TestForEach()
		{
			int a = 1, b = 2, c = 3;
			int[] values = new int[] { a, b, c };
			List<int> values2 = new List<int>();
			values.ForEach((x) => values2.Add(x + 1));
			Assert.AreEqual(new int[] { a + 1, b + 1, c + 1 }, values2.ToArray());
		}

		[Test]
		public void TestDictionary()
		{
			string a = "A", b = "B", c = "C";
			TestDataObject[] values = new TestDataObject[]
			{
					new TestDataObject(a, 1),
					new TestDataObject(b, 2),
					new TestDataObject(c, 3),
			};
			Dictionary<string, TestDataObject> dict = values.ToDictionary<string, TestDataObject>((x) => x.name);
			Assert.AreEqual(3, dict.Count);
			Assert.AreEqual(1, dict[a].value);
			Assert.AreEqual(2, dict[b].value);
			Assert.AreEqual(3, dict[c].value);
		}

		[Test]
		public void TestToArray()
		{
			int[] a = new int[] { 1, 2, 3, };
			string[] b = a.ToArray<int, string>((x) => x.ToString());
			Assert.AreEqual(new string[] { "1", "2", "3" }, b);
		}

		[Test]
		public void TestTruncateNull()
		{
			string a = "Hello", b = "Goodbye";
			string[] values = new string[] { a, null, b };
			Assert.AreEqual(new string[] { a, b }, values.TruncateNull().ToArray());
		}
	}

}