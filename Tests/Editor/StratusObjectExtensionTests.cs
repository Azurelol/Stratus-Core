using NUnit.Framework;

using Stratus.Extensions;

using System;

namespace Stratus.Editor.Tests
{
	public class StratusObjectExtensionTests
	{
		[Serializable]
		private class MockObject
		{
			public int a;
			public bool b;
			public string c;
		}

		[Test]
		public void GetsAttribute()
		{
			MockObject obj = new MockObject();
			var attribute = obj.GetAttribute<SerializableAttribute>();
			Assert.NotNull(attribute);
			Assert.AreEqual(typeof(SerializableAttribute), attribute.GetType());
		}

		[Test]
		public void ClonesObjectWithJSON()
		{
			MockObject obj = new MockObject();
			obj.a = 5;
			obj.b = true;
			obj.c = "woo";

			MockObject clone = obj.CloneJSON();
			Assert.AreNotEqual(obj, clone);
			Assert.AreEqual(obj.a, clone.a);
			Assert.AreEqual(obj.b, clone.b);
			Assert.AreEqual(obj.c, clone.c);
		}

		[Test]
		public void ClonesObject()
		{
			MockObject obj = new MockObject();
			obj.a = 5;
			obj.b = true;
			obj.c = "woo";

			MockObject clone = obj.Clone();
			Assert.AreNotEqual(obj, clone);
			Assert.AreEqual(obj.a, clone.a);
			Assert.AreEqual(obj.b, clone.b);
			Assert.AreEqual(obj.c, clone.c);
		}

		[Test]
		public void OverwritesObjectWithJSON()
		{
			MockObject obj = new MockObject();
			obj.a = 5;
			obj.b = true;
			obj.c = "woo";

			MockObject target = new MockObject();
			target.OverwriteJSON(obj);
			Assert.AreEqual(obj.a, target.a);
			Assert.AreEqual(obj.b, target.b);
			Assert.AreEqual(obj.c, target.c);
		}
	}

}