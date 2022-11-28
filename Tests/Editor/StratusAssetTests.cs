using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Drawing.Printing;
using Stratus.Utilities;
using Stratus.Editor.Tests;

namespace Stratus.Tests.Editor
{
	public partial class StratusAssetTests : StratusTest
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

		public class MockAssetReference : StratusAssetReference<MockAsset>
		{
		}

		public class MockAssetSource : CustomStratusAssetSource<MockAsset>
		{
			internal static MockAsset a = new MockAsset("a", "foobar");

			protected override IEnumerable<MockAsset> Generate()
			{
				yield return a;
			}
		}

		private class MockObject
		{
			public string name;
			[StratusAssetSource(sourceTypes = typeof(MockAssetSource))]
			public MockAssetReference value1 = new MockAssetReference();
		}

		[Test]
		public void AddsAssetToFile()
		{
			MockAsset a = new MockAsset("a", "foobar");

			StratusAssetCollection<MockAsset> collection = new StratusAssetCollection<MockAsset>();
			collection.Add(a);

			StratusFile<MockAsset> file = new StratusFile<MockAsset>();
			file.AtTemporaryPath().WithJSON();
			file.Serialize(a);

			var deserialization = file.Deserialize();
			Assert.True(deserialization);
			MockAsset aDeserialized = deserialization.result;
			AssertEqualFields(a, aDeserialized);
		}

		[Test]
		public void GetsAssetsFromSources()
		{
			MockObject obj = new MockObject();
			obj.value1.Set("a");
			Assert.AreEqual(MockAssetSource.a, obj.value1.asset);
		}
	}
}