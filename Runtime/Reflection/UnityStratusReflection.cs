using System;
using System.Linq;
using System.Reflection;
//using UnityEngine;
using Stratus.Extensions;
using Stratus.OdinSerializer;

using UnityEngine;

namespace Stratus.Utilities
{
	public static class UnityStratusReflection
	{
		public static FieldInfo[] GetFieldsWithNullReferences<T>(T behaviour) where T : Behaviour
		{
			Type type = behaviour.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			return (from f
				   in fields
					where (f.FieldType.IsSubclassOf(typeof(UnityEngine.Object))
					&& f.GetValue(behaviour).Equals(null))
					select f).ToArray();
		}

		/// <summary>
		/// Returns all fields that are being serialized by Unity
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, bool unitySerialized = true)
		{
			ISerializationPolicy policy = unitySerialized ? SerializationPolicies.Unity : SerializationPolicies.Strict;
			return type.GetSerializedFields(policy);
		}

		/// <summary>
		/// Returns all fields that are being serialized
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, ISerializationPolicy policy)
		{
			MemberInfo[] members = FormatterUtilities.GetSerializableMembers(type, policy);
			return members.OfType<FieldInfo>().ToArray();
		}
	}
}

