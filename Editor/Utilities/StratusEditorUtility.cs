using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Stratus.Extensions;
using Stratus.Reflection;

using UnityEditor;

using UnityEngine;

namespace Stratus.Editor
{
	public static partial class StratusEditorUtility
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/

		public delegate bool DefaultPropertyFieldDelegate(Rect position, SerializedProperty property, GUIContent label);

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static UnityEngine.Event currentEvent => UnityEngine.Event.current;
		public static bool currentEventUsed => currentEvent.type == EventType.Used;
		public static bool onRepaint => currentEvent.type == EventType.Repaint;
		public static Rect lastRect => GUILayoutUtility.GetLastRect();
		public static float lineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		public static float verticalSpacing => EditorGUIUtility.standardVerticalSpacing;
		public static float labelWidth => EditorGUIUtility.labelWidth;
		public static DefaultPropertyFieldDelegate defaultPropertyField { get; private set; }
		private static Dictionary<int, float> abstractListHeights { get; set; } = new Dictionary<int, float>();
		public static Rect lastEditorGUILayoutRect
		{
			get => ReflectionUtility.GetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout));
			set => ReflectionUtility.SetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout), value);
		}

		private static Assembly _unityEditorAssembly;
		public static Assembly UnityEditorAssembly
		{
			get
			{
				if (_unityEditorAssembly == null)
				{
					_unityEditorAssembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
				}

				return _unityEditorAssembly;
			}
		}

		private static Type _scriptAttributeUtility;
		public static Type scriptAttributeUtility
		{
			get
			{
				if (_scriptAttributeUtility == null)
				{
					_scriptAttributeUtility = UnityEditorAssembly.GetType("UnityEditor.ScriptAttributeUtility");
				}

				return _scriptAttributeUtility;
			}
		}

		private static TypeInformation _scriptAttributeReflection;
		public static TypeInformation scriptAttributeReflection
		{
			get
			{
				if (_scriptAttributeReflection == null)
				{
					_scriptAttributeReflection = new TypeInformation(scriptAttributeUtility);
				}
				return _scriptAttributeReflection;
			}
		}

		private static TypeInformation editorGUIReflection { get; } = new TypeInformation(typeof(EditorGUI));

		private static Stack<PropertyDrawer> _propertyDrawerStack;
		public static Stack<PropertyDrawer> propertyDrawerStack
		{
			get
			{
				if (_propertyDrawerStack == null)
				{
					_propertyDrawerStack = (Stack<PropertyDrawer>)scriptAttributeUtility.GetField("s_DrawerStack",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				}
				return _propertyDrawerStack;
			}
		}

		private static IDictionary _propertyDrawerTypeForType = null;
		public static IDictionary propertyDrawerTypeForType
		{
			get
			{
				if (_propertyDrawerTypeForType == null)
				{
					_propertyDrawerTypeForType = (IDictionary)scriptAttributeReflection.fieldsByName.GetValueOrDefault("s_DrawerTypeForType").GetValue(null);
				}

				return _propertyDrawerTypeForType;
			}
		}

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		static StratusEditorUtility()
		{
			Type t = typeof(EditorGUI);
			Type delegateType = typeof(DefaultPropertyFieldDelegate);
			MethodInfo m = t.GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);
			defaultPropertyField = (DefaultPropertyFieldDelegate)System.Delegate.CreateDelegate(delegateType, m);
		}

		public static void UseDefaultDrawer(Rect position, SerializedProperty property, GUIContent label, Type type)
		{
			defaultPropertyField(position, property, label);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static void OnMouseClick(Action onLeftClick, Action onRightClick, Action onDoubleClick, bool used = false)
		{
			if (!used && !currentEvent.isMouse)
			{
				return;
			}

			int button = currentEvent.button;

			// Left click
			if (button == 0)
			{
				if (currentEvent.clickCount == 1)
				{
					onLeftClick?.Invoke();
				}
				else if (currentEvent.clickCount > 1)
				{
					onDoubleClick?.Invoke();
				}

				currentEvent.Use();
			}
			// Right click
			else if (button == 1)
			{
				onRightClick?.Invoke();
				currentEvent.Use();
			}
		}

		public static void OnLastControlMouseClick(Action onLeftClick, Action onRightClick, Action onDoubleClick = null)
		{
			if (!IsMousedOver(GUILayoutUtility.GetLastRect()))
			{
				return;
			}

			OnMouseClick(onLeftClick, onRightClick, onDoubleClick);
		}

		/// <summary>
		/// Checks whether the mouse was within the boundaries of the last control
		/// </summary>
		/// <returns></returns>
		public static bool IsLastControlMousedOver()
		{
			Rect rect = GUILayoutUtility.GetLastRect();
			return IsMousedOver(rect);
		}

		/// <summary>
		/// Checks whether the mouse was within the boundaries of the last control
		/// </summary>
		/// <returns></returns>
		public static bool IsMousedOver(Rect rect)
		{
			return rect.Contains(Event.current.mousePosition);
		}

		/// <summary>
		/// Returns true if a GUI control was changed within the procedure
		/// </summary>
		/// <param name="procedure"></param>
		/// <returns></returns>
		public static bool CheckControlChange(Action procedure)
		{
			EditorGUI.BeginChangeCheck();
			procedure();
			return EditorGUI.EndChangeCheck();
		}

		public static void OnMouseClick(Rect rect, Action onLeftClick, Action onRightClick, Action onDoubleClick = null)
		{
			if (!IsMousedOver(rect))
			{
				return;
			}

			OnMouseClick(onLeftClick, onRightClick, onDoubleClick);
		}

		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="action"></param>
		/// <param name="obj"></param>
		public static void SaveOnControlChange(UnityEngine.Object obj, Action action)
		{
			EditorGUI.BeginChangeCheck();
			action();
			if (EditorGUI.EndChangeCheck())
			{
				SerializedObject serializedObject = new SerializedObject(obj);
				serializedObject.UpdateIfRequiredOrScript();
				serializedObject.ApplyModifiedProperties();
				UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
				StratusDebug.Log($"Saving change on {obj.name}");
			}
		}

		/// <summary>
		/// Finds the serialized property from a given object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static SerializedProperty FindSerializedProperty(UnityEngine.Object obj, string propertyName)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			return prop;
		}

		/// <summary>
		/// Disables mouse selection behind the given rect
		/// </summary>
		/// <param name="rect"></param>
		public static void DisableMouseSelection(Rect rect)
		{
			if (IsMousedOver(rect))
			{
				int control = GUIUtility.GetControlID(FocusType.Passive);
				GUIUtility.hotControl = control;
			}
		}

		/// <summary>
		/// Add define symbols as soon as Unity gets done compiling.
		/// </summary>
		public static void AddDefineSymbols(string[] symbols)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = new List<string>(definesString.Split(';'));
			allDefines.AddRange(symbols.Except(allDefines));
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				EditorUserBuildSettings.selectedBuildTargetGroup,
				string.Join(";", allDefines.ToArray()));
		}				

		public static Rect Pad(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			return Pad(rect, padding);
		}

		public static Rect Pad(Rect rect, float padding)
		{
			rect.y += padding;
			rect.height -= padding;
			rect.x += padding;
			rect.width -= padding;
			return rect;
		}

		public static Rect PadVertical(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			return PadVertical(rect, padding);
		}

		public static Rect PadVertical(Rect rect, float padding)
		{
			rect.y += padding;
			rect.height -= padding;
			return rect;
		}

		public static Rect RaiseVertical(Rect rect, float padding)
		{
			rect.y -= padding;
			rect.height += padding;
			return rect;
		}

		public static Rect PadHorizontal(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			rect.x += padding;
			rect.width -= padding;
			return rect;
		}

		public static bool LabelHasContent(GUIContent label)
		{
			if (label == null)
			{
				return true;
			}
			return label.text != string.Empty || label.image != null;
		}

		public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (float)ReflectionUtility.GetReflectedMethod("GetSinglePropertyHeight", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property, label });
		}

		internal static bool HasVisibleChildFields(SerializedProperty property)
		{
			return (bool)ReflectionUtility.GetReflectedMethod("HasVisibleChildFields", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property });
		}

		internal static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
		{
			return (bool)ReflectionUtility.GetReflectedMethod("DefaultPropertyField", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { position, property, label });
		}

		internal static Rect GetToggleRect(bool hasLabel, params GUILayoutOption[] options)
		{
			return (Rect)ReflectionUtility.GetReflectedMethod("GetToggleRect", typeof(UnityEditor.EditorGUILayout)).Invoke(null, new object[] { hasLabel, options });
		}

		internal static GUIContent TempContent(string t)
		{
			BindingFlags bindflags = BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo method = typeof(UnityEditor.EditorGUIUtility).GetMethod("TempContent", bindflags, null, new[] { typeof(string) }, null);
			return (GUIContent)method.Invoke(null, new[] { t });
		}
	}
}