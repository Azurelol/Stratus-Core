#define STRATUS_IMPORT_LINQ_EXTENSIONS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		#region STRATUS_IMPORT_LINQ_EXTENSIONS
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}
		#endregion
	}

}