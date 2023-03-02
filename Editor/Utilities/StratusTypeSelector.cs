using System;
using System.Linq;

using Stratus.Collections;
using Stratus.Types;

namespace Stratus
{
	/// <summary>
	/// Allows an easy interface for selecting subclasses from a given type
	/// </summary>
	public class StratusTypeSelector
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public Type baseType { get; private set; }
		public DropdownList<Type> subTypes { get; private set; }
		public Type selectedClass => this.subTypes.selected;
		private string selectedClassName => this.selectedClass.Name;
		public int selectedIndex => this.subTypes.selectedIndex;
		public string[] displayedOptions => this.subTypes.displayedOptions;
		public System.Action onSelectionChanged { get; set; }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusTypeSelector(Type baseType, bool includeAbstract, bool sortAlphabetically = false)
		{
			this.baseType = baseType;
			this.subTypes = new DropdownList<Type>(TypeUtility.SubclassesOf(baseType), Name);
			if (sortAlphabetically)
			{
				this.subTypes.Sort();
			}
		}

		public StratusTypeSelector(Type[] types, bool includeAbstract, bool sortAlphabetically = false)
		{
			this.baseType = this.baseType;
			this.subTypes = new DropdownList<Type>(types, Name);
			if (sortAlphabetically)
			{
				this.subTypes.Sort();
			}
		}

		public StratusTypeSelector(Type baseType, Type interfaceType, bool sortAlphabetically = false)
		{
			this.baseType = baseType;
			this.subTypes = new DropdownList<Type>(TypeUtility.InterfaceImplementations(baseType, interfaceType), (Type type) => type.Name);

			if (sortAlphabetically)
			{
				this.subTypes.Sort();
			}
		}

		public StratusTypeSelector(Type baseType, Predicate<Type> predicate, bool includeAbstract, bool sortAlphabetically = true)
			: this(TypeUtility.SubclassesOf(baseType).Where(x => predicate(x)).ToArray(), includeAbstract, sortAlphabetically)
		{
		}

		public static StratusTypeSelector FilteredSelector(Type baseType, Type excludedType, bool includeAbstract, bool sortAlphabetically = true)
		{
			Type[] types = TypeUtility.SubclassesOf(baseType).Where(x => !x.IsSubclassOf(excludedType)).ToArray();
			return new StratusTypeSelector(types, includeAbstract, sortAlphabetically);
		}

		private static string Name(Type type)
		{
			return type.Name;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void ResetSelection(int index = 0)
		{
			if (this.selectedIndex == index)
			{
				return;
			}

			this.subTypes.selectedIndex = index;
			this.OnSelectionChanged();
		}

		public Type AtIndex(int index)
		{
			return this.subTypes.AtIndex(index);
		}

		protected virtual void OnSelectionChanged()
		{
			this.onSelectionChanged?.Invoke();
		}

	}

}