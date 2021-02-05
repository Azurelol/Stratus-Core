using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public interface IStratusAssetToken<T> : IStratusAsset<T>
	{
		StratusAssetSourceType assetSourceType { get; }
	}

	public class StratusAssetToken<T> : IStratusAssetToken<T>
		where T : class
	{
		public string name { get; private set; }
		public StratusAssetSourceType assetSourceType { get; private set; }
		public T asset
		{
			get
			{
				if (!queried || IsNull(_asset))
				{
					switch (assetSourceType)
					{
						case StratusAssetSourceType.Invalid:
							break;
						case StratusAssetSourceType.Reference:
							_asset = assetFunction();
							break;
						case StratusAssetSourceType.Alias:
							_asset = aliasToAssetFunction(name);
							break;
					}
					queried = true;
				}
				return _asset;
			}
		}
		private T _asset = null;

		public bool queried { get; private set; }

		private Func<T> assetFunction;
		private Func<string, T> aliasToAssetFunction;

		public StratusAssetToken(string name, Func<T> assetFunction)
		{
			this.name = name;
			this.assetSourceType = StratusAssetSourceType.Reference;
			this.assetFunction = assetFunction;
		}

		public StratusAssetToken(string name, Func<string, T> aliasToAssetFunction)
		{
			this.name = name;
			this.assetSourceType = StratusAssetSourceType.Alias;
			this.aliasToAssetFunction = aliasToAssetFunction;
		}

		protected virtual bool IsNull(T asset) => asset == null;
		public override string ToString() => name;
	}

	public abstract class StratusUnityAssetReference<T>
		: StratusAssetToken<T>
		where T : UnityEngine.Object
	{
		protected StratusUnityAssetReference(string name, Func<T> assetFunction) 
			: base(name, assetFunction)
		{
		}

		protected StratusUnityAssetReference(string name, Func<string, T> aliasToAssetFunction) 
			: base(name, aliasToAssetFunction)
		{
		}

		protected override bool IsNull(T asset)
		{
			return Stratus.OdinSerializer.Utilities.UnityExtensions.SafeIsUnityNull(asset);
		}
	}

}