using UnityEngine;
using System.Collections.Generic;
using Stratus.Data;

namespace Stratus
{
	/// <summary>
	/// An asset containing a blackboard
	/// </summary>
	[CreateAssetMenu(fileName = "Blackboard", menuName = "Stratus/Blackboard")]
	public class BlackboardScriptable : StratusScriptable, IBlackboard
	{
		/// <summary>
		/// The blackboard object
		/// </summary>
		public Blackboard blackboard;

		/// <summary>
		/// Identifier for this particular blackboard at runtime
		/// </summary>
		public int id { get; set; }		

		/// <summary>
		/// A map of all blackboard instances. This is used to share globals among instances of 
		/// specific blackboards.
		/// </summary>
		private static Dictionary<int, BlackboardScriptable> instances = new();

		/// <summary>
		/// Returns an instance of this blackboard asset, making a copy of its locals
		/// and using a reference to an unique (static) instance for its globals
		/// (so that they can be shared among all agents using that blackboard)
		/// </summary>
		/// <returns></returns>
		public BlackboardScriptable Instantiate()
		{
			// Get the ID of this asset
			id = this.GetInstanceID();

			// If an instance of this blackboard has not already been instantiated, add it
			if (!instances.ContainsKey(id))
			{
				instances.Add(id, this);
			}

			// Now create a new blackboard instance, giving it its own local copy
			// and using a reference to the shared one
			var blackboard = ScriptableObject.CreateInstance<BlackboardScriptable>();
			blackboard.blackboard.globals = instances[id].blackboard.globals;
			blackboard.blackboard.locals = new SymbolTable(this.blackboard.locals);
			return blackboard;
		}
	}


}