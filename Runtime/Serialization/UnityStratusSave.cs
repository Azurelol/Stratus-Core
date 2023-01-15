using System.Collections;
using UnityEngine;
using System;
using Stratus.Serialization;
using Stratus.IO;

namespace Stratus
{
	public interface IUnityStratusSave : IStratusSave
	{
		Texture2D snapshot { get; }
		bool LoadSnapshot();
		bool SetSnapshot(Texture2D snapshot);
	}

	/// <summary>
	/// Base class for saves that embed other data.
	/// By default, whenever this is serialized, so is the data
	/// When this is deserialized, the data is NOT loaded by default.
	/// This is due to the save itself being a manifest of sorts for the much larger data file.
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	[Serializable]
	public class UnityStratusSave<DataType> : StratusSave, ISerializationCallbackReceiver
		where DataType : class, new()
	{
		#region Properties
		/// <summary>
		/// The data for this save
		/// </summary>
		public DataType data { get; private set; }

		/// <summary>
		/// A saved snapshot
		/// </summary>
		public Texture2D snapshot { get; private set; }

		/// <summary>
		/// The extension for the snapshot file
		/// </summary>
		public virtual StratusImageEncoding snapshotEncoding => StratusImageEncoding.JPG;

		/// <summary>
		/// Whether a snapshot of this save has been loaded
		/// </summary>
		public bool snapshotLoaded => snapshot != null;

		/// <summary>
		/// The data serializer
		/// </summary>
		public static readonly StratusJSONSerializer<DataType> dataSerializer = new StratusJSONSerializer<DataType>();

		/// <summary>
		/// The file path for where the encapsulated data is saved to
		/// </summary>
		public string dataFilePath
		{
			get
			{
				if (_dataFilePath == null)
				{
					_dataFilePath = FileUtility.ChangeExtension(file.path, dataExtension);
				}
				return _dataFilePath;
			}
		}
		private string _dataFilePath;

		/// <summary>
		/// The path for the snapshot image file taken for this save
		/// </summary>
		public string snapshotFilePath
		{
			get
			{
				if (_snapshotFilePath == null)
				{
					_snapshotFilePath = FileUtility.ChangeExtension(file.path, snapshotEncoding.ToExtension());
				}
				return _snapshotFilePath;
			}
		}
		private string _snapshotFilePath;

		/// <summary>
		/// Whether an associated snapshot file is found for this save
		/// </summary>
		public bool snapshotExists => FileUtility.FileExists(snapshotFilePath);

		/// <summary>
		/// Whether a data file exists for this save
		/// </summary>
		public bool dataFileExists => FileUtility.FileExists(dataFilePath);

		/// <summary>
		/// The extension used for save data
		/// </summary>
		public virtual string dataExtension => ".savedata";

		/// <summary>
		/// Whether the data for the save is loaded
		/// </summary>
		public bool dataLoaded => data != null;

		public override bool loaded => dataLoaded;
		#endregion

		#region Constructors
		public UnityStratusSave(DataType data)
		{
			this.data = data;
		}

		public UnityStratusSave()
		{
		}
		#endregion

		#region Overrides
		public override void OnAfterSerialize()
		{
			base.OnAfterSerialize();
			if (snapshot != null)
			{
				SaveSnapshot();
			}
			SaveData();
		}

		protected override void OnDelete()
		{
			if (dataFileExists)
			{
				FileUtility.DeleteFile(dataFilePath);
				_dataFilePath = null;
			}
		}

		public override StratusOperationResult Load()
		{
			return LoadData();
		}

		public override StratusOperationResult LoadAsync(Action onLoad)
		{
			return LoadDataAsync(onLoad);
		}

		public override void Unload()
		{
			base.Unload();
			UnloadSnapshot();
			UnloadData();
		}

		public override bool DeleteSerialization()
		{
			bool delete = base.DeleteSerialization();
			if (snapshotLoaded || snapshotExists)
			{
				FileUtility.DeleteFile(snapshotFilePath);
				_snapshotFilePath = null;
			}
			return delete;
		}

		public override void OnAfterDeserialize()
		{
		}

		public override void OnBeforeSerialize()
		{
		}
		#endregion

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void ResetData()
		{
			SetData(new DataType());
		}

		public void SetData(DataType data)
		{
			this.data = data;
		}

		public StratusOperationResult LoadData()
		{
			if (dataLoaded)
			{
				return new StratusOperationResult(true, "Data already loaded");
			}

			if (!serialized)
			{
				return new StratusOperationResult(false, "Cannot load data before the save has been serialized");
			}

			try
			{
				data = dataSerializer.Deserialize(dataFilePath);
			}
			catch (Exception e)
			{
				return new StratusOperationResult(false, e.ToString());
			}

			if (data == null)
			{
				return new StratusOperationResult(false, $"Failed to deserialize data from {dataFilePath}");
			}

			return new StratusOperationResult(true, $"Loaded data file from {dataFilePath}");
		}

		public StratusOperationResult LoadDataAsync(Action onLoad)
		{
			if (!Application.isPlaying)
			{
				return new StratusOperationResult(false, "Cannot load data asynchronously outside of playmode...");
			}

			IEnumerator routine()
			{
				yield return new WaitForEndOfFrame();
				LoadData();
				onLoad?.Invoke();
			}

			StratusCoroutineRunner.Run(routine());
			return new StratusOperationResult(true, "Now loading data asynchronously...");
		}

		public void UnloadData()
		{
			data = null;
		}

		public bool SaveData()
		{
			if (!serialized)
			{
				this.LogError("Cannot load data before the save has been serialized");
				return false;
			}

			if (data == null)
			{
				this.LogError("No data to serialize! This could mean that this save was created yet no data previously assigned to it");
				return false;
			}

			this.Log($"Saving data to {dataFilePath}");
			dataSerializer.Serialize(data, dataFilePath);
			return true;
		}

		/// <summary>
		/// Loads the snapshot associated with this save, if present
		/// </summary>
		/// <returns></returns>
		public bool LoadSnapshot()
		{
			if (snapshotLoaded)
			{
				return true;
			}

			if (!snapshotExists)
			{
				return false;
			}

			snapshot = StratusIO.LoadImage2D(snapshotFilePath);
			return true;
		}

		/// <summary>
		/// Sets the snapshot to be associated with this save
		/// </summary>
		/// <returns></returns>
		public bool SetSnapshot(Texture2D snapshot)
		{
			if (snapshot == null)
			{
				this.LogError("No valid snapshot texture given");
				return false;
			}
			this.snapshot = snapshot;
			return true;
		}

		/// <summary>
		/// Saves the current snapshot, if there's one
		/// and this save has been already serialized
		/// </summary>
		/// <returns></returns>
		public bool SaveSnapshot()
		{
			if (snapshot == null)
			{
				this.LogError("No valid snapshot texture assigned");
				return false;
			}
			if (!serialized)
			{
				this.LogError("This save has not yet been serialized. Cannot save snapshot yet");
				return false;
			}
			return StratusIO.SaveImage2D(snapshot, snapshotFilePath, snapshotEncoding);
		}

		/// <summary>
		/// Unloads the snapshot associated with this save, if present
		/// </summary>
		/// <returns></returns>
		public void UnloadSnapshot()
		{
			UnityEngine.Object.Destroy(snapshot);
			snapshot = null;
		}

		/// <summary>
		/// Invoked whenever this save is serialized/deserialized
		/// </summary>
		/// <param name="filePath"></param>
		public override void OnAnySerialization(string filePath)
		{
			this.file = new StratusSaveFileInfo(filePath);

			if (Application.isPlaying)
			{
				this.playtime += StratusTime.minutesSinceStartup;
			}
		}
	}
}