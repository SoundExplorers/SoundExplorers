using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  public interface IEntityColumnList : IList<IEntityColumn> {
    
    /// <summary>
    ///   Returns the entity column with the specified name (case-insensitive),
    ///   if found, otherwise returns a null reference.
    /// </summary>
    /// <param name="displayName">
    ///   The name of the column (case-insensitive).
    /// </param>
    /// <returns>
    ///   The entity column with the specified name (case-insensitive),
    ///   if found, otherwise a null reference.
    /// </returns>
    IEntityColumn this[string displayName] { get; }

    /// <summary>
    ///   Add the specified entity column to the list,
    ///   provided its name is unique in the list.
    /// </summary>
    /// <param name="entityColumn">
    ///   The entity column to be added.
    /// </param>
    new void Add([NotNull] IEntityColumn entityColumn);

    /// <summary>
    ///   Returns whether the list contains
    ///   an entity column with the specified name.
    /// </summary>
    /// <param name="displayName">
    ///   The name of the column.
    /// </param>
    /// <returns>
    ///   Whether the list contains
    ///   an entity column with the specified name.
    /// </returns>
    bool ContainsKey([NotNull] string displayName);
  }
}