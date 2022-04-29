using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Stratus;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public void AssertContains<T, V>(T key, IReadOnlyDictionary<T, V> dictionary)
		{
			Assert.True(dictionary.ContainsKey(key), $"Dictionary contains {dictionary.ToStringJoin()}");
		}

		public void AssertContainsExactly<T, V>(IReadOnlyDictionary<T, V> dictionary, params T[] keys)
		{
			Assert.AreEqual(keys.Length, dictionary.Count, $"Dictionary contains {dictionary.ToStringJoin()}");
			foreach(var key in keys)
			{
				AssertContains(key, dictionary);
			}
		}

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