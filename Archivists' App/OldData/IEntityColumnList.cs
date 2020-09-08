using System;
using System.Collections.Generic;

namespace SoundExplorers.OldData {
  public interface IEntityColumnList : IList<IEntityColumn> {
    /// <summary>
    ///   Returns the entity column with the specified name (case-insensitive),
    ///   if found, otherwise returns a null reference.
    /// </summary>
    /// <param name="columnName">
    ///   The name of the column (case-insensitive).
    /// </param>
    /// <returns>
    ///   The entity column with the specified name (case-insensitive),
    ///   if found, otherwise a null reference.
    /// </returns>
    IEntityColumn this[string columnName] { get; }

    /// <summary>
    ///   Add the specified entity column to the list,
    ///   provided its name is unique in the list.
    /// </summary>
    /// <param name="entityColumn">
    ///   The entity column to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The list already contains an entity column
    ///   of the same name.
    /// </exception>
    new void Add(IEntityColumn entityColumn);

    /// <summary>
    ///   Returns whether the list contains
    ///   an entity column with the specified name.
    /// </summary>
    /// <param name="columnName">
    ///   The name of the column.
    /// </param>
    /// <returns>
    ///   Whether the list contains
    ///   an entity column with the specified name.
    /// </returns>
    bool ContainsKey(string columnName);
  }
}