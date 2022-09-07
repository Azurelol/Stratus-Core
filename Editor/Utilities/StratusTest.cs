using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Stratus;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public static void AssertContains<T, V>(T key, IReadOnlyDictionary<T, V> dictionary)
		{
			Assert.True(dictionary.ContainsKey(key), $"Dictionary contains {dictionary.ToStringJoin()}");
		}

		public static void AssertContains<T>(HashSet<T> set, T key)
		{
			Assert.True(set.Contains(key), $"Hashset contains {set.ToStringJoin()}");
		}

		public static void AssertContainsExactly<T, V>(IReadOnlyDictionary<T, V> dictionary, params T[] keys)
		{
			Assert.AreEqual(keys.Length, dictionary.Count, $"Dictionary contains {dictionary.ToStringJoin()}");
			foreach(var key in keys)
			{
				AssertContains(key, dictionary);
			}
		}

		public static void AssertSuccess(StratusOperationResult result)
		{
			Assert.True(result.valid, result.message);
		}

		public static void AssertFailure(StratusOperationResult result)
		{
			Assert.False(result.valid, result.message);
		}

		public static void AssertEquality<T>(ICollection<T> first, ICollection<T> second)
		{
			AssertSuccess(first.IsEqualInValues(second));
		}

		public static void AssertEquality<T>(T[] first, T[] second)
		{
			AssertSuccess(first.IsEqualInValues(second));
		}

		public static void AssertLength<T>(int expected, IList<T> list)
		{
			Assert.AreEqual(expected, list.Count, $"List contained {list.ToStringJoin().Enclose(StratusStringEnclosure.SquareBracket)}");
		}
	}
}