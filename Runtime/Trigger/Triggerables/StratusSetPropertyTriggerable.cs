namespace Stratus
{
	/// <summary>
	/// Provides the ability to provide changes to a specified MonoBehaviour's properties at runtime
	/// </summary>
	public class StratusSetPropertyTriggerable : StratusTriggerable
	{
		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/    
		
		public StratusMemberSetterField[] setters;

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{
		}

		protected override void OnTrigger()
		{
			foreach (var property in setters)
				property.Set(this);
		}

	}
}