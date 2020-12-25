using System;
using UnityEditor;

namespace Stratus.Editor
{
	/// <summary>
	/// An interface for selecting from Stratus.Events
	/// </summary>
	public class StratusEventTypeSelector : StratusTypeSelector
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private SerializedProperty eventDataProperty;
		private StratusEvent eventObject;
		private StratusSerializedEditorObject serializedEvent;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusEventTypeSelector() : base(typeof(StratusEvent), false, true)
		{
		}

		private StratusEventTypeSelector(Type baseEventType) : base(baseEventType, false, true)
		{
		}

		public StratusEventTypeSelector Construct<T>() where T : StratusEvent
		{
			Type type = typeof(T);
			return new StratusEventTypeSelector(type);
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnSelectionChanged()
		{
			base.OnSelectionChanged();
			eventObject = (StratusEvent)Utilities.StratusReflection.Instantiate(selectedClass);
			serializedEvent = new StratusSerializedEditorObject(eventObject);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Serialize(SerializedProperty stringProperty)
		{
			this.serializedEvent.Serialize(stringProperty);
		}

		public void EditorGUILayout(SerializedProperty stringProperty)
		{
			if (serializedEvent.DrawEditorGUILayout())
				this.Serialize(stringProperty);
		}
	}
}