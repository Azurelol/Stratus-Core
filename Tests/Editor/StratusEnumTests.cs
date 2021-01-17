using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusEnumTests
	{
		private enum MockEnum
		{
			A,
			B,
			C
		}

		[Test]
		public void TestValues()
		{
			MockEnum[] values = StratusEnum.Values<MockEnum>();
			Assert.True(values.Length == 3);
			Assert.AreEqual(values[0], MockEnum.A);
			Assert.AreEqual(values[1], MockEnum.B);
			Assert.AreEqual(values[2], MockEnum.C);
		}

		[Test]
		public void TestValue()
		{
			MockEnum[] values = StratusEnum.Values<MockEnum>();
			for (int i = 0; i < values.Length; ++i)
			{
				Assert.AreEqual(StratusEnum.Value<MockEnum>(i), values[i]);
			}
		}

		[Test]
		public void TestNames()
		{
			MockEnum[] values = StratusEnum.Values<MockEnum>();
			string[] names = StratusEnum.Names<MockEnum>();
			for (int i = 0; i < values.Length; ++i)
			{
				Assert.AreEqual(names[i], values[i].ToString());
			}
		}
	}

}