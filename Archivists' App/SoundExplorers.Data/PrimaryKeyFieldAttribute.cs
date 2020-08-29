using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Indicates that a property of an entity class represents a
  ///   primary key field required for database access.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  internal class PrimaryKeyFieldAttribute : FieldAttribute {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="PrimaryKeyFieldAttribute" /> class.
    /// </summary>
    /// <param name="sequenceNo">
    ///   The one-based left-to-right sequence number of the column
    ///   in the main grid.  Zero if not to be included in the main grid.
    /// </param>
    public PrimaryKeyFieldAttribute(int sequenceNo = 0) : base(sequenceNo) { }
  }
}