using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using Stratus;
using Stratus.OdinSerializer;

namespace Stratus.Utilities
{
    public static class StratusTypeUtility
    {
		#region Properties
		private static Dictionary<Type, Type[]> genericTypeDefiniions { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Type[]> subclasses { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> subclassNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Type[]> subclassesIncludeAbstract { get; set; } = new Dictionary<Type, Type[]>();

		private static Dictionary<Type, Dictionary<Type, Type[]>> interfacesImplementationsByBaseType { get; set; } = new Dictionary<Type, Dictionary<Type, Type[]>>();
		private static Dictionary<Type, Type[]> interfaceImplementations { get; set; } = new Dictionary<Type, Type[]>();


		private static Assembly[] _allAssemblies;
		public static Assembly[] allAssemblies
		{
			get
			{
				if (_allAssemblies == null)
				{
					_allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}

				return _allAssemblies;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] GetSubclassNames(Type baseType, bool includeAbstract = false)
		{
			string[] typeNames;
			if (!subclassNames.ContainsKey(baseType))
			{
				Type[] types = SubclassesOf(baseType, includeAbstract);
				//Type[] types = Assembly.GetAssembly(baseType).GetTypes();
				typeNames = (from Type type in types where type.IsSubclassOf(baseType) && !type.IsAbstract select type.Name).ToArray();
				subclassNames.Add(baseType, typeNames);
			}

			return subclassNames[baseType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] SubclassesOf<TClass>(bool includeAbstract = false)
		{
			return SubclassesOf(typeof(TClass), includeAbstract);
		}

		/// <summary>
		/// For a given generic type, returns all the types that use its definition,
		/// mapped by the parameter. For example:
		/// [int : DerivedInt1, DerivedInt2>
		/// [bool : DerivedBool1, DerivedBool2]
		/// </summary>
		public static Dictionary<Type, Type[]> TypeDefinitionParameterMap(Type baseType)
		{
			if (!baseType.IsGenericType)
			{
				throw new ArgumentException($"The given type {baseType} is not generic!");
			}
			Dictionary<Type, List<Type>> result = new Dictionary<Type, List<Type>>();
			Type[] definitions = TypesDefinedFromGeneric(baseType);
			foreach (var type in definitions)
			{
				var typeArgs = type.BaseType.GenericTypeArguments;
				if (typeArgs.Length == 1)
				{
					Type paramType = typeArgs[0];
					if (!result.ContainsKey(paramType))
					{
						result.Add(paramType, new List<Type>());
					}
					result[paramType].Add(type);
				}
			}
			return result.ToDictionary(kp => kp.Key, kp => kp.Value.ToArray());
		}

		/// <summary>
		/// For a given generic, returns all the types that use its definition.
		/// </summary>
		public static Type[] TypesDefinedFromGeneric(Type genericType)
		{
			if (!genericTypeDefiniions.ContainsKey(genericType))
			{
				List<Type> result = new List<Type>();

				foreach (Assembly assembly in allAssemblies)
				{
					Type[] implementedTypes = (from Type t
											   in assembly.GetTypes()
											   where t.BaseType != null &&
												t.BaseType.IsGenericType &&
												t.BaseType.GetGenericTypeDefinition() == genericType
											   select t).ToArray();

					result.AddRange(implementedTypes);
				}
				genericTypeDefiniions.Add(genericType, result.ToArray());
			}

			return genericTypeDefiniions[genericType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] SubclassesOf(Type baseType, bool includeAbstract = false)
		{
			// Done the first time this type is queried, in order to cache
			// Abstract
			if (includeAbstract)
			{
				if (!subclassesIncludeAbstract.ContainsKey(baseType))
				{
					List<Type> types = new List<Type>();
					foreach (Assembly assembly in allAssemblies)
					{
						Type[] assemblyTypes = (from Type t
												in assembly.GetTypes()
												where t.IsSubclassOf(baseType)
												select t).ToArray();
						types.AddRange(assemblyTypes);
					}
					subclassesIncludeAbstract.Add(baseType, types.ToArray());
				}
			}
			// Non-Abstract
			else
			{
				if (!subclasses.ContainsKey(baseType))
				{
					List<Type> types = new List<Type>();
					foreach (Assembly assembly in allAssemblies)
					{
						Type[] assemblyTypes = (from Type t
												in assembly.GetTypes()
												where t.IsSubclassOf(baseType) && !t.IsAbstract
												select t).ToArray();

						types.AddRange(assemblyTypes);
					}
					subclasses.Add(baseType, types.ToArray());
				}
			}

			return includeAbstract ? subclassesIncludeAbstract[baseType] : subclasses[baseType];
		}

		[System.Diagnostics.DebuggerHidden]
		public static Type GetIndexedType(this ICollection collection)
		{
			PropertyInfo propertyInfo = collection == null ? null : collection.GetType().GetProperty("Item");
			return propertyInfo == null ? null : propertyInfo.PropertyType;
		}

		/// <summary>
		/// Gets all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllTypesWithAttributeAsEnumerable(this Assembly assembly, Type attribute)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(attribute.GetType(), true).Length > 0)
				{
					yield return type;
				}
			}
		}

		/// <summary>
		/// Get all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static Type[] GetAllTypesWithAttribute(this Assembly assembly, Type attribute)
		{
			return assembly.GetAllTypesWithAttributeAsEnumerable(attribute).ToArray();
		}

		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] GetSubclassNames<ClassType>(bool includeAbstract = false)
		{
			Type baseType = typeof(ClassType);
			return GetSubclassNames(baseType, includeAbstract);
		}

		private static Dictionary<string, Type> s_TypeMap = new Dictionary<string, Type>();

		public static Type ResolveType(string classRef)
		{
			if (!s_TypeMap.TryGetValue(classRef, out Type type))
			{
				type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
				s_TypeMap[classRef] = type;
			}
			return type;
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetInterfaces(Type baseType, Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfacesImplementationsByBaseType.ContainsKey(interfaceType))
			{
				interfacesImplementationsByBaseType.Add(interfaceType, new Dictionary<Type, Type[]>());
			}

			// Now for a selected interface type, find all implementations that derive from the base type
			if (!interfacesImplementationsByBaseType[interfaceType].ContainsKey(baseType))
			{
				Type[] implementedTypes = (from Type t
										   in SubclassesOf(baseType)
										   where t.IsSubclassOf(baseType) && t.GetInterfaces().Contains((interfaceType))
										   select t).ToArray();
				interfacesImplementationsByBaseType[interfaceType].Add(baseType, implementedTypes);
			}

			return interfacesImplementationsByBaseType[interfaceType][baseType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] GetInterfaces(Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfaceImplementations.ContainsKey(interfaceType))
			{
				List<Type> types = new List<Type>();
				foreach (Assembly assembly in allAssemblies)
				{
					Type[] implementedTypes = (from Type t
											   in assembly.GetTypes()
											   where t.GetInterfaces().Contains((interfaceType))
												&& (t.IsAbstract == includeAbstract)
											   select t).ToArray();

					types.AddRange(implementedTypes);
				}
				interfaceImplementations.Add(interfaceType, types.ToArray());
			}

			return interfaceImplementations[interfaceType];
		}

		/// <summary>
		/// Retrieves the <see cref="Type"/>s of all classes inheriting from <paramref name="baseType"/> that
		/// implement the interface <paramref name="interfaceType"/>
		/// </summary>

		public static IEnumerable<Type> GetInterfaceImplementations(Type baseType, Type interfaceType, Type[] interfaceParameters = null)
		{
			Type[] subClasses = SubclassesOf(baseType);
			foreach (var subClass in subClasses)
			{
				if (GetInterfaces(subClass, interfaceType)
					.Any(t =>
					{
						if (t.Equals(interfaceType))
						{
							if (interfaceParameters != null)
							{
								return t.GenericTypeArguments == interfaceParameters;
							}
							return true;
						}
						return false;
					}))
				{
					yield return subClass;
				}
			}
		}

		/// <summary>
		/// Gets the loadable types for a given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		/// <summary>
		/// A list containing all the subclasses deriving from a particular class
		/// </summary>
		public class ClassList : List<KeyValuePair<string, Type>> { }

		/// <summary>
		/// Generates a list of key-value pairs of classes that derive from this one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <returns></returns>
		public static ClassList GenerateClassList<ClassType>(bool includeAbstract = true)
		{
			ClassList list = new ClassList();

			Type[] classes = SubclassesOf<ClassType>();
			foreach (Type e in classes)
			{
				string name = e.FullName.Replace('+', '.');
				Type type = e.ReflectedType;

				if (!includeAbstract && type.IsAbstract)
				{
					continue;
				}

				list.Add(new KeyValuePair<string, Type>(name, type));
			}
			return list;
		}
		#endregion
	}
}