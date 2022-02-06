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
			return enumValues.GetValueOrGenerate(enumType, Enum.GetValues);
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
			return enumDisplayNames.GetValueOrGenerate(enumType, Enum.GetNames);
		}

		public static IEnumerable<TEnum> Flags<TEnum>(TEnum _value) where TEnum : Enum
		{
			ulong flag = 1;
			foreach (var value in Enum.GetValues(_value.GetType()).Cast<TEnum>())
			{
				ulong bits = Convert.ToUInt64(value);
				while (flag < bits)
				{
					flag <<= 1;
				}

				if (flag == bits && _value.HasFlag(value))
				{
					yield return value;
				}
			}
		}

		public static Dictionary<TEnum, TValue> Dictionary<TEnum, TValue>(TValue defaultValue = default)
			where TEnum : Enum
		{
			Dictionary<TEnum, TValue> result = new Dictionary<TEnum, TValue>();
			Values<TEnum>().ForEach(e => result.Add(e, defaultValue));
			return result;
		}
	}
}