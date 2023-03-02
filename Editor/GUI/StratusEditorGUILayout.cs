using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using Stratus.Collections;
using Stratus.Utilities;

namespace Stratus.Editor
{
	/// <summary>
	/// Provides methods using Unity's <see cref="GUILayout"/> and <see cref="EditorGUILayout"/>
	/// </summary>
	public static class StratusEditorGUILayout
	{
		public enum ContextMenuType
		{
			Add,
			Validation,
			Options
		}

		#region Properties
		private static TextAlignment currentAlignGroup { get; set; }
		private static bool alignGroupActive { get; set; } 
		#endregion

		public static void BeginAligned(TextAlignment alignment)
		{
			currentAlignGroup = alignment;
			GUILayout.BeginHorizontal();
			switch (alignment)
			{
				case TextAlignment.Left:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					GUILayout.FlexibleSpace();
					break;
			}
			alignGroupActive = true;
		}

		public static void EndAligned()
		{
			if (!alignGroupActive)
			{
				throw new ArgumentException("Missing a matching BeginAligned call!");
			}

			switch (currentAlignGroup)
			{
				case TextAlignment.Left:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					break;
			}
			GUILayout.EndHorizontal();
			alignGroupActive = false;
		}

		public static void Popup(string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
		{
			StratusSearchablePopup.Popup(label, selectedindex, displayedOptions, onSelected);
		}

		public static void Popup(string label, DropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void Popup(DropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static int Popup(string label, int selectedindex, string[] displayedOptions)
		{
			return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
		}

		/// <summary>
		/// Draws a field using our own drawer system
		/// <returns>True if the field was changed</returns>
		public static bool Field(FieldInfo field, object target)
		{
			return StratusSerializedEditorObject.GetFieldDrawer(field).DrawEditorGUILayout(target);
		}

		/// <summary>
		/// Modifies the given property on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool ModifyProperty(UnityEngine.Object obj, string propertyName, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(prop, options);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Modifies the given property on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool ModifyProperty(UnityEngine.Object obj, string propertyName, GUIContent label, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(prop, label, options);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Modifies all the given properties on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static void ModifyProperties(UnityEngine.Object obj, string[] propertyNames, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			EditorGUI.BeginChangeCheck();
			foreach (string name in propertyNames)
			{
				SerializedProperty prop = serializedObject.FindProperty(name);
				EditorGUILayout.PropertyField(prop, options);
			}
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		public static bool ObjectFieldWithHeader<T>(ref T objectField, string label) where T : UnityEngine.Object
		{
			EditorGUILayout.LabelField(label, StratusGUIStyles.header);
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}

		public static bool Button(string label, System.Action onClick)
		{
			if (GUILayout.Button(label, StratusGUIStyles.button))
			{
				onClick();
				return true;
			}
			return false;
		}

		public static void TextField(string label, ref string value)
		{
			value = EditorGUILayout.TextField(label, value);
		}

		public static bool ObjectField<T>(ref T objectField) where T : UnityEngine.Object
		{
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}

		public static void EnumToolbar<T>(ref T enumValue) where T : Enum
		{
			string[] options = EnumUtility.Names<T>();
			enumValue = (T)(object)GUILayout.Toolbar(Convert.ToInt32(enumValue), options, GUILayout.ExpandWidth(false));
		}

		public static bool Property(SerializedProperty serializedProperty, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedProperty.serializedObject.targetObject, serializedProperty.displayName);
				serializedProperty.serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		public static bool Property(SerializedProperty serializedProperty)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedProperty);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedProperty.serializedObject.targetObject, serializedProperty.displayName);
				serializedProperty.serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		public static string FolderPath(string title)
		{
			return EditorUtility.SaveFolderPanel(title, string.Empty, string.Empty);
		}


		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool Toggle(UnityEngine.Object obj, string propertyName, string label = null)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty property = serializedObject.FindProperty(propertyName);
			return Toggle(serializedObject, property, label);
		}

		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool Toggle(SerializedObject serializedObject, SerializedProperty prop, string label = null)
		{
			EditorGUI.BeginChangeCheck();
			GUIContent content = new GUIContent(label == null ? prop.displayName : label);
			prop.boolValue = GUILayout.Toggle(prop.boolValue, content);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
			return prop.boolValue;
		}

		/// <summary>
		/// Selects the subset of a given set of elements, organized vertically
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set"></param>
		/// <param name="subset"></param>
		public static void Subset<T>(T[] set, List<T> subset, Func<T, string> name)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Available", EditorStyles.centeredGreyMiniLabel);
					foreach (T element in set)
					{
						T matchingElement = subset.Find(x => x.Equals(element));
						if (matchingElement != null)
						{
							continue;
						}

						if (GUILayout.Button(name(element), EditorStyles.miniButton))
						{
							subset.Add(element);
						}
					}
				}
				EditorGUILayout.EndVertical();

				// Selected scenes
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Selected", EditorStyles.centeredGreyMiniLabel);
					int indexToRemove = -1;
					for (int i = 0; i < subset.Count; ++i)
					{
						T element = subset[i];
						if (GUILayout.Button(name(element), EditorStyles.miniButton))
						{
							indexToRemove = i;
						}
					}
					if (indexToRemove > -1)
					{
						subset.RemoveAt(indexToRemove);
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Selects the subset of a given set of elements, organized vertically
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set"></param>
		/// <param name="subset"></param>
		public static void Subset<T>(T[] set, List<T> subset) where T : UnityEngine.Object
		{
			string getName(T obj) => obj.name;
			Subset(set, subset, getName);
		}

		public static void Header(string text)
		{
			EditorGUILayout.LabelField(text, StratusGUIStyles.header);
		}

		public static bool ObjectField(FieldInfo field, object obj, GUIContent content = null)
		{
			object value = field.GetValue(obj);
			EditorGUI.BeginChangeCheck();
			{
				string name = ObjectNames.NicifyVariableName(field.Name);

				if (value is UnityEngine.Object)
				{
					field.SetValue(obj, EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, field.FieldType, true));
				}
				else if (value is bool)
				{
					field.SetValue(obj, EditorGUILayout.Toggle(name, (bool)value));
				}
				else if (value is int)
				{
					field.SetValue(obj, EditorGUILayout.IntField(name, (int)value));
				}
				else if (value is float)
				{
					field.SetValue(obj, EditorGUILayout.FloatField(name, (float)value));
				}
				else if (value is string)
				{
					field.SetValue(obj, EditorGUILayout.TextField(name, (string)value));
				}
				else if (value is Enum)
				{
					field.SetValue(obj, EditorGUILayout.EnumPopup(name, (Enum)value));
				}
				else if (value is Vector2)
				{
					field.SetValue(obj, EditorGUILayout.Vector2Field(name, (Vector2)value));
				}
				else if (value is Vector3)
				{
					field.SetValue(obj, EditorGUILayout.Vector3Field(name, (Vector3)value));
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				return true;
			}

			return false;
		}

		public static void ContextMenu(GenericMenu menu, ContextMenuType context)
		{
			Texture texture = null;
			switch (context)
			{
				case ContextMenuType.Add:
					texture = StratusGUIStyles.addIcon;
					break;
				case ContextMenuType.Validation:
					texture = StratusGUIStyles.validateIcon;
					break;
				case ContextMenuType.Options:
					texture = StratusGUIStyles.optionsIcon;
					break;
			}
			if (GUILayout.Button(texture, StratusGUIStyles.smallLayout))
			{
				menu.ShowAsContext();
			}
		}

		public static void ContextMenu(Func<GenericMenu> menuFunction, ContextMenuType context)
		{
			ContextMenu(menuFunction(), context);
		}

		public static void FadeGroup(AnimBool show, string label, System.Action drawFunction)
		{
			show.target = EditorGUILayout.Foldout(show.target, label);
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				drawFunction();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void VerticalFadeGroup(AnimBool show, string label, System.Action drawFunction, GUIStyle verticalStyle = null, bool validate = true)
		{
			show.target = EditorGUILayout.Foldout(show.target, label) && validate;
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				EditorGUILayout.BeginVertical(verticalStyle);
				drawFunction();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void VerticalFadeGroup<T>(AnimBool show, string label, System.Action<T> drawFunction, T argument, GUIStyle verticalStyle = null, bool validate = true)
		{
			show.target = EditorGUILayout.Foldout(show.target, label) && validate;
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				EditorGUILayout.BeginVertical(verticalStyle);
				drawFunction(argument);
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void ListView<T>(IEnumerable<T> list, Func<T, GUIContent> leftContent, Func<T, string> rightContent,
										   GUILayoutOption leftWidth, GUILayoutOption rightWidth, GUILayoutOption height)
		{
			foreach (T element in list)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(leftContent(element), StratusGUIStyles.listViewLabel, leftWidth, height);
					EditorGUILayout.SelectableLabel(rightContent(element), StratusGUIStyles.textField, rightWidth, height);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public static void Aligned(Action drawFunction, TextAlignment alignment)
		{
			GUILayout.BeginHorizontal();

			switch (alignment)
			{
				case TextAlignment.Left:
					drawFunction();
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					drawFunction();
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					GUILayout.FlexibleSpace();
					drawFunction();
					break;
			}
			GUILayout.EndHorizontal();
		}

		public static void Rows(Rect[] rows, params Action<Rect>[] onRow)
		{
			if (rows.Length != onRow.Length)
			{
				throw new ArgumentException($"The amount of rows {rows.Length} and actions {onRow.Length} for them is not equal");
			}

			for (int i = 0; i < rows.Length; i++)
			{
				Rect row = rows[i];
				Action<Rect> action = onRow[i];
				Debug.Log($"{i} : {row}");
				
				//GUILayout.BeginArea(row);
				action(row);
				//GUILayout.EndArea();
			}
		}
	}
}