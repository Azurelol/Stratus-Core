using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Stratus
{
	public static class StratusEnum
	{
		private static Dictionary<Type, string[]> enumDisplayNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Array> enumValues { get; set; } = new Dictionary<Type, Array>();

		public static T[] Values<T>() where T: Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
		}

		public static Array Values(Type enumType)
		{
			return enumValues.GetValueOrAdd(enumType, Enum.GetValues);
		}

		public static T Value<T>(int index) where T : Enum
		{
			return (T)Value(typeof(T), index);
		}

		public static Enum Value(Type enumType, int index)
		{
			return (Enum)Values(enumType).GetValue(index);
		}

		public static string[] Names<T>() where T : Enum
		{
			return Names(typeof(T));
		}

		public static string[] Names(Type enumType)
		{
			return enumDisplayNames.GetValueOrAdd(enumType, Enum.GetNames);
		}

	}
}