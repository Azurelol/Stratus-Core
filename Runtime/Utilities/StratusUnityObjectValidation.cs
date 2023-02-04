using System;
using System.Reflection;

using Stratus.Extensions;
using Stratus.Reflection;
using Stratus.Utilities;

using UnityEngine;

namespace Stratus
{
	public class StratusUnityObjectValidation : StratusObjectValidation
	{
		public StratusUnityObjectValidation(Level type, object target, Func<bool> onValidate = null) : base(type, target, onValidate)
		{
		}

		public StratusUnityObjectValidation(string message, Level type, object target, Func<bool> onValidate = null) : base(message, type, target, onValidate)
		{
		}

		public StratusUnityObjectValidation(string message, Level type, Action onSelect, Func<bool> onValidate = null) : base(message, type, onSelect, onValidate)
		{
		}

		public static StratusObjectValidation NullReference(Behaviour behaviour, string description = null)
		{
			FieldInfo[] nullFields = UnityStratusReflection.GetFieldsWithNullReferences(behaviour);
			if (nullFields.IsNullOrEmpty())
				return null;

			string label = behaviour.GetType().Name;
			if (description != null)
				label += $" {description}";

			string msg = $"{label} has the following fields currently set to null:";
			foreach (var f in nullFields)
				msg += $"\n - <i>{f.Name}</i>";
			StratusObjectValidation validation = new StratusObjectValidation(msg, Level.Warning, behaviour);
			return validation;
		}
	}
}