using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;

namespace Stratus.Tests
{
	public class StratusGameObjectInformationTest
	{
		//Scene scene;

		//[UnitySetUp]
		//public void LoadScene()
		//{
		//	scene = SceneManager.CreateScene(nameof(StratusGameObjectInformationTest));

		//}

		[UnityTest]
		public IEnumerator RetrievesInformation()
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Assert.NotNull(go);
			StratusGameObjectInformation info = new StratusGameObjectInformation(go);
			Assert.True(info.HasComponent<Transform>());
			Assert.True(info.HasComponent<MeshRenderer>());
			foreach (var component in info.components)
			{
			}
			yield return null;

		}
	}

}