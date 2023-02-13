using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stratus.Extensions;
using Stratus.Reflection;

namespace Stratus
{
	/// <summary>
	/// An abstract model for a property serializd either by Unity or by
	/// custom serialization
	/// </summary>
	public class StratusSerializedPropertyModel
	{
		public enum SerializationType
		{
			Unity,
			Custom
		}

		public struct Query
		{
			public StratusSerializedPropertyModel[] models;
			public SerializedProperty[] unitySerialized;
			public StratusSerializedField[] customSerialized;

			public Query(SerializedObject serializedObject, Type declaringType)
			{
				FieldInfo[] fields = declaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				List<StratusSerializedPropertyModel> propertyModels = new List<StratusSerializedPropertyModel>();
				List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
				List<StratusSerializedField> serializedFields = new List<StratusSerializedField>();

				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo field = fields[i];
					if (field != null && (field.Attributes != FieldAttributes.NotSerialized))
					{
						StratusSerializedPropertyModel propertyModel = null;

						// Unity
						bool serializedByUnity = OdinSerializer.UnitySerializationUtility.GuessIfUnityWillSerialize(field);
						if (serializedByUnity)
						{
							SerializedProperty property = serializedObject.FindProperty(field.Name);
							if (property != null)
							{
								serializedProperties.Add(property);
								propertyModel = new StratusSerializedPropertyModel(property, field);
							}
						}
						// Odin
						if (propertyModel == null)
						{
							var isSerializable = field.FieldType.HasAttribute<SerializableAttribute>();
							if (isSerializable)
							{
								bool serializedbyOdin = OdinSerializer.UnitySerializationUtility.OdinWillSerialize(field, true);
								if (serializedbyOdin)
								{
									StratusSerializedField serializedField = new StratusSerializedField(field, serializedObject.targetObject);
									serializedFields.Add(serializedField);
									propertyModel = new StratusSerializedPropertyModel(serializedField);
								}
							}

						}

						if (propertyModel != null)
						{
							propertyModels.Add(propertyModel);
						}

					}
				}

				this.models = propertyModels.ToArray();
				this.unitySerialized = serializedProperties.ToArray();
				this.customSerialized = serializedFields.ToArray();
			}

		}

		public SerializationType type { get; private set; }
		public SerializedProperty unitySerialization { get; private set; }
		public StratusSerializedField customSerialization { get; private set; }
		public FieldInfo field { get; private set; }
		public bool isExpanded
		{
			get => (this.type == SerializationType.Unity) ? this.unitySerialization.isExpanded : this.customSerialization.isExpanded;

			set
			{
				switch (this.type)
				{
					case SerializationType.Unity:
						this.unitySerialization.isExpanded = value;
						break;
					case SerializationType.Custom:
						this.customSerialization.isExpanded = value;
						break;
				}
			}
		}

		public Attribute[] attributes => field.GetAttributes().ToArray();

		public string displayName => (this.type == SerializationType.Unity) ? this.unitySerialization.displayName : this.customSerialization.displayName;

		public StratusSerializedPropertyModel(SerializedProperty serializedProperty, FieldInfo field)
		{
			this.unitySerialization = serializedProperty;
			this.field = field;
			this.type = SerializationType.Unity;
		}

		public StratusSerializedPropertyModel(StratusSerializedField serializedField)
		{
			this.customSerialization = serializedField;
			this.field = serializedField.field;
			this.type = SerializationType.Custom;
		}
	}

	

}