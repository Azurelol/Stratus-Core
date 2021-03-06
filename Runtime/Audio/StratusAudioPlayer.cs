﻿
using UnityEngine;

namespace Stratus
{
	public interface IStratusAudioPlayer
	{
		StratusAudioParameters defaultParameters { get; }

		StratusOperationResult Play(string name);
		StratusOperationResult Pause(bool pause);
		StratusOperationResult Pause();
		StratusOperationResult Resume();
		StratusOperationResult Stop();
		StratusOperationResult Mute(bool mute);
	}

	public abstract class StratusAudioPlayer : StratusBehaviour, IStratusAudioPlayer, IStratusDebuggable
	{
		[SerializeField]
		private bool _debug;
		[SerializeField]
		private StratusAudioParameters _parameters = new StratusAudioParameters();
		public StratusAudioParameters defaultParameters => _parameters;

		public bool debug 
		{
			get => _debug;
			set => _debug = value;
		}

		protected abstract void SetParameters(StratusAudioParameters parameters);
		public abstract StratusOperationResult Play(string name);
		public abstract StratusOperationResult Pause(bool pause);
		public StratusOperationResult Pause() => Pause(true);
		public StratusOperationResult Resume() => Pause(false);
		public abstract StratusOperationResult Stop();
		public abstract StratusOperationResult Mute(bool mute);


		private void Awake()
		{
			SetParameters(_parameters);
		}

		public void Toggle(bool toggle)
		{
			throw new System.NotImplementedException();
		}
	}
}