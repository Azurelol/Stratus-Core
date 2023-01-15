using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Stratus.Extensions;
using Stratus.OdinSerializer;

using UnityEngine;

namespace Stratus
{
	public class StratusSerializedTree<TreeElementType> : ISerializationCallbackReceiver
		where TreeElementType : StratusTreeElement, new()
	{
		#region Fields
		[OdinSerialize]
		protected List<TreeElementType> _elements = new List<TreeElementType>();
		[SerializeField]
		protected int idCounter = 0;
		[NonSerialized]
		private TreeElementType _root;
		[SerializeField]
		protected int _maxDepth;
		#endregion

		#region Properties
		public TreeElementType root
		{
			get
			{
				if (!this.valid)
				{
					this.BuildRootFromElements();
				}

				return this._root;
			}
		}
		/// <summary>
		/// The elements of the tree (including the root node at index 0)
		/// </summary>
		public TreeElementType[] elements => _elements.ToArray();
		/// <summary>
		/// Whether the tree is currently valid
		/// </summary>
		private bool valid => this._root != null;
		/// <summary>
		/// Whether the tree has elements (not including the root node)
		/// </summary>
		public bool hasElements => Count > 0;
		/// <summary>
		/// The current max depth of the tree, that is the depth of its deepest node
		/// </summary>
		public int maxDepth => this._maxDepth;
		/// <summary>
		/// Whether the tree has a root element
		/// </summary>
		public bool hasRoot => this.hasElements && this._elements[0].depth == rootDepth;
		/// <summary>
		/// The number of elements in the tree, not including the root node.
		/// </summary>
		public int Count => _elements.Count - 1;
		#endregion

		#region Constants
		public const int rootDepth = -1;
		public const int defaultDepth = 0;
		#endregion

		#region Constructors
		public StratusSerializedTree(IEnumerable<TreeElementType> elements)
		{
			this._elements.AddRange(elements);
			BuildRootFromElements();
		}

		public StratusSerializedTree()
		{
			this.AddRoot();
		}
		#endregion

		private void BuildRootFromElements()
		{
			this._root = StratusTreeElement.ListToTree(this._elements);
		}

		#region Messages
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.BuildRootFromElements();
		}
		#endregion

		#region Internal
		private void AddRoot()
		{
			TreeElementType root = new TreeElementType
			{
				name = "Root",
				depth = -1,
				id = this.idCounter++
			};
			this._elements.Insert(0, root);
		}

		protected TreeElementType GetElement(int index)
		{
			return this._elements[index];
		}

		protected int FindIndex(TreeElementType element)
		{
			int index = this._elements.IndexOf(element);
			return index;
		}

		protected int FindLastChildIndex(TreeElementType element)
		{
			int index = this.FindIndex(element);
			int lastIndex = index + element.totalChildrenCount;
			return lastIndex;
		}

		protected TreeElementType[] FindChildren(TreeElementType element)
		{
			int index = this.FindIndex(element);
			return this._elements.GetRange(index, element.totalChildrenCount).ToArray();
		}

		protected int GenerateID()
		{
			return this.idCounter++;
		}

		/// <summary>
		/// Creates and adds the element to the tree
		/// </summary>
		protected TreeElementType CreateElement(int depth)
		{
			TreeElementType element = new TreeElementType();
			OnAddElement(element, depth, true);
			return element;
		}

		private void OnAddElement(TreeElementType element, int depth, bool generateID)
		{
			element.depth = depth;
			if (generateID)
			{
				element.id = GenerateID();
			}
			if (depth > this.maxDepth)
			{
				this._maxDepth = element.depth;
			}
			_elements.Add(element);
		}
		#endregion

		#region Interface
		public void AddElement(TreeElementType element, int depth = defaultDepth, bool generateID = true)
		{
			OnAddElement(element, depth, generateID);
		}

		public void RemoveElement(TreeElementType element)
		{
			// Remove all children first
			if (element.hasChildren)
			{
				foreach (StratusTreeElement child in element.allChildren)
				{
					this._elements.Remove((TreeElementType)child);
				}
			}

			this._elements.Remove(element);
		}

		public void RemoveElementExcludeChildren(TreeElementType element)
		{
			StratusTreeElement parent = element.parent != null ? element.parent : this.root;

			if (element.hasChildren)
			{
				this.Reparent(parent, element.children);
			}

			this._elements.Remove(element);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TreeElementType parentElement, int insertionIndex, List<TreeElementType> elements)
		{
			this.MoveElements(parentElement, insertionIndex, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(StratusTreeElement parentElement, params StratusTreeElement[] elements)
		{
			StratusTreeElement.Parent(parentElement, elements);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(StratusTreeElement parentElement, List<StratusTreeElement> elements)
		{
			StratusTreeElement.Parent(parentElement, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TreeElementType parentElement, int insertionIndex, params TreeElementType[] elements)
		{
			if (insertionIndex < 0)
			{
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");
			}

			// Invalid reparenting input
			if (parentElement == null)
			{
				return;
			}

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
			{
				insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);
			}

			// Remove draggedItems from their parents
			foreach (TreeElementType draggedItem in elements)
			{
				draggedItem.parent.children.Remove(draggedItem);  // remove from old parent
				draggedItem.parent = parentElement;         // set new parent
			}

			if (parentElement.children == null)
			{
				parentElement.children = new List<StratusTreeElement>();
			}

			// Insert dragged items under new parent
			parentElement.children.InsertRange(insertionIndex, elements);

			StratusTreeElement.UpdateDepthValues(this.root);
		}

		public void Iterate(Action<TreeElementType> action)
		{
			if (!this.valid)
			{
				this.BuildRootFromElements();
			}

			foreach (TreeElementType element in this._elements)
			{
				action(element);
			}
		}

		public void Assert()
		{
			StratusTreeElement.Assert(this._elements);
		}

		public void Repair()
		{
			if (!this.hasRoot)
			{
				this.AddRoot();
			}
			StratusTreeElement.UpdateDepthValues(this.root);
		}

		public Exception Validate()
		{
			Exception exception = StratusTreeElement.Validate(this._elements);
			return exception;
		}

		public void Clear()
		{
			this._elements.Clear();
			this.idCounter = 0;
			this.AddRoot();
		}
		#endregion
	}

	/// <summary>
	/// A tree with an element type that encapsulates a data type
	/// </summary>
	/// <typeparam name="TreeElementType"></typeparam>
	/// <typeparam name="DataType"></typeparam>
	[Serializable]
	public class StratusSerializedTree<TreeElementType, DataType>
		: StratusSerializedTree<TreeElementType>

	  where TreeElementType : StratusTreeElement<DataType>, new()
	  where DataType : class, IStratusNamed
	{
		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedTree(IEnumerable<TreeElementType> elements)
			: base(elements)
		{
		}

		public StratusSerializedTree(IEnumerable<DataType> values) : this()
		{
			AddElements(values, 0);
		}

		public StratusSerializedTree() : base()
		{
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		public void AddElement(DataType data)
		{
			this.CreateElement(data, defaultDepth);
		}

		public TreeElementType AddChildElement(DataType data, TreeElementType parent)
		{
			// Insert element below the last child
			TreeElementType element = this.CreateElement(data, parent.depth + 1);
			int insertionIndex = this.FindLastChildIndex(parent) + 1;
			this._elements.Insert(insertionIndex, element);
			return element;
		}

		public void AddParentElement(DataType data, TreeElementType element)
		{
			// Insert element below the last child
			TreeElementType parentElement = this.CreateElement(data, element.depth);
			element.depth++;
			parentElement.parent = element.parent;

			int insertionIndex = this.FindIndex(element);

			foreach (TreeElementType child in this.FindChildren(element))
			{
				child.depth++;
			}

			this._elements.Insert(insertionIndex, parentElement);
		}

		public void ReplaceElement(TreeElementType originalElement, DataType replacementData)
		{
			TreeElementType replacementElement = this.AddChildElement(replacementData, (TreeElementType)originalElement.parent);
			if (originalElement.hasChildren)
			{
				this.Reparent(replacementElement, originalElement.children);
			}

			this.RemoveElement(originalElement);
		}

		public void AddElements(IEnumerable<DataType> elementsData, int depth)
		{
			foreach (DataType data in elementsData)
			{
				this.CreateElement(data, depth);
			}
		}

		#region Internal
		/// <summary>
		/// Creates and adds the element to the tree
		/// </summary>
		private TreeElementType CreateElement(DataType data, int depth)
		{
			TreeElementType element = CreateElement(depth);
			element.Set(data);
			return element;
		}
		#endregion
	}

}