using System;

namespace Stratus
{
	public class StratusUnityAssetQuery<AssetType> : StratusAssetQuery<AssetType>
		where AssetType : UnityEngine.Object
	{
		public StratusUnityAssetQuery(Func<AssetType[]> getAssetsFunction) : base(getAssetsFunction, x => x.name)
		{
		}
	}

}