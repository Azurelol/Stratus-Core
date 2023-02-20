
using UnityEngine;

namespace Stratus
{
	public interface IStratusAudioPlayer
	{
		StratusAudioParameters defaultParameters { get; }

		Result Play(string name);
		Result Pause(bool pause);
		Result Pause();
		Result Resume();
		Result Stop();
		Result Mute(bool mute);
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
		public abstract Result Play(string name);
		public abstract Result Pause(bool pause);
		public Result Pause() => Pause(true);
		public Result Resume() => Pause(false);
		public abstract Result Stop();
		public abstract Result Mute(bool mute);


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