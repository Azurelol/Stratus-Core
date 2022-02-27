using System.Collections.Generic;
using System;

namespace Stratus.Collections
{
	public abstract class StratusMap<TKey, TValue> : List<TValue>
	{
		private Dictionary<TKey, TValue> lookup
		{
			get
			{
				if (_lookup == null)
				{
					GenerateLookup();
				}
				return _lookup;
			}
		}

		[NonSerialized]
		private Dictionary<TKey, TValue> _lookup;

		public new void Add(TValue item)
		{
			base.Add(item);
		}
		public new void AddRange(IEnumerable<TValue> collection)
		{
			base.AddRange(collection);
		}

		public bool Contains(TKey key)
		{
			return _lookup.ContainsKey(key);
		}

		public TValue Get(TKey key)
		{
			return _lookup.GetValueOrDefault(key);
		}

		private void GenerateLookup()
		{
			_lookup = new Dictionary<TKey, TValue>();
		}

		protected abstract TKey GetKey(TValue value);
	}

	/// <summary>
	/// Non-serialized version
	/// </summary>
	public class StratusDefaultMap<TKey, TValue> : StratusMap<TKey, TValue>
	{
		private Func<TValue, TKey> keyFunction;

		public StratusDefaultMap(Func<TValue, TKey> keyFunction)
		{
			this.keyFunction = keyFunction;
		}

		protected override TKey GetKey(TValue value) => keyFunction(value);
	}

}