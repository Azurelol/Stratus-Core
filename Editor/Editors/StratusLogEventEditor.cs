using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusLogTriggerable))]
	public class LogEventEditor : TriggerableEditor<StratusLogTriggerable>
	{
		protected override void OnTriggerableEditorEnable()
		{
			SerializedProperty descriptionProperty = propertyMap[nameof(StratusTriggerBase.description)];
			propertyDrawOverrides.Remove(descriptionProperty);
		}

	}

}