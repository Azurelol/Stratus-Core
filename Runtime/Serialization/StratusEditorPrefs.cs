namespace Stratus
{
	/// <summary>
	/// Settings meant to be used by an editor
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public class StratusEditorPrefs<DataType> : StratusUserSettings<DataType>
		where DataType : class, new()
	{
		public StratusEditorPrefs(string key, bool appendDataPath = false) : base(key, appendDataPath)
		{
		}

		protected override string GetPrefs(string key)
		{
			string serialization = null;
#if UNITY_EDITOR
			serialization = UnityEditor.EditorPrefs.GetString(key);
#endif
			return serialization;
		}

		protected override void SetPrefs(string key, string serialization)
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetString(key, serialization);
#endif
		}
	}

}