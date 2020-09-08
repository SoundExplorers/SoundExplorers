using System;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   Indicates that a property of an entity class represents a
  ///   database field.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  internal class FieldAttribute : Attribute {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="FieldAttribute" /> class.
    /// </summary>
    /// <param name="sequenceNo">
    ///   The one-based left-to-right sequence number of the column
    ///   in the main grid.  Zero if not to be included in the main grid.
    /// </param>
    public FieldAttribute(int sequenceNo) {
      SequenceNo = sequenceNo;
    }

    /// <summary>
    ///   Gets the one-based left-to-right sequence number of the column
    ///   in the main grid.  Zero if not to be included in the main grid.
    /// </summary>
    public int SequenceNo { get; }
  }
}