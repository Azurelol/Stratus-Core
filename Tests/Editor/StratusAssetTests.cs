using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Drawing.Printing;

namespace Stratus.Tests.Editor
{
	public partial class StratusAssetTests
	{
		public class MockAsset
		{
			public string name;
			public string value;

			public MockAsset(string name, string value)
			{
				this.name = name;
				this.value = value;
			}
		}

		public class 

		public class MockAssetSource
		{
		}

		private class MockObject
		{
			public string name;
			[StratusAssetSource(sourceTypes = typeof(MockAssetSource))]
			public StratusAssetAlias<MockAsset> asset;
		}


		public void DoesThing()
		{
			MockAsset a = new MockAsset("a", "foobar");

			MockObject obj = new MockObject();
			obj.asset

		}
	}
}