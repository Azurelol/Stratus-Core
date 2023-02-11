using Stratus.Models;

using UnityEngine;

namespace Stratus.Unity.Models
{
	public class UnityTreeElement<TData> : StratusTreeElement<TData>, ISerializationCallbackReceiver
		where TData : class, IStratusNamed
	{
		#region Messages
		public void OnBeforeSerialize()
		{
			if (hasData)
			{
				this.UpdateName();
			}
		}

		public void OnAfterDeserialize()
		{
			this.childrenData = this.GetChildrenData();
		}
		#endregion
	}
}