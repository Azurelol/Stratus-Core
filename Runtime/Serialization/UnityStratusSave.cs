using System.Collections;
using UnityEngine;
using System;
using Stratus.Serialization;
using Stratus.IO;
using Stratus.Models.Saves;
using Stratus.Logging;

namespace Stratus
{
	public interface IUnityStratusSave : ISave
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
	/// <typeparam name="TData"></typeparam>
	[Serializable]
	public class UnityStratusSave<TData> : Save<TData>, ISerializationCallbackReceiver
		where TData : class, new()
	{
		#region Properties
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
		#endregion

		#region Constructors
		public UnityStratusSave(TData data) : base(data)
		{
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

		public override Result LoadDataAsync(Action onLoad)
		{
			if (!Application.isPlaying)
			{
				return new Result(false, "Cannot load data asynchronously outside of playmode...");
			}

			IEnumerator routine()
			{
				yield return new WaitForEndOfFrame();
				LoadData();
				onLoad?.Invoke();
			}

			StratusCoroutineRunner.Run(routine());
			return new Result(true, "Now loading data asynchronously...");
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
			base.OnAnySerialization(filePath);

			if (Application.isPlaying)
			{
				this.playtime += StratusTime.minutesSinceStartup;
			}
		}
	}
}