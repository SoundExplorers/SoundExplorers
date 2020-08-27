using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Indicates that a property of an entity class represents a
  ///   database field that is not to be shown in the table editor.
  /// </summary>
  /// <remarks>
  ///   Primary key columns that are shown in the parent grid
  ///   in the table editor will automatically be hidden in the main grid
  ///   and should not have this attribute.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  internal class HiddenFieldAttribute : FieldAttribute {
    public HiddenFieldAttribute() : base(0) { }
  }
}