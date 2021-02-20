using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	[Serializable]
	public class StratusSprite : StratusAssetReference<Sprite>
	{
	}

	[CreateAssetMenu(menuName = scriptablesMenu + "Sprite Scriptable")]
	public class StratusSpriteCollectionScriptable : StratusAssetCollectionScriptable<StratusSprite>
	{
	}

}