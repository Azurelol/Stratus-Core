using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus.Extensions;

namespace Stratus
{
	public abstract class StratusAttribute : Attribute
	{
	}

	public abstract class StratusInspectorAttribute : StratusAttribute
	{
		/// <summary>
		/// The name to be displayed in the inspector
		/// </summary>
		public string label { get; set; }

		public bool hasLabel => label.IsValid();
	}


}