using UnityEngine;
using System;
using System.Collections.Generic;
using Stratus.Timers;

namespace Stratus
{
	namespace Utilities
	{
		/// <summary>
		/// A component which updates all timers registered to it, automatically.
		/// This makes it so you don't have to handle the updating yourself in your
		/// MonoBehaviour's update messages.
		/// </summary>
		public class StratusTimerSystem : StratusSingletonBehaviour<StratusTimerSystem>
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      private List<Timer> All = new List<Timer>();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnAwake()
      {

      }

      private void Update()
      {
        foreach(var timer in All)
        {
          timer.Update(Time.deltaTime);
        }
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public static void Add(Timer timer)
      {
        instance.All.Add(timer);
      }

      public static void Remove(Timer timer)
      {
        instance.All.Remove(timer);
      }


    }
  }
}
