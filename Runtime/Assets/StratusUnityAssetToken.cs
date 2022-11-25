using System;

namespace Stratus
{
	public abstract class StratusUnityAssetToken<T>
		: StratusAssetToken<T>
		where T : UnityEngine.Object
	{
		protected StratusUnityAssetToken(string name, Func<T> assetFunction) 
			: base(name, assetFunction)
		{
		}

		protected StratusUnityAssetToken(string name, Func<string, T> aliasToAssetFunction) 
			: base(name, aliasToAssetFunction)
		{
		}

		protected override bool IsNull(T asset)
		{
			return Stratus.OdinSerializer.Utilities.UnityExtensions.SafeIsUnityNull(asset);
		}
	}

}