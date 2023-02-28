using UnityEngine;
using UnityEditor;
using Stratus.Data;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(Symbol.Reference))]
	public class StratusSymbolReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var keyProperty = property.FindPropertyRelative(nameof(Symbol.Reference.key));
			var typeProperty = property.FindPropertyRelative(nameof(Symbol.Reference.type));

			label = EditorGUI.BeginProperty(position, label, property);
			{
				Rect contentPosition = EditorGUI.PrefixLabel(position, label);
				var width = contentPosition.width;
				EditorGUI.indentLevel = 0;
				// Key
				contentPosition.width = width * 0.60f;
				EditorGUI.PropertyField(contentPosition, keyProperty, GUIContent.none);
				contentPosition.x += contentPosition.width + 4f;
				// Value
				contentPosition.width = width * 0.40f;
				EditorGUI.PropertyField(contentPosition, typeProperty, GUIContent.none);

			}
			EditorGUI.EndProperty();

		}

	}
}