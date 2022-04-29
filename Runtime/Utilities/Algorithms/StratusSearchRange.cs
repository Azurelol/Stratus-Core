using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	public class StratusSearchRange<TPosition, TCost> : Dictionary<TPosition, TCost>
	{
		public StratusSearchRange()
		{
		}

		public StratusSearchRange(IDictionary<TPosition, TCost> dictionary) : base(dictionary)
		{
		}

		public StratusSearchRange(IEqualityComparer<TPosition> comparer) : base(comparer)
		{
		}

		public StratusSearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection) : base(collection)
		{
		}

		public StratusSearchRange(IDictionary<TPosition, TCost> dictionary, IEqualityComparer<TPosition> comparer) : base(dictionary, comparer)
		{
		}

		public StratusSearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection, IEqualityComparer<TPosition> comparer) : base(collection, comparer)
		{
		}
	}

	/// <summary>
	/// Arguments to perform a range search
	/// </summary>
	public class StratusSearchRangeArguments
	{
		public StratusSearchRangeArguments(int minimum, int maximum)
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		public StratusSearchRangeArguments(int maximum)
		{
			this.minimum = 0;
			this.maximum = maximum;
		}

		public int minimum { get; }
		public int maximum { get; }
		public Func<Vector3Int, float> traversalCostFunction { get; set; }
		public StratusTraversalPredicate<Vector3Int> traversableFunction { get; set; }

		//public static implicit operator StratusSearchRangeArguments(Vector2Int vec)  => 
		//	new StratusSearchRangeArguments(vec.x, vec.y);
		//public static implicit operator StratusSearchRangeArguments(int n) =>
		//	new StratusSearchRangeArguments(n);
	}

	public class StratusGridRange : StratusSearchRange<Vector3Int, float>
	{
		public StratusGridRange()
		{
		}

		public StratusGridRange(IDictionary<Vector3Int, float> dictionary) : base(dictionary)
		{
		}

		public StratusGridRange(IEqualityComparer<Vector3Int> comparer) : base(comparer)
		{
		}

		public StratusGridRange(IEnumerable<KeyValuePair<Vector3Int, float>> collection) : base(collection)
		{
		}

		public StratusGridRange(IDictionary<Vector3Int, float> dictionary, IEqualityComparer<Vector3Int> comparer) : base(dictionary, comparer)
		{
		}

		public StratusGridRange(IEnumerable<KeyValuePair<Vector3Int, float>> collection, IEqualityComparer<Vector3Int> comparer) : base(collection, comparer)
		{
		}
	}
}