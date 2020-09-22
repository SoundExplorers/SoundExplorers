using System;
using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Interface for a list of entities that populates a DataTable.
  /// </summary>
  public interface IEntityList : IList {
    /// <summary>
    ///   Gets the binding list representing the list of entities.
    /// </summary>
    [CanBeNull]
    IBindingList BindingList { get; }

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
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    [NotNull]
    SessionBase Session { get; set; }

    [NotNull] string TableName { get; }

    /// <summary>
    ///   Derived classes that are identifying parents should
    ///   return a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    [CanBeNull]
    IList GetChildren(int rowIndex);
    
    void OnRowEntered(int rowIndex);

    /// <summary>
    ///   If the specified table row is new or its data has changed,
    ///   inserts (if new) or updates the corresponding the entity
    ///   on the database with the table row data.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void OnRowLeft(int rowIndex);

    /// <summary>
    ///   Populates and sorts the list and table.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities.
    ///   If null, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    void Populate(IList list = null);
  }
}