using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
	public interface IStratusAssetResolver<TAsset>
		where TAsset : class
	{
		bool HasAsset(string name);
		StratusAssetToken<TAsset> GetAsset(string name);
		string[] GetAssetNames();
	}

	public interface IStratusAssetSource<TAsset>
		where TAsset : class
	{
		IEnumerable<StratusAssetToken<TAsset>> Fetch();
	}

	public abstract class StratusAssetSource<TAsset>
		: IStratusAssetSource<TAsset>
		where TAsset : class
	{
		public abstract IEnumerable<StratusAssetToken<TAsset>> Fetch();
	}

	public abstract class StratusAssetResolver<TAsset> : IStratusAssetResolver<TAsset>
		where TAsset : class
	{
		public StratusSortedList<string, StratusAssetToken<TAsset>> assetsByName
		{
			get
			{
				if (_assetsByName == null)
				{
					_assetsByName = new StratusSortedList<string, StratusAssetToken<TAsset>>(GetKey,
						_assets.Count,
						StringComparer.InvariantCultureIgnoreCase);

					foreach (var source in sources)
					{

					}
					_assetsByName.AddRange(_assets);
				}
				return _assetsByName;
			}
		}
		private StratusSortedList<string, StratusAssetToken<TAsset>> _assetsByName;
		private List<StratusAssetToken<TAsset>> _assets;

		public abstract IEnumerable<StratusAssetSource<TAsset>> sources { get; }
		protected virtual string GetKey(StratusAssetToken<TAsset> element) => element.ToString();

		public void Resolve(bool force = false)
		{
			if (_assets == null || force)
			{
				_assets = new List<StratusAssetToken<TAsset>>();
				foreach (var source in sources)
				{
					_assets.AddRange(source.Fetch());
				}
			}
		}

		public bool HasAsset(string name)
		{
			return assetsByName.ContainsKey(name);
		}

		public StratusAssetToken<TAsset> GetAsset(string name)
		{
			return HasAsset(name) ? assetsByName[name] : null;
		}

		public string[] GetAssetNames()
		{
			return _assetsByName.Keys.ToArray();
		}
	}

	public class DefaultStratusAssetResolver<TAsset> : StratusAssetResolver<TAsset>
		where TAsset : class
	{
		private static readonly Lazy<Type[]> types = new Lazy<Type[]>(() =>
			StratusTypeUtility.TypesDefinedFromGeneric(typeof(StratusAssetSource<>)));

		public override IEnumerable<StratusAssetSource<TAsset>> sources { get; }
	}

	public abstract class CustomStratusAssetSource<TAsset> : StratusAssetSource<TAsset>
		where TAsset : class
	{
		public IEnumerable<TAsset> assets
		{
			get
			{
				if (_assets == null)
				{
					_assets = Generate();
				}
				return _assets;
			}
		}

		private IEnumerable<TAsset> _assets;
		protected abstract IEnumerable<TAsset> Generate();
		public override IEnumerable<StratusAssetToken<TAsset>> Fetch()
		{
			return assets.Select(a => new StratusAssetToken<TAsset>(a, () => a));
		}
	}

	public abstract class ResourcesStratusAssetSource<TAsset>
	{
	}

	public static class StratusAssetDatabase
	{
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class StratusAssetSourceAttribute : Attribute
	{
		public Type sourceTypes { get; set; }
	}

}