using System;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;
using System.Collections.Generic;
using Stratus.Events;

namespace Stratus
{

	/// <summary>
	/// Allows you to select any registered events within the Editor
	/// </summary>
	[Serializable]
	public class StratusEventField
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		[Header("Event")]
		[Tooltip("The scope of the event")]
		public Events.Event.Scope scope;
		[Tooltip("The GameObjects which we want to dispatch the event to")]
		public List<GameObject> targets = new List<GameObject>();
		[ClassExtends(typeof(Events.Event), Grouping = ClassGrouping.ByNamespace)]
		[Tooltip("What type of event this trigger will activate on")]
		public ClassTypeReference type = new ClassTypeReference();
		[SerializeField]
		private string eventData = string.Empty;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasType => type.Type != null;
		private Events.Event eventInstance { get; set; }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Dispatches the event onto the given target
		/// </summary>
		/// <returns></returns>
		public bool Dispatch()
		{
			if (!hasType)
				return false;

			if (eventInstance == null)
				eventInstance = Events.Event.Instantiate(type, eventData);

			switch (scope)
			{
				case Events.Event.Scope.Target:
					foreach (var target in targets)
					{
						if (target)
							target.Dispatch(eventInstance, type.Type);
					}
					break;
				case Events.Event.Scope.All:
					StratusScene.Dispatch(eventInstance, type.Type);
					break;
			}
			return true;
		}





	}

}