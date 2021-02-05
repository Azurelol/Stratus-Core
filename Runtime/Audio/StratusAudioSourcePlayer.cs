using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// The default audio channels used by Stratus components
	/// </summary>
	public enum StratusAudioChannel
	{
		/// <summary>
		/// Sound effects
		/// </summary>
		SFX,
		/// <summary>
		/// Voice-over
		/// </summary>
		VO,
		/// <summary>
		/// Music
		/// </summary>
		BGM
	}

	[RequireComponent(typeof(AudioSource))]
	public abstract class StratusAudioSourcePlayer<AudioClipSource> : StratusBehaviour
		where AudioClipSource : IStratusAssetSource<StratusAudioClip>
	{
		public AudioSource audioSource => GetComponentCached<AudioSource>();
		public AudioClipSource assets;

		public void Play(string name)
		{
			if (assets.HasAsset(name))
			{
				AudioClip clip = assets.GetAsset(name).asset.reference;
				audioSource.clip = clip;
				audioSource.Play();
			}
		}
	}
}