using UnityEditor;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusSceneLinker))]
  public class SceneLinkerEditor : StratusBehaviourEditor<StratusSceneLinker>
  {
    protected override void OnStratusEditorEnable()
    {
    }  
  }

}