using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
    private BindingColumnList? _columns;
    private QueryHelper? _queryHelper;
    private SessionBase? _session;

    /// <summary>
    ///   Base constructor for derived entity list classes.
    /// </summary>
    /// <param name="parentListType">
    ///   The type of (read-only) parent list (IEntityList) required when this is the
    ///   (updatable) main list. Null if a parent list is not required when this is the
    ///   main list.
    /// </param>
    protected EntityListBase(Type? parentListType = null) {
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

    private TBindingItem? BackupBindingItem { get; set; }
    private TBindingItem? BackupBindingItemToRestoreFrom { get; set; }
    private TBindingItem? BindingItemToFix { get; set; }
    private EntityComparer<TEntity> EntityComparer { get; }
    private TBindingItem? ErrorBindingItem { get; set; }
    internal bool IsChildList => ParentListType != null;
    private StatementType LastDatabaseChangeAction { get; set; }

    /// <summary>
    ///   The setter should only be needed for testing.
    /// </summary>
    internal QueryHelper QueryHelper {
      get => _queryHelper ??= QueryHelper.Instance;
      set => _queryHelper = value;
    }

    /// <summary>
    ///   Gets a strongly typed view of the binding list to facilitate testing.
    /// </summary>
    internal TypedBindingList<TEntity, TBindingItem> TypedBindingList =>
      (TypedBindingList<TEntity, TBindingItem>)BindingList!;

    private bool HasRowBeenEdited { get; set; }
    
    /// <summary>
    ///   Only applicable to a main list that is a child of a parent entity, this
    ///   specifies the list's identifying parent entity.
    /// </summary>
    public IEntity? IdentifyingParent { get; private set; }

    /// <summary>
    ///   Gets the binding list representing the list of entities and bound to the grid.
    /// </summary>
    public IBindingList? BindingList { get; private set; }

    /// <summary>
    ///   Gets metadata for the columns of the editor grid that represents the list of
    ///   entities.
    /// </summary>
    public BindingColumnList Columns =>
      _columns ??= CreateColumnsWithSession();

    public string EntityTypeName => typeof(TEntity).Name;

    /// <summary>
    ///   Gets whether the current grid row is the insertion row, which is for adding new
    ///   entities and is located at the bottom of the grid.
    /// </summary>
    /// <remarks>
    ///   TODO: Check reliability of IsInsertionRowCurrent throughout when the main grid has been entered
    ///   from the parent grid.
    ///   What we here call the insertion row is not necessarily the grid's new (i.e.
    ///   empty) row. Rather it is the row that, if committed, will provide the data for
    ///   an entity to be inserted into the database. The new row becomes the insertion
    ///   row when entered (see <see cref="BindingList_ListChanged" />) and ceases to be
    ///   the insertion row when and if it is left without having been edited. The
    ///   insertion row ceases to be the new row once any of its cells has been edited:
    ///   at that point, an empty new row is automatically added below the insertion row.
    ///   Therefore although the insertion row, if it exists, is not necessarily the new
    ///   row, it is always the current row.
    /// </remarks>
    public bool IsInsertionRowCurrent { get; private set; }

    public bool IsRemovingInvalidInsertionRow { get; set; }
    public DatabaseUpdateErrorException? LastDatabaseUpdateErrorException { get; set; }

    /// <summary>
    ///   Gets the type of parent list (IEntityList) required when this is the main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </summary>
    public Type? ParentListType { get; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database. The setter
    ///   should only be needed for testing.
    /// </summary>
    public SessionBase Session {
      get => _session ??= Global.Session;
      set => _session = value;
    }

    /// <summary>
    ///   Deletes the entity at the specified row index from the database and removes it
    ///   from the list.
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
      LastDatabaseChangeAction = StatementType.Delete;
      Session.BeginUpdate();
      try {
        //throw new ConstraintException("Test error message");
        Session.Unpersist(this[rowIndex]);
        RemoveAt(rowIndex);
      } catch (Exception exception) {
        BindingList!.Insert(rowIndex, BackupBindingItem);
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        Session.Commit();
      }
    }

    /// <summary>
    ///   Derived classes that can be parent lists must return a list of the child
    ///   entities of the entity at the specified row index that are to populate the main
    ///   list when this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    public virtual IdentifyingParentChildren GetIdentifyingParentChildrenForMainList(
      int rowIndex) {
      throw new NotSupportedException();
    }

    public IList<object> GetErrorValues() {
      return ErrorBindingItem!.GetPropertyValues();
    }

    /// <summary>
    ///   This is called when any row has been entered.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <remarks>
    ///   This is called when the grid's RowEnter event is raised. If the row is the
    ///   insertion row, this will be called after the ItemAdded ListChangedType of the
    ///   BindingList's ListChanged event.
    /// </remarks>
    public void OnRowEnter(int rowIndex) {
      Debug.WriteLine($"EntityListBase.OnRowEnter: row {rowIndex}");
      // Debug.WriteLine(
      //   $"{nameof(OnRowEnter)}:  Any row entered (after ItemAdded if insertion row)");
      HasRowBeenEdited = false;
      // Debug.WriteLine(
      //   $"EntityListBase.OnRowEnter: HasRowBeenEdited = {HasRowBeenEdited}");
      if (BackupBindingItemToRestoreFrom == null) {
        // Not forced to reenter row to fix an update error
        //Debug.WriteLine("    Creating BackupBindingItem");
        //
        // IsInsertionRowCurrent is not reliable here: it does not work if we are
        // entering the main grid from the parent grid.
        // BackupBindingItem = !IsInsertionRowCurrent
        BackupBindingItem = rowIndex < Count
          ? GetBindingItem(rowIndex).CreateBackup()
          : new TBindingItem {EntityList = this};
      }
    }

    /// <summary>
    ///   If the specified grid row is new or its data has changed, adds (if new) or
    ///   updates the corresponding the entity on the database with the row data and
    ///   saves the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <remarks>
    ///   Though this is called when the grid's RowValidated event is raised, that
    ///   actually happens even if the user did no edits, when any row left but after
    ///   the final ItemChanged ListChangedType, if any, of the BindingList's ListChanged
    ///   event.
    /// </remarks>
    /// <exception cref="DatabaseUpdateErrorException">
    ///   A database update error occured.
    /// </exception>
    public void OnRowValidated(int rowIndex) {
      Debug.WriteLine(
        $"EntityListBase.OnRowValidated: row {rowIndex}; HasRowBeenEdited == {HasRowBeenEdited}");
      if (!HasRowBeenEdited) {
        IsInsertionRowCurrent = false;
        return;
      }
      if (IsInsertionRowCurrent) {
        AddNewEntity(rowIndex);
      }
    }

    public void OnValidationError(int rowIndex, string? propertyName,
      Exception exception) {
      ErrorBindingItem = GetBindingItem(rowIndex);
      LastDatabaseChangeAction =
        IsInsertionRowCurrent ? StatementType.Insert : StatementType.Update;
      int columnIndex;
      string message;
      if (propertyName != null) {
        columnIndex = Columns.GetIndex(propertyName);
        message = IsIntegerSimpleKeyFormatError(exception, Columns[propertyName])
          ? EntityBase.GetIntegerSimpleKeyErrorMessage(propertyName)
          : $"Invalid {propertyName}:\r\n{exception.Message}";
      } else {
        columnIndex = 0;
        message = exception.Message;
      }
      LastDatabaseUpdateErrorException = new DatabaseUpdateErrorException(
        LastDatabaseChangeAction, message, rowIndex, columnIndex,
        exception);
    }

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
    public virtual void Populate(
      IdentifyingParentChildren? identifyingParentChildren = null,
      bool createBindingList = true) {
      Clear();
      bool isTransactionRequired = !Session.InTransaction;
      if (isTransactionRequired) {
        Session.BeginRead();
      }
      if (identifyingParentChildren != null) {
        IdentifyingParent = identifyingParentChildren.IdentifyingParent;
        AddRange((IList<TEntity>)identifyingParentChildren.Children);
      } else {
        AddRange(Session.AllObjects<TEntity>());
      }
      if (isTransactionRequired) {
        Session.Commit();
      }
      Sort(EntityComparer);
      if (!createBindingList) {
        return;
      }
      if (BindingList != null) {
        BindingList.ListChanged -= BindingList_ListChanged;
      }
      BindingList = CreateBindingList();
      BindingList.ListChanged += BindingList_ListChanged;
    }

    public void RemoveInsertionBindingItem() {
      // Debug.WriteLine("EntityListBase.RemoveInsertionBindingItem");
      IsInsertionRowCurrent = false;
      IsRemovingInvalidInsertionRow = false;
      BindingList?.RemoveAt(BindingList.Count - 1);
    }

    public void RestoreCurrentBindingItemOriginalValues() {
      //Debug.WriteLine("EntityListBase.RestoreCurrentBindingItemOriginalValues");
      ErrorBindingItem = BindingItemToFix!.CreateBackup();
      BindingItemToFix.RestorePropertyValuesFrom(BackupBindingItemToRestoreFrom!);
      BackupBindingItemToRestoreFrom = null;
      BindingItemToFix = null;
      HasRowBeenEdited = false;
      // Debug.WriteLine(
      //   $"EntityListBase.RestoreCurrentBindingItemOriginalValues: HasRowBeenEdited = {HasRowBeenEdited}");
    }

    public void RestoreReferencingPropertyOriginalValue(int rowIndex, int columnIndex) {
      //Debug.WriteLine($"EntityListBase.RestoreReferencingPropertyOriginalValue: row {rowIndex}");
      BackupBindingItemToRestoreFrom = null;
      BindingItemToFix = null;
      var bindingItem = GetBindingItem(rowIndex);
      string propertyName = Columns[columnIndex].PropertyName;
      var originalValue = BackupBindingItem!.GetPropertyValue(propertyName);
      //Debug.WriteLine($"    BackupBindingItem.{propertyName} = {originalValue}");
      bindingItem.SetPropertyValue(propertyName, originalValue);
    }

    protected abstract TBindingItem CreateBindingItem(TEntity entity);

    private TBindingItem CreateBindingItemWithEntityList(TEntity entity) {
      var result = CreateBindingItem(entity);
      result.EntityList = this;
      return result;
    }

    protected abstract BindingColumnList CreateColumns();

    private BindingColumnList CreateColumnsWithSession() {
      var result = CreateColumns();
      foreach (var column in result) {
        column.Session = Session;
      }
      return result;
    }

    /// <summary>
    ///   Adds a new entity to the list with the data in the specified grid row, which
    ///   should be the grid's insertion row, and saves the entity to the database.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="DatabaseUpdateErrorException">
    ///   A database update error occured.
    /// </exception>
    private void AddNewEntity(int rowIndex) {
      //Debug.WriteLine("EntityListBase.AddNewEntity");
      LastDatabaseChangeAction = StatementType.Insert;
      var bindingItem = GetBindingItem(rowIndex);
      bindingItem.EntityList = this;
      try {
        CheckForDuplicateKey(bindingItem);
      } catch (DuplicateNameException duplicateKeyException) {
        OnValidationError(rowIndex, null, duplicateKeyException);
        throw LastDatabaseUpdateErrorException!;
      }
      Session.BeginUpdate();
      try {
        var entity = bindingItem.CreateEntity();
        Session.Persist(entity);
        Add(entity);
        Session.Commit();
        IsRemovingInvalidInsertionRow = false;
      } catch (Exception exception) {
        //Debug.WriteLine(exception);
        Session.Abort();
        ErrorBindingItem = bindingItem;
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        IsInsertionRowCurrent = false;
      }
    }

    private void BindingList_ListChanged(object sender, ListChangedEventArgs e) {
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded: // Insertion row entered
          Debug.WriteLine("ListChangedType.ItemAdded: Insertion row entered");
          IsInsertionRowCurrent = true;
          break;
        case ListChangedType.ItemChanged: // Cell edit completed
          // Debug.WriteLine(
          //   $"EntityListBase.BindingList_ListChanged: ItemChanged, row {e.NewIndex}");
          // Debug.WriteLine(
          //   $"ListChangedType.ItemChanged:  {e.PropertyDescriptor.Name} = '{e.PropertyDescriptor.GetValue(BindingList[e.NewIndex])}', cell edit completed or cancelled");
          HasRowBeenEdited = true;
          // Debug.WriteLine(
          //   $"EntityListBase.BindingList_ListChanged: ItemChanged, HasRowBeenEdited = {HasRowBeenEdited}");
          if (!IsInsertionRowCurrent) {
            UpdateExistingEntityProperty(e.NewIndex, e.PropertyDescriptor.Name);
          }
          break;
      }
    }

    /// <summary>
    ///   There are also checks for duplicate keys in <see cref="EntityBase" /> in the
    ///   Data layer. This method preempts those by searching application data, on hand
    ///   in memory, rather than the database. So it is hoped that it will turn out to be
    ///   quicker with large volumes of data.
    /// </summary>
    /// <remarks>
    ///   Currently this duplicate key check does not give such nice error messages as
    ///   the ones in the Data layer.
    /// </remarks>
    private void CheckForDuplicateKey(TBindingItem bindingItem) {
      var newKey = bindingItem.CreateKey();
      if (!IsInsertionRowCurrent) {
        var originalKey = BackupBindingItem!.CreateKey();
        if (newKey == originalKey) {
          return;
        }
      }
      // Entity list could be a sorted list. Duplicate check might be faster. But it
      // would be a big job to do and I don't think there will be a performance problem.
      if ((from entity in this where entity.Key == newKey select entity).Any()) {
        string message =
          $"Another {EntityTypeName} with key '{newKey}' already exists.";
        throw new DuplicateNameException(message);
      }
    }

    private DatabaseUpdateErrorException CreateDatabaseUpdateErrorException(
      Exception exception, int rowIndex) {
      if (!IsDatabaseUpdateError(exception)) {
        // Terminal error.  In the Release compilation, the stack trace will be shown by
        // the terminal error handler in Program.cs.
        //Debug.WriteLine(exception);
        throw exception;
      }
      string? propertyName =
        exception is PropertyConstraintException propertyConstraintException
          ? propertyConstraintException.PropertyName
          : null;
      int columnIndex = propertyName != null ? Columns.GetIndex(propertyName) : 0;
      LastDatabaseUpdateErrorException = new DatabaseUpdateErrorException(
        LastDatabaseChangeAction, exception.Message, rowIndex, columnIndex, exception);
      return LastDatabaseUpdateErrorException;
    }

    private TypedBindingList<TEntity, TBindingItem> CreateBindingList() {
      return CreateBindingList((
        from entity in this
        select CreateBindingItemWithEntityList(entity)
      ).ToList());
    }

    protected virtual TypedBindingList<TEntity, TBindingItem> CreateBindingList(
      IList<TBindingItem> list) {
      return new TypedBindingList<TEntity, TBindingItem>(list);
    }

    private TBindingItem GetBindingItem(int rowIndex) {
      return BindingList![rowIndex] as TBindingItem
             ?? throw new InvalidOperationException(
               "In EntityListBase.GetBindingItem, " +
               "cannot cast BindingList[rowIndex] as TBindingItem");
    }

    /// <summary>
    ///   Returns whether the specified exception indicates that, for an anticipated
    ///   reason, a requested database update could not be done, in which case the
    ///   exception's message will need to be shown to the user to explain the error that
    ///   cause the update to be disallowed. If false, the exception should be treated as
    ///   a terminal error.
    /// </summary>
    private static bool IsDatabaseUpdateError(Exception exception) {
      return exception is ConstraintException; // Includes PropertyConstraintException
    }

    private static bool IsIntegerSimpleKeyFormatError(Exception exception,
      BindingColumn column) {
      if (!(exception is FormatException)) {
        return false;
      }
      return column.ValueType == typeof(int) && column.IsInKey;
    }

    private void UpdateExistingEntityProperty(int rowIndex, string propertyName) {
      //Debug.WriteLine($"EntityListBase.UpdateExistingEntityProperty: row {rowIndex}");
      LastDatabaseChangeAction = StatementType.Update;
      var bindingItem = GetBindingItem(rowIndex);
      if (Columns[propertyName].IsInKey) {
        CheckForDuplicateKey(bindingItem);
      }
      var entity = this[rowIndex];
      var backupBindingItem = CreateBindingItemWithEntityList(entity);
      Session.BeginUpdate();
      try {
        bindingItem.UpdateEntityProperty(propertyName, entity);
      } catch (Exception exception) {
        BindingItemToFix = bindingItem;
        backupBindingItem.RestoreToEntity(entity);
        BackupBindingItemToRestoreFrom = BackupBindingItem;
        // This exception will be passed to the grid's DataError event handler.
        throw CreateDatabaseUpdateErrorException(exception, rowIndex);
      } finally {
        Session.Commit();
      }
    }
  }
}