using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stratus
{
	public class StratusSceneTriggerable : StratusTriggerableBehaviour
	{
		public enum Type
		{
			[Tooltip("Loads a new scene")]
			Load,
			[Tooltip("Reloads the current scene")]
			Reload,
			[Tooltip("Unloads a specified scene (if loaded)")]
			Unload
		}

		[Tooltip("What type of scene action to perform")]
		public Type type = Type.Load;

		//[DrawIf("type", Type.Reload, ComparisonType.NotEqual)] 
		[Tooltip("The scene to load or unload")]
		public StratusSceneField scene = new StratusSceneField();

		//[DrawIf("type", Type.Load, ComparisonType.Equals)]
		[Tooltip("How to load this scene")]
		public LoadSceneMode loadingMode = LoadSceneMode.Additive;


		public override string automaticDescription => $"{type} {scene.name}";

		protected override void OnAwake()
		{

		}

		protected override void OnTrigger()
		{
			switch (type)
			{
				case Type.Load:
					scene.Load(loadingMode);
					break;
				case Type.Reload:
					StratusScene.Reload();
					break;
				case Type.Unload:
					scene.Unload();
					break;
				default:
					break;
			}

		}

		protected override void OnReset()
		{

		}

	}



}
