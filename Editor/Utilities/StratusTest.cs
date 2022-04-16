using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public void AssertResult(StratusOperationResult result)
		{
			Assert.True(result.valid, result.message);
		}
	}
}