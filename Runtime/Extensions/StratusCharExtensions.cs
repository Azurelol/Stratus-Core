using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	public static partial class Extensions
	{
		public static bool IsUpper(this char c)
		{
			return char.IsUpper(c);
		}

		public static bool IsLower(this char c)
		{
			return char.IsLower(c);
		}

		public static char ToUpper(this char c)
		{
			return char.ToUpper(c);
		}

		public static char ToLower(this char c)
		{
			return char.ToLower(c);
		}
	}

}