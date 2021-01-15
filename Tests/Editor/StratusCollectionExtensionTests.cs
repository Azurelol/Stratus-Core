using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace Stratus.Tests
{
	public class StratusCollectionExtensionsTests
	{
		[Test]
		public void TestEmpty()
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

		[Test]
		public void TestPushRange()
		{
			Stack<int> values = new Stack<int>();
			values.PushRange(1, 2, 3);
			Assert.AreEqual(new int[] { 3, 2, 1 }, values.ToArray());
		}

		[Test]
		public void TestEnqueueRange()
		{
			Queue<int> values = new Queue<int>();
			values.EnqueueRange(1, 2, 3);
			Assert.AreEqual(new int[] { 1, 2, 3 }, values.ToArray());
		}

		[Test]
		public void TestContainsIndex()
		{
			TestDataObject a = new TestDataObject("A", 1);
			TestDataObject[] values = new TestDataObject[]
			{
				a
			};

			Assert.False(values.ContainsIndex(-1));
			Assert.True(values.ContainsIndex(0));
			Assert.False(values.ContainsIndex(1));

			Assert.AreEqual(a, values.AtIndexOrDefault(0));
			Assert.AreEqual(a, values.AtIndexOrDefault(5, a));
		}

		[Test]
		public void TestTryContains()
		{
			IList<string> values;
			string value = "a";
			values = new string[] { value, "b", "c" };
			Assert.True(values.TryContains(value));
			values = new string[] { "b", "c" };
			Assert.False(values.TryContains(value));
			values = null;
			Assert.False(values.TryContains(value));
		}
	}
}