using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		/// <summary>
		/// Shuffles the list using a randomized range based on its size.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">A reference to the list.</param>
		/// <remarks>Courtesy of Mike Desjardins #UnityTips</remarks>
		/// <returns>A new, shuffled list.</returns>
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				T index = list[i];
				int randomIndex = UnityEngine.Random.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = index;
			}
		}

		/// <summary>
		/// Swaps 2 elements in a list by index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="indexA"></param>
		/// <param name="indexB"></param>
		public static void SwapAtIndex<T>(this IList<T> list, int indexA, int indexB)
		{
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		/// <summary>
		/// Swaps 2 elements in a list by looking up the index of the values
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static void Swap<T>(this IList<T> list, T a, T b)
		{
			int indexA = list.IndexOf(a);
			int indexB = list.IndexOf(b);
			list.SwapAtIndex(indexA, indexB);
		}

		/// <summary>
		/// Returns a random element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Random<T>(this IList<T> list)
		{
			int randomSelection = UnityEngine.Random.Range(0, list.Count);
			return list[randomSelection];
		}

		/// <summary>
		/// Returns the last index of the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static int LastIndex<T>(this IList<T> list)
		{
			return list.Count - 1;
		}

		/// <summary>
		/// Returns the first element if the list if there's one, or null 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>(this IList<T> list)
		{
			return list.IsValid() ? list[0] : default;
		}

		/// <summary>
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveFirst<T>(this IList<T> list)
		{
			if (list.IsValid())
			{
				list.RemoveAt(0);
			}
		}

		/// <summary>
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveLast<T>(this IList<T> list)
		{
			if (list.IsValid())
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		/// <summary>
		/// Returns the element at the given index, or the default (null for class types)
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index)
		{
			return list.ContainsIndex(index) ? list[index] : default(T);
		}

		/// <summary>
		/// Returns the element at the given index, or the given default value
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index, T defaultValue)
		{
			return list.ContainsIndex(index) ? list[index] : defaultValue;
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool Contains<T>(this IList<T> source, T value)
		{
			return source.IsValid() && source.Contains(value);
		}

		/// <summary>
		/// Returns true if the <see cref="IList"/> contains the given element
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool ContainsIndex<T>(this IList<T> source, int index)
		{
			if (source == null || source.Count == 0 || index < 0)
			{
				return false;
			}

			return index <= source.Count - 1;
		}

		/// <summary>
		/// Checks if the list contains any of the given values
		/// </summary>
		public static bool ContainsAny<T>(this IList<T> list, IEnumerable<T> values)
		{
			return values.Any(x => list.Contains(x));
		}

		/// <summary>
		/// Checks if the list contains any of the given values
		/// </summary>
		public static bool ContainsAll<T>(this IList<T> list, IEnumerable<T> values)
		{
			return values.All(x => list.Contains(x));
		}

	}
}