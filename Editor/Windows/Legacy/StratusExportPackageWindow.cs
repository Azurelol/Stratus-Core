using Stratus.IO;
using Stratus.Utilities;

using UnityEditor;

using UnityEngine;

namespace Stratus.Editor
{
	public class StratusExportPackageWindow : StratusEditorWindow<StratusExportPackageWindow>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		public StratusExportPackagePreset preset;
		private const string recentPresetKey = "Recent Export Package Preset";

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasPreset => !this.preset.IsNull();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			preset = StratusPreferences.GetObjectReference<StratusExportPackagePreset>(recentPresetKey);
		}

		protected override void OnWindowGUI()
		{
			if (this.InspectObjectFieldWithHeader(ref this.preset, "Preset"))
			{
				StratusPreferences.SaveObjectReference(recentPresetKey, preset);
			}
			if (this.hasPreset)
			{
				this.InspectProperties(this.preset, "Properties");
			}
			this.ExportControls();
		}

		[MenuItem(StratusCore.rootMenu + "Export Package")]
		public static void Open()
		{
			OpenWindow("Export Package");
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void Export(StratusExportPackageArguments arguments)
		{
			string location = StratusIO.GetFolderPath(arguments.path);
			AssetDatabase.ExportPackage(location, $"{arguments.name}.unitypackage", arguments.options);
			EditorUtility.RevealInFinder(location);
			StratusDebug.Log($"Exported {arguments.name} to {location}");
		}

		private void ExportControls()
		{
			StratusEditorGUILayout.BeginAligned(TextAlignment.Center);
			{
				StratusEditorGUILayout.Button("Export", () =>
				{

					if (this.hasPreset && this.preset.arguments.valid)
					{
						Export(this.preset.arguments);
					}

				});
			}
			StratusEditorGUILayout.EndAligned();
		}


	}

}