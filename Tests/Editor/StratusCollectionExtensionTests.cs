using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Stratus.Tests
{
	public partial class StratusExtensionsTests
	{	
		[Test]
		public void TestCollectionsExtensions()
		{
			{
				// Empty
				{
					List<string> values = new List<string>();
					Assert.True(values.Empty());
					Assert.False(values.NotEmpty());

					values.Add("Boo");
					Assert.True(values.NotEmpty());
					Assert.False(values.Empty());

					values.Clear();
					Assert.False(values.NotNullOrEmpty());
				}

				// Stack: Push Range
				{
					Stack<int> values = new Stack<int>();
					values.PushRange(1, 2, 3);
					Assert.AreEqual(new int[] { 3, 2, 1 }, values.ToArray());
				}

				// Queue : Enqueue Range
				{
					Queue<int> values = new Queue<int>();
					values.EnqueueRange(1, 2, 3);
					Assert.AreEqual(new int[] { 1, 2, 3 }, values.ToArray());
				}
			}
		}

		[Test]
		public void TestListExtensions()
		{
			// Remove Null
			{
				List<string> values = new List<string>
				{
					null
				};
				Assert.AreEqual(1, values.RemoveNull());
			}

			// Add Range
			{
				string a = "12", b = "34", c = "56";
				List<string> values = new List<string>();
				values.AddRange(a, b, c);
				Assert.AreEqual(new string[] { a, b, c }, values.ToArray());
			}

			// ForEach RemoveInvalid
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

			// First/Last 
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

				Assert.AreEqual(1, values.First());
				Assert.AreEqual(9, values.Last());

				values = values.TruncateFront();
				Assert.AreEqual(2, values.First());

				values = values.TruncateBack();
				Assert.AreEqual(8, values.Last());
			}

			// Remove Invalid
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

			// Clone
			{
				List<int> original = new List<int>() { 1, 2, 3, 4, 5 };
				List<int> copy = original.Clone();
				Assert.AreNotSame(original, copy);
			}

			// Add Range Where
			{
				TestDataObject a = new TestDataObject("A", 1);
				TestDataObject b = new TestDataObject("B", 2);

				List<TestDataObject> values = new List<TestDataObject>();
				values.AddRangeWhere((x) => x.value > 1, a, b);
				Assert.AreEqual(1, values.Count);
				Assert.AreEqual(b, values.First());
			}

			// Add Range Unique
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

		[Test]
		public void TestIListExtensions()
		{
			// Shuffle, Last Index
			{
				int[] values = new int[] { 1, 2, 3, 4, 5 };
				Assert.AreEqual(4, values.LastIndex());
				int[] shuffled = (int[])values.Clone();
				shuffled.Shuffle();
				Assert.AreNotEqual(values, shuffled);
			}

			// Swap
			{
				int[] values = new int[] { 1, 2, 3, 4, 5 };
				values.SwapAtIndex(0, 4);
				Assert.AreEqual(values, new int[] { 5, 2, 3, 4, 1 });
				values.Swap(1, 5);
				Assert.AreEqual(values, new int[] { 1, 2, 3, 4, 5 });
			}

			// First, Last, Random
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				Assert.AreEqual(1, values.First());
				Assert.AreEqual(9, values.Last());

				// You see, random can end up hitting the same index... since it's random
				int a = values.Random();
				int b = -1;
				for (int i = 0; i < values.Length; ++i)
				{
					b = values.Random();
					if (a != b)
					{
						break;
					}
				}
				Assert.AreNotEqual(a, b);
			}

			// Remove First/Last
			{
				List<int> values = new List<int>() { 1, 2, 3, 4 };
				values.RemoveFirst();
				values.RemoveLast();
				Assert.AreEqual(2, values.First());
				Assert.AreEqual(3, values.Last());
			}

			// Has Index, AtIndexOrDefault
			{
				TestDataObject a = new TestDataObject("A", 1);
				TestDataObject[] values = new TestDataObject[]
				{
					a
				};

				Assert.False(values.HasIndex(-1));
				Assert.True(values.HasIndex(0));
				Assert.False(values.HasIndex(1));

				Assert.AreEqual(a, values.AtIndexOrDefault(0));
				Assert.AreEqual(a, values.AtIndexOrDefault(5, a));

			}
		}

		[Test]
		public void TestDictionaryExtensions()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();

			TestDataObject a = new TestDataObject("A", 3);
			TestDataObject b = new TestDataObject("B", 5);
			TestDataObject c = new TestDataObject("C", 7);

			Func<TestDataObject, string> keyFunction = (x) => x.name;

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

			// Try Invoke
			{
				int result = 0;
				result = values.TryInvoke(a.name, (tdo) => tdo.value);
				Assert.AreEqual(result, a.value);

				result = -1;
				values.TryInvoke("NULL", (tdo) => { result = tdo.value; });
				Assert.AreNotEqual(result, b.value);
			}
		}
	}
}