using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Stratus.Editor
{
	[Serializable]
	public class StratusMemberInspectorTreeElement : StratusTreeElement<StratusComponentInformation.MemberReference>
	{

		public static List<StratusMemberInspectorTreeElement> GenerateFavoritesTree()
		{
			StratusComponentInformation.MemberReference[] members = StratusGameObjectBookmark.watchList;
			List<StratusMemberInspectorTreeElement> elements = StratusMemberInspectorTreeElement.GenerateFlatTree<StratusMemberInspectorTreeElement, StratusComponentInformation.MemberReference>(members);
			return elements;
		}

		public static IList<StratusMemberInspectorTreeElement> GenerateInspectorTree(StratusGameObjectInformation target)
		{
			var tree = new StratusSerializedTree<StratusMemberInspectorTreeElement, StratusComponentInformation.MemberReference>();
			tree.AddElements(target.members, 0);
			return tree.elements;
		}
	}

	public class StratusMemberInspectorTreeView : StratusMultiColumnTreeView<StratusMemberInspectorTreeElement, StratusMemberInspectorWindow.Column>
	{
		public StratusMemberInspectorTreeView(TreeViewState state, StratusTreeModel<StratusMemberInspectorTreeElement> model) : base(state, model)
		{
		}

		public StratusMemberInspectorTreeView(TreeViewState state, IList<StratusMemberInspectorTreeElement> data) : base(state, data)
		{
		}

		protected override TreeViewColumn BuildColumn(StratusMemberInspectorWindow.Column columnType)
		{
			TreeViewColumn column = null;
			switch (columnType)
			{
				case StratusMemberInspectorWindow.Column.Favorite:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent(StratusGUIStyles.starStackIcon, "Watch"),
						headerTextAlignment = TextAlignment.Center,
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Right,
						width = 30,
						minWidth = 30,
						maxWidth = 45,
						autoResize = false,
						allowToggleVisibility = false,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.isWatched.ToString()
					};
					break;
				case StratusMemberInspectorWindow.Column.GameObject:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("GameObject"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Right,
						width = 100,
						minWidth = 100,
						maxWidth = 120,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.gameObjectName
					};
					break;
				case StratusMemberInspectorWindow.Column.Component:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Component"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Right,
						width = 150,
						minWidth = 100,
						maxWidth = 250,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.componentName
					};
					break;
				case StratusMemberInspectorWindow.Column.Type:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Type"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Center,
						width = 60,
						minWidth = 60,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.type.ToString()
					};
					break;
				case StratusMemberInspectorWindow.Column.Member:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Member"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Center,
						width = 100,
						minWidth = 80,
						maxWidth = 120,
						autoResize = false,
						allowToggleVisibility = false,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.name
					};
					break;
				case StratusMemberInspectorWindow.Column.Value:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Value"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Left,
						width = 200,
						minWidth = 150,
						maxWidth = 250,
						autoResize = true,
						allowToggleVisibility = false,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.item.data.latestValueString
					};
					break;
			}
			return column;
		}

		protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<StratusMemberInspectorTreeElement> item, StratusMemberInspectorWindow.Column column, ref RowGUIArgs args)
		{
			switch (column)
			{
				case StratusMemberInspectorWindow.Column.Favorite:
					if (item.item.data.isWatched)
					{
						this.DrawIcon(cellRect, StratusGUIStyles.starIcon);
					}

					break;
				case StratusMemberInspectorWindow.Column.GameObject:
					DefaultGUI.Label(cellRect, item.item.data.gameObjectName, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Component:
					DefaultGUI.Label(cellRect, item.item.data.componentName, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Type:
					DefaultGUI.Label(cellRect, item.item.data.type.ToString(), args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Member:
					DefaultGUI.Label(cellRect, item.item.data.name, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Value:
					DefaultGUI.Label(cellRect, item.item.data.latestValueString, args.selected, args.focused);
					break;
			}
		}

		protected override StratusMemberInspectorWindow.Column GetColumn(int index)
		{
			return (StratusMemberInspectorWindow.Column)index;
		}

		protected override int GetColumnIndex(StratusMemberInspectorWindow.Column columnType)
		{
			return (int)columnType;
		}

		protected override void OnContextMenu(GenericMenu menu)
		{

		}

		protected override void OnItemContextMenu(GenericMenu menu, StratusMemberInspectorTreeElement treeElement)
		{
			StratusComponentInformation.MemberReference member = treeElement.data;

			// 1. Select
			menu.AddItem(new GUIContent("Select"), false, () => Selection.activeGameObject = member.componentInfo.gameObject);

			if (!Application.isPlaying)
			{
				// 2. Watch
				bool isFavorite = member.isWatched;
				if (isFavorite)
				{
					menu.AddItem(new GUIContent("Remove Watch"), false, () => member.componentInfo.RemoveWatch(member));
				}
				else
				{
					menu.AddItem(new GUIContent("Watch"), false, () =>
					{
						GameObject target = member.componentInfo.gameObject;
						member.componentInfo.Watch(member);
					});
				}
			}

		}

	}
}