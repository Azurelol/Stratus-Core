using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Settings meant to be used at runtime by a player
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public class StratusPlayerPrefs<DataType> : StratusUserSettings<DataType>,
		IStratusPrefs<DataType>
		where DataType : class, new()
	{
		public StratusPlayerPrefs(bool appendDataPath = false) : base(appendDataPath)
		{
		}

		public StratusPlayerPrefs(string key, bool appendDataPath = false) : base(key, appendDataPath)
		{
		}

		protected override string GetPrefs(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		protected override void SetPrefs(string key, string serialization)
		{
			PlayerPrefs.SetString(key, serialization);
		}
	}

}