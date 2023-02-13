using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using Stratus.Models.Graph;

namespace Stratus.Editor
{
	public class StratusDefaultMultiColumnTreeView : StratusMultiColumnTreeView<StratusDefaultTreeElement, StratusDefaultColumn>
	{
		public StratusDefaultMultiColumnTreeView(TreeViewState state, IEnumerable<StratusDefaultTreeElement> data) : base(state, data)
		{
		}

		protected override TreeViewColumn BuildColumn(StratusDefaultColumn columnType)
		{
			TreeViewColumn column = null;
			switch (columnType)
			{
				case StratusDefaultColumn.Property:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent(nameof(StratusDefaultColumn.Property)),
						minWidth = 200,
						width = 175,
						autoResize = true,
						sortedAscending = true,
						selectorFunction = (StratusTreeViewItem<StratusDefaultTreeElement> element) => element.element.name
					};
					break;
				case StratusDefaultColumn.Value:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent(nameof(StratusDefaultColumn.Value)),
						sortedAscending = true,
						minWidth = 200,
						width = 400,
						autoResize = true,
						selectorFunction = (StratusTreeViewItem<StratusDefaultTreeElement> element) => element.element.value
					};
					break;
			}
			return column;
		}

		protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<StratusDefaultTreeElement> item, StratusDefaultColumn column, ref RowGUIArgs args)
		{
			switch (column)
			{
				case StratusDefaultColumn.Property:
					DefaultGUI.Label(cellRect, item.element.name, args.selected, args.focused);
					break;
				case StratusDefaultColumn.Value:
					DefaultGUI.Label(cellRect, item.element.value, args.selected, args.focused);
					break;
			}
		}

		protected override StratusDefaultColumn GetColumn(int index)
		{
			return (StratusDefaultColumn)index;
		}

		protected override int GetColumnIndex(StratusDefaultColumn columnType)
		{
			return (int)columnType;
		}

		protected override void OnContextMenu(GenericMenu menu)
		{

		}

		protected override void OnItemContextMenu(GenericMenu menu, StratusDefaultTreeElement treeElement)
		{
			foreach (var action in treeElement.actions)
			{
				menu.AddItem(new GUIContent(action.label), false, () => action.action());
			}
		}

		protected override void OnItemDoubleClicked(StratusDefaultTreeElement element)
		{
		}
	}

	public class StratusDefaultTreeElement : TreeElement
	{
		public string value;
		public StratusLabeledAction[] actions;

		public StratusDefaultTreeElement()
		{
		}

		public StratusDefaultTreeElement(string name, string value, params StratusLabeledAction[] actions)
		{
			this.name = name;
			this.value = value;
			this.actions = actions;
		}

		public StratusDefaultTreeElement(string name, string value, int depth, int id)
			: this(name, depth, id)
		{
			this.value = value;
		}

		public StratusDefaultTreeElement(string name, int depth, int id) : base(name, depth, id)
		{
		}
	}

	public enum StratusDefaultColumn
	{
		Property,
		Value
	}

}