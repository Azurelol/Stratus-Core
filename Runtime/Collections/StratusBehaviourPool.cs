using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Stratus.Collections;
using Stratus.Logging;

namespace Stratus
{
	/// <summary>
	/// Base class for behaviour pools
	/// </summary>
	/// <typeparam name="BehaviourType"></typeparam>
	public abstract class StratusBehaviourPoolBase<BehaviourType> : IStratusLogger
		where BehaviourType : Behaviour
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The parent transform where instances are spawned as children to
		/// </summary>
		public Transform parent { get; protected set; }
		/// <summary>
		/// The prefab to instantiate for the behaviour pool
		/// </summary>
		public BehaviourType prefab { get; protected set; }
		/// <summary>
		/// The number of active instances
		/// </summary>
		public abstract int activeInstanceCount { get; }
		/// <summary>
		/// The total number of spawned instances, both active and inactive (recycled)
		/// </summary>
		public int instanceCount => activeInstanceCount + recycledInstances.Count;
		/// <summary>
		/// True if there are active instances spawned by the pool
		/// </summary>
		public bool instantiated => activeInstanceCount > 0;
		/// <summary>
		/// Whether to log debug output
		/// </summary>
		public bool debug { get; set; }
		/// <summary>
		/// The recycled instances
		/// </summary>
		protected Stack<BehaviourType> recycledInstances { get; set; }
		/// <summary>
		/// The default name used for spawned objects
		/// </summary>
		protected static readonly string instanceName = typeof(BehaviourType).Name;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusBehaviourPoolBase(Transform parent, BehaviourType prefab)
		{
			this.parent = parent;
			this.prefab = prefab;
			this.recycledInstances = new Stack<BehaviourType>();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		protected BehaviourType Recycle()
		{
			return recycledInstances.Pop();
		}

		protected BehaviourType InstantiateBehaviour()
		{
			BehaviourType instance;
			if (recycledInstances.Count > 0)
			{
				instance = Recycle();
			}
			else
			{
				instance = UnityEngine.Object.Instantiate(prefab, parent);
				instance.gameObject.name = $"{instanceName} {instanceCount}";
			}
			return instance;
		}
	}

	/// <summary>
	/// A basic behaviour pool where instantiated objects are removed via their id (assigned by Unity)
	/// </summary>
	/// <typeparam name="BehaviourType"></typeparam>
	public class StratusBehaviourPool<BehaviourType> : StratusBehaviourPoolBase<BehaviourType>
		where BehaviourType : Behaviour
	{
		public delegate void InstantiateFunction(BehaviourType behaviour);
		private SortedList<int, Tuple<int, BehaviourType>> instancesById { get; set; }

		public InstantiateFunction instantiateFunction { get; private set; }
		public override int activeInstanceCount => instancesById.Count;

		public StratusBehaviourPool(Transform parent,
				BehaviourType prefab,
				InstantiateFunction instantiateFunction) : base(parent, prefab)
		{
			this.instantiateFunction = instantiateFunction;
			this.instancesById = new SortedList<int, Tuple<int, BehaviourType>>();
		}

		public BehaviourType Instantiate()
		{
			BehaviourType instance = InstantiateBehaviour();
			int id = instance.GetInstanceID();
			instancesById.Add(id, new Tuple<int, BehaviourType>(id, instance));
			return instance;
		}

		public bool Remove(BehaviourType behaviour)
		{
			return Remove(behaviour.GetInstanceID());
		}

		public bool Remove(int id)
		{
			if (!instancesById.ContainsKey(id))
			{
				return false;
			}
			if (debug)
			{
				this.Log($"Removing {id}");
			}
			BehaviourType behaviour = instancesById[id].Item2;
			behaviour.gameObject.SetActive(false);
			instancesById.Remove(id);
			recycledInstances.Push(behaviour);
			return true;
		}
	}

	/// <summary>
	/// A behaviour pool where objects are removed via their unique datatype
	/// </summary>
	/// <typeparam name="BehaviourType"></typeparam>
	/// <typeparam name="DataType"></typeparam>
	public class StratusBehaviourPool<BehaviourType, DataType>
		: StratusBehaviourPoolBase<BehaviourType>
		where BehaviourType : MonoBehaviour
		where DataType : class
	{
		public delegate void InstantiateFunction(BehaviourType behaviour, DataType parameters);
		public InstantiateFunction instantiateFunction { get; private set; }
		protected Dictionary<DataType, Tuple<DataType, BehaviourType>> instancesByData { get; private set; }

		public override int activeInstanceCount => instancesByData.Count;

		public StratusBehaviourPool(Transform parent,
			BehaviourType prefab,
			InstantiateFunction instantiateFunction) : base(parent, prefab)
		{
			this.instantiateFunction = instantiateFunction;
			this.instancesByData = new Dictionary<DataType, Tuple<DataType, BehaviourType>>();
		}

		protected virtual void OnInstanceAdded(DataType data, BehaviourType instance)
		{
		}

		protected virtual void OnInstanceRemoved(DataType data)
		{
		}

		public BehaviourType Instantiate(DataType data)
		{
			if (data == null)
			{
				this.LogError("Cannot instantiate given null data");
				return null;
			}
			if (prefab == null)
			{
				this.LogError($"No prefab has been assigned");
				return null;
			}

			this.Log($"Instancing {data}");
			BehaviourType instance = InstantiateBehaviour();

			instantiateFunction(instance, data);
			instancesByData.Add(data, new Tuple<DataType, BehaviourType>(data, instance));
			instance.gameObject.SetActive(true);
			OnInstanceAdded(data, instance);
			return instance;
		}

		public bool Contains(DataType data)
		{
			if (data == null)
			{
				this.LogError($"Null data given");
				return false;
			}
			return instancesByData.ContainsKey(data);
		}

		public bool Remove(DataType data)
		{
			if (!instancesByData.ContainsKey(data))
			{
				return false;
			}
			if (debug)
			{
				this.Log($"Removing {data}");
			}
			BehaviourType behaviour = instancesByData[data].Item2;
			behaviour.gameObject.SetActive(false);
			instancesByData.Remove(data);
			recycledInstances.Push(behaviour);
			OnInstanceRemoved(data);
			return true;
		}

		public void Update(Action<BehaviourType, DataType> updateFunction)
		{
			foreach (var kp in instancesByData)
			{
				var instance = kp.Value;
				updateFunction(instance.Item2, instance.Item1);
			}
		}

		public bool Refresh(DataType data)
		{
			if (data == null)
			{
				return false;
			}
			if (!instancesByData.ContainsKey(data))
			{
				return false;
			}
			BehaviourType behaviour = instancesByData[data].Item2;
			instantiateFunction(behaviour, data);
			return true;
		}

		public void ForEach(Action<DataType, BehaviourType> action)
		{
			foreach (var kp in instancesByData.Values)
			{
				action(kp.Item1, kp.Item2);
			}
		}

		public void Clear()
		{
			foreach(var data in instancesByData.Keys.ToArray())
			{
				Remove(data);
			}
		}
	}

	public class StratusBehaviourPool<BehaviourType, DataType, KeyType>
		: StratusBehaviourPool<BehaviourType, DataType>
			where BehaviourType : MonoBehaviour
			where DataType : class
	{
		private AutoDictionary<KeyType, DataType> keytoData;

		public StratusBehaviourPool(Transform parent,
			BehaviourType prefab,
			InstantiateFunction instantiateFunction,
			 Func<DataType, KeyType> keySelector)
			: base(parent, prefab, instantiateFunction)
		{
			this.keytoData = new AutoDictionary<KeyType, DataType>(keySelector);
		}

		protected override void OnInstanceAdded(DataType data, BehaviourType instance)
		{
			base.OnInstanceAdded(data, instance);
			keytoData.Add(data);
		}

		protected override void OnInstanceRemoved(DataType data)
		{
			base.OnInstanceRemoved(data);
			keytoData.Remove(data);
		}

		public bool Contains(KeyType key)
		{
			return keytoData.ContainsKey(key);
		}

		public BehaviourType GetInstance(DataType data)
		{
			return instancesByData[data].Item2;
		}
	}

}