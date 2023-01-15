using System;

using UnityEngine;

namespace Stratus
{
	public static class ScriptableObjectExtensions
	{
		public static T AddInstanceToAsset<T>(this ScriptableObject assetObject) where T : ScriptableObject
		{
			return StratusAssetUtility.AddInstanceToAsset<T>(assetObject);
		}

		public static ScriptableObject AddInstanceToAsset(this ScriptableObject assetObject, Type type)
		{
			return StratusAssetUtility.AddInstanceToAsset(assetObject, type);
		}
	}

}