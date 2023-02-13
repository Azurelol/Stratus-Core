using Stratus.Models.Graph;

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Editor
{
	#region Inspector
	[Serializable]
	public class StratusMemberInspectorTreeElement : TreeElement<StratusComponentMemberInfo>
	{
		public static IList<StratusMemberInspectorTreeElement> Generate(StratusGameObjectInformation target)
		{
			var tree = new StratusSerializedTree<StratusMemberInspectorTreeElement, StratusComponentMemberInfo>();
			tree.AddElements(target.visibleMembers, 0);
			return tree.elements;
		}
	}

	public class StratusMemberInspectorTreeView : StratusMultiColumnTreeView<StratusMemberInspectorTreeElement, StratusMemberInspectorWindow.Column>
	{
		public StratusGameObjectInformation gameObject { get; private set; }
		public StratusComponentMemberWatchList watchList { get; private set; }

		public StratusMemberInspectorTreeView(TreeViewState state, StratusGameObjectInformation gameObject, IList<StratusMemberInspectorTreeElement> data, StratusComponentMemberWatchList watchList)
			: base(state, data)
		{
			this.gameObject = gameObject;
			this.watchList = watchList;
			this.watchList.onUpdated += this.Reload;
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
						selectorFunction = (StratusTreeViewItem<StratusMemberInspectorTreeElement> element) => watchList.Contains(element.element.data).ToString()
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
					if (watchList.Contains(item.element.data))
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

			menu.AddItem(new GUIContent("Fetch"), false, () => gameObject.UpdateValue(member));
			menu.AddItem(new GUIContent("Copy"), false, () => GUIUtility.systemCopyBuffer = member.latestValueString);

			// 2. Watch
			if (watchList.Contains(member))
			{
				menu.AddItem(new GUIContent("Remove Watch"), false, () => watchList.Remove(member));
			}
			else
			{
				menu.AddItem(new GUIContent("Watch"), false, () =>
				{
					watchList.Add(member);
				});
			}
		}

		protected override void OnItemDoubleClicked(StratusMemberInspectorTreeElement element)
		{
			watchList.Toggle(element.data);
		}
	}
	#endregion


	#region Watch
	[Serializable]
	public class StratusComponentMemberWatchTreeElement : TreeElement<StratusComponentMemberWatchInfo>
	{
		public static IList<StratusComponentMemberWatchTreeElement> Generate(StratusComponentMemberWatchList target)
		{
			var tree = new StratusSerializedTree<StratusComponentMemberWatchTreeElement, StratusComponentMemberWatchInfo>();
			tree.AddElements(target.members, 0);
			return tree.elements;
		}
	}

	public enum StratusComponentMemberWatchViewColumn
	{
		Component,
		Member,
		Type
	}

	public class StratusMemberInspectorWatchListTreeView : StratusMultiColumnTreeView<StratusComponentMemberWatchTreeElement, StratusComponentMemberWatchViewColumn>
	{
		public StratusComponentMemberWatchList watchList { get; private set; }

		public StratusMemberInspectorWatchListTreeView(TreeViewState state, StratusComponentMemberWatchList watchList)
			: base(state, new StratusProvider<IList<StratusComponentMemberWatchTreeElement>>(() => StratusComponentMemberWatchTreeElement.Generate(watchList)))
		{
			this.watchList = watchList;
			this.watchList.onUpdated += this.Reload;
		}

		protected override TreeViewColumn BuildColumn(StratusComponentMemberWatchViewColumn columnType)
		{
			TreeViewColumn column = null;
			switch (columnType)
			{
				case StratusComponentMemberWatchViewColumn.Component:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Component"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Right,
						width = 250,
						minWidth = 150,
						maxWidth = 300,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusComponentMemberWatchTreeElement> element) => element.element.data.componentName
					};
					break;
				case StratusComponentMemberWatchViewColumn.Type:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent("Type"),
						sortedAscending = true,
						sortingArrowAlignment = TextAlignment.Center,
						width = 100,
						minWidth = 100,
						autoResize = false,
						allowToggleVisibility = true,
						selectorFunction = (StratusTreeViewItem<StratusComponentMemberWatchTreeElement> element) => element.element.data.typeName
					};
					break;
				case StratusComponentMemberWatchViewColumn.Member:
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
						selectorFunction = (StratusTreeViewItem<StratusComponentMemberWatchTreeElement> element) => element.element.data.name
					};
					break;
			}
			return column;
		}

		protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<StratusComponentMemberWatchTreeElement> item, StratusComponentMemberWatchViewColumn column, ref RowGUIArgs args)
		{
			switch (column)
			{
				case StratusComponentMemberWatchViewColumn.Component:
					DefaultGUI.Label(cellRect, item.element.data.componentName, args.selected, args.focused);
					break;
				case StratusComponentMemberWatchViewColumn.Type:
					DefaultGUI.Label(cellRect, item.element.data.typeName, args.selected, args.focused);
					break;
				case StratusComponentMemberWatchViewColumn.Member:
					DefaultGUI.Label(cellRect, item.element.data.name, args.selected, args.focused);
					break;
			}
		}

		protected override StratusComponentMemberWatchViewColumn GetColumn(int index)
		{
			return (StratusComponentMemberWatchViewColumn)index;
		}

		protected override void OnContextMenu(GenericMenu menu)
		{
		}

		protected override void OnItemContextMenu(GenericMenu menu, StratusComponentMemberWatchTreeElement treeElement)
		{
			StratusComponentMemberWatchInfo member = treeElement.data;
			menu.AddItem(new GUIContent("Remove"), false, () =>
			{
				watchList.Remove(member);
			});
		}

		protected override void OnItemDoubleClicked(StratusComponentMemberWatchTreeElement element)
		{
		}
	}
	#endregion
}