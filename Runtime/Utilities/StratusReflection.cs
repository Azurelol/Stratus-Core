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
	public static partial class StratusReflection
	{
		/// <summary>
		/// An exception that is thrown whenever a field was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class FieldNotFoundException : Exception
		{
			public FieldNotFoundException() { }

			public FieldNotFoundException(string message) : base(message) { }

			public FieldNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected FieldNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// An exception that is thrown whenever a property was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class PropertyNotFoundException : Exception
		{
			public PropertyNotFoundException() { }

			public PropertyNotFoundException(string message) : base(message) { }

			public PropertyNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected PropertyNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// An exception that is thrown whenever a field or a property was not found inside of an object when using Reflection.
		/// </summary>
		[Serializable]
		public class PropertyOrFieldNotFoundException : Exception
		{
			public PropertyOrFieldNotFoundException() { }

			public PropertyOrFieldNotFoundException(string message) : base(message) { }

			public PropertyOrFieldNotFoundException(string message, Exception inner) : base(message, inner) { }

			protected PropertyOrFieldNotFoundException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}

		//----------------------------------------------------------------------/
		// Properties
		//----------------------------------------------------------------------/
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

		private static Dictionary<Type, Type[]> subclasses { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> subclassNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Type[]> subclassesIncludeAbstract { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Dictionary<Type, Type[]>> interfacesImplementationsByBaseType { get; set; } = new Dictionary<Type, Dictionary<Type, Type[]>>();
		private static Dictionary<Type, Type[]> interfaceImplementations { get; set; } = new Dictionary<Type, Type[]>();

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
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

		[System.Diagnostics.DebuggerHidden]
		public static Type GetIndexedType(this ICollection collection)
		{
			PropertyInfo propertyInfo = collection == null ? null : collection.GetType().GetProperty("Item");
			return propertyInfo == null ? null : propertyInfo.PropertyType;
		}

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

		public static FieldInfo[] GetFieldsWithNullReferences<T>(T behaviour) where T : Behaviour
		{
			Type type = behaviour.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			return (from f
				   in fields
					where (f.FieldType.IsSubclassOf(typeof(UnityEngine.Object))
					&& f.GetValue(behaviour).Equals(null))
					select f).ToArray();
		}

		public static Type GetPrivateType(string name, Type source)
		{
			Assembly assembly = source.Assembly;
			return assembly.GetType(name);
		}

		public static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return new Type[0];
			}
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException)
			{
				return new Type[0];
			}
		}

		public static Type GetPrivateType(string fqName)
		{
			return Type.GetType(fqName);
		}

		public static T GetField<T>(string name, Type type, bool isStatic = true, object instance = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo field = type.GetField(name, bindflags);

			return (T)field.GetValue(instance);
		}

		public static void SetField<T>(string name, Type type, T value, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			FieldInfo field = type.GetField(name, bindflags);

			if (instantce != null)
			{
				field = instantce.GetType().GetField(name, bindflags);
			}

			field.SetValue(instantce, value);
		}

		public static T GetProperty<T>(string name, Type type, object instance)
		{
			BindingFlags bindflags = BindingFlags.NonPublic | BindingFlags.Instance;
			PropertyInfo propInfo = type.GetProperty(name, bindflags);

			MethodInfo getAccessor = propInfo.GetGetMethod(true);

			return (T)getAccessor.Invoke(instance, null);
		}

		public static MethodInfo GetReflectedMethod(string name, Type type, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? (BindingFlags.NonPublic | BindingFlags.Static) : (BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo method = type.GetMethod(name, bindflags);

			return method;
		}

		/// <summary>
		/// Retrieves the name of this property / field as well as its owning object.
		/// Note: This is quite an expensive call so use sparingly.
		/// </summary>
		/// <param name="varExpr">A lambda expression capturing a reference to a field or property</param>
		/// <returns></returns>
		public static StratusMemberReference GetReference<T>(Expression<Func<T>> varExpr)
		{
			// Slow, probs
			return StratusMemberReference.Construct(varExpr);
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

			Type[] classes = StratusReflection.SubclassesOf<ClassType>();
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

		/// <summary>
		/// Uses compiled expressions to instantiate types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class New<T>
		{
			public static readonly Func<T> Instance = Creator();

			private static Func<T> Creator()
			{
				Type t = typeof(T);
				if (t == typeof(string))
				{
					return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
				}

				if (t.HasDefaultConstructor())
				{
					return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
				}

				return () => (T)FormatterServices.GetUninitializedObject(t);
			}
		}


		public static object Instantiate(Type t)
		{
			if (t == typeof(string))
			{
				return Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile();
			}

			if (t.HasDefaultConstructor())
			{
				return Activator.CreateInstance(t);
			}

			return FormatterServices.GetUninitializedObject(t);
		}

		public static T Instantiate<T>()
		{
			Type t = typeof(T);
			if (t == typeof(string))
			{
				return (T)(object)(Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile());
			}

			if (t.HasDefaultConstructor())
			{
				return (T)Activator.CreateInstance(t);
			}
			//return Expression.Lambda<Func<object>>(Expression.New(t)).Compile();

			return (T)FormatterServices.GetUninitializedObject(t);
		}

		public static IEnumerable<object> Instantiate(params Type[] types)
		{
			return types.Select(t => Instantiate(t));
		}

		public static IEnumerable<T> Instantiate<T>(params Type[] types)
		{
			return Instantiate(types).Select(obj => (T)obj);
		}

		/// <summary>
		/// Finds the most nested object inside of an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T GetNestedObject<T>(this object obj, string path)
		{
			foreach (string part in path.Split('.'))
			{
				obj = obj.GetFieldOrPropertyValue<T>(part);
			}
			return (T)obj;
		}

		/// <summary>
		/// Gets a property or a field of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object the field/property should be found in.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		/// <returns>The field/property.</returns>
		public static T GetFieldOrPropertyValue<T>(this object obj, string memberName, bool includeAllBases = false,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			if (includeAllBases)
			{
				Type objectType = obj.GetType();
				foreach (Type type in objectType.GetBaseClassesAndInterfaces())
				{
					field = type.GetField(memberName, bindingFlags);
					if (field != null)
					{
						return (T)field.GetValue(obj);
					}

					property = type.GetProperty(memberName, bindingFlags);
					if (property != null)
					{
						return (T)property.GetValue(obj, null);
					}
				}
			}

			throw new PropertyOrFieldNotFoundException($"Couldn't find a field or property with the name of {memberName} inside of the object {obj.GetType().Name }");
		}

		/// <summary>
		/// Gets a field inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object the field should be found in.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>
		/// <returns>The field.</returns>
		public static T GetField<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets a property inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object the property should be found in.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		/// <returns>The property.</returns>
		public static T GetProperty<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a field or a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object contaning the field/property.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="value">New value of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		public static bool SetFieldOrPropertyValue<T>(this object obj, string memberName, T value, bool includeAllBases = false,
			BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindings);
			if (field != null)
			{
				field.SetValue(obj, value);
				return true;
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindings);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return true;
			}

			if (includeAllBases)
			{
				Type objectType = obj.GetType();
				foreach (Type type in objectType.GetBaseClassesAndInterfaces())
				{
					field = type.GetField(memberName, bindings);
					if (field != null)
					{
						field.SetValue(obj, value);
						return true;
					}

					property = type.GetProperty(memberName, bindings);
					if (property != null)
					{
						property.SetValue(obj, value, null);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Sets a field inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object contaning the field.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="value">New value of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>>
		public static void SetField<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				field.SetValue(obj, value);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object contaning the property.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="value">New value of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		public static void SetProperty<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets all the properties and fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">The type of the fields/properties.</typeparam>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable<T> GetAllFieldsOrProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get the fields and the properties in the object.
			T[] fields = obj.GetAllFields<T>(bindingFlags).ToArray();
			T[] properties = obj.GetAllProperties<T>(bindingFlags).ToArray();

			// Only return the fields if fields were found.
			if (fields != null && fields.Length != 0)
			{
				// Loop through the fields and return each one.
				for (int i = 0; i < fields.Length; i++)
				{
					yield return fields[i];
				}
			}

			// Only return the properties if properties were found.
			if (properties != null && properties.Length != 0)
			{
				// Loop through the properties and return each one if they have the right type.
				for (int i = 0; i < properties.Length; i++)
				{
					yield return properties[i];
				}
			}
		}

		/// <summary>
		/// Gets all the properties and fields in object
		/// </summary>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable GetAllFieldsOrProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get the fields and the properties in the object.
			object[] fields = obj.GetAllFields(bindingFlags).Cast<object>().ToArray();
			object[] properties = obj.GetAllProperties(bindingFlags).Cast<object>().ToArray();

			// Only return the fields if fields were found.
			if (fields != null && fields.Length != 0)
			{
				// Loop through the fields and return each one.
				for (int i = 0; i < fields.Length; i++)
				{
					yield return fields[i];
				}
			}

			// Only return the properties if properties were found.
			if (properties != null && properties.Length != 0)
			{
				// Loop through the properties and return each one if they have the right type.
				for (int i = 0; i < properties.Length; i++)
				{
					yield return properties[i];
				}
			}
		}

		/// <summary>
		/// Gets all the fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the fields allowed.</typeparam>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable<T> GetAllFields<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

			// If there are no properties, break.
			if (fields == null || fields.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < fields.Length; i++)
			{
				object currentValue = fields[i].GetValue(obj);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Returns all fields that are being serialized by Unity
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, bool unitySerialized = true)
		{
			ISerializationPolicy policy = unitySerialized ? OdinSerializer.SerializationPolicies.Unity : OdinSerializer.SerializationPolicies.Strict;
			return type.GetSerializedFields(policy);
		}

		/// <summary>
		/// Returns all fields that are being serialized
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type, ISerializationPolicy policy)
		{
			MemberInfo[] members = OdinSerializer.FormatterUtilities.GetSerializableMembers(type, policy);
			return members.OfType<FieldInfo>().ToArray();
		}


		/// <summary>
		/// Gets all the fields in obj.
		/// </summary>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable GetAllFields(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

			// If there are no properties, break.
			if (fields == null || fields.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < fields.Length; i++)
			{
				yield return fields[i].GetValue(obj);
			}
		}

		/// <summary>
		/// Gets all the properties in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the properties allowed.</typeparam>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable<T> GetAllProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

			// If there are no properties, break.
			if (properties == null || properties.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < properties.Length; i++)
			{
				object currentValue = properties[i].GetValue(obj, null);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Gets all the properties in obj.
		/// </summary>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable GetAllProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

			// If there are no properties, break.
			if (properties == null || properties.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < properties.Length; i++)
			{
				yield return properties[i].GetValue(obj, null);
			}
		}

	}
}

