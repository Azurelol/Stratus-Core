using System;
using System.Collections.Generic;
using System.Reflection;

using Stratus.Events;
using Stratus.Extensions;
using Stratus.Models;
using Stratus.Models.Graph;
using Stratus.Types;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Editor
{
	/// <summary>
	/// Displays all present derived Stratus events in the assembly
	/// </summary>
	public class StratusEventBrowserWindow : StratusEditorWindow<StratusEventBrowserWindow>
	{
		//------------------------------------------------------------------------/
		// Tree View
		//------------------------------------------------------------------------/
		/// <summary>
		/// Basic information about an event
		/// </summary>
		public class EventInformation : IStratusNamed
		{
			public string @namespace;
			public string @class;
			public string name;
			public string members;
			private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			string IStratusNamed.name => this.name;

			public EventInformation(System.Type type)
			{
				this.name = type.Name;
				this.@class = type.DeclaringType != null ? type.DeclaringType.Name : string.Empty;
				this.@namespace = type.Namespace;
				List<string> members = new List<string>();
				members.AddRange(type.GetFields(bindingFlags).ToStringArray((FieldInfo member) => $"({member.FieldType.Name}) {member.Name}"));
				members.AddRange(type.GetProperties(bindingFlags).ToStringArray((PropertyInfo member) => $"({member.PropertyType.Name}) {member.Name}"));
				this.members = string.Join(", ", members);
			}

		}

		public class EventTreeElement : TreeElement<EventInformation>
		{
		}

		public enum Columns
		{
			Namespace,
			Class,
			Name,
			Members,
		}

		public class EventTreeView : StratusMultiColumnTreeView<EventTreeElement, Columns>
		{
			public EventTreeView(TreeViewState state, IList<EventTreeElement> data) : base(state, data)
			{
			}

			protected override TreeViewColumn BuildColumn(Columns columnType)
			{
				TreeViewColumn column = null;
				switch (columnType)
				{
					case Columns.Namespace:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Namespace"),
							minWidth = 200,
							width = 175,
							autoResize = true,
							sortedAscending = true,
							selectorFunction = (StratusTreeViewItem<EventTreeElement> element) => element.element.data.@namespace
						};
						break;
					case Columns.Class:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Class"),
							sortedAscending = true,
							minWidth = 200,
							width = 200,
							autoResize = true,
							selectorFunction = (StratusTreeViewItem<EventTreeElement> element) => element.element.data.members
						};
						break;
					case Columns.Name:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Name"),
							sortedAscending = true,
							minWidth = 200,
							width = 200,
							autoResize = true,
							selectorFunction = (StratusTreeViewItem<EventTreeElement> element) => element.element.data.name
						};
						break;

					case Columns.Members:
						column = new TreeViewColumn
						{
							headerContent = new GUIContent("Members"),
							sortedAscending = true,
							minWidth = 400,
							width = 450,
							autoResize = true,
							selectorFunction = (StratusTreeViewItem<EventTreeElement> element) => element.element.data.members
						};
						break;
				}
				return column;
			}

			protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<EventTreeElement> item, Columns column, ref RowGUIArgs args)
			{
				switch (column)
				{
					case Columns.Name:
						DefaultGUI.Label(cellRect, item.element.data.name, args.selected, args.focused);
						break;
					case Columns.Class:
						DefaultGUI.Label(cellRect, item.element.data.@class, args.selected, args.focused);
						break;
					case Columns.Members:
						DefaultGUI.Label(cellRect, item.element.data.members, args.selected, args.focused);
						break;
					case Columns.Namespace:
						DefaultGUI.Label(cellRect, item.element.data.@namespace, args.selected, args.focused);
						break;
				}
			}

			protected override Columns GetColumn(int index)
			{
				return (Columns)index;
			}

			protected override int GetColumnIndex(Columns columnType)
			{
				return (int)columnType;
			}

			protected override void OnContextMenu(GenericMenu menu)
			{

			}

			protected override void OnItemContextMenu(GenericMenu menu, EventTreeElement treeElement)
			{
				menu.AddItem(new GUIContent("Open file"), false, () =>
				{

				});
			}

			protected override void OnItemDoubleClicked(EventTreeElement element)
			{
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private TreeViewState treeViewState = new TreeViewState();
		[SerializeField]
		private EventTreeView treeView;
		[SerializeField]
		private StratusSerializedTree<EventTreeElement, EventInformation> tree;

		private Type[] events;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			this.treeView = new EventTreeView(this.treeViewState, this.BuildEventTree());
		}

		protected override void OnWindowGUI()
		{
			this.treeView.TreeViewGUI(this.positionToGUI);
		}

		[MenuItem(StratusCore.rootMenu + "Event Browser")]
		private static void Open()
		{
			OpenWindow("Event Browser");
		}

		//------------------------------------------------------------------------/
		// Data
		//------------------------------------------------------------------------/
		private IList<EventTreeElement> BuildEventTree()
		{
			this.events = TypeUtility.SubclassesOf<Events.Event>();
			EventInformation[] eventsInformation = new EventInformation[this.events.Length];
			for (int i = 0; i < this.events.Length; ++i)
			{
				eventsInformation[i] = new EventInformation(this.events[i]);
			}

			tree = new StratusSerializedTree<EventTreeElement, EventInformation>(eventsInformation);
			return tree.elements;
		}

	}

}