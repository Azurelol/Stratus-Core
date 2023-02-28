using UnityEngine;
using UnityEditor;
using Stratus.Data;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(Symbol))]
	public class StratusSymbolDrawer : PropertyDrawer
	{
		bool ShowSingleLine = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative(nameof(Symbol.value));
			var typeProperty = valueProperty.FindPropertyRelative("type");
			var type = (VariantType)typeProperty.enumValueIndex;

			label = EditorGUI.BeginProperty(position, label, property);

			if (ShowSingleLine)
			{
				Rect contentPosition = EditorGUI.PrefixLabel(position, label);
				var width = contentPosition.width;
				EditorGUI.indentLevel = 0;
				// Key
				contentPosition.width = width * 0.40f;
				EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative(nameof(Symbol.key)), GUIContent.none);
				contentPosition.x += contentPosition.width + 4f;
				// Value
				contentPosition.width = width * 0.60f;
				EditorGUI.PropertyField(contentPosition, valueProperty, GUIContent.none);
			}
			else
			{
				EditorGUI.LabelField(position, label);
				//EditorGUI.indentLevel = 1;
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Symbol.key)));
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, valueProperty);

			}
			EditorGUI.EndProperty();
		}
	}

	public class SymbolReferenceDrawer2 : StratusSerializedEditorObject.CustomObjectDrawer<Symbol.Reference>
	{
		protected override float GetHeight(Symbol.Reference value)
		{
			return lineHeight;
		}

		protected override void OnDrawEditorGUI(Rect position, Symbol.Reference value)
		{
			StratusEditorGUI.TextField(position, "Key", ref value.key);
		}

		protected override void OnDrawEditorGUILayout(Symbol.Reference value)
		{
			StratusEditorGUILayout.TextField("Key", ref value.key);
		}
	}
}