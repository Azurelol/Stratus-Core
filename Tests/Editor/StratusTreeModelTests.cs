using NUnit.Framework;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusTreeModelTests
	{
		[Test]
		public static void TreeModelCanAddElements()
		{
			var root = new StratusTreeElement { name = "Root", depth = -1 };
			var listOfElements = new List<StratusTreeElement>();
			listOfElements.Add(root);

			var model = new StratusTreeModel<StratusTreeElement>(listOfElements);
			// Element
			model.AddElement(new StratusTreeElement { name = "Element" }, root, 0);
			// Element 1
			model.AddElement(new StratusTreeElement { name = "Element " + root.totalChildrenCount }, root, 0);
			// Element 2
			model.AddElement(new StratusTreeElement { name = "Element " + root.totalChildrenCount }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Sub Element" }, root.children[1], 0);

			// Assert order is correct
			string[] namesInCorrectOrder = { "Root", "Element 2", "Element 1", "Sub Element", "Element" };
			Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
			for (int i = 0; i < namesInCorrectOrder.Length; ++i)
			{
				Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].name);
			}

			// Assert depths are valid
			StratusTreeElement.Assert(listOfElements);
		}

		[Test]
		public static void TreeModelCanRemoveElements()
		{
			var root = new StratusTreeElement { name = "Root", depth = -1 };
			var listOfElements = new List<StratusTreeElement>();
			listOfElements.Add(root);

			var model = new StratusTreeModel<StratusTreeElement>(listOfElements);
			model.AddElement(new StratusTreeElement { name = "Element" }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Sub Element" }, root.children[1], 0);

			model.RemoveElements(new[] { root.children[1].children[0], root.children[1] });

			// Assert order is correct
			string[] namesInCorrectOrder = { "Root", "Element 2", "Element" };
			Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
			for (int i = 0; i < namesInCorrectOrder.Length; ++i)
			{
				Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].name);
			}

			// Assert depths are valid
			StratusTreeElement.Assert(listOfElements);
		}
	}
}