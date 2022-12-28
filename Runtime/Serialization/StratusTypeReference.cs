using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stratus
{
	/// <summary>
	/// Provides a serializable reference to a class <see cref="Type"/>
	/// </summary>
	[Serializable]
	public class StratusTypeReference : ISerializationCallbackReceiver
	{
		#region Fields
		[SerializeField]
		private string _reference; 
		#endregion

		#region Properties
		public Type type
		{
			get { return _type; }
			set
			{
				if (value != null && !value.IsClass)
					throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");

				_type = value;
				_reference = GetClassRef(value);
			}
		}
		private Type _type;
		#endregion

		#region Constructors
		public StratusTypeReference()
		{
		}

		public StratusTypeReference(string assemblyQualifiedName)
		{
			this.type = !string.IsNullOrEmpty(assemblyQualifiedName)
				? Type.GetType(assemblyQualifiedName)
				: null;
		}
		
		public StratusTypeReference(Type type)
		{
			this.type = type;
		}
		#endregion

		public static string GetClassRef(Type type)
		{
			return type != null
			  ? type.FullName + ", " + type.Assembly.GetName().Name
			  : "";
		}

		#region ISerializationCallbackReceiver Members
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (!string.IsNullOrEmpty(_reference))
			{
				_type = Type.GetType(_reference);

				if (_type == null)
				{
					Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", _reference));
				}
			}
			else
			{
				_type = null;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}
		#endregion

		#region Operators
		public static implicit operator string(StratusTypeReference typeReference)
		{
			return typeReference._reference;
		}

		public static implicit operator Type(StratusTypeReference typeReference)
		{
			return typeReference.type;
		}

		public static implicit operator StratusTypeReference(Type type)
		{
			return new StratusTypeReference(type);
		}
		#endregion
	}

	[Serializable]
	public class StratusTypeReferenceCollection<T> where T : class
	{
		public class Initializer
		{
			public HashSet<StratusTypeReference> values = new HashSet<StratusTypeReference>();
			public Initializer With<U>() where U : T
			{
				values.Add(typeof(U));
				return this;
			}
		}

		private static readonly Type baseType = typeof(T);

		private List<StratusTypeReference> _values = new List<StratusTypeReference>();
		public IReadOnlyList<StratusTypeReference> values => _values;

		public bool Add<U>() where U : T
		{
			var type = typeof(U);
			return Add(type);
		}

		public void AddRange(Initializer initializer)
		{
			foreach (var v in initializer.values)
			{
				_values.Add(v);
			}
		}

		public bool Add(Type type)
		{
			if (type.IsAssignableFrom(baseType))
			{
				return false;
			}

			if (Contains(type))
			{
				return false;
			}

			_values.Add(new StratusTypeReference(type));
			return true;
		}

		public bool Contains<U>() where U : T
		{
			return Contains(typeof(U));
		}

		public bool Contains(Type type)
		{
			return (_values.Find(r => r.type == type) != null);
		}

		public void Clear()
		{
			_values.Clear();
		}
	}
}