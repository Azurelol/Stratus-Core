using System.Collections.Generic;
using System;
using UnityEngine;
using Stratus.Collections;
using Stratus.Utilities;
using Stratus.Logging;

namespace Stratus
{
	[StratusSingleton(instantiate = false)]
	public class StratusAudioSourceChannelPlayer : StratusSingletonBehaviour<StratusAudioSourceChannelPlayer>
	{
		[SerializeField]
		private bool debug = false;
		[SerializeField]
		private List<StratusAudioChannel> _channels = new List<StratusAudioChannel>();
		public AutoSortedList<string, StratusAudioChannel> channelsByName { get; private set; }

		protected override void OnAwake()
		{
			RegisterChannels();
		}

		private void Reset()
		{
			_channels = new List<StratusAudioChannel>();
			foreach (var channel in EnumUtility.Values<StratusDefaultAudioChannel>())
			{
				_channels.Add(new StratusAudioChannel(channel.ToString()));
			}
		}

		private void RegisterChannels()
		{
			channelsByName = new AutoSortedList<string, StratusAudioChannel>(x => x.name, _channels.Count, StringComparer.InvariantCultureIgnoreCase);
			channelsByName.AddRange(_channels);
		}

		public Result Play(string channel, string name)
		{
			return InvokeAudioChannel(channel, (player) => player.Play(name));
		}

		public Result Stop(string channel)
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

		public Result Pause(string channel, bool pause)
		{
			return InvokeAudioChannel(channel, (player) => player.Pause(pause));
		}

		public Result Pause(string channel) => InvokeAudioChannel(channel, (player) => player.Pause());
		public Result Resume(string channel) => InvokeAudioChannel(channel, (player) => player.Resume());

		public Result Mute(string channel, bool mute)
		{
			return InvokeAudioChannel(channel, (player) => player.Mute(mute));
		}

		private Result InvokeAudioChannel(string channel, Func<StratusAudioPlayer, Result> onChannel)
		{
			Result result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new Result(false, $"The audio channel {channel} was not found!");
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