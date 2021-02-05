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

	public abstract class StratusAssetAlias<T> : StratusAssetAlias
	{
		public T asset => GetAsset(alias);
		protected override string[] availableAssetNames => base.availableAssetNames;
		protected abstract T GetAsset(string alias);
		protected abstract string[] GetAvailableAssetNames();
	}

	[Serializable]
	public abstract class StratusAssetReference<T> : StratusAsset
		where T : UnityEngine.Object
	{
		[SerializeField]
		private T[] _references;
		public T reference => GetAsset(_references);
		protected virtual T GetAsset(T[] values) => values.Random();

		public StratusAssetReference()
		{
		}

		public StratusAssetReference(string name, params T[] assets)
			: base(name)
		{
			_references = assets;
		}
	}

}