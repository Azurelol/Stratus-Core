using System.Collections.Generic;
using System.IO;

namespace Stratus
{
	public abstract class StratusSaveFormat
	{
		/// <summary>
		/// The extension used by main save file (of type <see cref="StratusSave"/>)
		/// </summary>
		public string extension { get; set; }

		public abstract string ComposeSavePath(string path, string fileName);
		public abstract IEnumerable<StratusSaveFileInfo> GetSaveFiles(string path);


		protected StratusSaveFormat(string extension)
		{
			this.extension = extension;
		}

		/// <summary>
		/// Generates a save name for this save. By default it will be the time stamp
		/// </summary>
		/// <returns></returns>
		protected virtual string GenerateSaveName()
		{
			return StratusIO.GetTimestamp();
		}

		/// <summary>
		/// Saves the data to the default path in the application's persistent path
		/// using the specified name
		/// </summary>
		public string GenerateSaveFilePath(string path, string fileName)
		{
			string filePath = ComposeSavePath(path, fileName);
			return filePath;
		}
	}

	/// <summary>
	/// Save files are stored at uncompressed at given path with a given extension
	/// </summary>
	public class StratusSaveDefaultFormat : StratusSaveFormat
	{
		/// <summary>
		/// Whether to create subdirectories for each save
		/// </summary>
		public bool createDirectoryPerSave { get; set; }

		public StratusSaveDefaultFormat(bool createDirectoryPerSave = false, string extension = StratusSave.defaultExtension)
			: base(extension)
		{
			this.createDirectoryPerSave = createDirectoryPerSave;
		}

		public override string ComposeSavePath(string path, string fileName)
		{
			if (createDirectoryPerSave)
			{
				string subDirectory = StratusIO.RemoveExtension(fileName);
				return StratusIO.CombinePath(path, subDirectory, StratusIO.ChangeExtension(fileName, extension));
			}

			return StratusIO.CombinePath(path, StratusIO.ChangeExtension(fileName, extension));
		}

		public override IEnumerable<StratusSaveFileInfo> GetSaveFiles(string path)
		{
			IEnumerable<StratusSaveFileInfo> get(string directoryPath)
			{
				var files = Directory.GetFiles(directoryPath, $"*{extension}", SearchOption.TopDirectoryOnly);
				for (int i = 0; i < files.Length; i++)
				{
					string file = files[i];
					yield return new StratusSaveFileInfo(file);
				}
			}

			if (createDirectoryPerSave)
			{
				var directories = Directory.GetDirectories(path);
				foreach (var directoryPath in directories)
				{
					foreach(var file in get(directoryPath))
					{
						yield return file;
					}
				}
			}
			else
			{
				foreach (var file in get(path))
				{
					yield return file;
				}
			}

		}
	}

}