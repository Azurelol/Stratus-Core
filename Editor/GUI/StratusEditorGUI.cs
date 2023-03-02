using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Reflection;

using System;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Stratus.Editor
{
	/// <summary>
	/// Provides methods using Unity's <see cref="EditorGUI"/> annd other GUI-drawing functions that work directly with <see cref="Rect"/>
	/// </summary>
	public static class StratusEditorGUI
	{
		#region Properties
		public static float standardPadding { get; } = 8f; 
		#endregion

		#region Methods
		public static void Popup(Rect position, string label, DropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(position, label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void Popup(Rect position, DropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(position, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void Popup(Rect position, string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
		{
			StratusSearchablePopup.Popup(position, label, selectedindex, displayedOptions, onSelected);
		}

		public static void Field(Rect position, FieldInfo field, object target)
		{
			StratusSerializedFieldType propertyType = SerializedFieldTypeExtensions.Deduce(field);
			string name = ObjectNames.NicifyVariableName(field.Name);
			object value = null;
			switch (propertyType)
			{
				case StratusSerializedFieldType.ObjectReference:
					value = EditorGUI.ObjectField(position, name, (UnityEngine.Object)field.GetValue(target), field.FieldType, true);
					break;
				case StratusSerializedFieldType.Integer:
					value = EditorGUI.IntField(position, field.GetValue<int>(target));
					break;
				case StratusSerializedFieldType.Boolean:
					value = EditorGUI.Toggle(position, name, field.GetValue<bool>(target));
					break;
				case StratusSerializedFieldType.Float:
					value = EditorGUI.FloatField(position, field.GetValue<float>(target));
					break;
				case StratusSerializedFieldType.String:
					value = EditorGUI.TextField(position, name, field.GetValue<string>(target));
					break;
				case StratusSerializedFieldType.Color:
					value = EditorGUI.ColorField(position, name, field.GetValue<Color>(target));
					break;
				case StratusSerializedFieldType.Enum:
					StratusSearchableEnum.EnumPopup(position, name, field.GetValue<Enum>(target), (Enum selected) => field.SetValue(target, selected));
					break;
				case StratusSerializedFieldType.Vector2:
					value = EditorGUI.Vector2Field(position, name, field.GetValue<Vector2>(target));
					break;
				case StratusSerializedFieldType.Vector3:
					value = EditorGUI.Vector3Field(position, name, field.GetValue<Vector3>(target));
					break;
				case StratusSerializedFieldType.Vector4:
					value = EditorGUI.Vector4Field(position, name, field.GetValue<Vector4>(target));
					break;
				case StratusSerializedFieldType.Rect:
					value = EditorGUI.RectField(position, name, field.GetValue<Rect>(target));
					break;
				default:
					EditorGUI.LabelField(position, $"No supported drawer for type {field.FieldType.Name}!");
					break;
			}
		}

		public static void TextField(Rect position, string label, ref string value)
		{
			value = EditorGUI.TextField(position, label, value);
		}

		public static bool ObjectFieldWithHeader<T>(Rect rect, ref T objectField, string label) where T : UnityEngine.Object
		{
			GUILayout.Label(label, StratusGUIStyles.header);
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUI.ObjectField(rect, objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}
		#endregion
	}

}