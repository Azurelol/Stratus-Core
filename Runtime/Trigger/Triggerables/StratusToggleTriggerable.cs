using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// </summary>
	public class StratusToggleTriggerable : StratusTriggerableBehaviour
	{
		public enum Scope 
		{ 
			GameObject, 
			Behaviour 
		}

		public enum Action 
		{ 
			Enable, 
			Disable, 
			Toggle 
		}

		public Scope scope = Scope.GameObject;

		[Tooltip("The GameObject being enabled")]
		[DrawIf(nameof(StratusToggleTriggerable.scope), Scope.GameObject, ComparisonType.Equals)]
		public GameObject target;

		[Tooltip("The Component being enabled")]
		[DrawIf(nameof(StratusToggleTriggerable.scope), Scope.Behaviour, ComparisonType.Equals)]
		public Behaviour targetBehaviour;

		[Tooltip("Whether the target is being enabled or disabled")]
		public Action action = Action.Enable;

		public override string automaticDescription
		{
			get
			{
				switch (scope)
				{
					case Scope.GameObject:
						if (target)
							return $"{action} {target.name}";
						break;
					case Scope.Behaviour:
						if (targetBehaviour)
							return $"{action} {targetBehaviour}";
						break;
				}
				return string.Empty;
			}
		}

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger(object data = null)
		{
			if ((scope == Scope.GameObject && !target) ||
			   (scope == Scope.Behaviour && !targetBehaviour))
			{
				Error("No valid target set!", this);
				return;
			}

			switch (this.action)
			{
				case Action.Enable:
					if (scope == Scope.GameObject)
						this.target.SetActive(true);
					else if (scope == Scope.Behaviour)
						this.targetBehaviour.enabled = true;
					break;

				case Action.Disable:
					if (scope == Scope.GameObject)
						this.target.SetActive(false);
					else if (scope == Scope.Behaviour)
						this.targetBehaviour.enabled = false;
					break;

				case Action.Toggle:
					if (scope == Scope.GameObject)
						this.target.SetActive(!this.target.activeSelf);
					else if (scope == Scope.Behaviour)
						this.targetBehaviour.enabled = !this.targetBehaviour.enabled;
					break;
			}
		}
	}

}