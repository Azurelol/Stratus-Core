using UnityEditor;

using UnityEngine;

namespace Stratus.Editor
{
	[CustomPropertyDrawer(typeof(StratusSymbolTable), true)]
	public class StratusSymbolTableDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty symbols = property.FindPropertyRelative("symbols");
			StratusReorderableList.List(symbols);
		}
	}
}