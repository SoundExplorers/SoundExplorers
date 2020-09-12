﻿using System;
using System.Collections;
using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Interface for a list of entities that populates a DataTable.
  /// </summary>
  public interface IEntityList {
    /// <summary>
    ///   Gets metadata for the columns of the Table that represents
    ///   the list of entities.
    /// </summary>
    [NotNull]
    EntityColumnList Columns { get; }

    /// <summary>
    ///   True if this is a (read-only) parent list.
    ///   False (the default) if this is the (updatable) main (and maybe only) list.
    /// </summary>
    bool IsParentList { get; set; }

    /// <summary>
    ///   Gets the type of parent list (IEntityList) required when this is the main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </summary>
    [CanBeNull]
    Type ParentListType { get; }

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    [NotNull]
    DataTable Table { get; }

    /// <summary>
    ///   Deletes the entity at the specified row index
    ///   from the database and removes it from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void DeleteEntity(int rowIndex);

    /// <summary>
    ///   Returns a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list if this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    [CanBeNull]
    IList GetChildren(int rowIndex);

    /// <summary>
    ///   Populates the list and table.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities.
    ///   If null, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    void Populate(IList list = null);

    /// <summary>
    ///   Updates the entity at the specified row index
    ///   with the data in the corresponding table row.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void UpdateEntity(int rowIndex);
  }
}