using System;

namespace SoundExplorers.Model {
  internal class ReferenceType {
    public ReferenceType(Type? referencedEntityListType, string? referencedPropertyName) {
      ReferencedEntityListType = referencedEntityListType;
      ReferencedPropertyName = referencedPropertyName;
    }

    /// <summary>
    ///   Gets the type of the referenced entity list. Null if the column does not
    ///   reference a column on another entity list.
    /// </summary>
    public Type? ReferencedEntityListType { get; }

    /// <summary>
    ///   Gets the name of the corresponding property of the referenced entity. Null if
    ///   the column does not reference a another entity type.
    /// </summary>
    public string? ReferencedPropertyName { get; }
  }
}