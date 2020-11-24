using System;
using System.Collections;
using System.Collections.Generic;
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
    IBindingList BindingList { get; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents
    ///   the list of entities.
    /// </summary>
    [NotNull]
    BindingColumnList Columns { get; }

    [NotNull] string EntityTypeName { get; }
    
    //bool HasRowBeenEdited { get; set; }

    /// <summary>
    ///   For unknown reason, the grid's RowRemoved event is raised 2 or 3 times
    ///   while data is being loaded into the grid.
    ///   So this indicates whether the data has been completely loaded
    ///   and that it the RowRemoved event may indicate that
    ///   an entity deletion is required.
    /// </summary>
    bool IsDataLoadComplete { get; }

    /// <summary>
    ///   Gets whether the current grid row is the insertion row,
    ///   which is for adding new entities and is located at the bottom of the grid.
    /// </summary>
    bool IsInsertionRowCurrent { get;  }
    
    /// <summary>
    ///   Gets or sets whether this is a (read-only) parent list.
    ///   False (the default) if this is the (updatable) main (and maybe only) list.
    /// </summary>
    bool IsParentList { get; set; }

    DatabaseUpdateErrorException LastDatabaseUpdateErrorException { get; set; }

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
    SessionBase Session { set; }

    /// <summary>
    ///   Deletes the entity at the specified row index
    ///   from the database and removes it from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void DeleteEntity(int rowIndex);

    /// <summary>
    ///   Derived classes that are identifying parents should
    ///   return a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    [CanBeNull]
    IList GetChildrenForMainList(int rowIndex);

    [NotNull]
    IList<object> GetErrorValues();

    /// <summary>
    ///   This is called when any row has been entered.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void OnRowEnter(int rowIndex);

    /// <summary>
    ///   If the specified grid row is new or its data has changed,
    ///   adds (if new) or updates the corresponding the entity
    ///   on the database with the row data and
    ///   saves the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void OnRowValidated(int rowIndex);

    void OnValidationError(int rowIndex, [CanBeNull] string propertyName,
      [NotNull] Exception exception);

    /// <summary>
    ///   Populates and sorts the list and table.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities.
    ///   If null, the default, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    /// <param name="createBindingList">
    ///   Optionally specifies whether the <see cref="BindingList"/>,
    ///   which will be bound to a grid in the editor window,
    ///   is to be populated along with the list of entities.
    ///   Default: true.
    ///   Set to false if entity list is not to be used to populate a grid.
    /// </param>
    void Populate(IList list = null, bool createBindingList = true);

    void RestoreCurrentBindingItemOriginalValues();
    void RemoveInsertionBindingItem();
    void RestoreReferencingPropertyOriginalValue(int rowIndex, int columnIndex);
  }
}