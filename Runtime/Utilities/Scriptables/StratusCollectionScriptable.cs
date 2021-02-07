using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{

	public abstract class StratusAssetCollectionScriptable<T> : StratusScriptable<List<T>>,
		IStratusAssetSource<T>
		where T : class
	{
		public StratusSortedList<string, T> dataByName
		{
			get
			{
				if (_dataByName == null)
				{
					_dataByName = new StratusSortedList<string, T>(GetKey, data.Count, StringComparer.InvariantCultureIgnoreCase);
					_dataByName.AddRange(data);
				}
				return _dataByName;
			}
		}

		private StratusSortedList<string, T> _dataByName;
		protected virtual string GetKey(T element) => element.ToString();
		public bool HasAsset(string name) => dataByName.ContainsKey(name);
		public StratusAssetToken<T> GetAsset(string name)
		{
			return new StratusAssetToken<T>(name, () => dataByName.GetValueOrDefault(name));
		}
	}

	public abstract class StratusUnityAssetCollectionScriptable<T> :
		StratusAssetCollectionScriptable<T>
		where T: UnityEngine.Object
	{
		protected override string GetKey(T element) => element.name;
	}

}