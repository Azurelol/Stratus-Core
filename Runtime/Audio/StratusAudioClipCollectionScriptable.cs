using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	[Serializable]
	public class StratusAudioClip : StratusAssetReference<AudioClip>
	{
	}

	[CreateAssetMenu(menuName = scriptablesMenu + "Audio Clip Scriptable")]
	public class StratusAudioClipCollectionScriptable : StratusAssetCollectionScriptable<StratusAudioClip>
	{
	}

}