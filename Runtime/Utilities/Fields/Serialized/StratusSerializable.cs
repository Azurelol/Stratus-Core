using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  /// <summary>
  /// Provides useful functions and a generic property drawer that can handle 
  /// multiple fields, etc. Make sure to still use the [Serializable] attribute!
  /// </summary>
  [Serializable]
  public abstract class StratusSerializable
  {
    //public enum DrawingMethod
    //{
    //  Default,
    //  SingleLine
    //}
    //
    //public int fieldCount { get; private set; }
    //protected virtual DrawingMethod drawingMethod { get; } = DrawingMethod.Default;
    //
    //public StratusSerializable()
    //{
    //  Type declaredType = GetType().DeclaringType;
    //  fieldCount = declaredType.GetFields().Length;
    //}

  }


}