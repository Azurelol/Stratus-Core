using UnityEngine;
using System;

namespace Stratus
{
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

	public abstract class StratusAssetAlias<TAsset> : StratusAssetAlias
		where TAsset : class
	{
		public TAsset asset => GetAsset(alias);
		public abstract StratusAssetSource<TAsset> source { get; }
		protected override string[] availableAssetNames => source.GetAssetNames();
		protected abstract TAsset GetAsset(string alias);
		protected abstract string[] GetAvailableAssetNames();
	}
}