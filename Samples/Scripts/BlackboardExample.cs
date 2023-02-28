using Stratus.Data;

using UnityEngine;

namespace Stratus.Samples
{
	public class BlackboardExample : StratusBehaviour
	{
		[Header("Default")]
		public BlackboardScriptable blackboard;
		public Blackboard.Scope scope;
		public string key = "Dogs";
		public int intValue = 5;

		[Header("Selector")]
		public Blackboard.Selector selector = new Blackboard.Selector();
		public StratusRuntimeMethodField runtimeMethod;


		private void Awake()
		{
			runtimeMethod = new StratusRuntimeMethodField(GetValue, GetValueWithSelector, SetValue);
			blackboard.blackboard.onLocalSymbolChanged += OnLocalSymbolChanged;
			blackboard.blackboard.onGlobalSymbolChanged += OnGlobalSymbolChanged;
		}

		// Examples -----------------------------------------------------------
		//--------------------------------------------------------------------/
		// Retrieving Values
		//--------------------------------------------------------------------/
		private void GetValue()
		{
			object value = null;
			switch (scope)
			{
				case Blackboard.Scope.Local:
					value = blackboard.blackboard.GetLocal(gameObject, key);
					break;
				case Blackboard.Scope.Global:
					value = blackboard.blackboard.GetGlobal(key);
					break;
			}
			StratusDebug.Log($"The value of {key} is {value}", this);
		}

		private void GetValueWithSelector()
		{
			object value = selector.Get(gameObject);
			StratusDebug.Log($"The value of {selector.key} is {value}", this);
		}

		//--------------------------------------------------------------------/
		// Setting Values
		//--------------------------------------------------------------------/
		private void SetValue()
		{
			// Example of how such a value would be set...
			switch (scope)
			{
				case Blackboard.Scope.Local:
					// ... to the table for local symbols in the blackboard, instantiated for
					// each GameObject on access
					blackboard.blackboard.SetLocal(gameObject, key, intValue);
					break;
				case Blackboard.Scope.Global:
					// ... to the table for global symbols in the blackboard
					blackboard.blackboard.SetGlobal(key, intValue);
					break;
			}

		}

		private void SetValueWithSelector()
		{
			// Example of how such a value would be set using a selector
			selector.Set(gameObject, intValue);
		}

		//--------------------------------------------------------------------/
		// Callbacks
		//--------------------------------------------------------------------/
		private void OnLocalSymbolChanged(object gameObject, Symbol symbol)
		{
			StratusDebug.Log($"The value on local symbol {symbol.key} on the GameObject {gameObject} was changed to {symbol.value}", this);
		}

		private void OnGlobalSymbolChanged(Symbol symbol)
		{
			StratusDebug.Log($"The value on global symbol {symbol} was changed to {symbol.value}", this);
		}

	}
}
