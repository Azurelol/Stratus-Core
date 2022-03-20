using UnityEngine;
using System;

namespace Stratus.Serialization
{
	/// <summary>
	/// A variable saved using PlayerPrefs
	/// </summary>
	public class StratusPlayerPrefsVariable : StratusPrefsVariable
	{
		public StratusPlayerPrefsVariable(string key, VariableType type) : base(key, type)
		{
		}

		public StratusPlayerPrefsVariable(Type key, VariableType type) : base(key, type)
		{
		}

		public override bool GetBool()
		{
			return PlayerPrefs.GetInt(key) == 0 ? true : false;
		}

		public override float GetFloat()
		{
			return PlayerPrefs.GetFloat(key);
		}

		public override int GetInt()
		{
			return PlayerPrefs.GetInt(key);
		}

		public override string GetString()
		{
			return PlayerPrefs.GetString(key);
		}

		public override void SetBool(bool value)
		{
			PlayerPrefs.SetInt(key, value ? 0 : 1);
		}

		public override void SetFloat(float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public override void SetInt(int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public override void SetString(string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public override void Delete()
		{
			PlayerPrefs.DeleteKey(key);
		}

		public override bool Exists()
		{
			return PlayerPrefs.HasKey(key);
		}
	}

}