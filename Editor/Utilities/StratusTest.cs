using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Stratus;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public void AssertResult(StratusOperationResult result)
		{
			Assert.True(result.valid, result.message);
		}

		public void AssertCollections<T>(ICollection<T> first, ICollection<T> second)
		{
			AssertResult(first.IsEqualInValues(second));
		}
	}
}