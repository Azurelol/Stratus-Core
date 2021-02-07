using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	public interface IStratusAudioPlayer
	{
		StratusOperationResult Play(string name);
		StratusOperationResult Stop();
		StratusOperationResult Mute(bool mute);
	}

	public abstract class StratusAudioPlayer : StratusBehaviour, IStratusAudioPlayer
	{
		public abstract StratusOperationResult Play(string name);
		public abstract StratusOperationResult Stop();
		public abstract StratusOperationResult Mute(bool mute);
	}

	[RequireComponent(typeof(AudioSource))]
	public abstract class StratusAudioSourcePlayer : StratusAudioPlayer
	{
		public AudioSource audioSource => GetComponentCached<AudioSource>();

		protected abstract StratusOperationResult OnPlay(string name);

		public override StratusOperationResult Play(string name)
		{
			return OnPlay(name);
		}

		public override StratusOperationResult Mute(bool mute)
		{
			if (audioSource.isPlaying)
			{
				audioSource.mute = mute;
			}
			return false;
		}

		public override StratusOperationResult Stop()
		{
			if (audioSource.isPlaying)
			{
				audioSource.Stop();
				return true;
			}
			return false;
		}
	}

	public abstract class StratusAudioSourcePlayer<AudioClipSource> : StratusAudioSourcePlayer
		where AudioClipSource : IStratusAssetSource<StratusAudioClip>
	{		
		public AudioClipSource assets;

		protected override StratusOperationResult OnPlay(string name)
		{
			if (assets.HasAsset(name))
			{
				AudioClip clip = assets.GetAsset(name).asset.reference;
				audioSource.clip = clip;
				audioSource.Play();
				return true;
			}
			return new StratusOperationResult(false, $"Could not find the asset named {name}");
		}
	}
}