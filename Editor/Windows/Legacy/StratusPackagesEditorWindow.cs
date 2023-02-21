using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using Stratus.Extensions;
using Stratus.Logging;

namespace Stratus.Editor
{
	//public class StratusUnityPackageInfo
	//{
	//	public string name;
	//	public string version;
	//	public string displayName;


	//	public string filePath { get; private set; }
	//	public bool valid { get; private set;
	//	}
	//	public static StratusUnityPackageInfo Parse(string filePath)
	//	{
	//		UnityEditor.PackageManager.PackageInfo.FindForAssetPath
	//		StratusUnityPackageInfo info = null;
	//		try
	//		{
	//			string json = File.ReadAllText(filePath);
	//			info = JsonUtility.FromJson<StratusUnityPackageInfo>(json);

	//		}
	//		catch (Exception e)
	//		{
	//		}
	//		return info;
	//	}

	//}

	public class StratusPackagesEditorWindow : StratusEditorWindow<StratusPackagesEditorWindow>
	{
		public const string packageFilename = "package.json";
		public const string windowTitle = "Stratus Package Development";
		public UnityEditor.PackageManager.PackageInfo[] packages { get; private set; }

		[MenuItem(StratusCore.rootMenu + "Package Development")]
		public static void Open()
		{
			OpenWindow(windowTitle, true);
		}

		protected override void OnWindowEnable()
		{
			SearchForPackageManifestsInProject();
		}

		protected override void OnWindowGUI()
		{
		}

		private void SearchForPackageManifestsInProject()
		{
			var packageAssets = AssetDatabase.FindAssets(packageFilename);
			if (packageAssets.IsNullOrEmpty())
			{
				this.LogWarning("No package manfiests were found");
				return;
			}
			packages = packageAssets.Transform(x => UnityEditor.PackageManager.PackageInfo.FindForAssetPath(x)).ToArray();
			this.Log($"Found {packages.Length} packages!");
		}

		public static void EditPackageManifest()
		{
		}
	}

}