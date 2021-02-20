using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public class StratusDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>
	{
		private Func<ValueType, KeyType> keyFunction;

		public StratusDictionary(Func<ValueType, KeyType> keyFunction,
								 int capacity = 0,
								 IEqualityComparer<KeyType> comparer = null)
								 : base(capacity, comparer)
		{
			this.keyFunction = keyFunction;
		}

		public StratusDictionary(Func<ValueType, KeyType> keyFunction, 
								IEnumerable<ValueType> values,
								 int capacity = 0,
								 IEqualityComparer<KeyType> comparer = null)
								 : this(keyFunction, capacity, comparer)
		{
			AddRange(values);
		}


		public bool Add(ValueType value)
		{
			KeyType key = keyFunction(value);
			if (ContainsKey(key))
			{
				StratusDebug.LogError($"Value with key '{key}' already exists in this collection!");
				return false;
			}
			Add(key, value);
			return true;
		}

		public int AddRange(IEnumerable<ValueType> values)
		{
			if (values == null)
			{
				return 0;
			}

			int failCount = 0;
			foreach (ValueType value in values)
			{
				if (!Add(value))
				{
					failCount++;
				}
			}
			return failCount;
		}

		public bool Remove(ValueType value)
		{
			KeyType key = keyFunction(value);
			if (!ContainsKey(key))
			{
				return false;
			}
			Remove(key);
			return true;
		}

		public ValueType GetValueOrDefault(KeyType key)
		{
			return ContainsKey(key) ? this[key] : default;
		}
	}
}