using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	[Serializable]
	public class StratusSprite : StratusUnityAssetReference<Sprite>
	{
	}

	[CreateAssetMenu(menuName = scriptablesMenu + "Sprites")]
	public class StratusSpriteCollectionScriptable : StratusAssetCollectionScriptable<StratusSprite>
	{
	}

}