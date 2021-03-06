using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Interfaces;

namespace Stratus
{
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  public class StratusTriggerSystem : StratusBehaviour, IStratusValidator, IStratusValidatorAggregator
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public enum ConnectionDisplay
    {
      Selection,
      Grouping
    }

    public enum ConnectionStatus
    {
      Connected,
      Disconnected,
      Selected,
      Disjoint
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool showDescriptions = true;
    public ConnectionDisplay connectionDisplay = ConnectionDisplay.Selection;
    public bool outlines = false;

    public List<StratusTriggerBehaviour> triggers = new List<StratusTriggerBehaviour>();
    public List<StratusTriggerableBehaviour> triggerables = new List<StratusTriggerableBehaviour>();
    public bool descriptionsWithLabel = false;
    private Dictionary<StratusTriggerBehaviour, bool> triggersInitialState = new Dictionary<StratusTriggerBehaviour, bool>();
    private Dictionary<StratusTriggerableBehaviour, bool> triggerablesInitialState = new Dictionary<StratusTriggerableBehaviour, bool>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether there no components in the system
    /// </summary>    
    public bool isEmpty => triggers.Empty() && triggerables.Empty();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      RecordTriggerStates();
    }

    private void OnDestroy()
    {
      ShowComponents(true);
    }

    private void OnEnable()
    {
      Refresh();
    }

    private void Reset()
    {
      Refresh();
    }

    private void OnValidate()
    {
      //ToggleComponents(enabled);
      triggers.RemoveNull();
      triggerables.RemoveNull();
      ShowComponents(false);
    }

    StratusObjectValidation[] IStratusValidatorAggregator.Validate()
    {
      var messages = new List<StratusObjectValidation>();
      messages.AddIfNotNull(StratusObjectValidation.Generate(this));
      messages.AddRange(StratusObjectValidation.Aggregate(triggers));
      messages.AddRange(StratusObjectValidation.Aggregate(triggerables));      
      return messages.ToArray();
    }

    StratusObjectValidation IStratusValidator.Validate()
    {
      return ValidateConnections();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void RecordTriggerStates()
    {
      foreach (var trigger in triggers)
        triggersInitialState.Add(trigger, trigger.enabled);

      foreach (var triggerable in triggerables)
        triggerablesInitialState.Add(triggerable, triggerable.enabled);
    }

    /// <summary>
    /// Restars the state of all the triggers in this system to their intitial values
    /// </summary>
    public void Restart()
    {
      foreach (var trigger in triggers)
        trigger.Restart();

      foreach (var triggerable in triggerables)
        triggerable.Restart();
    }

    /// <summary>
    /// Toggles all eligible triggers in the system on/off
    /// </summary>
    public void ToggleTriggers(bool toggle)
    {
      foreach (var trigger in triggers)
      {
        // Skip triggers that were marked as not persistent and have been activated
        if (!trigger.persistent && trigger.activated)
          continue;

        trigger.enabled = toggle;
      }
    }
    
    /// <summary>
    /// Toggles all components in the system on/off
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleComponents(bool toggle)
    {
      foreach (var trigger in triggers)
      {
        if (!trigger.awoke)
          continue;

        trigger.enabled = toggle;
      }

      foreach (var triggerable in triggerables)
      {
        if (!triggerable.awoke)
          continue;

        triggerable.enabled = toggle;
      }
    }


    /// <summary>
    /// Adds a trigger to the system
    /// </summary>
    /// <param name="baseTrigger"></param>
    public void Add(StratusTriggerBase baseTrigger)
    {
      if (baseTrigger is StratusTriggerBehaviour)
        triggers.Add(baseTrigger as StratusTriggerBehaviour);
      else if (baseTrigger is StratusTriggerableBehaviour)
        triggerables.Add(baseTrigger as StratusTriggerableBehaviour);
    }

    /// <summary>
    /// Controls visibility for all the base trigger components
    /// </summary>
    /// <param name="show"></param>
    public void ShowComponents(bool show)
    {
      HideFlags flag = show ? HideFlags.None : HideFlags.HideInInspector;
      foreach (var trigger in triggers)
        trigger.hideFlags = flag;
      foreach (var triggerable in triggerables)
        triggerable.hideFlags = flag;
    }
    
    /// <summary>
    /// Refreshes the state of this TriggerSystem
    /// </summary>
    private void Refresh()
    {
      // Remove any invalid
      triggers.RemoveNull();
      triggerables.RemoveNull();

      // Add previously not found
      triggers.AddRangeUnique(GetComponents<StratusTriggerBehaviour>());
      triggerables.AddRangeUnique(GetComponents<StratusTriggerableBehaviour>());

      // Hide any triggers managed by the system
      ShowComponents(false);

      // Validate triggers
      ValidateTriggers();
    }
    
    private void ValidateTriggers()
    {
      foreach (var trigger in triggers)
      {
        trigger.scope = StratusTriggerBehaviour.Scope.Component;
      }        
    }

    public static bool IsConnected(StratusTriggerBehaviour trigger, StratusTriggerableBehaviour triggerable)
    {
      if (trigger.targets.Contains(triggerable))
        return true;
      return false;
    }

    public static bool IsConnected(StratusTriggerBehaviour trigger)
    {
      return trigger.targets.NotEmpty();
    }

    public StratusObjectValidation ValidateConnections()
    {
      List<StratusTriggerBase> disconnected = new List<StratusTriggerBase>();
      foreach (var t in triggers)
      {
        if (!IsConnected(t))
          disconnected.Add(t);
      }

      //foreach (var t in triggerables)
      //{
      //  if (!IsConnected(t))
      //    disconnected.Add(t);
      //}

      if (disconnected.Empty())
        return null;

      string msg = $"Triggers marked as disconnected ({disconnected.Count}):";
      foreach (var t in disconnected)
        msg += $"\n- {t.GetType().Name} : <i>{t.description}</i>";
      return new StratusObjectValidation(msg, StratusObjectValidation.Level.Warning, this);
    }   

  }

}