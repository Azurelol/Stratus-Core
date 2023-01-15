using UnityEngine;
using System;
using Stratus.Extensions;

namespace Stratus
{
	[Serializable]
	public abstract class StratusUnityAssetReference<TAsset> : StratusAsset
		where TAsset : UnityEngine.Object
	{
		[SerializeField]
		private TAsset[] _references;
		public TAsset reference => GetAsset(_references);
		protected virtual TAsset GetAsset(TAsset[] values) => values.Random();

		public StratusUnityAssetReference()
		{
		}

		public StratusUnityAssetReference(string name, params TAsset[] assets)
			: base(name)
		{
			_references = assets;
		}
	}

	[Serializable]
	public abstract class StratusUnityAssetReference<TAsset, TParameter> : StratusUnityAssetReference<TAsset>
		where TAsset : UnityEngine.Object
		where TParameter : class, new()
	{
		[SerializeField]
		private TParameter _parameters;
		public TParameter parameters => _parameters;
		public bool hasParameters => _parameters != null;

		protected StratusUnityAssetReference()
		{
		}

		protected StratusUnityAssetReference(string name, params TAsset[] assets) : base(name, assets)
		{
		}
	}
}