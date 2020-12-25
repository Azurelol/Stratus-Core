using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(StratusSceneField))]
	public class StratusSceneFieldDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty sceneAssetProp = property.FindPropertyRelative(nameof(StratusSceneField.sceneAsset));

			EditorGUI.BeginProperty(position, label, sceneAssetProp);
			EditorGUI.PropertyField(position, sceneAssetProp, label);
			EditorGUI.EndProperty();
		}
	}
}