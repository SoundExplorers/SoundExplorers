using System;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Column metadata for a binding list that links entities to a grid.
  /// </summary>
  public class BindingColumn {
    public BindingColumn([NotNull] string name,
      Type referencedEntityListType = null, string referencedColumnName = null) {
      Name = name ?? throw new ArgumentNullException(nameof(name));
      if (referencedEntityListType != null &&
          !referencedEntityListType.GetInterfaces().Contains(typeof(IEntityList))) {
        throw new ArgumentException(
          $"The Type specified by {nameof(referencedEntityListType)} " +
          $"'{referencedEntityListType}' is invalid. If specified, it must " +
          $"implement the {nameof(IEntityList)} interface.",
          nameof(referencedEntityListType));
      }
      if (referencedEntityListType != null && referencedColumnName == null ||
          referencedEntityListType == null && referencedColumnName != null) {
        throw new InvalidOperationException(
          $"Both or neither of {nameof(referencedEntityListType)} and "
          + $"{nameof(referencedColumnName)} must be specified.");
      }
      ReferencedEntityListType = referencedEntityListType;
      ReferencedColumnName = referencedColumnName;
    }

    /// <summary>
    ///   Gets the display name to be used for reporting.
    ///   If null, <see cref="Name" /> should be used.
    /// </summary>
    [CanBeNull]
    public string DisplayName { get; set; }

    /// <summary>
    ///   Gets the column's property name.
    /// </summary>
    [NotNull]
    public string Name { get; }

    /// <summary>
    ///   Gets the name of the corresponding column on the referenced table.
    ///   Null if the column does not reference a column on another table.
    /// </summary>
    [CanBeNull]
    public string ReferencedColumnName { get; }

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