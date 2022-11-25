using System;

namespace Stratus
{
	public interface IStratusAssetSource<TAsset> where TAsset : class
	{
		bool HasAsset(string name);
		StratusAssetToken<TAsset> GetAsset(string name);
		string[] GetAssetNames();
	}

	public abstract class StratusAssetSource<TAsset>
		: IStratusAssetSource<TAsset>
		where TAsset : class
	{
		public abstract StratusAssetToken<TAsset> GetAsset(string name);
		public abstract string[] GetAssetNames();
		public abstract bool HasAsset(string name);
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