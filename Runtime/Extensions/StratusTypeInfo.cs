using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// Stores reflection information about a given type
	/// </summary>
	public class StratusTypeInfo
	{
		public Type type { get; private set; }
		public FieldInfo[] fields { get; private set; }
		public PropertyInfo[] properties { get; private set; }
		public MethodInfo[] methods { get; private set; }
		public Dictionary<string, FieldInfo> fieldsByName { get; private set; }
		public Dictionary<string, PropertyInfo> propertiesByName { get; private set; }
		public Dictionary<string, MethodInfo> methodsByName { get; private set; }

		public const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

		public StratusTypeInfo(Type type)
		{
			this.fields = type.GetFields(flags);
			this.fieldsByName = fields.ToDictionary((x) => x.Name, false);
			this.methods = type.GetMethods(flags);
			this.methodsByName = methods.ToDictionary((x) => x.Name, false);
			this.properties = type.GetProperties(flags);
			this.propertiesByName = properties.ToDictionary((x) => x.Name, false);
		}
	}

}