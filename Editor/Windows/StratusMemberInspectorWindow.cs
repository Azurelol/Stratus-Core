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
using Stratus;

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
		private Column watchColumn = Column.Component;
		[SerializeField]
		private GameObject target;
		[SerializeField]
		private StratusGameObjectInformation targetTemporaryInformation;
		[SerializeReference]
		private StratusGameObjectInformation _currentTargetInformation;
		/// <summary>
		/// How quickly <see cref="StratusMemberReference"/> values are updated
		/// </summary>
		[SerializeField]
		private float updateSpeed = defaultUpdateSpeed;
		[SerializeField]
		private StratusMemberInspectorTreeView memberInspector;
		[SerializeField]
		private TreeViewState treeViewState;

		#region Constants
		public const float minimumUpdateSpeed = 0.2f;
		public const float defaultUpdateSpeed = 1f;
		public const float maximuUpdateSpeed = 0.2f;
		static readonly float[] rowWeights = new float[]
		{
			0.2f,
			0.7f,
			0.1f
		};
		#endregion

		private StratusCountdown pollTimer;
		private const string displayName = "Watcher";
		private string[] toolbarOptions = StratusEnum.Names<Mode>();

		#region Properties
		public InformationMode informationMode { get; private set; }
		public StratusGameObjectInformation currentTargetInformation
		{
			get => _currentTargetInformation;
			private set
			{
				if (value == null)
				{
					this.Log("Clearing current target information");
				}
				this.Log($"Setting target information for {value}");
				_currentTargetInformation = value;
			}
		}

		private static readonly Type gameObjectType = typeof(GameObject);
		private bool hasTarget => this.currentTargetInformation != null;
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

			if (currentTargetInformation == null && target != null)
			{
				this.CreateTargetInformation();
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
			DrawMemberInspectorGUI();
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
							this.currentTargetInformation.UpdateWatchValues();
						}
						break;

					case Mode.WatchList:
						if (StratusGameObjectBookmark.hasAvailableInformation)
						{
							foreach (var targetInfo in StratusGameObjectBookmark.availableInformation)
							{
								//targetInfo.Refresh();
								targetInfo.UpdateWatchValues();
							}
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
						this.currentTargetInformation.Refresh();
						//this.CreateTargetInformation();
					}
					break;

				// Don't bother trying to update while exiting
				case PlayModeStateChange.ExitingEditMode:
					updateTreeView = false;
					break;
				case PlayModeStateChange.ExitingPlayMode:
					currentTargetInformation.ClearValues();
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
				this.SetTargetTreeView();
			}
		}

		private void OnGameObjectInformationChanged(StratusGameObjectInformation information, StratusOperationResult<StratusGameObjectInformation.Change> change)
		{
			this.Log($"Information changed for {information.gameObject.name}, change = {change.result}:\n{change.message}");
			if (change.result == StratusGameObjectInformation.Change.ComponentsAndWatchList)
			{
				StratusGameObjectBookmark.UpdateWatchList();
			}

			this.SetTargetTreeView();
		}
		#endregion

		#region Target Selection
		private void SelectTarget(GameObject target)
		{
			if (this.currentTargetInformation != null && this.currentTargetInformation.gameObject == target)
			{
				this.LogWarning($"The GameObject {target.name} is already the target");
				return;
			}

			this.target = target;
			this.currentTargetInformation = null;
			this.memberInspector = null;
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
				this.currentTargetInformation = bookmark.information;
				this.Log("Setting target from bookmark");
			}
			// Otherwise recreate the current target information
			else if (this.currentTargetInformation == null || this.currentTargetInformation.gameObject != this.target)
			{
				this.Log("Recreating current target information");
				this.informationMode = InformationMode.Temporary;
				this.targetTemporaryInformation = new StratusGameObjectInformation(this.target);
				this.currentTargetInformation = this.targetTemporaryInformation;
			}

			this.currentTargetInformation.Refresh();
			this.SetTargetTreeView();
		}

		/// <summary>
		/// Creates the tree view for the current target
		/// </summary>
		private void SetTargetTreeView()
		{
			IList<StratusMemberInspectorTreeElement> members = null;
			switch (this.mode)
			{
				case Mode.WatchList:
					members = StratusMemberInspectorTreeElement.GenerateFavoritesTree();
					break;

				case Mode.Inspector:
					if (this.hasTarget)
					{
						this.Log("Generating inspector tree elements from current state of the target");
						members = StratusMemberInspectorTreeElement.GenerateInspectorTree(this.currentTargetInformation);
					}
					else
						return;
					break;
			}

			if (this.memberInspector == null)
			{
				this.Log("Generating inspector tree");
				this.memberInspector = new StratusMemberInspectorTreeView(this.treeViewState, this.currentTargetInformation, members);
			}
			else
			{
				this.memberInspector.SetTree(members);
				this.memberInspector.onColumnSortedChanged += (col) =>
				{
					this.watchColumn = col;
				};
			}

			switch (this.mode)
			{
				case Mode.WatchList:
					this.memberInspector.EnableColumn(Column.GameObject);
					this.memberInspector.DisableColumn(Column.Watch);
					this.memberInspector.SortByColumn(watchColumn);
					break;

				case Mode.Inspector:
					this.memberInspector.DisableColumn(Column.GameObject);
					this.memberInspector.EnableColumn(Column.Watch);
					break;
			}
		}
		#endregion

		#region GUI
		private void DrawMemberInspectorGUI()
		{
			void drawToolbar()
			{
				// Toolbar      
				EditorGUI.BeginChangeCheck();
				{
					this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));

				}
				if (EditorGUI.EndChangeCheck() || this.memberInspector == null)
				{
					this.SetTargetTreeView();

				}
			}

			//void drawUpdateSlider()
			//{
			//	updateSpeed = EditorGUILayout.Slider("Update", updateSpeed, minimumUpdateSpeed, maximuUpdateSpeed);
			//	if (pollTimer.total != updateSpeed)
			//	{
			//		ResetUpdateTimer();
			//	}
			//}

			void drawControls(Rect rect)
			{
				StratusEditorGUILayout.Aligned(drawToolbar, TextAlignment.Center);
				//drawUpdateSlider();
				switch (this.mode)
				{
					case Mode.Inspector:
						this.DrawTargetSelector();
						break;
				}
			}

			void drawInspector(Rect rect)
			{
				switch (this.mode)
				{
					case Mode.Inspector:
						if (this.hasTarget)
						{
							this.memberInspector.TreeViewGUI(rect);
						}
						break;

					case Mode.WatchList:
						if (StratusGameObjectBookmark.hasWatchList)
						{
							this.memberInspector.TreeViewGUI(rect);
						}
						break;
				}
			}

			void drawUpdate(Rect rect)
			{
				GUILayout.BeginArea(rect);
				//rect = rect.Intend(4f);

				GUILayout.EndArea();
				//EditorGUI.ProgressBar(rect, pollTimer.normalizedProgress, "Update");
			}

			Rect[] rows = positionToGUI.Column(rowWeights);
			rows.ForEach(drawControls, drawInspector, drawUpdate);
		}

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
						this.currentTargetInformation.ClearWatchList();
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
			if (instance == null || instance.memberInspector == null)
				return false;
			return instance.memberInspector.TryOpenAsset(instanceID, line);
		}

		public static void Inspect(GameObject target)
		{
			Open();
			instance.SelectTarget(target);
		}

		public static void SetBookmark(GameObject target)
		{
			StratusGameObjectBookmark bookmark = StratusGameObjectBookmark.Add(target);
			bookmark.SetInformation(instance.currentTargetInformation);
			instance.currentTargetInformation = bookmark.information;
			instance.informationMode = InformationMode.Bookmark;
		}

		public static void RemoveBookmark(GameObject target)
		{
			instance.targetTemporaryInformation = (StratusGameObjectInformation)instance.currentTargetInformation.CloneJSON();
			instance.targetTemporaryInformation.ClearWatchList();
			instance.currentTargetInformation = instance.targetTemporaryInformation;
			StratusGameObjectBookmark.Remove(target);
			instance.informationMode = InformationMode.Temporary;
		}
		#endregion

	}
}