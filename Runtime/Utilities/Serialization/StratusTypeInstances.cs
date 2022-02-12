using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// Used for managing default instances of the subclasses of a given class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusSubclassInstancer<T> where T : class
	{
		public Type baseType { get; private set; }
		private Type[] types;
		private Dictionary<Type, T> instances;

		public StratusSubclassInstancer()
		{
			baseType = typeof(T);
			types = Utilities.StratusReflection.SubclassesOf<T>();
			instances = types.ToDictionaryFromKey((Type t) => (T)Activator.CreateInstance(t));
		}

		public U Get<U>() where U : T
		{
			return (U)instances.GetValueOrDefault(typeof(U));
		}

		public T Get(Type type)
		{
			return instances.GetValueOrDefault(type);
		}

	}

}