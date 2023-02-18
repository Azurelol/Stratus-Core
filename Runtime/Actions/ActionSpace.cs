using Stratus.Interpolation;
using Stratus.Utilities;

using System;
using System.Linq.Expressions;
using System.Reflection;

using UnityEngine;

namespace Stratus.Unity.Interpolation
{
	/// <summary>
	/// Interface class that the client will be using for constructing action sets
	/// </summary>
	[StratusSingleton("Action Space", true, true)]
	public class ActionSpace : StratusSingletonBehaviour<ActionSpace>
	{
		#region Properties
		public ActionScheduler<GameObject> system { get; } = new ActionScheduler<GameObject>();
		#endregion

		#region Messages
		protected override void OnAwake()
		{
			system.onConnect += this.OnConnect;
			system.onDisconnect += this.OnDisconnect;
		}

		private void OnConnect(GameObject obj)
		{
			gameObject.GetOrAddComponent<StratusActionsRegistration>();
		}

		private void OnDisconnect(GameObject obj)
		{
			gameObject.RemoveComponent<StratusActionsRegistration>();
		}

		private void FixedUpdate()
		{
			system.Update(Time.deltaTime);
		}
		#endregion

		/// <summary>
		/// Whether we are currently debugging actions
		/// </summary>
		public static bool debug = false;

		public static ActionContainer Actions(GameObject go)
		{
			// Subscribe this GameObject to the ActionSpace
			return instance.system.Connect(go);
		}

		/// <summary>
		/// Cancels all active actions of the ActionsOwner.
		/// </summary>
		/// <param name="owner">A reference to the owner of this action sequence.</param>
		public static void Cancel(ActionContainer owner)
		{
			owner.Clear();
		}

		/// <summary>
		/// Cancels all active actions in the component.
		/// </summary>
		/// <param name="component"></param>
		public static void Cancel(MonoBehaviour component)
		{
			instance.system.Disconnect(component.gameObject);
		}

		/// <summary>
		/// Creates an action sequence.
		/// </summary>
		/// <param name="owner">A reference to the owner of this action sequence.</param>
		/// <returns>An ActionSet object, used for Action constructors.</returns>
		public static ActionSet Sequence(ActionContainer owner)
		{
			// Construct the sequence
			ActionSet sequence = new ActionSequence();
			// Add it to the owner
			owner.Add(sequence);
			// Return it
			return sequence;
		}

		/// <summary>
		/// Creates an action sequence.
		/// </summary>
		/// <param name="owner">The component which is creating the action.</param>
		/// <returns>An ActionSet object, used for Action constructors.</returns>
		public static ActionSet Sequence(MonoBehaviour component)
		{
			// Construct the sequence
			ActionSet sequence = new ActionSequence();
			// Add it to the owner
			Actions(component.gameObject).Add(sequence);
			// Return it
			return sequence;
		}

		/// <summary>
		/// Creates an action group, a set which runs all actions in parallel
		/// </summary>
		/// <param name="owner">A reference to the owner of this action sequence.</param>
		/// <returns></returns>
		public static ActionSet Group(ActionContainer owner)
		{
			// Construct the sequence
			ActionSet sequence = new ActionGroup();
			// Add it to the owner
			owner.Add(sequence);
			// Return it
			return sequence;
		}

		/// <summary>
		/// Creates an action group, a set which runs all actions in parallel
		/// </summary>
		/// <param name="owner">A reference to the owner of this action sequence.</param>
		/// <returns></returns>
		public static ActionSet Group(MonoBehaviour component)
		{
			// Construct the sequence
			ActionSet group = new ActionGroup();
			// Add it to the owner
			Actions(component.gameObject).Add(group);
			// Return it
			return group;
		}

		/// <summary>
		/// Creates an ActionDelay and adds it to the specified set.
		/// </summary>
		/// <param name="set">A reference to the ActionSet that this action belongs to.</param>
		/// <param name="duration"> duration How long should the delay run for.</param>
		public static ActionBase Delay(ActionSet set, float duration)
		{
			ActionBase delay = new ActionDelay(duration);
			set.Add(delay);
			return delay;
		}

		/// <summary>
		/// Adds a function to be invoked as part of the action set, alongside any arguments. 
		/// Optionally, it also can add a delay before the function is invoked.
		/// </summary>
		/// <param name="set">A reference to the action set.</param>
		/// <param name="func">The function to which to call.</param>
		public static ActionBase Call(ActionSet set, ActionCall.Delegate func, float delay = 0.0f)
		{
			ActionBase call = new ActionCall(func);

			// Optionally, add a delay
			if (delay != 0.0f)
			{
				Delay(set, delay);
			}

			set.Add(call);
			return call;
		}

		/// <summary>
		/// Adds a trace, adding it to the specified set.
		/// </summary>
		/// <param name="set">A reference to the set.</param>
		/// <param name="message">The message which to print.</param>
		public static ActionBase Trace(ActionSet set, object message)
		{
			ActionBase trace = new StratusActionLog(message);
			set.Add(trace);
			return trace;
		}

		/// <summary>
		/// Destroys the specified object after a set amount of time
		/// </summary>
		/// <param name="set"></param>
		/// <param name="obj"></param>
		/// <param name="delay"></param>
		public static void Destroy(ActionSet set, UnityEngine.Object obj, float delay = 0.0f)
		{
			Call(set, () => UnityEngine.Object.Destroy(obj, delay));
		}

		/// <summary>
		/// Adds a property change to the action set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set">A reference to the set.</param>
		/// <param name="varExpr">A lambda expression encapsulating a reference to the property which will be modified</param>
		/// <param name="value">The new value for the property</param>
		/// <param name="duration">Over how long should the property be changed</param>
		/// <param name="ease">What interpolation algorithm to use</param>
		public static void Property<T>(ActionSet set, Expression<Func<T>> varExpr, T value, float duration, Ease ease)
		{
			MemberExpression memberExpr = varExpr.Body as MemberExpression;
			Expression inst = memberExpr.Expression;
			string variableName = memberExpr.Member.Name;
			object targetObj = Expression.Lambda<Func<object>>(inst).Compile()();

			// Construct an action then branch depending on whether the member to be
			// interpolated is a property or a field
			ActionBase action = null;

			// Property
			PropertyInfo property = targetObj.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				Type propertyType = property.PropertyType;

				if (propertyType == typeof(float))
				{
					action = new ActionPropertyFloat(targetObj, property, Convert.ToSingle(value), duration, ease);
				}
				else if (propertyType == typeof(int))
				{
					action = new ActionPropertyInteger(targetObj, property, Convert.ToInt32(value), duration, ease);
				}
				else if (propertyType == typeof(bool))
				{
					action = new ActionPropertyBoolean(targetObj, property, Convert.ToBoolean(value), duration, ease);
				}
				else if (propertyType == typeof(Vector2))
				{
					action = new ActionPropertyVector2(targetObj, property, (System.Numerics.Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
				}
				else if (propertyType == typeof(Vector3))
				{
					action = new ActionPropertyVector3(targetObj, property, (System.Numerics.Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
				}
				else if (propertyType == typeof(Vector4))
				{
					action = new ActionPropertyVector4(targetObj, property, (System.Numerics.Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
				}
				else if (propertyType == typeof(Color))
				{
					action = new ActionPropertyColor(targetObj, property, (Color)Convert.ChangeType(value, typeof(Color)), duration, ease);
				}
				else if (propertyType == typeof(Quaternion))
				{
					action = new ActionPropertyQuaternion(targetObj, property, (Quaternion)Convert.ChangeType(value, typeof(Quaternion)), duration, ease);
				}
				else
				{
					StratusDebug.Log("Couldn't find the property!");
				}
			}
			// Field
			else
			{
				FieldInfo field = targetObj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
				Type fieldType = field.FieldType;

				if (fieldType == typeof(float))
				{
					action = new ActionPropertyFloat(targetObj, field, Convert.ToSingle(value), duration, ease);
				}
				else if (fieldType == typeof(int))
				{
					action = new ActionPropertyInteger(targetObj, field, Convert.ToInt32(value), duration, ease);
				}
				else if (fieldType == typeof(bool))
				{
					action = new ActionPropertyBoolean(targetObj, field, Convert.ToBoolean(value), duration, ease);
				}
				else if (fieldType == typeof(Vector2))
				{
					action = new ActionPropertyVector2(targetObj, field, (System.Numerics.Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
				}
				else if (fieldType == typeof(Vector3))
				{
					action = new ActionPropertyVector3(targetObj, field, (System.Numerics.Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
				}
				else if (fieldType == typeof(Vector4))
				{
					action = new ActionPropertyVector4(targetObj, field, (System.Numerics.Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
				}
				else if (fieldType == typeof(Color))
				{
					action = new ActionPropertyColor(targetObj, field, (Color)Convert.ChangeType(value, typeof(Color)), duration, ease);
				}
				else if (fieldType == typeof(Quaternion))
				{
					action = new ActionPropertyQuaternion(targetObj, field, (Quaternion)Convert.ChangeType(value, typeof(Quaternion)), duration, ease);
				}
				else
				{
					StratusDebug.Log("Couldn't find the field!");
				}
			}
			// Now add it!
			set.Add(action);
		}
	}

	public static class ActionExtensions
	{

	}


}

