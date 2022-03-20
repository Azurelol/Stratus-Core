using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using Stratus.Utilities;
using System.Linq;
using Stratus.Serialization;

namespace Stratus
{
	public interface IStratusSaveSystem
	{
		void RefreshSaveFiles();
		void ClearSaveFiles();
		void LoadAllSaves(bool force = false);
	}

	public interface IStratusSaveSystem<SaveType> : IStratusSaveSystem
		where SaveType : StratusSave, new()
	{
		SaveType[] saves { get; }
		SaveType CreateSave(Action<SaveType> onCreated = null);
		StratusOperationResult SaveAs(SaveType save, string fileName);
		StratusOperationResult Save(SaveType save);
		StratusOperationResult SaveAsync(SaveType save, Action onFinished);
		SaveType Load(StratusSaveFileInfo file);
		SaveType GetSaveAtIndex(int index);
	}

	public enum StratusSaveType
	{
		/// <summary>
		/// Manual saves triggered by the player
		/// </summary>
		Manual,
		/// <summary>
		/// Automatic saves triggered by the game
		/// </summary>
		Auto,
		/// <summary>
		/// Saves usually triggered by a hotkey
		/// </summary>
		Quick
	}

	/// <summary>
	/// Configurable attributes for a save system
	/// </summary>
	public class StratusSaveSystemConfiguration
	{
		/// <summary>
		/// Whether the save data system is being debugged
		/// </summary>
		public bool debug { get; set; }

		/// <summary>
		/// If assigned, will store saves within this folder rather than the root
		/// of <see cref="StratusSaveSystem.rootSaveDirectoryPath"/>
		/// </summary>
		public string folder { get; set; }
		/// <summary>
		/// The save format
		/// </summary>
		public StratusSaveFormat format
		{
			get => _format;
			set
			{
				_format = value;
				onChanged?.Invoke();
			}
		}
		private StratusSaveFormat _format;
		/// <summary>
		/// What naming convention to use for a save file of this type
		/// </summary>
		public StratusFileNamingConvention namingConvention { get; set; }
		/// <summary>
		/// The maximum amount of saves allowed. If 0, the saves are unlimited.
		/// </summary>
		public int saveLimit = 1000;

		/// <summary>
		/// The default save extension
		/// </summary>
		public const string defaultSaveExtension = ".save";

		public event Action onChanged;

		public StratusSaveSystemConfiguration(StratusSaveFormat format, StratusFileNamingConvention namingConvention)
		{
			this.format = format;
			this.namingConvention = namingConvention;
		}

		public string GenerateSaveFilePath(string path, StratusSaveFileQuery files)
		{
			return format.GenerateSaveFilePath(path, namingConvention.GenerateFileName(files));
		}
	}

	/// <summary>
	/// File information about a save
	/// </summary>
	public class StratusSaveFileInfo
	{
		public string path { get; private set; }
		public string name { get; private set; }
		public string directoryPath { get; private set; }

		public bool valid => path.IsValid();

		public StratusSaveFileInfo(string filePath)
		{
			this.path = filePath;
			this.name = StratusIO.GetFileName(filePath);
		}

		public bool Delete()
		{
			if (directoryPath.IsValid())
			{
				return StratusIO.DeleteDirectory(directoryPath);
			}
			return StratusIO.DeleteFile(path);
		}

		public override string ToString()
		{
			return path;
		}
	}

	public class StratusSaveFileQuery : StratusAssetQuery<StratusSaveFileInfo>
	{
		public StratusSaveFileQuery(Func<IList<StratusSaveFileInfo>> getAssetsFunction, Func<StratusSaveFileInfo, string> keyFunction) : base(getAssetsFunction, keyFunction)
		{
		}
	}

	public abstract class StratusSaveSystem : IStratusLogger
	{
		/// <summary>
		/// The persistent data path that Unity is using,
		/// For example in Windows it usually is 'Users/$USER/AppData/LocalLow/$COMPANY_NAME/$APPLICATION_NAME'.
		/// </summary>
		public static string rootSaveDirectoryPath => Application.persistentDataPath;
	}

	/// <summary>
	/// An abstract class for handling runtime save-data. Useful for player profiles, etc...
	/// </summary>
	public class StratusSaveSystem<SaveType, SerializerType> : StratusSaveSystem,
		 IStratusSaveSystem<SaveType>
		where SaveType : StratusSave, new()
		where SerializerType : StratusSerializer<SaveType>, new()
	{
		#region Properties
		/// <summary>
		/// Serializer used for saves
		/// </summary>
		private static readonly SerializerType serializer = new SerializerType();

		/// <summary>
		/// The attribute containing data about this class
		/// </summary>
		public StratusSaveSystemConfiguration configuration { get; private set; }

		/// <summary>
		/// The root path to the directory being used by this save data
		/// </summary>
		public string saveDirectoryPath => StratusIO.CombinePath(StratusSaveSystem.rootSaveDirectoryPath, configuration.folder);

		/// <summary>
		/// Returns all instances of the save data from the path
		/// </summary>
		public StratusSaveFileQuery saveFiles
		{
			get
			{
				if (_saveFiles == null)
				{
					RefreshSaveFiles();
				}
				return _saveFiles;
			}
		}
		private StratusSaveFileQuery _saveFiles;

		/// <summary>
		/// Loaded saves
		/// </summary>
		public SaveType[] saves { get; private set; }

		/// <summary>
		/// Whether saves have been already loaded
		/// </summary>
		public bool savesLoaded => saves != null;

		/// <summary>
		/// The number of save files present in the specified folder for save data
		/// </summary>
		public int saveCount => saveFiles != null ? saveFiles.assetCount : 0;

		/// <summary>
		/// The currently loaded save
		/// </summary>
		public SaveType latestSave { get; private set; }

		/// <summary>
		/// If there's unlimited saving allowed
		/// </summary>
		public bool unlimitedSaves => configuration.saveLimit == 0;
		#endregion

		#region Events
		public event Action<SaveType> onCreateSave;
		public event Action<SaveType> onBeforeSerialize;
		public event Action<SaveType> onSaveAsyncStarted;
		public event Action<SaveType> onSaveAsyncEnded;
		#endregion

		#region Virtual
		protected virtual void OnCreateSave(SaveType save) { }
		protected virtual void OnBeforeSave(SaveType save) { }
		#endregion

		#region Messages
		public StratusSaveSystem(StratusSaveSystemConfiguration configuration)
		{
			this.configuration = configuration;
			this._saveFiles = new StratusSaveFileQuery(GetSaveFiles, x => x.name);
			this.configuration.onChanged += RefreshSaveFiles;
		}
		#endregion

		/// <summary>
		/// Searches for all recognized save files in the default path
		/// </summary>
		public void RefreshSaveFiles()
		{
			if (!Directory.Exists(saveDirectoryPath))
			{
				return;
			}

			_saveFiles.Update();
		}

		private IList<StratusSaveFileInfo> GetSaveFiles()
		{
			StratusSaveFileInfo[] files = configuration.format.GetSaveFiles(saveDirectoryPath).ToArray();
			if (configuration.debug)
			{
				if (files.Length > 0)
				{
					this.Log($"Found {files.Length} save files at {saveDirectoryPath}!");
				}
				else
				{
					this.Log($"Found no save files at {saveDirectoryPath}...");
				}
			}
			return files;
		}

		/// <summary>
		/// Loads all save files, building a dictionary of them
		/// by their save name
		/// </summary>
		public void LoadAllSaves(bool force = false)
		{
			if (savesLoaded && !force)
			{
				return;
			}

			List<SaveType> _saves = new List<SaveType>();
			foreach (var file in saveFiles.assets)
			{
				SaveType save = Load(file);
				if (save != null)
				{
					_saves.Add(save);
				}
				else
				{
					this.LogError($"Failed to load save {file}");
				}
			}
			saves = _saves.ToArray();

			if (configuration.debug)
			{
				this.Log($"Loaded {saves.Length} saves...");
			}
		}

		/// <summary>
		/// Clears all found save files (searches for them first)
		/// </summary>
		public void ClearSaveFiles()
		{
			RefreshSaveFiles();
			if (saveFiles.valid)
			{
				int saveFileCount = saveFiles.assetCount;
				foreach (var file in saveFiles.assets)
				{
					file.Delete();
				}
				if (configuration.debug)
				{
					this.Log($"Deleted {saveFileCount} save files!");
				}
			}
			else if (configuration.debug)
			{
				this.LogWarning("No save files were found to be deleted.");
			}
		}

		/// <summary>
		/// Reveals the default save file directory path
		/// </summary>
		public void RevealSaveFiles()
		{
			StratusIO.Open(saveDirectoryPath);
		}

		/// <summary>
		/// Returns the save at the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SaveType GetSaveAtIndex(int index)
		{
			if (!savesLoaded)
			{
				this.LogError("No saves loaded yet");
				return null;
			}
			return saves[index];
		}

		/// <summary>
		/// Creates a save with the given name on a generated file.
		/// This calls derived classes construction, so its best
		/// to create saves purely through this method
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SaveType CreateSave(Action<SaveType> onCreated = null)
		{
			SaveType save = new SaveType();
			OnCreateSave(save);
			onCreated?.Invoke(save);
			return save;
		}

		/// <summary>
		/// Saves the data to the default path in the application's persistent path
		/// using the specified name
		/// </summary>
		public StratusOperationResult SaveAs(SaveType save, string filePath)
		{
			// Create the directory if missing
			if (!Directory.Exists(saveDirectoryPath))
			{
				Directory.CreateDirectory(saveDirectoryPath);
			}

			// Invoke a function before saving
			onBeforeSerialize?.Invoke(save);
			OnBeforeSave(save);

			// Now serialize the save
			Serialize(save, filePath);
			if (configuration.debug)
			{
				StratusDebug.Log($"Saved {save} at path {filePath}");
			}

			latestSave = save;
			saveFiles.Add(save.file);
			return true;
		}

		/// <summary>
		/// Saves new data with a generated save name
		/// </summary>
		/// <param name="save"></param>
		public StratusOperationResult Save(SaveType save)
		{
			if (save.serialized)
			{
				return SaveAs(save, save.file.path);
			}

			// If not serialized, compose the save file path
			string filePath = configuration.GenerateSaveFilePath(saveDirectoryPath, saveFiles);
			return SaveAs(save, filePath);
		}

		/// <summary>
		/// Saves data asynchronously, at runtime
		/// </summary>
		/// <param name="save"></param>
		public StratusOperationResult SaveAsync(SaveType save, Action onFinished)
		{
			if (!Application.isPlaying)
			{
				return new StratusOperationResult(false, "Cannot save asynchronously outside of playmode...");
			}

			IEnumerator routine()
			{
				if (configuration.debug)
				{
					this.Log($"Asynchronous save on {save} started");
				}
				onSaveAsyncStarted?.Invoke(save);
				yield return new WaitForEndOfFrame();
				Save(save);
				onFinished?.Invoke();
				onSaveAsyncEnded?.Invoke(save);
				if (configuration.debug)
				{
					this.Log($"Asynchronous save on {save} ended");
				}
			}

			StratusCoroutineRunner.Run(routine());
			return true;
		}

		/// <summary>
		/// Loads a save data file with a given name from the given folder and returns it
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public SaveType Load(StratusSaveFileInfo file)
		{
			SaveType save = Deserialize(file.path);
			if (save == null)
			{
				this.LogError($"Failed to load save from {file}");
			}
			else
			{
				if (configuration.debug)
				{
					this.Log($"Loaded save {file}'");
				}
				latestSave = save;
			}
			return save;
		}

		/// <summary>
		/// Loads a save from a save file by the given name if it can be found
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public SaveType Load(string fileName)
		{
			if (!saveFiles.HasAsset(fileName))
			{
				return null;
			}
			return Load(saveFiles[fileName]);
		}

		/// <summary>
		/// Performs the serialization operation
		/// </summary>
		/// <param name="save"></param>
		/// <param name="filePath"></param>
		private void Serialize(SaveType save, string filePath)
		{
			StratusIO.EnsureDirectoryAt(filePath);
			// Update the time to save at
			save.date = DateTime.Now.ToString();
			// Call a customized function before writing to disk
			save.OnBeforeSerialize();
			// Write to disk
			serializer.Serialize(save, filePath);
			// Note that it has been saved
			save.OnAnySerialization(filePath);
			save.OnAfterSerialize();
		}

		/// <summary>
		/// Performs the deserialization operation
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>

		public SaveType Deserialize(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException("The file was not found!");

			SaveType saveData = serializer.Deserialize(filePath);
			saveData.OnAfterDeserialize();
			saveData.OnAnySerialization(filePath);
			return saveData;
		}
	}

}