using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Data;

namespace Stratus
{
	[CustomPropertyDrawer(typeof(FloatRange))]
  public class FloatRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(FloatRange.minimum);
    protected override string secondProperty { get; } = nameof(FloatRange.maximum);
  }

  [CustomPropertyDrawer(typeof(StratusIntegerRange))]
  public class IntegerRangeDrawer : DualPropertyDrawer
  {
    protected override string firstProperty { get; } = nameof(StratusIntegerRange.minimum);
    protected override string secondProperty { get; } = nameof(StratusIntegerRange.maximum);
  }

}