using Stratus.IO;
using Stratus.Models.Graph;

using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Editor
{
	public class StratusSaveSystemWindow : StratusEditorWindow<StratusSaveSystemWindow>
	{
		[SerializeField]
		private TreeViewState treeViewState = new TreeViewState();
		[SerializeField]
		private StratusDefaultMultiColumnTreeView treeView;

		protected override void OnWindowEnable()
		{
			treeView = new StratusDefaultMultiColumnTreeView(treeViewState, BuildTree());
		}

		protected override void OnWindowGUI()
		{
			this.treeView.TreeViewGUI(this.positionToGUI);
		}

		[MenuItem(StratusCore.rootMenu + "Save System")]
		private static void OpenFromMenu() => OpenWindow("Stratus Save System", true);

		private IEnumerable<StratusDefaultTreeElement> BuildTree()
		{
			StratusSerializedTree<StratusDefaultTreeElement> tree = new StratusSerializedTree<StratusDefaultTreeElement>();
			//tree.AddElement(new StratusDefaultTreeElement(nameof(StratusSaveSystem.rootSaveDirectoryPath), StratusSaveSystem.rootSaveDirectoryPath,
			//	RevealPath(StratusSaveSystem.rootSaveDirectoryPath)), 0);
			return tree.elements;			
		}

		/// <summary>
		/// Constructs an action that will reveal the given file/directory path
		/// </summary>
		public static StratusLabeledAction RevealPath(string path)
		{
			return new StratusLabeledAction("Reveal", () => StratusIO.Open(path));
		}
	}
}