using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity list interface.
  /// </summary>
  public interface IEntityList : IEntityColumnContainer, IList {
    /// <summary>
    ///   Gets the data set containing the main <see cref="Table" />
    ///   and, if specified, the parent table.
    /// </summary>
    DataSet DataSet { get; }

    /// <summary>
    ///   Gets the list of entities representing the main table's
    ///   parent table, if specified.
    /// </summary>
    [CanBeNull]
    IEntityList ParentList { get; }

    /// <summary>
    ///   Gets name of the the data table containing the database records
    ///   represented by the list of entities.
    /// </summary>
    DataTable Table { get; }

    /// <summary>
    ///   Adds an Entity to the end of the list.
    /// </summary>
    /// <param name="entity">The Entity to added</param>
    void Add(IEntity entity);

    /// <summary>
    ///   An event that is raised when there is an error on
    ///   attempting to insert, update or delete a <see cref="DataRow" />.
    /// </summary>
    event EventHandler<RowErrorEventArgs> RowError;

    /// <summary>
    ///   An event that is raised when a <see cref="DataRow" />
    ///   has been successfully inserted or updated on the database.
    /// </summary>
    event EventHandler<RowUpdatedEventArgs> RowUpdated;

    /// <summary>
    ///   Updates the database table with any changes that have been input
    ///   and refreshes the list of Entities.
    /// </summary>
    void Update(IDictionary<string, object> oldKeyFieldValues = null);
  } //End of interface
} //End of interface