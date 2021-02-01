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
    IBindingList? BindingList { get; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents the list of
    ///   entities.
    /// </summary>
    BindingColumnList Columns { get; }

    string EntityTypeName { get; }
    
    /// <summary>
    ///   Gets whether this is a main list that is to be populated with children of an
    ///   identifying parent entity.
    /// </summary>
    bool IsChildList { get; }

    /// <summary>
    ///   Gets whether the current grid row is the insertion row, which is for adding new
    ///   entities and is located at the bottom of the grid.
    /// </summary>
    bool IsInsertionRowCurrent { get; }

    bool IsRemovingInvalidInsertionRow { get; set; }
    DatabaseUpdateErrorException? LastDatabaseUpdateErrorException { get; set; }

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
    ///   Deletes the entity at the specified row index from the database and removes it
    ///   from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    void DeleteEntity(int rowIndex);

    /// <summary>
    ///   Derived classes that are identifying parents should return a list of the child
    ///   entities of the entity at the specified row index that are to populate the main
    ///   list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    IdentifyingParentChildren? GetIdentifyingParentChildrenForMainList(int rowIndex);

    IList<object> GetErrorValues();

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
    /// <param name="identifyingParentChildren">
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
    void Populate(IdentifyingParentChildren? identifyingParentChildren = null,
      bool createBindingList = true);

    void RestoreCurrentBindingItemOriginalValues();
    void RemoveInsertionBindingItem();
    void RestoreReferencingPropertyOriginalValue(int rowIndex, int columnIndex);
  }
}