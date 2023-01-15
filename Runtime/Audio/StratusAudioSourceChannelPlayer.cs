using System.Collections.Generic;
using System;
using UnityEngine;
using Stratus.Collections;

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
			return InvokeAudioChannel(channel, (player) => player.Play(name));
		}

		public StratusOperationResult Stop(string channel)
		{
			return InvokeAudioChannel(channel, (player) => player.Stop());
		}

		/// <summary>
		/// Stops playback on all channels
		/// </summary>
		public void Stop()
		{
			foreach(var channel in _channels)
			{
				channel.player.Stop();
			}
		}

		public StratusOperationResult Pause(string channel, bool pause)
		{
			return InvokeAudioChannel(channel, (player) => player.Pause(pause));
		}

		public StratusOperationResult Pause(string channel) => InvokeAudioChannel(channel, (player) => player.Pause());
		public StratusOperationResult Resume(string channel) => InvokeAudioChannel(channel, (player) => player.Resume());

		public StratusOperationResult Mute(string channel, bool mute)
		{
			return InvokeAudioChannel(channel, (player) => player.Mute(mute));
		}

		private StratusOperationResult InvokeAudioChannel(string channel, Func<StratusAudioPlayer, StratusOperationResult> onChannel)
		{
			StratusOperationResult result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new StratusOperationResult(false, $"The audio channel {channel} was not found!");
			}
			else
			{
				result = onChannel(channelsByName[channel].player);
			}
			this.Log(result);
			return result;
		}
	}
}