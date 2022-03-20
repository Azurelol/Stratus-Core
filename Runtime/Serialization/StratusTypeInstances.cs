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
	public class StratusTypeInstancer<T> where T : class
	{
		private Type[] types;
		private Dictionary<Type, T> instances;
		public Type baseType { get; private set; }

		public StratusTypeInstancer()
		{
			baseType = typeof(T);
			types = Utilities.StratusReflection.SubclassesOf<T>();
			instances = types.ToDictionaryFromKey((Type t) => (T)Activator.CreateInstance(t));
		}

		public IEnumerable<T> GetAll() => instances.Values;

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