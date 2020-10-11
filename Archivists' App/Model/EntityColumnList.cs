using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Metadata for the columns of a table representing
  ///   a list of entities of a specific entity type.
  /// </summary>
  /// <remarks>
  ///   A keyed list of entity columns with
  ///   <see cref="EntityColumn.Name" /> as the key.
  /// </remarks>
  public class EntityColumnList : List<EntityColumn> {
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
    public EntityColumn this[string displayName] =>
    (
      from EntityColumn entityColumn in this
      where string.Compare(entityColumn.Name, displayName,
        StringComparison.OrdinalIgnoreCase) == 0
      select entityColumn).FirstOrDefault();

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
    public new void Add(EntityColumn entityColumn) {
      if (ContainsKey(entityColumn.Name)) {
        throw new ArgumentException(
          $"The list already contains an entity column named {entityColumn.Name}.",
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
    private bool ContainsKey(string displayName) {
      return (
        from EntityColumn entityColumn in this
        where entityColumn.Name == displayName
        select entityColumn).Any();
    }
  } //End of class
} //End of namespace