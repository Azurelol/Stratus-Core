using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;


namespace Stratus.Editor.Tests
{
	public class StratusSubclassInstancerTest
	{
		public abstract class A { }

		public class B : A { }
		public class B1 : B { }
		public abstract class B2 : B { }

		public class C : A { }
		public class D { }

		[Test]
		public void TestSubclassInstancer()
		{
			var instancer = new StratusSubclassInstancer<A>();
			Assert.Null(instancer.Get<A>());
			Assert.NotNull(instancer.Get<B>());
			Assert.NotNull(instancer.Get<B1>());
			Assert.Null(instancer.Get<B2>());
			Assert.NotNull(instancer.Get<C>());
		}
	}
}