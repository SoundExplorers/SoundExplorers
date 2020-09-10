﻿using System;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity column metadata.
  /// </summary>
  public class EntityColumn {
    public EntityColumn([NotNull] string displayName, [NotNull] Type dataType,
      string referencedTableName = null, string referencedColumnDisplayName = null) {
      DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
      DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
      if (referencedTableName != null && referencedColumnDisplayName == null ||
          referencedTableName == null && referencedColumnDisplayName != null) {
        throw new InvalidOperationException(
          $"Both or neither of {nameof(referencedTableName)} and " 
          + $"{nameof(referencedColumnDisplayName)} must be specified.");
      }
      ReferencedTableName = referencedTableName;
      ReferencedColumnDisplayName = referencedColumnDisplayName;
      IsVisible = true;
    }

    /// <summary>
    ///   Gets the type of data stored in the column.
    /// </summary>
    [NotNull]
    public Type DataType { get; }

    /// <summary>
    ///   Gets the display name to be used for reporting.
    /// </summary>
    [NotNull]
    public string DisplayName { get; }

    /// <summary>
    ///   Gets whether the column is to be shown in the table editor.
    ///   Default: true.
    /// </summary>
    public bool IsVisible { get; internal set; }

    /// <summary>
    ///   Gets the display name of the corresponding column on the referenced table.
    ///   Null if the column does not reference a column on another table.
    /// </summary>
    [CanBeNull]
    public string ReferencedColumnDisplayName { get; }

    /// <summary>
    ///   Gets the name of the referenced table.
    ///   Null if the column does not reference a column on another table.
    /// </summary>
    [CanBeNull]
    public string ReferencedTableName { get; }
  } //End of class
} //End of namespace