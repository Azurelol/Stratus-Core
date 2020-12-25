using UnityEditor;

using UnityEngine;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(StratusTagField))]
	public class StratusTagFieldDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty tag = property.FindPropertyRelative("tag");

			EditorGUI.BeginProperty(position, label, tag);
			tag.stringValue = EditorGUI.TagField(position, label, tag.stringValue);
			EditorGUI.EndProperty();
		}

	}
}