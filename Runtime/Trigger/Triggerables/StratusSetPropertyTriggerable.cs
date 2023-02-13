//namespace Stratus
//{
//	/// <summary>
//	/// Provides the ability to provide changes to a specified MonoBehaviour's properties at runtime
//	/// </summary>
//	public class StratusSetPropertyTriggerable : StratusTriggerableBehaviour
//	{
//		//--------------------------------------------------------------------------------------------/
//		// Fields
//		//--------------------------------------------------------------------------------------------/    
		
//		public StratusMemberSetterField[] setters;

//		//--------------------------------------------------------------------------------------------/
//		// Messages
//		//--------------------------------------------------------------------------------------------/
//		protected override void OnAwake()
//		{
//		}

//		protected override void OnReset()
//		{
//		}

//		protected override void OnTrigger(object data = null)
//		{
//			foreach (var property in setters)
//				property.Set(this);
//		}

//	}
//}