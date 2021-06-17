using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;
using UnityEngine.Events;
using System.Text;

namespace Stratus
{
	/// <summary>
	/// Information about a gameobject and all its components
	/// </summary>
	[Serializable]
	public class StratusGameObjectInformation : ISerializationCallbackReceiver
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
		public GameObject target;
		public StratusComponentInformation[] components;
		public int fieldCount;
		public int propertyCount;
		#endregion

		#region Properties
		public bool initialized { get; private set; }
		/// <summary>
		/// The recorded number of compoents of the gameobject
		/// </summary>
		public int numberofComponents => components.Length;
		public Dictionary<Type, StratusComponentInformation> componentsByType { get; private set; }
		public StratusComponentInformation.MemberReference[] members { get; private set; }
		public StratusComponentInformation.MemberReference[] watchList { get; private set; }
		public int memberCount => fieldCount + propertyCount;
		public bool isValid => target != null && this.numberofComponents > 0;
		public static UnityAction<StratusGameObjectInformation, StratusOperationResult<Change>> onChanged { get; set; } = 
			new UnityAction<StratusGameObjectInformation, StratusOperationResult<Change>>((StratusGameObjectInformation information, StratusOperationResult<Change> change) => { });
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

			// Verify that components are still valid
			//this.ValidateComponents();

			// Cache current member references
			this.CacheReferences();
		}
		#endregion

		#region Constructor
		public StratusGameObjectInformation(GameObject target)
		{
			// Set target
			this.target = target;

			// Set components
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
			this.componentsByType = components.ToDictionary(c => c.type);

			// Now cache member references
			this.CacheReferences();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clears the watchlist for every component
		/// </summary>
		public void ClearWatchList()
		{
			foreach (var component in this.components)
			{
				component.ClearWatchList(false);
			}

			StratusGameObjectBookmark.UpdateWatchList();
		}

		/// <summary>
		/// Updates the values of all the favorite members for this GameObject
		/// </summary>
		public void UpdateWatchValues()
		{
			foreach (var component in this.components)
			{
				component.UpdateWatchValues();
			}
		}

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

		public bool HasComponent(Type t) => componentsByType.ContainsKey(t);
		public bool HasComponent<T>() where T : Component => HasComponent(typeof(T));

		/// <summary>
		/// Caches all member references from among their components
		/// </summary>
		public void CacheReferences()
		{
			// Now cache!
			List<StratusComponentInformation.MemberReference> memberReferences = new List<StratusComponentInformation.MemberReference>();
			foreach (var component in this.components)
			{
				memberReferences.AddRange(component.memberReferences);
			}
			this.members = memberReferences.ToArray();

			Debug.Log("Member references recorded");
			this.CacheWatchList();
			this.initialized = true;
		}

		/// <summary>
		/// Caches all member references under a watchlist for each component
		/// </summary>
		public void CacheWatchList()
		{
			List<StratusComponentInformation.MemberReference> watchList = new List<StratusComponentInformation.MemberReference>();
			foreach (var component in this.components)
			{
				if (component.valid)
					watchList.AddRange(component.watchList);
			}
			this.watchList = watchList.ToArray();
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
					this.CacheReferences();
					onChanged(this, validation);
					break;
				case Change.ComponentsAndWatchList:
					this.CacheReferences();
					onChanged(this, validation);
					break;
				case Change.None:
					break;
			}
		}
		#endregion

		#region Procedures
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
					if (component.hasWatchList)
					{
						watchlistChanged = true;
					}
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
			Component[] targetComponents = target.GetComponents<Component>();
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