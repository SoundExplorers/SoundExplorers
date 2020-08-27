using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Indicates that a property of an entity class represents a
  ///   unique key field.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  internal class UniqueKeyFieldAttribute : FieldAttribute {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="UniqueKeyFieldAttribute" /> class.
    /// </summary>
    /// <param name="sequenceNo">
    ///   The one-based left-to-right sequence number of the column
    ///   in the grid.
    /// </param>
    public UniqueKeyFieldAttribute(int sequenceNo) : base(sequenceNo) { }
  }
}