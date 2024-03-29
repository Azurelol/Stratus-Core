﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;
using UnityEngine.Assertions;

namespace Stratus.Editor
{
	public abstract class StratusMultiColumnTreeView<TreeElementType, ColumnType> 
		: StratusTreeViewWithTreeModel<TreeElementType>
		where TreeElementType : StratusTreeElement
		where ColumnType : Enum
	{
		#region Declarations
		public class TreeViewColumn : MultiColumnHeaderState.Column
		{
			/// <summary>
			/// The function used to select a value for this column
			/// </summary>
			public Func<StratusTreeViewItem<TreeElementType>, string> selectorFunction;
			/// <summary>
			/// An unique icon for this column
			/// </summary>
			public Texture2D icon;

			public TreeViewColumn(string label, Func<StratusTreeViewItem<TreeElementType>, string> selectorFunction)
			{
				this.headerContent = new GUIContent(label);
				this.headerTextAlignment = TextAlignment.Center;
				this.width = this.minWidth = GUI.skin.label.CalcSize(this.headerContent).x;
				this.selectorFunction = selectorFunction;
			}

			public TreeViewColumn()
			{
			}
		}
		#endregion

		#region Fields
		public bool showControls = true;
		private float rowHeights = 20f;
		private float toggleWidth = 18f;
		#endregion

		#region Properties
		protected TreeViewColumn[] columns { get; private set; }
		public bool initialized { get; private set; }
		public StratusMultiColumnHeader stratusMultiColumnHeader { get; set; }
		#endregion

		#region Virtual
		protected abstract TreeViewColumn BuildColumn(ColumnType columnType);
		protected abstract void DrawColumn(Rect cellRect, StratusTreeViewItem<TreeElementType> item, ColumnType column, ref RowGUIArgs args);
		protected abstract ColumnType GetColumn(int index);
		protected virtual int GetColumnIndex(ColumnType columnType) => (int)(object)columnType;
		protected abstract void OnItemDoubleClicked(TreeElementType element);
		#endregion

		#region Events
		public event Action<ColumnType> onColumnSortedChanged;
		#endregion

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusMultiColumnTreeView(TreeViewState state, StratusProvider<IList<TreeElementType>> data)
			: base(state, new StratusTreeModel<TreeElementType>(data))
		{
			this.columns = this.BuildColumns();
			MultiColumnHeaderState headerState = BuildMultiColumnHeaderState(this.columns);
			this.multiColumnHeader = this.stratusMultiColumnHeader = new StratusMultiColumnHeader(headerState);
			this.InitializeMultiColumnTreeView();
		}

		public StratusMultiColumnTreeView(TreeViewState state, IEnumerable<TreeElementType> data)
			: this(state, new StratusProvider<IList<TreeElementType>>(data.ToList()))
		{
		}

		public StratusMultiColumnTreeView(TreeViewState state, StratusTreeModel<TreeElementType> model)
		: base(state, model)
		{
			this.columns = this.BuildColumns();
			MultiColumnHeaderState headerState = BuildMultiColumnHeaderState(this.columns);
			this.multiColumnHeader = new StratusMultiColumnHeader(headerState);
			this.InitializeMultiColumnTreeView();
		}

		protected void InitializeMultiColumnTreeView()
		{
			this.columnIndexForTreeFoldouts = 0;
			this.showAlternatingRowBackgrounds = true;
			this.showBorder = true;

			// Center foldout in the row since we also center content. See RowGUI
			this.rowHeight = this.rowHeights;
			this.customFoldoutYOffset = (this.rowHeight - EditorGUIUtility.singleLineHeight) * 0.5f;
			this.extraSpaceBeforeIconAndLabel = this.toggleWidth;

			// Callbacks
			this.multiColumnHeader.sortingChanged += this.OnSortingChanged;

			this.Reload();
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			this.SortIfNeeded(this.rootItem, this.GetRows());
			this.onColumnSortedChanged?.Invoke(GetColumn(multiColumnHeader.sortedColumnIndex));
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			IList<TreeViewItem> rows = base.BuildRows(root);
			this.SortIfNeeded(root, rows);
			return rows;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			StratusTreeViewItem<TreeElementType> item = (StratusTreeViewItem<TreeElementType>)args.item;
			for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
			{
				Rect cellRect = args.GetCellRect(i);
				this.CenterRectUsingSingleLineHeight(ref cellRect);
				this.DrawColumn(cellRect, item, this.GetColumn(args.GetColumn(i)), ref args);

			}
		}

		protected override bool CanRename(TreeViewItem item)
		{
			// Only allow rename if we can showw the rename overlay with a certain width 
			// (label might be clipped by other columns)
			Rect renameRect = this.GetRenameRect(this.treeViewRect, 0, item);
			return renameRect.width > 30;
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
			// Set the backend name and reload the tree to reflect the new model
			if (args.acceptedRename)
			{
				TreeElementType element = this.treeModel.Find(args.itemID);
				element.name = args.newName;
				this.Reload();
			}
		}

		protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			Rect cellRect = this.GetCellRectForTreeFoldouts(rowRect);
			this.CenterRectUsingSingleLineHeight(ref cellRect);
			return base.GetRenameRect(cellRect, row, item);
		}

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return true;
		}

		protected override void DoubleClickedItem(int id)
		{
			var element = GetElement(id);
			OnItemDoubleClicked(element);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		protected static MultiColumnHeaderState BuildMultiColumnHeaderState(TreeViewColumn[] columns)
		{
			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(ColumnType)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");
			MultiColumnHeaderState state = new MultiColumnHeaderState(columns);
			return state;
		}

		private TreeViewColumn[] BuildColumns()
		{
			int numberOfColumns = Enum.GetValues(typeof(ColumnType)).Length;
			TreeViewColumn[] columns = new TreeViewColumn[numberOfColumns];
			for (int c = 0; c < numberOfColumns; ++c)
			{
				ColumnType columnType = this.GetColumn(c);
				columns[c] = this.BuildColumn(columnType);
				if (columns[c] == null)
				{
					throw new Exception($"Column implementation missing for {columnType.ToString()}");
				}
			}
			return columns;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Checks whether the given instance id is a valid asset for this tree view,
		/// if so it sets it
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		public bool TryOpenAsset(int instanceID, int line)
		{
			StratusTreeAsset<TreeElementType> treeAsset = EditorUtility.InstanceIDToObject(instanceID) as StratusTreeAsset<TreeElementType>;
			if (treeAsset != null)
			{
				this.SetTreeAsset(treeAsset);
				return true;
			}

			return false;
		}

		//------------------------------------------------------------------------/
		// Methods: GUI
		//------------------------------------------------------------------------/
		/// <summary>
		/// Toggles the column
		/// </summary>
		/// <param name="column"></param>
		public void ToggleColumn(ColumnType column)
		{
			this.stratusMultiColumnHeader.ToggleColumn(this.GetColumnIndex(column));
		}

		/// <summary>
		/// Toggles the column
		/// </summary>
		/// <param name="column"></param>
		public void EnableColumn(ColumnType column)
		{
			this.stratusMultiColumnHeader.EnableColumn(this.GetColumnIndex(column));
		}

		/// <summary>
		/// Toggles the column
		/// </summary>
		/// <param name="column"></param>
		public void DisableColumn(ColumnType column)
		{
			this.stratusMultiColumnHeader.DisableColumn(this.GetColumnIndex(column));
		}

		public void SortByColumn(ColumnType column, bool ascending = false)
		{			
			this.stratusMultiColumnHeader.SetSorting(GetColumnIndex(column), ascending);
		}

		//------------------------------------------------------------------------/
		// Methods: Sorting
		//------------------------------------------------------------------------/
		private void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
		{
			// If there's only one row orr if there's no columns to sort of
			if (rows.Count <= 1 || this.multiColumnHeader.sortedColumnIndex == -1)
			{
				return;
			}

			// Sort the roots of the existing tree items
			this.SortByMultipleColumns();
			//TreeElement.TreeToList(root, rows);
			TreeToList(root, rows);
			this.Repaint();
		}

		private void SortByMultipleColumns()
		{
			int[] sortedColumns = this.multiColumnHeader.state.sortedColumns;
			if (sortedColumns.Empty())
			{
				return;
			}

			IEnumerable<StratusTreeViewItem<TreeElementType>> types = this.rootItem.children.Cast<StratusTreeViewItem<TreeElementType>>();
			IOrderedEnumerable<StratusTreeViewItem<TreeElementType>> orderedQuery = this.GetInitialOrder(types, sortedColumns);
			for (int c = 1; c < sortedColumns.Length; ++c)
			{
				int index = sortedColumns[c];
				TreeViewColumn column = this.columns[index];
				bool ascending = this.multiColumnHeader.IsSortedAscending(index);
				orderedQuery = orderedQuery.ThenBy(l => column.selectorFunction(l), ascending);
			}

			this.rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
		}

		private IOrderedEnumerable<StratusTreeViewItem<TreeElementType>> GetInitialOrder(IEnumerable<StratusTreeViewItem<TreeElementType>> types, int[] history)
		{
			int index = history[0];
			TreeViewColumn column = this.columns[index];
			bool ascending = this.multiColumnHeader.IsSortedAscending(index);

			// If a sorting function was provided
			if (column.selectorFunction != null)
			{
				return types.Order(l => column.selectorFunction(l), ascending);
			}

			// Default
			return types.Order(l => l.element.name, ascending);
		}



		//------------------------------------------------------------------------/
		// Utility Methods: Sorting
		//------------------------------------------------------------------------/
		protected void DrawIcon(Rect rect, Texture2D icon)
		{
			GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
		}

		protected void DrawToggle(Rect rect, StratusTreeViewItem<TreeElementType> item, ref bool toggle, ref RowGUIArgs args)
		{
			Rect toggleRect = rect;
			toggleRect.x = this.GetContentIndent(item);
			toggleRect.width = this.toggleWidth;

			// Hide when outside cell rect
			if (toggleRect.xMax < rect.xMax)
			{
				toggle = EditorGUI.Toggle(toggleRect, toggle);
			}

			args.rowRect = rect;
			base.RowGUI(args);
		}

		protected void DrawSlider(Rect cellRect, ref float value, float min, float max)
		{
			// When showing controls, make some extra spacing
			const float spacing = 5f;
			cellRect.xMin += spacing;
			value = EditorGUI.Slider(cellRect, GUIContent.none, value, min, max);
		}

		protected void DrawValue(Rect cellRect, string value, bool selected, bool focused)
		{
			DefaultGUI.Label(cellRect, value, selected, focused);
		}
	}

}