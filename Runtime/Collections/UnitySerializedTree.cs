using Stratus.Models.Graph;

using UnityEngine;
//using Stratus.OdinSerializer;

//using UnityEngine;

namespace Stratus
{
	public class UnitySerializedTree<TreeElementType> : StratusSerializedTree<TreeElementType>, ISerializationCallbackReceiver
		where TreeElementType : TreeElement, new()
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.BuildRootFromElements();
		}
	}

}