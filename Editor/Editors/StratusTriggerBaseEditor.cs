using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusTriggerBase), true), CanEditMultipleObjects]
	public abstract class StratusTriggerBaseEditor<T> : StratusBehaviourEditor<T> where T : StratusTriggerBase
	{
		abstract internal void OnTriggerBaseEditorEnable();

		protected override void OnStratusEditorEnable()
		{
			// Custom description support
			SerializedProperty descriptionModeProperty = propertyMap[nameof(StratusTriggerBase.descriptionMode)];
			propertyConstraints.Add(descriptionModeProperty, False);
			SerializedProperty descriptionProperty = propertyMap[nameof(StratusTriggerBase.description)];

			propertyDrawOverrides.Add(descriptionProperty, (SerializedProperty property) =>
			{
				EditorGUILayout.BeginHorizontal();
				if (target.descriptionMode == StratusTriggerBase.DescriptionMode.Automatic)
				{
			  //EditorGUILayout.SelectableLabel(target.automaticDescription, GUILayout.ExpandWidth(true));
			  GUI.enabled = false;
					EditorGUILayout.PropertyField(property, true, GUILayout.ExpandWidth(true));
					GUI.enabled = true;
				}
				else if (target.descriptionMode == StratusTriggerBase.DescriptionMode.Manual)
				{
					EditorGUILayout.PropertyField(property, true, GUILayout.ExpandWidth(true));
				}
				EditorGUILayout.PropertyField(descriptionModeProperty, GUIContent.none, true, GUILayout.Width(85));
				EditorGUILayout.EndHorizontal();
			});

			UpdateDescription();

			OnTriggerBaseEditorEnable();
		}

		protected override void OnBehaviourEditorValidate()
		{
			if (target == null)
				StratusDebug.Log($"Null at {GetType().Name}");

			UpdateDescription();
		}

		private void UpdateDescription()
		{
			if (target.descriptionMode == StratusTriggerBase.DescriptionMode.Automatic)
				target.description = target.automaticDescription;
		}

	}

}