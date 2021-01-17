using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusVectorTests
	{
		private static readonly float min = 0f, max = 1f;
		private static readonly Vector2 value_0_1 = new Vector2(min, max);

		[Test]
		public void TestInclusive()
		{
			for (float i = min; i < max; i += 0.1f)
			{
				Assert.True(value_0_1.ContainsInclusive(i));
			}
			Assert.False(value_0_1.ContainsInclusive(-0.01f));
			Assert.False(value_0_1.ContainsInclusive(1.25f));
		}

		[Test]
		public void TestExclusive()
		{
			Assert.False(value_0_1.ContainsExclusive(max));
			Assert.False(value_0_1.ContainsExclusive(min));
		}

		[Test]
		public void TestAverage()
		{
			float average = min + max / 2f;
			Assert.AreEqual(average, value_0_1.Average());
		}

		[Test]
		public void TestXYZ()
		{
			float x = 1f, y = 2f, z = 3f;
			Vector3 value2 = new Vector3(x, y, z);
			Assert.AreEqual(new Vector2(x, y), value2.XY());
			Assert.AreEqual(new Vector2(x, z), value2.XZ());
			Assert.AreEqual(new Vector2(y, z), value2.YZ());
		}
	}
}
