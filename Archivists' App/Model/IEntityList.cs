using System.Data;
using JetBrains.Annotations;

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
    ///   Gets the data set containing the main <see cref="Table" />
    ///   and, if specified, the parent table.
    /// </summary>
    [CanBeNull]
    DataSet DataSet { get; }

    /// <summary>
    ///   Gets the list of entities represented in the main table's
    ///   parent table, or null if a parent list is not required.
    /// </summary>
    [CanBeNull]
    IEntityList ParentList { get; }

    /// <summary>
    ///   Gets the data columns that uniquely identify the a row in the table.
    /// </summary>
    [NotNull]
    DataColumn[] PrimaryKeyDataColumns { get; }

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    [NotNull]
    DataTable Table { get; }

    /// <summary>
    ///   Fetches the required entities from the database
    ///   and populates the list and table with them.
    /// </summary>
    void Fetch();
  }
}