﻿using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Editor
{
	[Serializable]
	public class StratusMemberInspectorTreeElement : StratusTreeElement<StratusComponentMemberInfo>
	{
		public static List<StratusMemberInspectorTreeElement> GenerateFavoritesTree()
		{
			// @TODO : NOt impl
			StratusComponentMemberInfo[] members = new StratusComponentMemberInfo[] { };
			//StratusGameObjectBookmark.watchList;
			List<StratusMemberInspectorTreeElement> elements = StratusMemberInspectorTreeElement.GenerateFlatTree<StratusMemberInspectorTreeElement, StratusComponentMemberInfo>(members);
			return elements;
		}

		public static IList<StratusMemberInspectorTreeElement> GenerateInspectorTree(StratusGameObjectInformation target)
		{
			var tree = new StratusSerializedTree<StratusMemberInspectorTreeElement, StratusComponentMemberInfo>();
			tree.AddElements(target.visibleMembers, 0);
			return tree.elements;
		}
	}

	public class StratusMemberInspectorTreeView : StratusMultiColumnTreeView<StratusMemberInspectorTreeElement, StratusMemberInspectorWindow.Column>
	{
		public StratusGameObjectInformation gameObject { get; private set; }

		public StratusMemberInspectorTreeView(TreeViewState state, StratusGameObjectInformation gameObject, IList<StratusMemberInspectorTreeElement> data) : base(state, data)
		{
			this.gameObject = gameObject;
		}

		protected override TreeViewColumn BuildColumn(StratusMemberInspectorWindow.Column columnType)
		{
			TreeViewColumn column = null;
			switch (columnType)
			{
				case StratusMemberInspectorWindow.Column.Watch:
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => gameObject.IsWatched(element.element.data).ToString()
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.element.data.gameObjectName
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.element.data.componentName
					};
					break;
				case StratusMemberInspectorWindow.Column.Type:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Type"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Center,
						width = 100,
						minWidth = 100,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.element.data.typeName
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.element.data.name
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => element.element.data.latestValueString
					};
					break;
			}
			return column;
		}

		protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<StratusMemberInspectorTreeElement> item, StratusMemberInspectorWindow.Column column, ref RowGUIArgs args)
		{
			switch (column)
			{
				case StratusMemberInspectorWindow.Column.Watch:
					if (gameObject.IsWatched(item.element.data))
					{
						this.DrawIcon(cellRect, StratusGUIStyles.starIcon);
					}

					break;
				case StratusMemberInspectorWindow.Column.GameObject:
					DefaultGUI.Label(cellRect, item.element.data.gameObjectName, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Component:
					DefaultGUI.Label(cellRect, item.element.data.componentName, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Type:
					DefaultGUI.Label(cellRect, item.element.data.typeName, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Member:
					DefaultGUI.Label(cellRect, item.element.data.name, args.selected, args.focused);
					break;
				case StratusMemberInspectorWindow.Column.Value:
					DefaultGUI.Label(cellRect, item.element.data.latestValueString, args.selected, args.focused);
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
			StratusComponentMemberInfo member = treeElement.data;

			//// 1. Select
			//menu.AddItem(new GUIContent("Select"), false, () => Selection.activeGameObject = member.componentInfo.gameObject);

			menu.AddItem(new GUIContent("Fetch"), false, () => gameObject.UpdateValue(member));
			menu.AddItem(new GUIContent("Copy"), false, () => GUIUtility.systemCopyBuffer = member.latestValueString);

			// 2. Watch
			if (gameObject.IsWatched(member))
			{
				menu.AddItem(new GUIContent("Remove Watch"), false, () => gameObject.RemoveWatch(member));
			}
			else
			{
				menu.AddItem(new GUIContent("Watch"), false, () =>
				{
					gameObject.AddWatch(member);
				});
			}
		}


		protected override void OnItemDoubleClicked(StratusMemberInspectorTreeElement element)
		{
			gameObject.ToggleWatch(element.data);
		}
	}
}