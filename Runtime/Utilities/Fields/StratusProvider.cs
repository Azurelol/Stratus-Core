using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public enum StratusProviderSource
	{
		Invalid,
		Reference,
		Value
	}

	public class StratusProvider<T>
	{
		public StratusProviderSource source { get; private set; }
		public T value
		{
			get
			{
				switch (source)
				{
					case StratusProviderSource.Reference:
						return _getter();
					case StratusProviderSource.Value:
						return _value;
				}
				throw new Exception("No value source was set");
			}
		}

		private T _value;
		private Func<T> _getter;

		public bool valid => source != StratusProviderSource.Invalid;

		public StratusProvider(Func<T> getter)
		{
			if (getter == null)
			{
				this.source = StratusProviderSource.Invalid;
				return;
			}
			this._getter = getter;
			this.source = StratusProviderSource.Reference;
		}

		public StratusProvider(T value)
		{
			if (value == null)
			{
				this.source = StratusProviderSource.Invalid;
				return;
			}
			this._value = value;
			this.source = StratusProviderSource.Value;
		}

		public static implicit operator StratusProvider<T>(T value) => new StratusProvider<T>(value);
	}

}