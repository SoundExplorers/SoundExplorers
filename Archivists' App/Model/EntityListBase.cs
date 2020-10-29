using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Base class for a list of entities that populates a binding list.
  /// </summary>
  /// <typeparam name="TEntity">
  ///   Entity type parameter
  /// </typeparam>
  /// <typeparam name="TBindingItem">
  ///   Binding list item type parameter
  /// </typeparam>
  public abstract class EntityListBase<TEntity, TBindingItem> : List<TEntity>, IEntityList
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private BindingColumnList _columns;
    private SessionBase _session;

    /// <summary>
    ///   Base constructor for derived entity list classes.
    /// </summary>
    /// <param name="parentListType">
    ///   The type of (read-only) parent list (IEntityList)
    ///   required when this is the (updatable) main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </param>
    protected EntityListBase(Type parentListType = null) {
      if (parentListType != null) {
        ParentListType = parentListType.GetInterfaces().Contains(typeof(IEntityList))
          ? parentListType
          : throw new ArgumentException(
            $"If specified, {nameof(parentListType)} must implement {nameof(IEntityList)}.",
            nameof(parentListType));
      } else {
        ParentListType = null;
      }
      EntityComparer = new EntityComparer<TEntity>();
    }

    // private TEntity BackupEntity { get; set; }
    private TBindingItem BackupBindingItem { get; set; }
    private TBindingItem BackupBindingItemToRestoreFrom { get; set; }
    private TBindingItem BindingItemToFix { get; set; }
    private EntityComparer<TEntity> EntityComparer { get; }
    private TBindingItem ErrorBindingItem { get; set; }
    private bool HasRowBeenEdited { get; set; }
    private ChangeAction LastDatabaseChangeAction { get; set; }

    /// <summary>
    ///   Gets whether the current grid row is the insertion row,
    ///   which is for adding new entities and is located at the bottom of the grid.
    /// </summary>
    private bool IsInsertionRowCurrent { get; set; }

    /// <summary>
    ///   Gets the binding list representing the list of entities
    ///   and bound to the grid.
    /// </summary>
    public IBindingList BindingList { get; private set; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents
    ///   the list of entities.
    /// </summary>
    public BindingColumnList Columns => _columns ?? (_columns = CreateColumns());

    /// <summary>
    ///   For unknown reason, the grid's RowRemoved event is raised 2 or 3 times
    ///   while data is being loaded into the grid.
    ///   So this indicates whether the data has been completely loaded
    ///   and that it the RowRemoved event may indicate that
    ///   an entity deletion is required.
    /// </summary>
    public bool IsDataLoadComplete { get; private set; }

    /// <summary>
    ///   If there's an error on adding a new entity,
    ///   the data to be fixed will be in a row before the insertion row
    ///   to allow the user to either fix the error and try the add again
    ///   or cancel the add.
    ///   Either way, we temporarily have a grid row that's neither the insertion row
    ///   not bound to an entity.  So it needs special housekeeping to make
    ///   sure an entity gets persisted if the error is fixed
    ///   or the row gets removed from the grid when if the insertion is cancelled.
    /// </summary>
    public bool IsFixingNewRow { get; set; }

    /// <summary>
    ///   True if this is a (read-only) parent list.
    ///   False (the default) if this is the (updatable) main (and maybe only) list.
    /// </summary>
    public bool IsParentList { get; set; }

    public DatabaseUpdateErrorException LastDatabaseUpdateErrorException { get; set; }

    /// <summary>
    ///   Gets the type of parent list (IEntityList) required when this is the main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </summary>
    public Type ParentListType { get; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    public SessionBase Session {
      get => _session ?? (_session = Global.Session);
      // ReSharper disable once UnusedMember.Global
      set => _session = value;
    }

    public string TableName => typeof(TEntity).Name;

    /// <summary>
    ///   Deletes the entity at the specified row index
    ///   from the database and removes it from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="DatabaseUpdateErrorException">
    ///   A database update error occured.
    /// </exception>
    public void DeleteEntity(int rowIndex) {
      // Debug.WriteLine(
      //   $"{nameof(DeleteEntity)}: IsInsertionRowCurrent = {IsInsertionRowCurrent}; BindingList.Count = {BindingList.Count}");
      LastDatabaseChangeAction = ChangeAction.Delete;
      Session.BeginUpdate();
      try {
        //throw new ConstraintException("Test error message");
        Session.Unpersist(this[rowIndex]);
        RemoveAt(rowIndex);
      } catch (Exception exception) {
        BindingList.Insert(rowIndex, BackupBindingItem);
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        Session.Commit();
      }
    }

    /// <summary>
    ///   Derived classes that can be parent lists should
    ///   return a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    [NotNull]
    public abstract IList GetChildrenForMainList(int rowIndex);

    public IList<object> GetErrorValues() {
      return ErrorBindingItem.GetPropertyValues();
    }

    /// <summary>
    ///   This is called when any row has been entered.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <remarks>
    ///   This is called when the grid's RowEnter event is raised.
    ///   If the row is the insertion row,
    ///   this will be called after the ItemAdded ListChangedType
    ///   of the BindingList's ListChanged event.
    /// </remarks>
    public void OnRowEnter(int rowIndex) {
      // Debug.WriteLine(
      //   $"{nameof(OnRowEnter)}:  Any row entered (after ItemAdded if insertion row)");
      HasRowBeenEdited = false;
      if (BackupBindingItemToRestoreFrom == null) {
        // Not forced to reenter row to fix an update error
        BackupBindingItem = !IsInsertionRowCurrent
          ? ((TBindingItem)BindingList[rowIndex]).CreateBackup()
          : new TBindingItem();
      }
    }

    /// <summary>
    ///   If the specified grid row is new or its data has changed,
    ///   adds (if new) or updates the corresponding the entity
    ///   on the database with the row data and
    ///   saves the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <remarks>
    ///   Though this is called when the grid's RowValidated event is raised,
    ///   that actually happens even if the user did no edits,
    ///   when any row left but after the final ItemChanged ListChangedType,
    ///   if any, of the BindingList's ListChanged event.
    /// </remarks>
    /// <exception cref="DatabaseUpdateErrorException">
    ///   A database update error occured.
    /// </exception>
    public void OnRowValidated(int rowIndex) {
      if (!HasRowBeenEdited) {
        IsInsertionRowCurrent = false;
        if (IsFixingNewRow) {
          // When an insertion error message was shown,
          // focus was forced back to the error row,
          // now no longer the insertion row,
          // in EditorController.ShowDatabaseUpdateError.
          // That raised the EditorView.MainGridOnRowEnter event.
          // Then the user opted to cancel out of adding the new row
          // rather than fixing it so that the add would work.
          // That raised the EditorView.MainRowValidated event
          // even though nothing has changed.
          // That got us here.
          // We need to remove the unwanted new row from the grid.
          BindingList.RemoveAt(rowIndex);
          IsFixingNewRow = false;
        }
        return;
      }
      if (IsInsertionRowCurrent || IsFixingNewRow) {
        IsInsertionRowCurrent = false;
        AddNewEntity(rowIndex);
      } else {
        SaveChangesToExistingEntity(rowIndex);
      }
    }

    /// <summary>
    ///   Populates and sorts the list and table.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities.
    ///   If null, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    public void Populate(IList list = null) {
      Clear();
      if (list != null) {
        AddRange((IList<TEntity>)list);
      } else {
        Session.BeginRead();
        AddRange(Session.AllObjects<TEntity>());
        Session.Commit();
        Sort(EntityComparer);
      }
      if (BindingList != null) {
        BindingList.ListChanged -= BindingListOnListChanged;
      }
      BindingList = CreateBindingList();
      BindingList.ListChanged += BindingListOnListChanged;
    }

    public void RemoveCurrentBindingItem() {
      BindingList?.Remove(BindingItemToFix);
      BindingItemToFix = null;
    }

    public void RestoreCurrentBindingItemOriginalValues() {
      ErrorBindingItem = BindingItemToFix.CreateBackup();
      BindingItemToFix.RestorePropertyValuesFrom(BackupBindingItemToRestoreFrom);
      BackupBindingItemToRestoreFrom = null;
      BindingItemToFix = null;
      HasRowBeenEdited = false;
    }

    [NotNull]
    protected abstract TBindingItem CreateBindingItem([NotNull] TEntity entity);

    [NotNull]
    protected abstract BindingColumnList CreateColumns();

    /// <summary>
    ///   Adds a new entity to the list with the data in the specified grid row,
    ///   which should be the grid's insertion row,
    ///   and saves the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="DatabaseUpdateErrorException">
    ///   A database update error occured.
    /// </exception>
    private void AddNewEntity(int rowIndex) {
      LastDatabaseChangeAction = ChangeAction.Insert;
      var bindingItem = (TBindingItem)BindingList[rowIndex];
      Session.BeginUpdate();
      try {
        var entity = bindingItem.CreateEntity();
        Session.Persist(entity);
        Add(entity);
        IsFixingNewRow = false;
      } catch (Exception exception) {
        ErrorBindingItem = bindingItem;
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        Session.Commit();
      }
    }

    private void BindingListOnListChanged(object sender, ListChangedEventArgs e) {
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded: // Insertion row entered
          //Debug.WriteLine("ListChangedType.ItemAdded: Insertion row entered");
          IsDataLoadComplete = true;
          IsInsertionRowCurrent = true;
          break;
        case ListChangedType.ItemChanged: // Cell edit completed 
          // Debug.WriteLine(
          //   $"ListChangedType.ItemChanged:  {e.PropertyDescriptor.Name} = '{e.PropertyDescriptor.GetValue(BindingList[e.NewIndex])}', cell edit completed or cancelled");
          HasRowBeenEdited = true;
          if (!IsInsertionRowCurrent && !IsFixingNewRow) {
            UpdateExistingEntityProperty(e.NewIndex, e.PropertyDescriptor.Name);
          }
          break;
      }
    }

    [NotNull]
    private DatabaseUpdateErrorException CreateDatabaseUpdateErrorException(
      [NotNull] Exception exception, int rowIndex) {
      if (!IsDatabaseUpdateError(exception)) {
        // Terminal error.  In the Release compilation,
        // the stack trace will be shown by the terminal error handler in Program.cs.
        throw exception;
      }
      string propertyName =
        exception is PropertyConstraintException propertyConstraintException
          ? propertyConstraintException.PropertyName
          : null;
      int columnIndex = propertyName != null ? Columns.IndexOf(Columns[propertyName]) : 0;
      LastDatabaseUpdateErrorException = new DatabaseUpdateErrorException(
        LastDatabaseChangeAction, exception.Message, rowIndex, columnIndex, exception);
      return LastDatabaseUpdateErrorException;
    }

    [NotNull]
    private BindingList<TBindingItem> CreateBindingList() {
      var list = (
        from entity in this
        select CreateBindingItem(entity)
      ).ToList();
      return new BindingList<TBindingItem>(list);
    }

    /// <summary>
    ///   Returns whether the specified exception indicates that,
    ///   for an anticipated reason, a requested database update could not be done,
    ///   in which case the exception's message will need to be shown to the user to
    ///   explain the error that cause the update to be disallowed.
    ///   If false, the exception should be treated as a terminal error.
    /// </summary>
    private static bool IsDatabaseUpdateError(Exception exception) {
      return exception is ConstraintException; // Includes PropertyConstraintException
    }

    /// <summary>
    ///   Saves changes to an existing entity.
    /// </summary>
    /// <remarks>
    ///   For reasons I don't fully understand,
    ///   even though the individual property values
    ///   of the entity have already been updated,
    ///   including calling UpdateNonIndexField for each property,
    ///   we still have to call UpdateNonIndexField one more time,
    ///   now that row validation is complete,
    ///   in order to save the changes to the database.
    ///   <para>
    ///   Because validation by property has already been done,
    ///   user errors are not expected at this stage.
    ///   </para>
    /// </remarks>
    /// <param name="rowIndex">Zero-based row index</param>
    private void SaveChangesToExistingEntity(int rowIndex) {
      var entity = this[rowIndex];
      Session.BeginUpdate();
      try {
        entity.UpdateNonIndexField();
      } finally {
        Session.Commit();
      }
    }

    private void UpdateExistingEntityProperty(int rowIndex, [NotNull] string propertyName) {
      //Debug.WriteLine("EntityListBase.UpdateExistingEntityProperty");
      LastDatabaseChangeAction = ChangeAction.Update;
      var bindingItem = (TBindingItem)BindingList[rowIndex];
      var entity = this[rowIndex];
      //Debug.WriteLine($"Backing up {entity}");
      var backupBindingItem = CreateBindingItem(entity);
      Session.BeginUpdate();
      try {
        //Debug.WriteLine($"IsPersistent before update = {entity.IsPersistent}");
        bindingItem.UpdateEntityProperty(propertyName, entity);
      } catch (Exception exception) {
        BindingItemToFix = bindingItem;
        backupBindingItem.CopyPropertyValuesToEntity(entity);
        BackupBindingItemToRestoreFrom = BackupBindingItem;
        // This exception will be passed to the grid's DataError event handler.
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        Session.Commit();
      }
    }
  }
}