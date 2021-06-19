using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;
using UnityEngine.Events;
using System.Text;
using System.Linq;

namespace Stratus
{
	/// <summary>
	/// Information about a gameobject and all its components
	/// </summary>
	[Serializable]
	public class StratusGameObjectInformation : ISerializationCallbackReceiver, IStratusLogger
	{
		#region Declarations
		public enum Change
		{
			Components,
			WatchList,
			ComponentsAndWatchList,
			None
		}
		#endregion

		#region Fields
		[SerializeField]
		private GameObject _gameObject;
		public StratusComponentInformation[] components;
		[SerializeField]
		private List<StratusComponentMemberWatchInfo> _watchList;
		public int fieldCount;
		public int propertyCount;
		#endregion

		#region Properties
		public GameObject gameObject => _gameObject;
		public StratusComponentMemberWatchInfo[] watchList => _watchList.ToArray();
		private Dictionary<string, StratusComponentMemberWatchInfo> watchByPath
		{
			get
			{
				if (_watchByPath == null)
				{
					_watchByPath = this._watchList.ToDictionary(m => m.path);
				}
				return _watchByPath;
			}
		}
		Dictionary<string, StratusComponentMemberWatchInfo> _watchByPath;
		public bool initialized { get; private set; }
		public bool debug { get; set; } = true;
		/// <summary>
		/// The recorded number of compoents of the gameobject
		/// </summary>
		public int numberofComponents => components.Length;
		/// <summary>
		/// The components of this GameObject, by their name
		/// </summary>
		public Dictionary<string, List<StratusComponentInformation>> componentsByName
		{
			get
			{
				if (_componentsByName == null)
				{
					_componentsByName = components.ToDictionaryOfList(c => c.name);
				}
				return _componentsByName;
			}
		}
		private Dictionary<string, List<StratusComponentInformation>> _componentsByName;

		/// <summary>
		/// The components of this GameObject, by theit type
		/// </summary>
		public Dictionary<Type, List<StratusComponentInformation>> componentsByType
		{
			get
			{
				if (_componentsByType == null)
				{
					_componentsByType = components.ToDictionaryOfList(c => c.type);
				}
				return _componentsByType;
			}
		}
		private Dictionary<Type, List<StratusComponentInformation>> _componentsByType;

		public StratusComponentMemberInfo[] members { get; private set; }
		public StratusComponentMemberInfo[] visibleMembers { get; private set; }
		public int memberCount => fieldCount + propertyCount;
		public bool isValid => gameObject != null && this.numberofComponents > 0;
		public static UnityAction<StratusGameObjectInformation, StratusOperationResult<Change>> onChanged { get; set; } =
			new UnityAction<StratusGameObjectInformation, StratusOperationResult<Change>>((StratusGameObjectInformation information, StratusOperationResult<Change> change) => { });
		#endregion

		#region Constants
		public static readonly HashSet<string> hiddenTypeNames = new HashSet<string>()
		{
			typeof(Component).Name
		};
		#endregion

		#region Messages
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.components == null)
			{
				return;
			}
			this.UpdateMemberReferences();
		}

		public override string ToString()
		{
			return gameObject.name;
		}
		#endregion

		#region Constructor
		public StratusGameObjectInformation(GameObject target)
		{
			this._gameObject = target;
			InitializeComponents(target);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Clears the watchlist for every component
		/// </summary>
		public void ClearWatchList()
		{
			_watchList.Clear();
			watchByPath.Clear();
			StratusGameObjectBookmark.UpdateWatchList();
		}

		/// <summary>
		/// Clears the values of all watch members
		/// </summary>
		public void ClearValues()
		{
			foreach (var component in this.components)
			{
				component.ClearValues();
			}
		}

		#region Watch
		/// <summary>
		/// Returns true if the member of the component is being watched
		/// </summary>
		public bool IsWatched(StratusComponentMemberInfo member)
		{
			return watchByPath.ContainsKey(member.path);
		}

		/// <summary>
		/// Adds a member to the watch list
		/// </summary>
		/// <param name="member"></param>
		/// <param name="componentInfo"></param>
		/// <param name="memberIndex"></param>
		public void AddWatch(StratusComponentMemberInfo member)
		{
			// Don't add duplicates since we only hold one and 
			// handle duplicate components
			if (!IsWatched(member))
			{
				var watch = member.ToWatch();
				_watchList.Add(watch);
				watchByPath.Add(member.path, watch);
				StratusGameObjectBookmark.UpdateWatchList(true);
			}
		}

		/// <summary>
		/// Removes a member from the watch list
		/// </summary>
		/// <param name="member"></param>
		public void RemoveWatch(StratusComponentMemberInfo member)
		{
			if (watchByPath.ContainsKey(member.path))
			{
				_watchList.RemoveAll(m => m.path == member.path);
				watchByPath.Remove(member.path);
				StratusGameObjectBookmark.UpdateWatchList(true);
				//if (this.AssertMemberIndex(member))
			}
		}

		/// <summary>
		/// Adds or removes the watch for the given member
		/// </summary>
		/// <param name="member"></param>
		public void ToggleWatch(StratusComponentMemberInfo member)
		{
			if (IsWatched(member))
			{
				RemoveWatch(member);
			}
			else
			{
				AddWatch(member);
			}
		}

		/// <summary>
		/// Updates the values of all the favorite members for this GameObject
		/// </summary>
		public void UpdateWatchValues()
		{
			bool valid = true;
			foreach (var member in _watchList)
			{
				if (!UpdateValue(member))
				{
					valid = false;
				}
			}
			if (!valid)
			{
				this.Log("GameObject information is out of date. Refreshing...");
				Refresh();
			}
		}

		/// <summary>
		/// Clears the watchlist
		/// </summary>
		public void ClearWatchList(bool updateBookmark = true)
		{
			_watchList.Clear();
			watchByPath.Clear();
			if (updateBookmark)
			{
				StratusGameObjectBookmark.UpdateWatchList();
			}
		}

		/// <summary>
		/// Clears the values of all watch members
		/// </summary>
		public void ClearWatchValues()
		{
			foreach (var member in this._watchList)
			{
				ClearValue(member);
			}
		}
		#endregion

		/// <summary>
		/// Updates the values of all the members for this GameObject
		/// </summary>
		public void UpdateValues()
		{
			foreach (var component in this.components)
			{
				component.UpdateValues();
			}
		}

		/// <summary>
		/// Clears the values of the given member (if its component is present)
		/// and of any duplicates
		/// </summary>
		public bool ClearValue(IStratusComponentMemberInfo member)
		{
			if (!componentsByName.ContainsKey(member.componentName))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.ClearValue(member));
			return true;
		}

		/// <summary>
		/// Updates the values of the given member (if its component is present)
		/// It will also update it for any duplicate components
		/// </summary>
		public bool UpdateValue(StratusComponentMemberInfo member)
		{
			if (!componentsByName.ContainsKey(member.componentName))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.UpdateValue(member));
			return true;
		}

		/// <summary>
		/// Updates the values of the given member (if its component is present)
		/// It will also update it for any duplicate components
		/// </summary>
		public bool UpdateValue(StratusComponentMemberWatchInfo member)
		{
			if (!componentsByName.ContainsKey(member.componentName))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.UpdateValue(member));
			return true;
		}

		public bool HasComponent(Type t) => componentsByType.ContainsKey(t);
		public bool HasComponent<T>() where T : Component => HasComponent(typeof(T));

		/// <summary>
		/// Caches all member references from among their components
		/// </summary>
		public void UpdateMemberReferences()
		{
			// Now cache!
			List<StratusComponentMemberInfo> memberReferences = new List<StratusComponentMemberInfo>();
			foreach (var component in this.components)
			{
				memberReferences.AddRange(component.memberReferences);
			}
			this.members = memberReferences.ToArray();
			this.visibleMembers = this.members.Where(m => !hiddenTypeNames.Contains(m.typeName)).ToArray();
			this.initialized = true;
			Debug.Log("Updating member references");
		}

		/// <summary>
		/// Refreshes the information for the target GameObject. If any components wwere added or removed,
		/// it will update the cache
		/// </summary>
		public void Refresh()
		{
			var validation = ValidateComponents();
			Change change = validation.result;
			switch (change)
			{
				case Change.Components:
					this.UpdateMemberReferences();
					onChanged(this, validation);
					break;
				case Change.ComponentsAndWatchList:
					this.UpdateMemberReferences();
					onChanged(this, validation);
					break;
				case Change.None:
					break;
			}
		}
		#endregion

		#region Procedures
		/// <summary>
		/// Initializes the information of the components of this game object
		/// </summary>
		/// <param name="target"></param>
		private void InitializeComponents(GameObject target)
		{
			this.fieldCount = 0;
			this.propertyCount = 0;
			Component[] targetComponents = target.GetComponents<Component>();
			List<StratusComponentInformation> components = new List<StratusComponentInformation>();
			for (int i = 0; i < targetComponents.Length; ++i)
			{
				Component component = targetComponents[i];
				if (component == null)
				{
					throw new Exception($"The component at index {i} is null!");
				}

				StratusComponentInformation componentInfo = new StratusComponentInformation(component);
				this.fieldCount += componentInfo.fieldCount;
				this.propertyCount += componentInfo.propertyCount;
				components.Add(componentInfo);
			}

			this.components = components.ToArray();
			this.UpdateMemberReferences();
			this._watchList = new List<StratusComponentMemberWatchInfo>();
		}

		/// <summary>
		/// Verifies that the component references for this GameObject are still valid
		/// </summary>
		private StratusOperationResult<Change> ValidateComponents()
		{
			bool watchlistChanged = false;
			bool changed = false;
			Change change;

			StringBuilder message = new StringBuilder();

			// Check if any components are null
			foreach (StratusComponentInformation component in this.components)
			{
				if (component.component == null)
				{
					changed = true;
					message.AppendLine($"Component {component.name} is now null");
					//if (component.hasWatchList)
					//{
					//	watchlistChanged = true;
					//}
				}
				else
				{
					if (component.valid)
					{
						changed |= component.Refresh();
						message.AppendLine($"The members of component {component.name} have changed");
					}
				}
			}

			// Check for other component changes
			Component[] targetComponents = gameObject.GetComponents<Component>();
			if (this.numberofComponents != targetComponents.Length)
			{
				changed = true;
				message.AppendLine($"The number of components have changed {numberofComponents} -> {targetComponents.Length}");
			}

			// If there's noticeable changes, let's add any components that were not there before
			if (changed)
			{
				List<StratusComponentInformation> currentComponents = new List<StratusComponentInformation>();
				currentComponents.AddRangeWhere((StratusComponentInformation component) => { return component.component != null; }, this.components);

				// If there's no information for this component, let's add it
				foreach (var component in targetComponents)
				{
					StratusComponentInformation ci = currentComponents.Find(x => x.component == component);

					if (ci == null)
					{
						ci = new StratusComponentInformation(component);
						currentComponents.Add(ci);
						message.AppendLine($"Recording new component {ci.name}");
					}
				}

				// Now update the list of components
				this.components = currentComponents.ToArray();
			}

			if (changed)
			{
				if (watchlistChanged)
				{
					change = Change.ComponentsAndWatchList;
				}

				change = Change.Components;
			}
			change = Change.None;

			return new StratusOperationResult<Change>(change == Change.None, change, message.ToString());
		}
		#endregion


	}

}