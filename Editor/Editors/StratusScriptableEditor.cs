using System;

using UnityEngine;

namespace Stratus.Editor
{
	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class StratusScriptableEditor<T> : StratusEditor where T : ScriptableObject
	{
		public override bool drawTypeLabels => false;
		protected new T target { get; private set; }
		protected override Type baseType => typeof(ScriptableObject);

		protected virtual void OnScriptableEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.target = base.target as T;
			drawTypeLabels = true;
		}

		internal override void OnGenericStratusEditorValidate()
		{
			if (this.target)
			{
				this.OnScriptableEditorValidate();
			}
		}
	}

}