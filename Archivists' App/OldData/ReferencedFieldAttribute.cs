using System;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   Indicates that a property of an entity class represents a
  ///   field required for database access,
  ///   where the field is populated from a column
  ///   on a referenced database table whose table name
  ///   and Entity name is the property name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  internal class ReferencedFieldAttribute : FieldAttribute {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="ReferencedFieldAttribute" /> class.
    /// </summary>
    /// <param name="name">
    ///   The name of the field property on
    ///   the referenced Entity,
    ///   optionally prefixed by the
    ///   table name followed by a dot.
    /// </param>
    /// <param name="sequenceNo">
    ///   The one-based left-to-right sequence number of the column
    ///   in the main grid.  Zero if not to be included in the main grid.
    /// </param>
    /// <remarks>
    ///   The table name, if not included in
    ///   <paramref name="name" />,
    ///   will be assumed to be the
    ///   name of the field property on
    ///   the referencing Entity.
    /// </remarks>
    public ReferencedFieldAttribute(
      string name, int sequenceNo = 0) : base(sequenceNo) {
      Name = name;
    }

    /// <summary>
    ///   Gets the name of the field property on
    ///   the referenced Entity,
    ///   optionally prefixed by the
    ///   table name followed by a dot.
    /// </summary>
    /// <remarks>
    ///   The table name, if not included,
    ///   will be assumed to be the
    ///   name of the field property on
    ///   the referencing Entity.
    /// </remarks>
    public string Name { get; }
  }
}