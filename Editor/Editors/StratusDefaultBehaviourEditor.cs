using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StratusBehaviour), true)]
	public class StratusDefaultBehaviourEditor : StratusBehaviourEditor<StratusBehaviour>
	{
		protected override void OnStratusEditorEnable()
		{
		}
	}

}