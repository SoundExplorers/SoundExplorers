using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
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
    where TEntity : IEntity, new()
    where TBindingItem : BindingItemBase, new() {
    private EntityColumnList _columns;
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

    private TBindingItem BackupBindingItem { get; set; }
    private TBindingItem BackupBindingItemToRestoreFrom { get; set; }
    private TBindingItem BindingItemToFix { get; set; }
    private EntityComparer<TEntity> EntityComparer { get; }
    private bool HasRowBeenEdited { get; set; }
    private ChangeAction LastDatabaseChangeAction { get; set; }

    /// <summary>
    ///   Gets whether the current grid row is the insertion row,
    ///   which is for adding new entities and is located at the bottom of the grid.
    /// </summary>
    private bool IsInsertionRowCurrent { get; set; }
    
    private TEntity NewEntity { get; set; }

    /// <summary>
    ///   Gets the binding list representing the list of entities
    ///   and bound to the grid.
    /// </summary>
    public IBindingList BindingList { get; private set; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents
    ///   the list of entities.
    /// </summary>
    public EntityColumnList Columns => _columns ?? (_columns = CreateColumns());

    /// <summary>
    ///   For unknown reason, the grid's RowRemoved event is raised 2 or 3 times
    ///   while data is being loaded into the grid.
    ///   So this indicates whether the data has been completely loaded
    ///   and that it the RowRemoved event may indicate that
    ///   an entity deletion is required.
    /// </summary>
    public bool IsDataLoadComplete { get; private set; }

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
    ///   If the specified grid row is new,
    ///   adds a new entity to the list with the row data and
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
    public void InsertEntityIfNew(int rowIndex) {
      if (!(IsInsertionRowCurrent && HasRowBeenEdited)) {
        IsInsertionRowCurrent = false;
        return;
      }
      AddNewEntity(rowIndex);
    }

    /// <summary>
    ///   Derived classes that are identifying parents should
    ///   return a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    [NotNull]
    public abstract IList GetChildren(int rowIndex);

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
        // Not forced to reenter row to fix error
        BackupBindingItem = !IsInsertionRowCurrent
          ? CreateBackupBindingItem((TBindingItem)BindingList[rowIndex])
          : new TBindingItem();
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
      RestoreBindingItemPropertiesFromBackup(BackupBindingItemToRestoreFrom,
        BindingItemToFix);
      BackupBindingItemToRestoreFrom = null;
      BindingItemToFix = null;
      HasRowBeenEdited = false;
    }

    [NotNull]
    protected abstract TBindingItem CreateBackupBindingItem(
      [NotNull] TBindingItem bindingItem);

    [NotNull]
    protected abstract TEntity CreateBackupEntity(
      [NotNull] TEntity entity);

    [NotNull]
    protected abstract TBindingItem CreateBindingItem([NotNull] TEntity entity);

    [NotNull]
    protected abstract EntityColumnList CreateColumns();

    protected abstract void RestoreBindingItemPropertiesFromBackup(
      [NotNull] TBindingItem backupBindingItem,
      [NotNull] TBindingItem bindingItemToRestore);

    protected abstract void RestoreEntityPropertiesFromBackup(
      [NotNull] TEntity backupEntity, [NotNull] TEntity entityToRestore);

    protected abstract void UpdateEntity([NotNull] TBindingItem bindingItem,
      [NotNull] TEntity entity);

    protected abstract void UpdateEntityProperty([NotNull] string propertyName,
      [CanBeNull] object newValue, [NotNull] TEntity entity);

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
      Session.BeginUpdate();
      try {
        Session.Persist(NewEntity);
        Add(NewEntity);
      } catch (Exception exception) {
        BindingItemToFix = (TBindingItem)BindingList[rowIndex];
        BackupBindingItemToRestoreFrom = null;
        // TODO: Persistence (and other data) exceptions must be PropertyConstraintException to provide error property name  
        throw CreateDatabaseUpdateErrorException(exception, rowIndex, "Date");
      } finally {
        Session.Commit();
        IsInsertionRowCurrent = false;
      }
    }

    private void BindingListOnListChanged(object sender, ListChangedEventArgs e) {
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded: // Insertion row entered
          // Debug.WriteLine("ListChangedType.ItemAdded: Insertion row entered");
          IsDataLoadComplete = true;
          IsInsertionRowCurrent = true;
          break;
        case ListChangedType.ItemChanged: // Cell edit completed 
          // Debug.WriteLine(
          //   $"ListChangedType.ItemChanged:  {e.PropertyDescriptor.Name} = '{e.PropertyDescriptor.GetValue(BindingList[e.NewIndex])}', cell edit completed or cancelled");
          if (!HasRowBeenEdited) {
            HasRowBeenEdited = true;
            if (IsInsertionRowCurrent) {
              NewEntity = CreateEntity();
              LastDatabaseChangeAction = ChangeAction.Insert;
            }
          }
          var newValue = e.PropertyDescriptor.GetValue(BindingList[e.NewIndex]); 
          if (IsInsertionRowCurrent) {
            UpdateNewEntityProperty(e.NewIndex, e.PropertyDescriptor.Name, newValue);
          } else {
            UpdateExistingEntityProperty(e.NewIndex, e.PropertyDescriptor.Name, newValue);
          }
          break;
        case ListChangedType.ItemDeleted: // Insertion row left without saving data
          // Debug.WriteLine(
          //   "ListChangedType.ItemDeleted:  Insertion row left without saving data");
          break;
        default:
          // Debug.WriteLine("ListChangedType default");
          throw new NotSupportedException(e.ListChangedType.ToString());
      }
    }

    [NotNull]
    private DatabaseUpdateErrorException CreateDatabaseUpdateErrorException(
      [NotNull] Exception exception, int rowIndex, string propertyName = null) {
      if (!IsDatabaseUpdateError(exception)) {
        throw exception; // Terminal error
      }
      int columnIndex = propertyName != null ? Columns.IndexOf(Columns[propertyName]) : 0;
      var errorValue = propertyName != null
        ? BindingItemToFix?.GetPropertyValue(propertyName)
        : null;
      LastDatabaseUpdateErrorException = new DatabaseUpdateErrorException(
        LastDatabaseChangeAction, exception.Message, rowIndex, columnIndex, errorValue,
        exception);
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

    private static TEntity CreateEntity() {
      try {
        return (TEntity)Activator.CreateInstance(typeof(TEntity));
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }

    /// <summary>
    ///   Returns whether the specified exception indicates that,
    ///   for an anticipated reason, a requested database update could not be done,
    ///   in which case the exception's message will need to be shown to the user to
    ///   explain the error that cause the update to be disallowed.
    ///   If false, the exception should be treated as a terminal error.
    /// </summary>
    private static bool IsDatabaseUpdateError(Exception exception) {
      return exception is DataException || exception is DuplicateKeyException ||
             exception is FormatException;
    }

    private void UpdateExistingEntityProperty(int rowIndex, [NotNull] string propertyName,
      [CanBeNull] object newValue) {
      LastDatabaseChangeAction = ChangeAction.Update;
      var bindingItem = (TBindingItem)BindingList[rowIndex];
      var entity = this[rowIndex];
      var backupEntity = CreateBackupEntity(entity);
      Session.BeginUpdate();
      try {
        UpdateEntityProperty(propertyName, newValue, entity);
      } catch (Exception exception) {
        BindingItemToFix = bindingItem;
        RestoreEntityPropertiesFromBackup(backupEntity, entity);
        BackupBindingItemToRestoreFrom = BackupBindingItem;
        // This exception will be passed to the grid's DataError event handler.
        throw CreateDatabaseUpdateErrorException(exception, rowIndex, propertyName);
      } finally {
        Session.Commit();
      }
    }

    private void UpdateNewEntityProperty(int rowIndex, [NotNull] string propertyName,
      [CanBeNull] object newValue) {
      try {
        UpdateEntityProperty(propertyName, newValue, NewEntity);
      } catch (Exception exception) {
        BindingItemToFix = (TBindingItem)BindingList[rowIndex];
        // This exception will be passed to the grid's DataError event handler.
        throw CreateDatabaseUpdateErrorException(exception, rowIndex, propertyName);
      }
    }
  }
}