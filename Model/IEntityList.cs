using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    IDictionary<string, ReferenceableItemList>?
      ChildColumnReferenceableItemLists { get; }

    Type? ChildListType { set; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents the list of
    ///   entities.
    /// </summary>
    BindingColumnList Columns { get; }

    string EntityTypeName { get; }

    /// <summary>
    ///   Gets whether the current grid row is the insertion row, which is for adding new
    ///   entities and is located at the bottom of the grid.
    /// </summary>
    bool IsInsertionRowCurrent { get; }

    DatabaseUpdateErrorException? LastDatabaseUpdateErrorException { get; }
    ListRole ListRole { get; }
    IEntityList? ParentList { set; }

    /// <summary>
    ///   Gets the type of parent list (IEntityList) required when this is the main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </summary>
    Type? ParentListType { get; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database. The setter
    ///   should only be needed for testing.
    /// </summary>
    SessionBase Session { set; }

    /// <summary>
    ///   Removes an erroneous insertion binding item after first backing it up to be
    ///   restored when a new row is subsequently added.
    /// </summary>
    void BackupAndRemoveInsertionErrorBindingItem();

    /// <summary>
    ///   A main list that is a child list must implement this to instantiate its
    ///   parent list.
    /// </summary>
    IEntityList CreateParentList();

    /// <summary>
    ///   Deletes the entity at the specified row index from the database and removes it
    ///   from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void DeleteEntity(int rowIndex);

    /// <summary>
    ///   Derived classes that can be parent lists must return a list of the child
    ///   entities of the entity at the specified row index that are to populate the
    ///   child (main) list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    IdentifyingParentAndChildren? GetIdentifyingParentAndChildrenForChildList(
      int rowIndex);

    IList<object> GetErrorValues();

    /// <summary>
    ///   Occurs when an exception is thrown on ending a cell edit.
    /// </summary>
    /// <remarks>
    ///   A <see cref="DatabaseUpdateErrorException" />, which is explicitly thrown by
    ///   the application's code, is thrown at end of cell edit on existing rows but on
    ///   row validation for the insertion row, when this event is not raised.
    /// </remarks>
    void OnCellEditException(int rowIndex, string columnName, Exception exception);

    /// <summary>
    ///   This is called when any row has been entered.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void OnRowEnter(int rowIndex);

    /// <summary>
    ///   If the specified grid row is new or its data has changed, adds (if new) or
    ///   updates the corresponding entity on the database with the row data and saves
    ///   the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void OnRowValidated(int rowIndex);

    void OnValidationError(int rowIndex, string? propertyName, Exception exception);

    /// <summary>
    ///   Populates and sorts the list and table.
    /// </summary>
    /// <param name="identifyingParentAndChildren">
    ///   Optionally specifies the required list of entities, together with their
    ///   identifying parent. If null, the default, all entities of the class's entity
    ///   type will be fetched from the database.
    /// </param>
    /// <param name="createBindingList">
    ///   Optionally specifies whether the <see cref="BindingList" />, which will be
    ///   bound to a grid in the editor window, is to be populated along with the list of
    ///   entities. Default: true. Set to false if entity list is not to be used to
    ///   populate a grid.
    /// </param>
    void Populate(IdentifyingParentAndChildren? identifyingParentAndChildren = null,
      bool createBindingList = true);

    /// <summary>
    ///   After showing an error message for an invalid cell value on editing an existing
    ///   entity's row with the invalid value still shown, we now need to revert the cell
    ///   value to its original, as on the database.
    /// </summary>
    void ReplaceErrorBindingValueWithOriginal();
  }
}