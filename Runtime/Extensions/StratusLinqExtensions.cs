using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
#if !UNITY_2021_1_OR_NEWER
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		} 
#endif
	}
}