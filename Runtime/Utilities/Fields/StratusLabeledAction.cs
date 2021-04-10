using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public class StratusLabeledAction
	{
		public string label { get; private set; }
		public Action action { get; private set; }

		public StratusLabeledAction(string label, Action action)
		{
			this.label = label;
			this.action = action;
		}

		public static implicit operator Action(StratusLabeledAction action) => action.action;

		/// <summary>
		/// Constructs an action that will reveal the given file/directory path
		/// </summary>
		public static StratusLabeledAction RevealPath(string path)
		{
			return new StratusLabeledAction("Reveal", () => StratusIO.Open(path));
		}
	}

	public struct StratusLabeledContextAction<T> where T : class
	{
		public string label;
		public Action action;
		public T context;

		public StratusLabeledContextAction(string label, Action action, T context)
		{
			this.label = label;
			this.action = action;
			this.context = context;
		}
	}
}