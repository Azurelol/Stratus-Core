using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.IO;
using Stratus.OdinSerializer.Utilities;
using System;
using Stratus.Serialization;

namespace Stratus.Editor.Windows
{
	public class StratusSerializedAssetEditorWindow : StratusEditorWindow
	{
		private const string _title = "Serialized Assets";
		[UnityEngine.SerializeField]
		private string _assetFolder = "Assets";


		[MenuItem("Stratus/Core/" + _title)]
		public static void ShowExample()
		{
			StratusSerializedAssetEditorWindow wnd = GetWindow<StratusSerializedAssetEditorWindow>();
			wnd.titleContent = new GUIContent(_title);

		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;

			var visualTree = Resources.Load<VisualTreeAsset>("StratusSerializedAssetEditorWindow");
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			var styleSheet = Resources.Load<StyleSheet>("StratusSerializedAssetEditorWindow");
			root.styleSheets.Add(styleSheet);

			var assetListElement = root.Q<ListView>("Assets");
			var createAssets = StratusSerializedAsset.types.Value.Select(t =>
			{
				return new StratusLabeledAction(t.Name, null)
				{
					data = t
				}
				;
			}).ToList();

			assetListElement.itemsSource = createAssets;
			assetListElement.selectedIndex = 0;

			var controls = root.Q("Controls");
			var createButton = controls.Q<Button>("Create");
			createButton.clicked += () =>
			{
				StratusLabeledAction current = (StratusLabeledAction)assetListElement.selectedItem;
				Type type = current.Data<Type>();
				var attr = type.GetAttribute<StratusSerializedAssetAttribute>();
				string fileName = attr.name ?? type.Name;

				var path = EditorUtility.SaveFilePanel("Create Asset", _assetFolder, fileName, attr.extension);
				if (path != null)
				{
					_assetFolder = FileUtil.GetProjectRelativePath(path);
					CreateAsset(type, path);
				}
			};
		}

		private void CreateAsset(Type type, string path)
		{
			StratusDebug.Log($"Creating {type.Name} at {path}");
			StratusSerializedAsset.Create(type, path, new JsonObjectSerializer());
		}
	}
}