using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Stratus.Types;
using Stratus.Events;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusDispatchEventTriggerable))]
	public class DispatchEventEditor : TriggerableEditor<StratusDispatchEventTriggerable>
	{
		private Events.Event eventObject;
		private StratusSerializedEditorObject serializedEvent;
		private Type type => triggerable.type.Type;
		private SerializedProperty eventDataProperty;

		protected override void OnTriggerableEditorEnable()
		{
			AddConstraint(() => triggerable.eventScope == Events.Event.Scope.Target, nameof(StratusDispatchEventTriggerable.targets));
			eventDataProperty = serializedObject.FindProperty("eventData");
			drawGroupRequests.Add(new DrawGroupRequest(SetMembers, () => triggerable.hasType && serializedEvent != null && serializedEvent.drawer.isDrawable));
			propertyChangeCallbacks.Add(propertyMap[nameof(StratusDispatchEventTriggerable.type)], OnEventChanged);

			if (triggerable.hasType)
				OnEventChanged();
		}

		private void SetMembers(Rect rect)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);

			if (serializedEvent.DrawEditorGUILayout())
				serializedEvent.Serialize(target, eventDataProperty);
		}

		private void OnEventChanged()
		{
			endOfFrameRequests.Add(UpdateEventObject);
		}

		void UpdateEventObject()
		{
			if (!triggerable.hasType)
				return;

			eventObject = (Events.Event)ObjectUtility.Instantiate(type);
			serializedEvent = new StratusSerializedEditorObject(eventObject);
			serializedEvent.Deserialize(eventDataProperty);
		}

	}
}