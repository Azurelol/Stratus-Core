using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public abstract class StratusCollectionScriptable<T> : StratusScriptable<List<T>>
	{
		public StratusSortedList<string, T> map
		{
			get
			{
				if (_map == null)
				{
					_map = new StratusSortedList<string, T>(GetKey);
					_map.AddRange(data);
				}
				return _map;
			}
		}

		private StratusSortedList<string, T> _map;
		protected abstract string GetKey(T element);
	}

	public abstract class StratusUnityObjectCollectionScriptable<T> 
		: StratusCollectionScriptable<T>
		where T : UnityEngine.Object
	{
		protected override string GetKey(T element)
		{
			return element.name;
		}
	}

}