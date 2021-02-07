using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;

namespace Stratus
{
	public enum StratusAssetSourceType
	{
		Invalid,
		Reference,
		Alias
	}

	public interface IStratusAsset
	{
		/// <summary>
		/// The name of the asset
		/// </summary>
		string name { get; }
	}

	public interface IStratusAsset<T> : IStratusAsset
	{
		T asset { get; }
	}

	public abstract class StratusAsset
	{
		[SerializeField]
		private string _name;
		public string name => _name;

		public StratusAsset()
		{
		}

		public StratusAsset(string name)
		{
			this._name = name;
		}

		public override string ToString()
		{
			return _name;
		}
	}

	[Serializable]
	public class StratusAssetAlias : StratusAsset
	{
		[SerializeField]
		[StratusDropdown(nameof(availableAssetNames))]
		private string[] _aliases;
		public string alias => GetAlias(_aliases);
		protected virtual string GetAlias(string[] values) => values.Random();

		public StratusAssetAlias()
		{
		}

		public StratusAssetAlias(string name, params string[] aliases) : base(name)
		{
			_aliases = aliases;
		}

		protected virtual string[] availableAssetNames { get; }
	}

	public abstract class StratusAssetAlias<AssetType> : StratusAssetAlias
	{
		public AssetType asset => GetAsset(alias);
		protected override string[] availableAssetNames => base.availableAssetNames;
		protected abstract AssetType GetAsset(string alias);
		protected abstract string[] GetAvailableAssetNames();
	}

	[Serializable]
	public abstract class StratusAssetReference<AssetType> : StratusAsset
		where AssetType : UnityEngine.Object
	{
		[SerializeField]
		private AssetType[] _references;
		public AssetType reference => GetAsset(_references);
		protected virtual AssetType GetAsset(AssetType[] values) => values.Random();

		public StratusAssetReference()
		{
		}

		public StratusAssetReference(string name, params AssetType[] assets)
			: base(name)
		{
			_references = assets;
		}
	}

	[Serializable]
	public abstract class StratusAssetReference<AssetType, ParameterType> : StratusAssetReference<AssetType>
		where AssetType : UnityEngine.Object
		where ParameterType : class, new()
	{
		[SerializeField]
		private ParameterType _parameters;
		public ParameterType parameters => _parameters;
		public bool hasParameters => _parameters != null;

		protected StratusAssetReference()
		{
		}

		protected StratusAssetReference(string name, params AssetType[] assets) : base(name, assets)
		{
		}

	}

}