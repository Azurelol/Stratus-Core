using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Dependencies.Ludiq.Reflection;
using System;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor.IMGUI.Controls;
using UnityEditor.AnimatedValues;
using UnityEditor.Callbacks;
using Stratus.Extensions;

namespace Stratus.Editor
{
	/// <summary>
	/// A window used for inspecting the members of an object at runtime
	/// </summary>
	public class StratusMemberInspectorWindow : StratusEditorWindow<StratusMemberInspectorWindow>
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/ 
		/// <summary>
		/// The current mode for this window
		/// </summary>
		public enum Mode
		{
			Inspector = 0,
			WatchList = 1,
		}

		/// <summary>
		/// How the current information is being stored
		/// </summary>
		public enum InformationMode
		{
			Temporary,
			Bookmark
		}

		public enum Column
		{
			Watch,
			GameObject,
			Component,
			Member,
			Type,
			Value,
		}



		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/   
		[SerializeField]
		private Mode mode = Mode.Inspector;
		[SerializeField]
		private GameObject target;
		[SerializeField]
		private StratusComponentMemberWatchList watchList = new StratusComponentMemberWatchList();
		[SerializeReference]
		private StratusGameObjectInformation _currentTargetInformation;
		/// <summary>
		/// How quickly <see cref="StratusMemberReference"/> values are updated
		/// </summary>
		[SerializeField]
		private float updateSpeed = defaultUpdateSpeed;
		[SerializeField]
		private StratusMemberInspectorTreeView inspectorTreeView;
		[SerializeField]
		private StratusMemberInspectorWatchListTreeView watchListTreeView;
		[SerializeField]
		private TreeViewState treeViewState;

		#region Constants
		public const float minimumUpdateSpeed = 0.2f;
		public const float defaultUpdateSpeed = 1f;
		public const float maximuUpdateSpeed = 0.2f;
		static readonly float[] rowWeights = new float[]
		{
			0.2f,
			0.8f
		};
		#endregion

		private StratusCountdown pollTimer;
		private const string displayName = "Watcher";
		private string[] toolbarOptions = StratusEnum.Names<Mode>();

		#region Properties
		public InformationMode informationMode { get; private set; }
		public StratusGameObjectInformation targetInformation
		{
			get => _currentTargetInformation;
			private set
			{
				_currentTargetInformation = value;
			}
		}

		private static readonly Type gameObjectType = typeof(GameObject);
		private bool hasTarget => this.target != null && this.targetInformation != null;
		private bool updateTreeView { get; set; }
		#endregion

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			if (this.treeViewState == null)
			{
				this.treeViewState = new TreeViewState();
			}

			if (this.watchList == null)
			{
				this.watchList = new StratusComponentMemberWatchList();
			}

			if (targetInformation == null && target != null)
			{
				this.CreateTargetInformation();
				this.SetTreeView();
			}

			ResetUpdateTimer();

			// Update tree view on assembly reload
			StratusGameObjectBookmark.onUpdate += this.OnBookmarkUpdate;
			StratusGameObjectInformation.onChanged += this.OnGameObjectInformationChanged;
		}

		private void ResetUpdateTimer()
		{
			this.pollTimer = new StratusCountdown(this.updateSpeed);
		}

		protected override void OnWindowGUI()
		{
			void drawToolbar()
			{
				EditorGUI.BeginChangeCheck();
				{
					this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));

				}
				if (EditorGUI.EndChangeCheck())
				{
					this.SetTreeView();
				}
			}

			void drawControls(Rect rect)
			{
				StratusEditorGUILayout.Aligned(drawToolbar, TextAlignment.Center);
				switch (this.mode)
				{
					case Mode.Inspector:
						this.DrawTargetSelector();
						break;
				}
			}

			void drawTreeView(Rect rect)
			{
				switch (this.mode)
				{
					case Mode.Inspector:
						//if (inspectorTreeView == null)
						//{
						//	SetTreeView();
						//}
						if (this.hasTarget)
						{
							this.inspectorTreeView?.TreeViewGUI(rect);
						}
						break;

					case Mode.WatchList:
						//if (watchListTreeView == null)
						//{
						//	SetTreeView();
						//}
						this.watchListTreeView?.TreeViewGUI(rect);
						break;
				}
			}

			Rect[] rows = positionToGUI.Column(rowWeights);
			rows.ForEach(drawControls, drawTreeView);
		}

		protected override void OnWindowUpdate()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			// Check whether values need to be updated
			pollTimer.Update(Time.deltaTime);
			if (pollTimer.isFinished)
			{
				switch (this.mode)
				{
					case Mode.Inspector:
						if (this.hasTarget)
						{
							this.targetInformation.UpdateValues(watchList);
						}
						break;
				}

				// Reset the poll timer
				pollTimer.Reset();
				this.Repaint();
			}
		}
		protected override void OnPlayModeStateChange(PlayModeStateChange stateChange)
		{
			switch (stateChange)
			{
				// Update the tree view only when entering 
				case PlayModeStateChange.EnteredPlayMode:
				case PlayModeStateChange.EnteredEditMode:
					updateTreeView = true;
					if (this.target)
					{
						this.targetInformation.Refresh();
						SetTreeView();
					}
					break;

				// Don't bother trying to update while exiting
				case PlayModeStateChange.ExitingEditMode:
					updateTreeView = false;
					break;
				case PlayModeStateChange.ExitingPlayMode:
					targetInformation.ClearValues();
					updateTreeView = false;
					break;
			}
		}

		#region Event Handlers
		private void OnBookmarkUpdate()
		{
			if (mode == Mode.Inspector)
			{
				return;
			}

			if (updateTreeView)
			{
				this.SetTreeView();
			}
		}

		private void OnGameObjectInformationChanged(StratusGameObjectInformation information, StratusOperationResult<StratusGameObjectInformation.Change> change)
		{
			this.Log($"Information changed for {information.gameObject.name}, change = {change.result}:\n{change.message}");
			this.SetTreeView();
		}
		#endregion

		#region Target Selection
		private void SelectTarget(GameObject target)
		{
			if (this.targetInformation != null && this.targetInformation.gameObject == target)
			{
				this.LogWarning($"The GameObject {target.name} is already the target");
				return;
			}

			this.target = target;
			this.targetInformation = null;
			this.inspectorTreeView = null;
			this.CreateTargetInformation();
		}

		/// <summary>
		/// If there's no target information or if the target is different from the previous,
		/// the current target information needs to be created
		/// </summary>
		private void CreateTargetInformation()
		{
			if (this.target == null)
			{
				return;
			}
			// If the target has as bookmark, use that information instead
			StratusGameObjectBookmark bookmark = this.target.GetComponent<StratusGameObjectBookmark>();
			if (bookmark != null)
			{
				this.informationMode = InformationMode.Bookmark;
				this.targetInformation = bookmark.information;
			}
			// Otherwise recreate the current target information
			else if (this.targetInformation == null || this.targetInformation.gameObject != this.target)
			{
				this.informationMode = InformationMode.Temporary;
				this.targetInformation = new StratusGameObjectInformation(this.target);
			}

			this.targetInformation.Refresh();
			this.SetTreeView();
		}

		/// <summary>
		/// Creates the tree view for the current target
		/// </summary>
		private void SetTreeView()
		{
			switch (this.mode)
			{
				case Mode.Inspector:
					{
						IList<StratusMemberInspectorTreeElement> members = null;
						if (this.hasTarget)
						{
							members = StratusMemberInspectorTreeElement.Generate(this.targetInformation);
						}
						if (this.inspectorTreeView == null)
						{
							this.inspectorTreeView = new StratusMemberInspectorTreeView(this.treeViewState, this.targetInformation, members, this.watchList);
						}
						else
						{
							this.inspectorTreeView.SetTree(new StratusProvider<IList<StratusMemberInspectorTreeElement>>(members));
						}

						this.inspectorTreeView.DisableColumn(Column.GameObject);
						this.inspectorTreeView.EnableColumn(Column.Watch);
						this.inspectorTreeView.SortByColumn(Column.Watch, false);
					}

					break;

				case Mode.WatchList:
					{
						if (this.watchListTreeView == null)
						{
							watchListTreeView = new StratusMemberInspectorWatchListTreeView(treeViewState, watchList);
						}
					}
					break;
			}
		}
		#endregion

		#region GUI
		private void DrawTargetSelector()
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Target", StratusGUIStyles.header);
			GameObject target = null;
			bool changed = StratusEditorUtility.CheckControlChange(() =>
			{
				target = (GameObject)EditorGUILayout.ObjectField(this.target, gameObjectType, true);
				StratusEditorUtility.OnLastControlMouseClick(null, () =>
				{
					bool hasBookmark = this.target.HasComponent<StratusGameObjectBookmark>();
					string bookmarkLabel = hasBookmark ? "Remove Bookmark" : "Bookmark";
					GenericMenu menu = new GenericMenu();

					// 1. Bookmark
					if (hasBookmark)
					{
						menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
						{
							RemoveBookmark(target);
						});
					}
					else
					{
						menu.AddItem(new GUIContent(bookmarkLabel), false, () =>
						{
							SetBookmark(target);
						});
					}

					// 2. Clear Watch List
					menu.AddItem(new GUIContent("Clear Watch List"), false, () =>
					{
						this.Log("Clearing watch list");
						this.watchList.Clear();
					});

					menu.ShowAsContext();
				});
			});

			if (changed)
			{
				SelectTarget(target);
			}
		}


		#endregion

		#region Static Methods
		[MenuItem(StratusCore.rootMenu + "Member Inspector")]
		private static void Open() => OpenWindow(displayName);

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			if (instance == null || instance.inspectorTreeView == null)
				return false;
			return instance.inspectorTreeView.TryOpenAsset(instanceID, line);
		}

		public static void Inspect(GameObject target)
		{
			Open();
			instance.SelectTarget(target);
		}

		public static void SetBookmark(GameObject target)
		{
			StratusGameObjectBookmark bookmark = StratusGameObjectBookmark.Add(target);
			bookmark.SetInformation(instance.targetInformation);
			instance.targetInformation = bookmark.information;
			instance.informationMode = InformationMode.Bookmark;
		}

		public static void RemoveBookmark(GameObject target)
		{
			StratusGameObjectBookmark.Remove(target);
			instance.informationMode = InformationMode.Temporary;
		}
		#endregion

	}
}