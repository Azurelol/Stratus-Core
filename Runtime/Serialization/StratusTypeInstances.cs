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
		[Serializable]
		public class Reference
		{
			public string typeName;
		}

		private Lazy<Type[]> _types;
		private Lazy<Dictionary<Type, T>> _instances;

		public Type baseType { get; private set; }
		public Type[] types => _types.Value;

		public StratusTypeInstancer()
		{
			baseType = typeof(T);
			_types = new Lazy<Type[]>(() => Utilities.StratusReflection.SubclassesOf<T>());
			_instances = new Lazy<Dictionary<Type, T>>(() => types.ToDictionaryFromKey((Type t) => (T)Activator.CreateInstance(t)));
		}

		public IEnumerable<T> GetAll() => _instances.Value.Values;

		public U Get<U>() where U : T
		{
			return (U)_instances.Value.GetValueOrDefault(typeof(U));
		}

		public T Get(Type type)
		{
			return _instances.Value.GetValueOrDefault(type);
		}
	}
}