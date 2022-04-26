using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Stratus;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public void AssertSuccess(StratusOperationResult result)
		{
			Assert.True(result.valid, result.message);
		}

		public void AssertFailure(StratusOperationResult result)
		{
			Assert.False(result.valid, result.message);
		}

		public void AssertEquality<T>(ICollection<T> first, ICollection<T> second)
		{
			AssertSuccess(first.IsEqualInValues(second));
		}
	}
}