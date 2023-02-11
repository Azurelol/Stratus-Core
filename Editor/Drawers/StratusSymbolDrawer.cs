using UnityEngine;
using UnityEditor;
using Stratus.Data;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(StratusSymbol))]
	public class StratusSymbolDrawer : PropertyDrawer
	{
		bool ShowSingleLine = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative(nameof(StratusSymbol.value));
			var typeProperty = valueProperty.FindPropertyRelative("type");
			var type = (StratusVariant.VariantType)typeProperty.enumValueIndex;

			label = EditorGUI.BeginProperty(position, label, property);

			if (ShowSingleLine)
			{
				Rect contentPosition = EditorGUI.PrefixLabel(position, label);
				var width = contentPosition.width;
				EditorGUI.indentLevel = 0;
				// Key
				contentPosition.width = width * 0.40f;
				EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative(nameof(StratusSymbol.key)), GUIContent.none);
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
				EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(StratusSymbol.key)));
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, valueProperty);

			}
			EditorGUI.EndProperty();
		}
	}

	public class SymbolReferenceDrawer2 : StratusSerializedEditorObject.CustomObjectDrawer<StratusSymbol.Reference>
	{
		protected override float GetHeight(StratusSymbol.Reference value)
		{
			return lineHeight;
		}

		protected override void OnDrawEditorGUI(Rect position, StratusSymbol.Reference value)
		{
			StratusEditorGUI.TextField(position, "Key", ref value.key);
		}

		protected override void OnDrawEditorGUILayout(StratusSymbol.Reference value)
		{
			StratusEditorGUILayout.TextField("Key", ref value.key);
		}
	}
}