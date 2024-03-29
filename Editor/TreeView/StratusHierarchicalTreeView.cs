﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;
using UnityEditor;

namespace Stratus
{
	public abstract class StratusHierarchicalTreeView<TreeElementType> : StratusTreeViewWithTreeModel<TreeElementType>
	  where TreeElementType : StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusHierarchicalTreeView(TreeViewState state, StratusTreeModel<TreeElementType> model) : base(state, model)
		{
			this.InitializeHierarchicalTreeView();
		}

		public StratusHierarchicalTreeView(TreeViewState state, StratusProvider<IList<TreeElementType>> data) 
			: base(state, new StratusTreeModel<TreeElementType>(data))
		{
			this.InitializeHierarchicalTreeView();
		}

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected virtual void OnBeforeRow(Rect rect, StratusTreeViewItem<TreeElementType> treeViewItem) { }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void InitializeHierarchicalTreeView()
		{
			this.Reload();
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnMainGUI(Rect rect)
		{
			GUI.DrawTexture(rect, EditorStyles.toolbar.normal.background);
			base.OnMainGUI(rect);
		}

		protected override void DoubleClickedItem(int id)
		{
			this.SetExpanded(id, !this.IsExpanded(id));
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			this.OnBeforeRow(args.rowRect, (StratusTreeViewItem<TreeElementType>)args.item);
			base.RowGUI(args);
		}
	}
}
