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

	[Serializable]
	public class StratusAudioParameters
	{
		[Range(0f, 1f)]
		public float volume = 1f;
		[Range(0f, 1f)]
		public float pitch = 1f;
		public bool loop = false;

		public StratusEase fadeIn = StratusEase.Linear;
		public float fadeInDuration = 0f;
		public StratusEase fadeOut = StratusEase.Linear;
		public float fadeOutDuration = 0f;
	}

	[CreateAssetMenu(menuName = scriptablesMenu + "Audio Clip Scriptable")]
	public class StratusAudioClipCollectionScriptable : StratusAssetCollectionScriptable<StratusAudioClip>
	{
	}

}