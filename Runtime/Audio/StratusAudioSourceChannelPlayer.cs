using System.Collections.Generic;
using System;
using UnityEngine;

namespace Stratus
{
	[StratusSingleton(instantiate = false)]
	public class StratusAudioSourceChannelPlayer : StratusSingletonBehaviour<StratusAudioSourceChannelPlayer>
	{
		[SerializeField]
		private bool debug = false;
		[SerializeField]
		private List<StratusAudioChannel> _channels = new List<StratusAudioChannel>();
		public StratusSortedList<string, StratusAudioChannel> channelsByName { get; private set; }

		protected override void OnAwake()
		{
			RegisterChannels();
		}

		private void Reset()
		{
			_channels = new List<StratusAudioChannel>();
			foreach (var channel in StratusEnum.Values<StratusDefaultAudioChannel>())
			{
				_channels.Add(new StratusAudioChannel(channel.ToString()));
			}
		}

		private void RegisterChannels()
		{
			channelsByName = new StratusSortedList<string, StratusAudioChannel>(x => x.name, _channels.Count, StringComparer.InvariantCultureIgnoreCase);
			channelsByName.AddRange(_channels);
		}

		public StratusOperationResult Play(string channel, string name)
		{
			StratusOperationResult result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new StratusOperationResult(false, $"The audio channel {channel} was not found!");
			}
			else
			{
				result = channelsByName[channel].player.Play(name);
			}
			this.Log(result);
			return result;
		}

		public StratusOperationResult Stop(string channel)
		{
			StratusOperationResult result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new StratusOperationResult(false, $"The audio channel {channel} was not found!");
			}
			else
			{
				result = channelsByName[channel].player.Stop();
			}
			this.Log(result);
			return result;
		}

		public StratusOperationResult Mute(string channel, bool mute)
		{
			StratusOperationResult result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new StratusOperationResult(false, $"The audio channel {channel} was not found!");
			}
			else
			{
				result = channelsByName[channel].player.Mute(mute);
			}
			this.Log(result);
			return result;
		}

	}
}