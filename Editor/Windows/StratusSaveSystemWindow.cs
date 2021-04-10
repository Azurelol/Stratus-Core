using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

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
			this.treeView.TreeViewGUI(this.guiPosition);
		}

		[MenuItem(StratusCore.rootMenu + "Save System")]
		private static void OpenFromMenu() => OpenWindow("Stratus Save System", true);

		private IEnumerable<StratusDefaultTreeElement> BuildTree()
		{
			StratusSerializedTree<StratusDefaultTreeElement> tree = new StratusSerializedTree<StratusDefaultTreeElement>();
			tree.AddElement(new StratusDefaultTreeElement(nameof(StratusSaveSystem.rootSaveDirectoryPath), StratusSaveSystem.rootSaveDirectoryPath,
				StratusLabeledAction.RevealPath(StratusSaveSystem.rootSaveDirectoryPath)), 0);
			return tree.elements;
			
		}
	}
}