using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Tests
{
	public class StratusEnumerableExtensionTests
	{
		[Test]
		public void TestEnumerableExtensions()
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

			// Names 
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

			// Duplicate Keys
			{
				int[] values = new int[] { 1, 2, 3, 3, 4 };
				Assert.True(values.HasDuplicateKeys());
				Assert.AreEqual(3, values.FindFirstDuplicate());
			}

			// Null
			{
				string a = "Hello", b = "Goodbye";
				string[] values = new string[] { a, null, b };
				Assert.AreEqual(new string[] { a, b }, values.TruncateNull());
			}

			// ForEach
			{
				int a = 1, b = 2, c = 3;
				int[] values = new int[] { a, b, c };
				List<int> values2 = new List<int>();
				values.ForEach((x) => values2.Add(x + 1));
				Assert.AreEqual(new int[] { a + 1, b + 1, c + 1 }, values2.ToArray());
			}

			// Find First
			{
				string a = "12", b = "34", c = "56";
				string[] values = new string[] { a, b, c };
				Assert.AreEqual(b, values.FindFirst(x => x.Contains("3")));
			}

			// Clone
			{
				int[] original = new int[] { 1, 2, 3, 4, 5 };
				int[] copy = (int[])original.Clone();
				Assert.AreNotSame(original, copy);
			}

			// Min/Max/Sum
			{
				{
					int[] values = new int[] { 1, 3, 5, 7, 9 };
					Assert.AreEqual(1, values.Min());
					Assert.AreEqual(9, values.Max());
					Assert.AreEqual(1 + 3 + 5 + 7 + 9, values.Sum());
				}

				{
					TestDataObject min = new TestDataObject("A", 1);
					TestDataObject max = new TestDataObject("D", 12);
					Func<TestDataObject, int> selector = (x) => x.value;

					TestDataObject[] values = new TestDataObject[]
					{
						new TestDataObject("B", 3),
						min,
						new TestDataObject("C", 6),
						max,
						new TestDataObject("E", 5),
					};

					Assert.AreEqual(min.value, values.Min(selector));
					Assert.AreEqual(max.value, values.Max(selector));
					Assert.AreEqual(min, values.SelectMin(selector));
					Assert.AreEqual(max, values.SelectMax(selector));
				}
			}

			// To Dictionary
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

			// Convert
			{
				int[] a = new int[] { 1, 2, 3, };
				string[] b = a.ToArray<int, string>((x) => x.ToString());
				Assert.AreEqual(new string[] { "1", "2", "3" }, b);
			}
		}
	}

}