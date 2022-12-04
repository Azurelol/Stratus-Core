using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Stratus;
using Stratus.OdinSerializer;

namespace Stratus.Utilities
{
	/// <summary>
	/// Utility methods for <see cref="Type"/>
	/// </summary>
	public static class StratusTypeUtility
	{
		#region Properties
		private static Dictionary<Type, Type[]> genericTypeDefiniions { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Type[]> subclasses { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> subclassNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Type[]> subclassesIncludeAbstract { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Dictionary<Type, Type[]>> interfacesImplementationsByBaseType { get; set; } = new Dictionary<Type, Dictionary<Type, Type[]>>();
		private static Dictionary<Type, Type[]> interfaceImplementations { get; set; } = new Dictionary<Type, Type[]>();

		private static Lazy<List<Type>> classes = new Lazy<List<Type>>(() =>
			allAssemblies.SelectMany(a => a.GetTypes()).ToList());

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

		private static Lazy<Dictionary<Type, List<Type>>> typesWithAttribute
			= new Lazy<Dictionary<Type, List<Type>>>(() =>
			{
				Dictionary<Type, List<Type>> result = new Dictionary<Type, List<Type>>();
				foreach (var type in classes.Value)
				{
					var attributes = type.GetCustomAttributes();
					foreach (var attr in attributes)
					{
						var attrType = attr.GetType();
						if (!result.ContainsKey(attr.GetType()))
						{
							result.Add(attrType, new List<Type>());
						}
						result[attr.GetType()].Add(type);
					}
				}
				return result;
			});

		private static Lazy<Dictionary<Assembly, Type[]>> typesByAssembly = new Lazy<Dictionary<Assembly, Type[]>>
			(() => allAssemblies.ToDictionary(a => a.GetTypes()));

		private static Lazy<Type[]> allTypes = new Lazy<Type[]>(() =>
			typesByAssembly.Value.SelectMany(a => a.Value).ToArray());

		#endregion

		#region Methods
		public static Type[] GetTypesFromAssembly(Assembly assembly)
			=> typesByAssembly.Value.GetValueOrDefault(assembly);

		public static Type[] GetAllTypes() => allTypes.Value;

		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] SubclassNames(Type baseType, bool includeAbstract = false)
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
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] SubclassNames<T>(bool includeAbstract = false)
		{
			Type baseType = typeof(T);
			return SubclassNames(baseType, includeAbstract);
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
		/// Gets all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllTypesWithAttribute(Type attribute)
		{
			return typesWithAttribute.Value[attribute];
		}

		private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

		/// <summary>
		/// Attempts to resolve the <see cref="Type"/> from the given type name
		/// </summary>
		public static Type ResolveType(string typeName)
		{
			if (!typeMap.TryGetValue(typeName, out Type type))
			{
				type = !string.IsNullOrEmpty(typeName) ? Type.GetType(typeName) : null;
				typeMap[typeName] = type;
			}
			return type;
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] InterfaceImplementations(Type baseType, Type interfaceType, bool includeAbstract = false)
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
		#endregion

		/// <summary>
		/// Finds the element type of the given collection
		/// </summary>
		[System.Diagnostics.DebuggerHidden]
		public static Type GetElementType(this ICollection collection)
		{
			PropertyInfo propertyInfo = collection == null ? null : collection.GetType().GetProperty("Item");
			return propertyInfo == null ? null : propertyInfo.PropertyType;
		}

		public static Type GetPrivateType(string name, Type source)
		{
			Assembly assembly = source.Assembly;
			return assembly.GetType(name);
		}

		public static Type GetPrivateType(string fqName)
		{
			return Type.GetType(fqName);
		}

		public static StratusTypeInfo TypeInfo<T>() => new StratusTypeInfo(typeof(T));
	}

}