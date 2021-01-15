using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Tests
{
	public class StratusArrayExtensionTests
	{
		[Test]
		public void TestFind()
		{
			// FindIndex
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				Assert.AreEqual(4, values.FindIndex(5));
			}

			// FindIndex, Find, Exists
			{
				string[] values = new string[] { "Hello", "There", "Brown", "Cow" };
				Assert.AreEqual(2, values.FindIndex("Brown"));
				Assert.AreEqual("Hello", values.Find((x) => x.Contains("llo")));
				Assert.IsTrue(values.Contains((x) => x.Equals("There")));
			}
		}

		[Test]
		public void TestSort()
		{
			float a = 1.25f, b = 2.5f, c = 3.75f, d = 5.0f;
			// Custom comparer
			{
				float[] values = new float[] { d, b, a, c };
				values.Sort((x, y) => x > y ? 1 : x < y ? -1 : 0);
				Assert.AreEqual(new float[] { a, b, c, d }, values);
			}
			// Default (from interface)
			{
				float[] values = new float[] { d, b, a, c };
				values.Sort();
				Assert.AreEqual(new float[] { a, b, c, d }, values);
			}
		}

		[Test]
		public void TestTruncate()
		{
			int[] values = new int[] { 1, 2, 3 };
			Assert.AreEqual(new int[] { 2, 3 }, values.TruncateFront());
			Assert.AreEqual(new int[] { 1, 2 }, values.TruncateBack());
			Assert.AreEqual(new int[] { 1, 3 }, values.Truncate(2));
			Assert.AreEqual(new int[] { 1, 2 }, values.Truncate(3));
			Assert.AreEqual(new int[] { 2, 3 }, values.Truncate(1));
		}

		[Test]
		public void TestAppend()
		{
			int a = 1, b = 2, c = 3, d = 4;
			int[] first = new int[] { a, b };
			int[] second = new int[] { c, d };

			int[] third = first.Append(second);
			Assert.AreEqual(new int[] { a, b, c, d }, third);
			int[] fourth = first.Prepend(second);
			Assert.AreEqual(new int[] { c, d, a, b }, fourth);

			int[] fifth = first.AppendWhere((x) => x < 4, second);
			Assert.AreEqual(new int[] { a, b, c }, fifth);

			int[] sixth = first.PrependWhere((x) => x > 3, second);
			Assert.AreEqual(new int[] { d, a, b }, sixth);
		}

		[Test]
		public void TestConcat()
		{
			int[] a = new int[] { 1, 3, 5 }, b = new int[] { 2, 4, 6 };
			Assert.AreEqual(new int[] { 1, 3, 5, 2, 4, 6 }, a.Concat(b));
		}

		[Test]
		public void TestLenghOrZero()
		{

		}
	}

}