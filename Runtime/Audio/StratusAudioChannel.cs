using System;

namespace Stratus
{
	[Serializable]
	public class StratusAudioChannel
	{
		public string name;
		public StratusAudioPlayer player;

		public StratusAudioChannel()
		{
		}

		public StratusAudioChannel(string name)
		{
			this.name = name;
		}
	}


	/// <summary>
	/// The default audio channels used by Stratus components
	/// </summary>
	public enum StratusDefaultAudioChannel
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
}