using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Utilities;
using System.IO;
using System.Linq;
using Stratus.Extensions;

namespace Stratus
{
	/// <summary>
	/// Utility class for managing assets at runtime
	/// </summary>
	public static partial class StratusAssetUtility
	{

		/// <summary>
		/// Currently loaded resources
		/// </summary>
		private static Dictionary<Type, Dictionary<string, UnityEngine.Object>> loadedResources = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();

		/// <summary>
		/// Loads the resources of the given type, then returns the one with the given name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns>The resource, if it has been found</returns>
		public static T GetResource<T>(string name) where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}

			T resource = (T)loadedResources[type][name];
			return resource;
		}

		/// <summary>
		/// Loads the resources of the given type, returning them if they were found
		/// </summary>
		public static IEnumerable<T> GetResources<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}
			var resources = loadedResources[type];
			return resources.Values.Select(x => (T)x);
		}

		/// <summary>
		/// Loads the resources of the given type, returning them if they were found
		/// </summary>
		public static Dictionary<string, T> GetResourceMap<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}
			var resources = loadedResources[type];
			return resources.TransformValues(x => (T)x);
		}

		/// <summary>
		/// Loads all resources of the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private static void AddResourcesOfType<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
			T[] resources = Resources.FindObjectsOfTypeAll<T>();
			foreach (var resource in resources)
			{
				StratusDebug.Log($"Loaded {resource.name}");
				loadedResources[type].Add(resource.name, resource);
			}

		}

		private static void AddResourcesOfType(Type type)
		{
			loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
			UnityEngine.Object[] resources = Resources.FindObjectsOfTypeAll(type);
			foreach (var resource in resources)
			{
				loadedResources[type].Add(resource.name, resource);
			}

		}





	}
}