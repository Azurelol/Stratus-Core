using Stratus.Collections;
using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public abstract class StratusAssetCollectionScriptable<T> : StratusScriptable<List<T>>,
		IStratusAssetSource<T>, IStratusAssetResolver<T>
		where T : class
	{
		public AutoSortedList<string, T> assetsByName
		{
			get
			{
				if (_assetsByName == null)
				{
					_assetsByName = new AutoSortedList<string, T>(GetKey, data.Count, StringComparer.InvariantCultureIgnoreCase);
					_assetsByName.AddRange(data);
				}
				return _assetsByName;
			}
		}
		private AutoSortedList<string, T> _assetsByName;

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

		public string[] GetAssetNames() => assetNames;

		public IEnumerable<StratusAssetToken<T>> Fetch()
		{
			return assets.Select(a => new StratusAssetToken<T>(a));
		}
	}

	public abstract class StratusUnityAssetCollectionScriptable<T> :
		StratusAssetCollectionScriptable<T>
		where T: UnityEngine.Object
	{
		protected override string GetKey(T element) => element.name;
	}

}