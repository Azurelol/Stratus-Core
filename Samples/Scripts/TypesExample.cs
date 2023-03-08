using Stratus.Data;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	namespace Examples
	{
		/// <summary>
		/// A simple component showcasing the most useful types provided by the Stratus library
		/// </summary>
		public class TypesExample : StratusBehaviour
		{
			[Header("Stratus Field Types")]
			public StratusSceneField scene = new StratusSceneField();
			public StratusTagField tagField = new StratusTagField();
			public FloatRange floatRange = new FloatRange();
			public StratusIntegerRange intRange = new StratusIntegerRange();
			public BoundedFloat variable = new BoundedFloat();
			public KeyCode enumDrawer;

			public StratusLayerField layer = new StratusLayerField();

			void TryLoadingScene()
			{
				scene.Load(UnityEngine.SceneManagement.LoadSceneMode.Single);
			}

			void TryTag()
			{
				if (gameObject.CompareTag(tagField))
					StratusDebug.Log("The GameObject's tag and selected tag field match! (" + tagField + ")");
			}

			[StratusInvokeMethodAttribute("TryLayer")]
			void TryLayer()
			{
				if (layer.Matches(this.gameObject))
					StratusDebug.Log("The GameObject's layer and selected layer field are a match! (" + layer + ")");
			}


		}
	}

}