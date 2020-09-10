using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Metadata for the columns of a table representing
  ///   a list of entities of a specific type.
  /// </summary>
  public interface IEntityColumnList : IList<EntityColumn> {
    
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
    EntityColumn this[string displayName] { get; }

    /// <summary>
    ///   Add the specified entity column to the list,
    ///   provided its name is unique in the list.
    /// </summary>
    /// <param name="entityColumn">
    ///   The entity column to be added.
    /// </param>
    new void Add([NotNull] EntityColumn entityColumn);
  }
}