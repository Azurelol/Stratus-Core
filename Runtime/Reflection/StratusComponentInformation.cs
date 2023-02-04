using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;
using System;
using Stratus.Extensions;

namespace Stratus
{

	/// <summary>
	/// Information about a component
	/// </summary>
	[Serializable]
	public class StratusComponentInformation : ISerializationCallbackReceiver
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The name of the component type
		/// </summary>
		public string name;
		/// <summary>
		/// The component this information is about
		/// </summary>
		public Component component;
		/// <summary>
		/// Whether the members of this component shoudl be sorted alphabetically
		/// </summary>
		public bool alphabeticalSorted;
		/// <summary>
		/// A list of all members fields and properties of this component
		/// </summary>
		public List<StratusComponentMemberInfo> memberReferences;

		[NonSerialized]
		public Type type;
		[NonSerialized]
		public object[] fieldValues, propertyValues, favoriteValues;
		[NonSerialized]
		public string[] fieldValueStrings, propertyValueStrings, favoriteValueStrings;

		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public GameObject gameObject => component.gameObject;
		public FieldInfo[] fields { get; private set; }
		public int fieldCount => fields.Length;
		public PropertyInfo[] properties { get; private set; }
		public int propertyCount => properties.Length;
		public bool hasFields => fieldCount > 0;
		public bool hasProperties => propertyCount > 0;
		public Dictionary<string, StratusComponentMemberInfo> membersByName
		{
			get
			{
				if (_membersByName == null)
				{
					_membersByName = new Dictionary<string, StratusComponentMemberInfo>();
					_membersByName.AddRangeUnique((StratusComponentMemberInfo member) => member.name, this.memberReferences);

				}
				return _membersByName;
			}
		}
		private Dictionary<string, StratusComponentMemberInfo> _membersByName;
		public bool valid => component != null;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			// If there's no component, stuff is gone!
			if (!valid)
			{
				return;
			}

			// Initialize step
			this.InitializeComponentInformation();
			this.InitializeMemberReferences();
		}

		#region Constructors
		public StratusComponentInformation(Component component, bool alphabeticalSort = false)
		{
			if (component == null)
			{
				return;
			}

			this.component = component;
			this.name = component.GetType().Name;
			this.InitializeComponentInformation();
			this.CreateAllMemberReferences();
		}

		/// <summary>
		/// Runtime: Record all type information about the members of the component
		/// </summary>
		private void InitializeComponentInformation()
		{
			// Type
			this.type = this.component.GetType();

			// Fields
			this.fields = this.type.GetFields(bindingFlags);
			if (this.alphabeticalSorted)
				Array.Sort(this.fields, delegate (FieldInfo a, FieldInfo b) { return a.Name.CompareTo(b.Name); });
			this.fieldValues = new object[this.fields.Length];
			this.fieldValueStrings = new string[this.fields.Length];

			// Properties
			this.properties = this.type.GetProperties(bindingFlags);
			if (this.alphabeticalSorted)
				Array.Sort(this.properties, delegate (PropertyInfo a, PropertyInfo b) { return a.Name.CompareTo(b.Name); });
			this.propertyValues = new object[this.properties.Length];
			this.propertyValueStrings = new string[this.properties.Length];
		}
		#endregion

		#region Methods
		/// <summary>
		/// Updates the values of all fields and properties for this component
		/// </summary>
		public void UpdateValues()
		{
			// Some properties may fail in editor or in play mode
			for (int f = 0; f < fields.Length; ++f)
			{
				try
				{
					object value = this.GetValue(this.fields[f]);
					this.fieldValues[f] = this.GetValue(this.fields[f]);
					this.fieldValueStrings[f] = value.ToString();
				}
				catch
				{
				}
			}

			for (int p = 0; p < properties.Length; ++p)
			{
				try
				{
					object value = this.GetValue(this.properties[p]);
					this.propertyValues[p] = value;
					this.propertyValueStrings[p] = value.ToString();
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Updates the value of the given member (if it belongs to this component)
		/// </summary>
		/// <param name="component"></param>
		public bool UpdateValue(IStratusComponentMemberInfo member)
		{
			if (!membersByName.ContainsKey(member.name)
				|| member.componentName != this.name)
			{
				return false;
			}

			return membersByName[member.name].UpdateValue(this);
		}

		/// <summary>
		/// Updates the value of the given member (if it belongs to this component)
		/// </summary>
		/// <param name="component"></param>
		public bool ClearValue(IStratusComponentMemberInfo member)
		{
			if (!membersByName.ContainsKey(member.name)
				|| member.componentName != this.name)
			{
				return false;
			}

			membersByName[member.name].ClearValue();
			return true;
		}

		/// <summary>
		/// Updates the values of all the variables for this component
		/// </summary>
		public void ClearValues()
		{
			foreach (var member in this.memberReferences)
			{
				member.ClearValue();
			}
		}	

		/// <summary>
		/// Saves all member references for this GameObject
		/// </summary>
		/// <returns></returns>
		private void CreateAllMemberReferences()
		{
			List<StratusComponentMemberInfo> memberReferences = new List<StratusComponentMemberInfo>();
			for (int f = 0; f < this.fields.Length; ++f)
			{
				StratusComponentMemberInfo memberReference = new StratusComponentMemberInfo(this.fields[f], this, f);
				memberReferences.Add(memberReference);
			}

			for (int p = 0; p < this.properties.Length; ++p)
			{
				StratusComponentMemberInfo memberReference = new StratusComponentMemberInfo(this.properties[p], this, p);
				memberReferences.Add(memberReference);
			}
			this.memberReferences = memberReferences;
		}

		/// <summary>
		/// Initializes all member references (done after deserialization)
		/// </summary>
		private void InitializeMemberReferences()
		{
			// Validate
			Predicate<StratusComponentMemberInfo> validate = (StratusComponentMemberInfo member) =>
			{
				return AssertMemberIndex(member);
			};

			// Iteration
			Action<StratusComponentMemberInfo> iterate = (StratusComponentMemberInfo member) =>
			{
			};

			this.memberReferences.ForEachRemoveInvalid(iterate, validate);
		}

		/// <summary>
		/// Checks for any new members added onto the component. Returns true if any members
		/// were added
		/// </summary>
		public bool Refresh()
		{
			bool changed = false;
			for (int i = 0; i < this.fields.Length; ++i)
			{
				FieldInfo field = this.fields[i];
				if (field == null)
					return true;

				if (!this.membersByName.ContainsKey(field.Name))
				{
					changed |= true;
					Debug.Log($"New field detected! {field.Name}");
					StratusComponentMemberInfo memberReference = new StratusComponentMemberInfo(field, this, i);
					this.memberReferences.Add(memberReference);
					this.membersByName.Add(field.Name, memberReference);
				}
			}

			for (int i = 0; i < this.properties.Length; ++i)
			{
				PropertyInfo property = this.properties[i];
				if (property == null)
					return true;

				if (!this.membersByName.ContainsKey(property.Name))
				{
					changed |= true;
					Debug.Log($"New property detected! {property.Name}");
					StratusComponentMemberInfo memberReference = new StratusComponentMemberInfo(property, this, i);
					this.memberReferences.Add(memberReference);
					this.membersByName.Add(property.Name, memberReference);
				}
			}
			return changed;
		}

		/// <summary>
		/// If the member at the index doesn't match the member reference index,
		/// this means the member could have been removed or rearranged
		/// </summary>
		/// <param name="memberReference"></param>
		/// <returns></returns>
		private bool AssertMemberIndex(StratusComponentMemberInfo memberReference)
		{
			int index = memberReference.memberIndex;
			switch (memberReference.memberType)
			{
				case MemberTypes.Field:
					if (!this.fields.ContainsIndex(index))
						return false;
					if (this.fields[index].Name != memberReference.name)
						return UpdateMemberIndex(memberReference);
					break;

				case MemberTypes.Property:
					if (!this.properties.ContainsIndex(index))
						return false;
					if (this.properties[index].Name != memberReference.name)
						return UpdateMemberIndex(memberReference);
					break;
			}
			return true;
		}



		/// <summary>
		/// Attempts to update the member index for this member, if possible
		/// </summary>
		/// <param name="memberReference"></param>
		/// <returns></returns>
		private bool UpdateMemberIndex(StratusComponentMemberInfo memberReference)
		{
			if (memberReference.memberType == MemberTypes.Field)
			{
				for (int i = 0; i < this.fields.Length; ++i)
				{
					// Found a field with the same name
					if (this.fields[i].Name == memberReference.name)
					{
						memberReference.memberIndex = i;
						return true;
					}
				}
			}
			else if (memberReference.memberType == MemberTypes.Property)
			{
				for (int i = 0; i < this.properties.Length; ++i)
				{
					// Found a property with the same name
					if (this.properties[i].Name == memberReference.name)
					{
						memberReference.memberIndex = i;
						return true;
					}
				}
			}

			// Couldn't update this member reference
			return false;
		}
		#endregion

		/// <summary>
		/// Retrieves the value of the selected field
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object GetValue(FieldInfo field) => field.GetValue(component);

		/// <summary>
		/// Retrieves the value of the selected property
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object GetValue(PropertyInfo property) => property.GetValue(component);
	}


}