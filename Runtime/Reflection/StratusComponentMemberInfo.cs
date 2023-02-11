using System.Reflection;
using System;
using UnityEngine;
using Stratus.Models;

namespace Stratus
{
	public interface IStratusComponentMemberInfo : IStratusNamed
	{
		public string componentName { get; }
		public string typeName { get; }
		public string assemblyQualifiedName { get; }
		public string path { get; }
	}

	/// <summary>
	/// Serialized information of a <see cref="MemberInfo"/> of a <see cref="Component"/>
	/// </summary>
	[Serializable]
	public class StratusComponentMemberInfo : IStratusComponentMemberInfo
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The name of this member
		/// </summary>
		public string name;
		/// <summary>
		/// Whether it's a property, field
		/// </summary>
		public MemberTypes memberType;
		/// <summary>
		/// The name of the <see cref="Type"/> for this member
		/// </summary>
		public string typeName;
		/// <summary>
		/// The assembly qualified name of the <see cref="Type"/> for this member
		/// </summary>
		public string assemblyQualifiedName;
		/// <summary>
		/// The name of the <see cref="Component"/> this member is part of
		/// </summary>
		public string componentName;
		/// <summary>
		/// The name of the GameObject
		/// </summary>
		public string gameObjectName;
		/// <summary>
		/// The index to this member for either the fields or properties of the component
		/// </summary>
		public int memberIndex;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string path => $"{componentName}.{name}";
		public string latestValueString { get; private set; }
		public object latestValue { get; private set; }
		public bool initialized { get; private set; } = false;
		string IStratusComponentMemberInfo.componentName => componentName;
		string IStratusComponentMemberInfo.typeName => typeName;
		string IStratusComponentMemberInfo.assemblyQualifiedName => assemblyQualifiedName;
		string IStratusComponentMemberInfo.path => path;
		string IStratusNamed.name => name;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusComponentMemberInfo(FieldInfo member, StratusComponentInformation component, int index)
		{
			Initialize(member, member.FieldType, component, index);
		}

		public StratusComponentMemberInfo(PropertyInfo member, StratusComponentInformation component, int index)
		{
			Initialize(member, member.PropertyType, component, index);
		}

		private void Initialize(MemberInfo member, Type type, StratusComponentInformation component, int index)
		{
			this.name = member.Name;
			this.typeName = type.Name;
			this.assemblyQualifiedName = type.AssemblyQualifiedName;
			this.componentName = component.name;
			this.gameObjectName = component.gameObject.name;
			this.memberType = member.MemberType;
			this.memberIndex = index;
		}

		public override string ToString() => path;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Retrieves the latest value for this member
		/// </summary>
		public bool UpdateValue(StratusComponentInformation component)
		{
			bool updated = false;
			object value = null;
			try
			{
				switch (this.memberType)
				{
					case MemberTypes.Field:
						value = component.fields[memberIndex].GetValue(component.component);
						break;
					case MemberTypes.Property:
						value = component.properties[memberIndex].GetValue(component.component);
						break;
				}
				updated = true;
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to update value of {this}:\n{e}");
				updated = false;
			}

			this.latestValue = value;
			this.latestValueString = value != null ? value.ToString().Trim() : string.Empty;
			return updated;
		}

		/// <summary>
		/// Clears the latest recorded value
		/// </summary>
		public void ClearValue()
		{
			this.latestValue = null;
			this.latestValueString = string.Empty;
		}

		public StratusComponentMemberWatchInfo ToWatch()
		{
			return new StratusComponentMemberWatchInfo(this);
		}
	}

	/// <summary>
	/// Information for watching the member of a <see cref="Component"/>
	/// </summary>
	[Serializable]
	public class StratusComponentMemberWatchInfo : IStratusComponentMemberInfo
	{
		public string name;
		public string componentName;
		public string typeName;
		public string assemblyQualifiedName;
		public string path;

		public StratusComponentMemberWatchInfo(StratusComponentMemberInfo member)
		{
			this.name = member.name;
			this.componentName = member.componentName;
			this.path = member.path;
			this.typeName = member.typeName;
			this.assemblyQualifiedName = member.assemblyQualifiedName;
		}

		string IStratusNamed.name => name;
		string IStratusComponentMemberInfo.componentName => componentName;
		string IStratusComponentMemberInfo.typeName => typeName;
		string IStratusComponentMemberInfo.assemblyQualifiedName => assemblyQualifiedName;
		string IStratusComponentMemberInfo.path => path;
	}
}