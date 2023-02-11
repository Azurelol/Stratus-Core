using NUnit.Framework;

using System;

using UnityEngine;

namespace Stratus.Tests.Editor
{
	public class StratusInputActionTests
	{
		public enum MockInputActions
		{
			Console,
			Pan,
			Pause
		}

		public class MockInputActionMap : StratusInputActionMap<MockInputActions>
		{
			public Action console;
			public Action<Vector2> pan;
			public Action pause;

			public override string name { get; } = "Default";

			protected override void OnInitialize()
			{
				TryBindActions();
			}
		}

		[Test]
		public void BindsActions()
		{
			var map = new MockInputActionMap();
			Assert.AreEqual(3, map.count);
			Assert.True(map.Contains(nameof(MockInputActionMap.console)));
			Assert.True(map.Contains(nameof(MockInputActionMap.pan)));
			Assert.True(map.Contains(nameof(MockInputActionMap.pause)));
		}
	}
}