using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.TestTools;
using NUnit.Framework;

namespace Stratus
{
	public class StratusSaveSystemTests
	{
		internal class MockSaveData
		{
			public int a;
			public string b;
			public bool c;
		}

		internal class MockSave : StratusSave<MockSaveData>
		{
			public MockSave()
			{
			}

			public MockSave(MockSaveData data) : base(data)
			{
			}

			public override void OnAfterDeserialize()
			{
			}

			public override void OnBeforeSerialize()
			{
			}
		}
		
		internal class MockSaveSystem : StratusSaveSystem<MockSave, StratusJSONSerializer<MockSave>>
		{
			public MockSaveSystem(StratusSaveSystemConfiguration configuration) : base(configuration)
			{
			}
		}

		private MockSaveData data = new MockSaveData();

		[SetUp]
		public void Setup()
		{
		}

		[SetUp]
		public void TearDown()
		{
		}

		private MockSaveSystem GetDefault(bool createDirectoryPerSave)
		{
			var namingConvention = new StratusIncrementalFileNamingConvention("SAVE");
			var format = new StratusSaveDefaultFormat(createDirectoryPerSave);
			var configuration = new StratusSaveSystemConfiguration(format, namingConvention);
			configuration.debug = true;
			configuration.folder = "MockData";
			return new MockSaveSystem(configuration);
		}

		[Test]
		public void CreatesSaveWithDefaultFormat()
		{
			var saveSystem = GetDefault(false);

			int a = 7;
			data.a = a;
			var save = new MockSave(data);

			Assert.True(saveSystem.Save(save));
			Assert.True(save.serialized);

			string expectedPath = StratusIO.CombinePath(saveSystem.saveDirectoryPath, 
				StratusIO.ChangeExtension(save.file.name, saveSystem.configuration.format.extension));
			Assert.True(StratusIO.FileExists(expectedPath), $"No save file at {expectedPath}");

			var saveAgain = saveSystem.Load(save.file.name);
			Assert.NotNull(saveAgain);
			Assert.False(saveAgain.dataLoaded);
			Assert.True(saveAgain.LoadData());
			Assert.AreEqual(a, saveAgain.data.a);
		}

		[Test]
		public void CreatesSaveWitSnapshot()
		{
			var saveSystem = GetDefault(false);
			var save = new MockSave(data);
			save.SetSnapshot(Texture2D.whiteTexture);
			Assert.True(saveSystem.Save(save));
			Assert.True(save.serialized);

			string expectedPath = StratusIO.CombinePath(saveSystem.saveDirectoryPath,
				StratusIO.GetFileName(save.snapshotFilePath));
			Assert.True(StratusIO.FileExists(expectedPath));
		}

		[Test]
		public void CreatesSaveOnDirectory()
		{
			var saveSystem = GetDefault(true);
			saveSystem.configuration.format = new StratusSaveDefaultFormat(true);

			var save = new MockSave(data);
			Assert.True(saveSystem.Save(save));
			Assert.True(save.serialized);

			string expectedPath = StratusIO.CombinePath(saveSystem.saveDirectoryPath, 
				StratusIO.RemoveExtension(save.file.name),
				StratusIO.ChangeExtension(save.file.name, saveSystem.configuration.format.extension));
			Assert.True(StratusIO.FileExists(expectedPath));
		}

		[TestCase("SAV_001.sav", 1)]
		[TestCase("SAV99_001.dat", 1)]
		[TestCase("SAVE45", 45)]
		[TestCase("SAVE45.save", 45)]
		[TestCase("SAVE34_44_45.save", 45)]
		[TestCase("SAVE_3_4_1999_7.save", 7)]
		public void ParsesIndexFromFileName(string fileName, int expected)
		{
			int actual = StratusIncrementalFileNamingConvention.ParseIndex(fileName);
			Assert.AreEqual(expected, actual);
		}

		[TestCase("SAV_001.sav", 2)]
		[TestCase("SAV_10.sav", 11)]
		public void IncrementIndexFromFileName(string fileName, int expected)
		{
			int actual = StratusIncrementalFileNamingConvention.ParseIndex(fileName);
			Assert.AreEqual(expected, actual + 1);
		}
	}

}