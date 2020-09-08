using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A keyed list of entity columns with
  ///   <see cref="IEntityColumn.DisplayName" /> as the key.
  /// </summary>
  public class EntityColumnList : List<IEntityColumn>, IEntityColumnList {
    /// <summary>
    ///   Add the specified entity column to the list,
    ///   provided its display name is unique in the list.
    /// </summary>
    /// <param name="entityColumn">
    ///   The entity column to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The list already contains an entity column
    ///   of the same display name.
    /// </exception>
    public new void Add(IEntityColumn entityColumn) {
      if (ContainsKey(entityColumn.DisplayName)) {
        throw new ArgumentException(
          $"The list already contains an entity column named {entityColumn.DisplayName}.",
          nameof(entityColumn));
      }
      base.Add(entityColumn);
    }

    /// <summary>
    ///   Returns whether the list contains
    ///   an entity column with the specified display name.
    /// </summary>
    /// <param name="displayName">
    ///   The display name of the column.
    /// </param>
    /// <returns>
    ///   Whether the list contains
    ///   an entity column with the specified display name.
    /// </returns>
    public bool ContainsKey(string displayName) {
      return (
        from IEntityColumn entityColumn in this
        where entityColumn.DisplayName == displayName
        select entityColumn).Any();
    }

    /// <summary>
    ///   Returns the entity column with the specified display name (case-insensitive),
    ///   if found, otherwise returns a null reference.
    /// </summary>
    /// <param name="displayName">
    ///   The display name of the column (case-insensitive).
    /// </param>
    /// <returns>
    ///   The entity column with the specified display name (case-insensitive),
    ///   if found, otherwise a null reference.
    /// </returns>
    [CanBeNull]
    public IEntityColumn this[string displayName] =>
    (
      from IEntityColumn entityColumn in this
      where string.Compare(entityColumn.DisplayName, displayName,
        StringComparison.OrdinalIgnoreCase) == 0
      select entityColumn).FirstOrDefault();
  } //End of class
} //End of namespace