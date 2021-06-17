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
	public class StratusMemberInspectorWindow : StratusEditorWindow<StratusMemberInspectorWindow>, ISerializationCallbackReceiver
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
			Value,
			Type,
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/   
		[SerializeField]
		private Mode mode = Mode.Inspector;

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
		private float updateSpeed = 1f;

		[SerializeField]
		private StratusMemberInspectorTreeView memberInspector;

		[SerializeField]
		private TreeViewState treeViewState;

		private StratusCountdown pollTimer;
		private const string displayName = "Watcher";
		private string[] toolbarOptions = StratusEnum.Names<Mode>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/     
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
				_currentTargetInformation = value;
			}
		}

		private Type gameObjectType { get; set; }
		private bool hasTarget => this.target != null && this.currentTargetInformation != null;
		private int selectedIndex { get; set; }
		private bool updateTreeView { get; set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			if (this.treeViewState == null)
			{
				this.treeViewState = new TreeViewState();
			}

			this.pollTimer = new StratusCountdown(this.updateSpeed);
			this.gameObjectType = typeof(GameObject);
			this.CheckTarget();

			// Update tree view on assembly reload
			StratusGameObjectBookmark.onUpdate += this.OnBookmarkUpdate;
			StratusGameObjectInformation.onChanged += this.OnGameObjectInformationChanged;
			this.Log("Enabled");
		}

		static readonly float[] rowWeights = new float[] 
		{
			0.2f,
			0.7f,
			0.1f
		};

		protected override void OnWindowGUI()
		{
			void drawControls(Rect rect)
			{
				StratusEditorGUILayout.Aligned(this.DrawControls, TextAlignment.Center);

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
				rect = rect.Intend(4f);
				EditorGUI.ProgressBar(rect, pollTimer.normalizedProgress, "Update");
			}

			Rect[] rows = positionToGUI.Column(rowWeights);
			rows.ForEach(drawControls, drawInspector, drawUpdate);
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
							this.currentTargetInformation.Refresh();
							this.currentTargetInformation.UpdateWatchValues();
						}
						break;

					case Mode.WatchList:
						if (StratusGameObjectBookmark.hasAvailableInformation)
						{
							foreach (var targetInfo in StratusGameObjectBookmark.availableInformation)
							{
								targetInfo.Refresh();
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
						this.CreateTargetInformation();
					}
					break;

				// Don't bother trying to update while exiting
				case PlayModeStateChange.ExitingEditMode:
				case PlayModeStateChange.ExitingPlayMode:
					updateTreeView = false;
					break;
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		private void OnBookmarkUpdate()
		{
			if (updateTreeView)
			{
				this.SetTreeView();
			}
		}

		private void OnGameObjectInformationChanged(StratusGameObjectInformation information, StratusOperationResult<StratusGameObjectInformation.Change> change)
		{
			this.Log($"Information changed for {information.target.name}, change = {change.result}:\n{change.message}");
			if (change.result == StratusGameObjectInformation.Change.ComponentsAndWatchList)
			{
				StratusGameObjectBookmark.UpdateWatchList();
			}

			this.SetTreeView();
		}

		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/
		[MenuItem(StratusCore.rootMenu + "Member Inspector")]
		private static void Open() => OpenWindow(displayName);

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			if (instance == null || instance.memberInspector == null)
				return false;
			return instance.memberInspector.TryOpenAsset(instanceID, line);
		}

		/// <param name="target"></param>
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

		//------------------------------------------------------------------------/
		// Methods: Target Selection
		//------------------------------------------------------------------------/
		private void DrawTargetSelector()
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Target", StratusGUIStyles.header);
			bool changed = StratusEditorUtility.CheckControlChange(() =>
			{
				this.target = (GameObject)EditorGUILayout.ObjectField(this.target, this.gameObjectType, true);
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
				if (this.target)
				{
					this.CreateTargetInformation();
				}
				else
				{
					this.currentTargetInformation = null;

					if (this.informationMode == InformationMode.Temporary)
					{
						this.targetTemporaryInformation = null;
					}
				}
			}
		}

		private void SelectTarget(GameObject target)
		{
			this.target = target;
			this.selectedIndex = 0;
			this.CreateTargetInformation();
		}

		private void CheckTarget()
		{
			if (target)
			{
				this.CreateTargetInformation();
			}
			else
			{
				this.Log("Clearing target");
				this.currentTargetInformation = this.targetTemporaryInformation = null;
			}
		}

		private void CreateTargetInformation()
		{
			// If there's no target information or if the target is different from the previous
			///if (this.currentTargetInformation == null || this.currentTargetInformation.target != this.target)
			{
				// If the target has as bookmark, use that information instead
				StratusGameObjectBookmark bookmark = this.target.GetComponent<StratusGameObjectBookmark>();
				if (bookmark != null)
				{
					this.informationMode = InformationMode.Bookmark;
					this.currentTargetInformation = bookmark.information;
					this.Log("Setting target from bookmark");
				}
				// Otherwise recreate the current target information
				else if (this.currentTargetInformation == null || this.currentTargetInformation.target != this.target)
				{
					this.Log("Recreating current target information");
					this.informationMode = InformationMode.Temporary;
					this.targetTemporaryInformation = new StratusGameObjectInformation(this.target);
					this.currentTargetInformation = this.targetTemporaryInformation;
				}

				//Trace.Script($"Setting target information for {this.target.name}");
			}

			//this.showComponent = this.GenerateAnimBools(this.currentTargetInformation.numberofComponents, false);
			//this.componentList = new DropdownList<ComponentInformation>(this.currentTargetInformation.components, (ComponentInformation component) => component.name, this.lastComponentIndex);
			this.SetTreeView();
		}

		//------------------------------------------------------------------------/
		// Methods: Draw
		//------------------------------------------------------------------------/
		private void DrawControls()
		{
			// Toolbar      
			EditorGUI.BeginChangeCheck();
			{
				this.mode = (Mode)GUILayout.Toolbar((int)this.mode, this.toolbarOptions, GUILayout.ExpandWidth(false));
			}
			if (EditorGUI.EndChangeCheck() || this.memberInspector == null)
			{
				this.SetTreeView();
			}
		}

		private void SetTreeView()
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
						this.Log("Generating inspector tree");
						members = StratusMemberInspectorTreeElement.GenerateInspectorTree(this.currentTargetInformation);
					}
					else
						return;
					break;
			}

			if (this.memberInspector == null)
			{
				this.memberInspector = new StratusMemberInspectorTreeView(this.treeViewState, members);
			}
			else
			{
				this.memberInspector.SetTree(members);
			}

			switch (this.mode)
			{
				case Mode.WatchList:
					this.memberInspector.EnableColumn(Column.GameObject);
					this.memberInspector.DisableColumn(Column.Watch);
					break;

				case Mode.Inspector:
					this.memberInspector.DisableColumn(Column.GameObject);
					this.memberInspector.EnableColumn(Column.Watch);
					break;
			}

		}

	}
}