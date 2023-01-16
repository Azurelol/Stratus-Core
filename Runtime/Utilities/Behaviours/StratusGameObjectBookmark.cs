using Stratus.Extensions;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	public class StratusGameObjectBookmark : StratusEditorBehaviour<StratusGameObjectBookmark>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Information about this GameObject
		/// </summary>    
		[SerializeField]
		private StratusGameObjectInformation _information;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusGameObjectInformation information => this._information;
		public static StratusGameObjectInformation[] availableInformation { get; private set; } = new StratusGameObjectInformation[0];
		public static bool hasAvailableInformation => availableInformation != null && availableInformation.Length > 0;
		public static System.Action onUpdate { get; set; } = new System.Action(() => { });

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnReset()
		{
			this._information = new StratusGameObjectInformation(this.gameObject);
		}

		protected override void OnStratusEditorBehaviourEnable()
		{
			if (this._information == null)
				this._information = new StratusGameObjectInformation(gameObject);
			StratusGameObjectBookmark.UpdateAvailable();
		}

		protected override void OnStratusEditorBehaviourDisable()
		{
			StratusGameObjectBookmark.UpdateAvailable();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static StratusGameObjectBookmark Add(GameObject gameObject)
		{
			StratusGameObjectBookmark bookmark = gameObject.AddComponent<StratusGameObjectBookmark>();
			return bookmark;
		}

		public static void Remove(GameObject gameObject)
		{
			gameObject.RemoveComponent<StratusGameObjectBookmark>();
		}

		public static void Toggle(GameObject gameObject)
		{
			if (gameObject.HasComponent<StratusGameObjectBookmark>())
			{
				gameObject.RemoveComponent<StratusGameObjectBookmark>();
			}
			else
			{
				gameObject.AddComponent<StratusGameObjectBookmark>();
			}
		}

		public void SetInformation(StratusGameObjectInformation information)
		{
			if (information.gameObject != this.gameObject)
				return;

			this._information = (StratusGameObjectInformation)information.CloneJSON();
			this._information.UpdateMemberReferences();
			StratusGameObjectBookmark.UpdateAvailable();
		}

		//------------------------------------------------------------------------/
		// Methods: Watch
		//------------------------------------------------------------------------/
		/// <summary>
		/// Updates the list of currently watched members
		/// </summary>
		public static void UpdateWatchList(bool invokeDelegate = false)
		{
			if (invokeDelegate)
			{
				StratusGameObjectBookmark.onUpdate();
			}
		}

		/// <summary>
		/// Updates the list of available information from enabled bookmarks
		/// </summary>
		private static void UpdateInformation(bool invokeDelegate = false)
		{
			List<StratusGameObjectInformation> availableInformation = new List<StratusGameObjectInformation>();
			foreach (var bookmark in StratusGameObjectBookmark.available)
			{
				availableInformation.Add(bookmark.Value.information);
			}
			StratusGameObjectBookmark.availableInformation = availableInformation.ToArray();

			if (invokeDelegate)
			{
				StratusGameObjectBookmark.onUpdate();
			}
		}

		/// <summary>
		/// Updates all available data for bookmarks
		/// </summary>
		public static void UpdateAvailable()
		{
			StratusGameObjectBookmark.UpdateInformation();
			StratusGameObjectBookmark.onUpdate();
		}

	}

}