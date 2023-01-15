using System.Collections.Generic;
using System.Linq;

namespace Stratus.Extensions
{
	public static class StratusListExtensions
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
	}
}