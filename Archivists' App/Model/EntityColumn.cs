using System;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Entity column metadata.
  /// </summary>
  public class EntityColumn {
    public EntityColumn([NotNull] string displayName,
      Type referencedEntityListType = null, string referencedColumnDisplayName = null) {
      DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
      if (referencedEntityListType != null && referencedColumnDisplayName == null ||
          referencedEntityListType == null && referencedColumnDisplayName != null) {
        throw new InvalidOperationException(
          $"Both or neither of {nameof(referencedEntityListType)} and "
          + $"{nameof(referencedColumnDisplayName)} must be specified.");
      }
      ReferencedEntityListType = referencedEntityListType;
      ReferencedColumnDisplayName = referencedColumnDisplayName;
    }

    /// <summary>
    ///   Gets the display name to be used for reporting.
    /// </summary>
    [NotNull]
    public string DisplayName { get; }

    /// <summary>
    ///   Gets the display name of the corresponding column on the referenced table.
    ///   Null if the column does not reference a column on another table.
    /// </summary>
    [CanBeNull]
    public string ReferencedColumnDisplayName { get; }

    /// <summary>
    ///   Gets the type of the referenced entity list.
    ///   Null if the column does not reference a column on another entity list.
    /// </summary>
    [CanBeNull]
    public Type ReferencedEntityListType { get; }

    /// <summary>
    ///   Gets the name of the referenced entity list's main table.
    ///   Null if the column does not reference a column on another entity list.
    /// </summary>
    [CanBeNull]
    public string ReferencedTableName =>
      ReferencedEntityListType?.Name.Replace("List", string.Empty);
  } //End of class
} //End of namespace