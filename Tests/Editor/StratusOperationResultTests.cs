﻿using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusOperationResultTests
	{
		[Test]
		public static void TestConstructors()
		{
			bool valid = true;
			string message = "Blah blah BLAH!";

			StratusOperationResult result = null;

			result = new StratusOperationResult(valid, message);
			Assert.AreEqual(result.valid, valid);
			Assert.AreEqual(result.message, message);

			result = new StratusOperationResult(valid);
			Assert.AreEqual(result.valid, valid);
			Assert.Null(result.message);

			result = new StratusOperationResult(new Exception(message));
			Assert.False(result.valid);
			Assert.False(result);
			Assert.AreEqual(result.message, message);

			int value = 7;
			var result2 = new StratusOperationResult<int>(valid, value, message);
			Assert.AreEqual(result2.valid, valid);
			Assert.AreEqual(result2.message, message);
			Assert.AreEqual(result2.result, value);

		}

		[Test]
		public static void TestImplicitOperators()
		{
			string msg = "Dominus";
			{
				StratusOperationResult result = new StratusOperationResult(true, msg);
				Assert.True(result);
				result = false;
				Assert.False(result);
				Assert.Null(result.message);
			}
			{
				int value = 42;
				StratusOperationResult<int> result = 42;
				Assert.True(result);
				Assert.True(result.valid);
				Assert.True(result);
				Assert.AreEqual(value, (int)result);
			}

		}
	}
}