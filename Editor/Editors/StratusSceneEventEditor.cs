using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusSceneTriggerable))]
	public class SceneEventEditor : TriggerableEditor<StratusSceneTriggerable>
	{
		StratusSceneTriggerable sceneEvent => target as StratusSceneTriggerable;

		protected override void OnTriggerableEditorEnable()
		{

		}

		protected override bool DrawDeclaredProperties()
		{
			bool changed = false;
			changed |= DrawSerializedProperty(declaredProperties.Item2[0].unitySerialization, serializedObject);
			if (sceneEvent.type == StratusSceneTriggerable.Type.Load || sceneEvent.type == StratusSceneTriggerable.Type.Unload)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[1].unitySerialization, serializedObject);
			}
			if (sceneEvent.type == StratusSceneTriggerable.Type.Load)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[2].unitySerialization, serializedObject);
			}
			return changed;
		}


	}
}