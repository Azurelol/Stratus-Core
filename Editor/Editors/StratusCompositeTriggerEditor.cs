using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusCompositeTrigger))]
	public class CompositeTriggerEditor : TriggerEditor<StratusCompositeTrigger>
	{
		protected override void OnTriggerEditorEnable()
		{
			propertyConstraints.Add(propertyMap["triggers"], () => trigger.type == StratusCompositeTrigger.Type.Trigger);
			propertyConstraints.Add(propertyMap["triggerables"], () => trigger.type == StratusCompositeTrigger.Type.Triggerable);
		}

		protected override bool DrawDeclaredProperties()
		{
			bool changed = false;

			EditorGUI.BeginChangeCheck();
			DrawSerializedProperty(nameof(StratusCompositeTrigger.type));
			DrawSerializedProperty(nameof(StratusCompositeTrigger.criteria));

			if (trigger.criteria == StratusCompositeTrigger.Criteria.Subset)
				trigger.needed = EditorGUILayout.IntSlider(trigger.needed, 1, trigger.count);

			if (trigger.type == StratusCompositeTrigger.Type.Trigger)
			{
				DrawSerializedProperty(nameof(StratusCompositeTrigger.triggers));
			}
			else if (trigger.type == StratusCompositeTrigger.Type.Triggerable)
			{
				DrawSerializedProperty(nameof(StratusCompositeTrigger.triggerables));
			}
			changed = EditorGUI.EndChangeCheck();
			return changed;
		}

	}

}