using System.Collections;
using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Base class for a list of entities that populates a DataTable.
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
    ///   Populates the list and table.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities.
    ///   If null, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    void Populate(IList list = null);
  }
}