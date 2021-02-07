namespace Stratus
{
	public interface IStratusAssetSource<T> where T : class
	{
		bool HasAsset(string name);
		StratusAssetToken<T> GetAsset(string name);
	}

}