using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusLogEvent))]
	public class LogEventEditor : TriggerableEditor<StratusLogEvent>
	{
		protected override void OnTriggerableEditorEnable()
		{
			SerializedProperty descriptionProperty = propertyMap[nameof(StratusTriggerBase.description)];
			propertyDrawOverrides.Remove(descriptionProperty);
		}

	}

}