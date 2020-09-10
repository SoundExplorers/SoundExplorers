using System.Data;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Entity list interface.
  /// </summary>
  public interface IEntityList {
    /// <summary>
    ///   Gets metadata for the columns of a table representing
    ///   a list of entities of a specific type.
    /// </summary>
    [NotNull]
    EntityColumnList Columns { get; }

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    [NotNull]
    DataTable Table { get; }

    /// <summary>
    ///   Fetches the required entities from the database
    ///   and populates the list and table with them.
    /// </summary>
    /// <param name="identifyingParent">
    ///   Optionally specifies the identifying parent entity
    ///   whose child entities of the class's entity type are to be listed.
    ///   Null if all entities of the class's entity type are to be listed.
    /// </param>
    void Fetch(EntityBase identifyingParent = null);
  }
}