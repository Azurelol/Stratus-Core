using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Stratus
{
	public abstract class StratusAssetCollectionScriptable<T> : StratusScriptable<List<T>>,
		IStratusAssetSource<T>
		where T : class
	{
		public StratusSortedList<string, T> assetsByName
		{
			get
			{
				if (_assetsByName == null)
				{
					_assetsByName = new StratusSortedList<string, T>(GetKey, data.Count, StringComparer.InvariantCultureIgnoreCase);
					_assetsByName.AddRange(data);
				}
				return _assetsByName;
			}
		}
		private StratusSortedList<string, T> _assetsByName;

		public StratusAssetToken<T> this[string key]
		{
			get => GetAsset(key);
		}

		public T[] assets => data.ToArray();
		public string[] assetNames => assetsByName.Keys.ToArray();

		public bool HasAsset(string name) => name.IsValid() && assetsByName.ContainsKey(name);
		protected virtual string GetKey(T element) => element.ToString();
		public StratusAssetToken<T> GetAsset(string name)
		{
			return new StratusAssetToken<T>(name, () => assetsByName.GetValueOrDefault(name));
		}
	}

	public abstract class StratusUnityAssetCollectionScriptable<T> :
		StratusAssetCollectionScriptable<T>
		where T: UnityEngine.Object
	{
		protected override string GetKey(T element) => element.name;
	}

}