using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model; 

/// <summary>
///   Base class for a list of entities that populates a binding list.
/// </summary>
/// <typeparam name="TEntity">
///   Entity type parameter
/// </typeparam>
/// <typeparam name="TBindingItem">
///   Binding list item type parameter
/// </typeparam>
public abstract class EntityListBase<TEntity, TBindingItem> : List<TEntity>,
  IEntityList
  where TEntity : EntityBase, new()
  where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
  private TypedBindingList<TEntity, TBindingItem>? _bindingList;
  private BindingColumnList? _columns;
  private IComparer<TEntity>? _entityComparer;
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
  }

  /// <summary>
  ///   Gets a strongly typed view of the binding list representing the list of
  ///   entities and bound to the grid.
  /// </summary>
  internal TypedBindingList<TEntity, TBindingItem> BindingList {
    get => _bindingList!;
    private set {
      if (_bindingList != null) {
        _bindingList.ListChanged -= BindingList_ListChanged!;
      }
      _bindingList = value;
      _bindingList.ListChanged += BindingList_ListChanged!;
    }
  }

  /// <summary>
  ///   Only applicable to a main list that is to be populated with children of an
  ///   identifying parent entity, this specifies the identifying parent entity.
  /// </summary>
  internal IEntity? IdentifyingParent { get; private set; }

  /// <summary>
  ///   The setter should only be needed for testing.
  /// </summary>
  internal QueryHelper QueryHelper {
    get => _queryHelper ??= QueryHelper.Instance;
    set => _queryHelper = value;
  }

  private BackupItem<TBindingItem>? BackupItem { get; set; }
  private BackupItem<TBindingItem>? BackupItemToRestoreFrom { get; set; }

  private IComparer<TEntity> EntityComparer =>
    _entityComparer ??= CreateEntityComparer();

  /// <summary>
  ///   Used for restoring the error value for correction or edit cancellation after an
  ///   error message hase been shown for a cell edit error on an existing row.
  /// </summary>
  private BackupItem<TBindingItem>? ExistingEntityPropertyErrorBackupItem { get; set; }

  private StatementType LastDatabaseChangeAction { get; set; }
  private bool IsReplacingErrorBindingValueWithOriginal { get; set; }
  private bool HasRowBeenEdited { get; set; }

  /// <summary>
  ///   Gets the binding list representing the list of entities and bound to the grid.
  /// </summary>
  IBindingList IEntityList.BindingList => BindingList;

  public IDictionary<string, ReferenceableItemList>?
    ChildColumnReferenceableItemLists { get; private set; }

  public Type? ChildListType { get; set; }

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

  public DatabaseUpdateErrorException? LastDatabaseUpdateErrorException {
    get;
    private set;
  }

  public ListRole ListRole => ParentListType != null ? ListRole.Child :
    ChildListType != null ? ListRole.Parent : ListRole.StandAlone;

  public IEntityList? ParentList { get; set; }

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
  ///   Removes an erroneous insertion binding item after first backing it up to be
  ///   restored when a new row is subsequently added.
  /// </summary>
  public void BackupAndRemoveInsertionErrorBindingItem() {
    // Debug.WriteLine("EntityListBase.BackupAndRemoveInsertionErrorBindingItem");
    int insertionRowIndex = BindingList.Count - 1;
    BindingList.InsertionErrorItem = BindingList[insertionRowIndex];
    BindingList.RemoveAt(insertionRowIndex);
  }

  /// <summary>
  ///   A derived class that can be a child list must override this method to
  ///   instantiate its parent list.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public virtual IEntityList CreateParentList() {
    throw new NotSupportedException();
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
    Debug.WriteLine($"EntityListBase.DeleteEntity: row {rowIndex}");
    // Debug.WriteLine(
    //   $"{nameof(DeleteEntity)}: IsInsertionRowCurrent = {IsInsertionRowCurrent}; BindingList.Count = {BindingList.Count}");
    LastDatabaseChangeAction = StatementType.Delete;
    Session.BeginUpdate();
    try {
      //throw new ConstraintException("Test error message");
      Session.Unpersist(this[rowIndex]);
      Session.Commit();
      RemoveAt(rowIndex);
    } catch (Exception exception) {
      Session.Abort();
      BindingList.Insert(rowIndex, BackupItem!.CreateBindingItem());
      throw CreateDatabaseUpdateErrorException(exception, rowIndex);
    }
  }

  /// <summary>
  ///   Derived classes that can be parent lists must return a list of the child
  ///   entities of the entity at the specified row index that are to populate the
  ///   child (main) list when this is the parent list.
  /// </summary>
  /// <param name="rowIndex">
  ///   Zero-based row index.
  /// </param>
  public IdentifyingParentAndChildren GetIdentifyingParentAndChildrenForChildList(
    int rowIndex) {
    Session.BeginRead();
    var children = GetChildList(rowIndex);
    Session.Commit();
    return new IdentifyingParentAndChildren(this[rowIndex], children);
  }

  public IList<object> GetErrorValues() {
    return ExistingEntityPropertyErrorBackupItem!.GetValues();
  }

  /// <summary>
  ///   Occurs when an exception is thrown on ending a cell edit.
  /// </summary>
  /// <remarks>
  ///   A <see cref="DatabaseUpdateErrorException" />, which is explicitly thrown by
  ///   the application's code, is thrown at end of cell edit on existing rows but on
  ///   row validation for the insertion row, when this event is not raised.
  /// </remarks>
  public void OnCellEditException(int rowIndex, string columnName,
    Exception exception) {
    Debug.WriteLine(
      $"EntityListBase.OnCellEditException: rowIndex = {rowIndex}; columnName = {columnName}, {exception.GetType().Name}");
    switch (exception) {
      case DatabaseUpdateErrorException databaseUpdateErrorException:
        LastDatabaseUpdateErrorException = databaseUpdateErrorException;
        break;
      case DuplicateNameException duplicateKeyException:
        OnValidationError(rowIndex, columnName, duplicateKeyException);
        break;
      case FormatException formatException:
        // An invalidly formatted value was pasted into a cell. Currently that can only
        // happen when text is pasted into a Date or boolean cell, i.e. an Event's Date
        // or Newsletter cell or a Newsletter's Date cell or a Set's IsPublic cell. As
        // those cell types have non-text embedded editors, the only way to get a
        // format error is by pasting. Integer and TimeSpan cells are bound to string
        // properties of binding items, in order to ensure that the application has
        // total control over validation. So there should be no possibility of a
        // FormatException or other system exception when an invalid value is typed or
        // pasted into a cell of one of those types.
        OnValidationError(rowIndex, columnName, formatException);
        break;
      case RowNotInTableException referencedEntityNotFoundException:
        // A combo box cell value does not match any of it's embedded combo box's
        // items. So the combo box's selected index and text could not be updated. As
        // the combo boxes are all dropdown lists, the only way this can have happened
        // is that the unmatched value was pasted into the cell. If the cell value had
        // been changed by selecting an item on the embedded combo box, it could only
        // be a matching value.
        OnValidationError(rowIndex, columnName, referencedEntityNotFoundException);
        break;
      default:
        // Terminal error. In the Release build, the stack trace will be shown by
        // the terminal error handler in Program.cs.
        throw exception;
    }
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
    HasRowBeenEdited = false;
    if (BackupItemToRestoreFrom == null) {
      // Not forced to reenter row to fix an update error
      var currentBindingItem = BindingList[rowIndex];
      // Actually, EntityList should already be set unless this is the new row.
      currentBindingItem.EntityList = this;
      BackupItem = CreateBackupItem(currentBindingItem);
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
    Debug.WriteLine(
      $"EntityListBase.OnValidationError: row {rowIndex}; property {propertyName}; {exception.GetType().Name}");
    BackupItemToRestoreFrom = BackupItem;
    LastDatabaseChangeAction =
      IsInsertionRowCurrent ? StatementType.Insert : StatementType.Update;
    int columnIndex;
    string message;
    if (propertyName != null) {
      columnIndex = Columns.GetIndex(propertyName);
      // FormatException is currently only expected for date and boolean property paste
      // errors, i.e. for Event.Date, Event.Newsletter, Newsletter.Date and
      // Set.IsProperty. Validation for all other properties is completely done at the
      // model level, in BindingItemBase and its derived classes, and will not throw 
      // FormatException. If there is a FormatException, prepend an indication of which
      // property is invalid to its message. Otherwise, the message can be assumed to 
      // have been generated by the application and therefore already indicate which
      // property is invalid.
      message = exception is FormatException
        ? $"Invalid {propertyName}:\r\n{exception.Message}"
        : exception.Message;
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
  public virtual void Populate(
    IdentifyingParentAndChildren? identifyingParentAndChildren = null,
    bool createBindingList = true) {
    Clear();
    bool isTransactionRequired = !Session.InTransaction;
    if (isTransactionRequired) {
      Session.BeginUpdate(); // Required for Index use.
    }
    if (identifyingParentAndChildren != null) {
      IdentifyingParent = identifyingParentAndChildren.IdentifyingParent;
      AddRange((IEnumerable<TEntity>)identifyingParentAndChildren.Children);
    } else {
      var entities = Session.Index<TEntity>();
      if (entities != null) {
        AddRange(entities);
      }
    }
    if (createBindingList) {
      GetReferenceableItemLists();
    }
    // If this is a child list, it is already in the right order, as its data source
    // consists of the values of a SortedEntityCollection. (We still have to sort
    // top-level entities, as the entity class Index order is case-sensitive, which
    // would sort simple keys beginning with lower-case letters to the end,
    // like 'Zebra' before 'aardvark', which is not what we want.)
    if (Count > 0 && ListRole != ListRole.Child) {
      Sort(EntityComparer);
    }
    // We need to stay in the update transaction till after the sort. This became a
    // requirement when class Indexes were introduced. It is important for some entity
    // types, such as Credit, and not others, such as Act.
    if (isTransactionRequired) {
      Session.Commit(); 
    }
    if (createBindingList) {
      BindingList = CreateBindingList();
    }
  }

  /// <summary>
  ///   After showing an error message for an invalid cell value on editing an existing
  ///   entity's row with the invalid value still shown, we now need to revert the cell
  ///   value to its original, as on the database.
  /// </summary>
  public void ReplaceErrorBindingValueWithOriginal() {
    int errorRowIndex = LastDatabaseUpdateErrorException!.RowIndex;
    Debug.WriteLine(
      $"EntityListBase.ReplaceErrorBindingValueWithOriginal: row {errorRowIndex}");
    var bindingItem = BindingList[errorRowIndex];
    string propertyName =
      Columns[LastDatabaseUpdateErrorException.ColumnIndex].PropertyName;
    var originalValue = BackupItemToRestoreFrom!.GetPropertyValue(propertyName);
    IsReplacingErrorBindingValueWithOriginal = true;
    bindingItem.SetPropertyValue(propertyName, originalValue);
    BackupItemToRestoreFrom = null;
  }

  /// <summary>
  ///   Derived classes that are identifying parents should return a list of the child
  ///   entities of the entity at the specified row index that are to populate the main
  ///   list when this is the parent list.
  /// </summary>
  [ExcludeFromCodeCoverage]
  protected virtual IList GetChildList(int rowIndex) {
    throw new NotSupportedException();
  }

  protected abstract TBindingItem CreateBindingItem(TEntity entity);

  protected virtual TypedBindingList<TEntity, TBindingItem> CreateBindingList(
    IList<TBindingItem> list) {
    return new TypedBindingList<TEntity, TBindingItem>(list);
  }

  protected abstract BindingColumnList CreateColumns();

  [ExcludeFromCodeCoverage]
  protected virtual IComparer<TEntity> CreateEntityComparer() {
    return new TopLevelEntityComparer<TEntity>();
  }

  private static BackupItem<TBindingItem> CreateBackupItem(TBindingItem bindingItem) {
    return new BackupItem<TBindingItem>(bindingItem);
  }

  /// <summary>
  ///   Returns whether the specified exception indicates that, for an anticipated
  ///   reason, a requested database update could not be done, in which case the
  ///   exception's message will need to be shown to the user to explain the error that
  ///   cause the update to be disallowed. If false, the exception either requires
  ///   special handling or should be treated as a terminal error.
  /// </summary>
  private static bool IsDatabaseUpdateError(Exception exception) {
    return exception is ConstraintException; // Includes PropertyConstraintException
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
    var bindingItem = BindingList[rowIndex];
    bindingItem.EntityList = this;
    try {
      bindingItem.ValidateInsertion();
    } catch (Exception exception) {
      OnValidationError(rowIndex,
        (exception as PropertyConstraintException)?.PropertyName ?? null, exception);
      throw LastDatabaseUpdateErrorException!;
    }
    Session.BeginUpdate();
    try {
      var entity = bindingItem.CreateEntity();
      Session.Persist(entity);
      Add(entity);
      Session.Commit();
    } catch (Exception exception) {
      //Debug.WriteLine(exception);
      Session.Abort();
      throw CreateDatabaseUpdateErrorException(exception, rowIndex);
    } finally {
      IsInsertionRowCurrent = false;
    }
  }

  private void BindingList_ListChanged(object sender, ListChangedEventArgs e) {
    switch (e.ListChangedType) {
      case ListChangedType.ItemAdded: // Insertion row entered
        // Debug.WriteLine("ListChangedType.ItemAdded: Insertion row entered");
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
          if (!IsReplacingErrorBindingValueWithOriginal) {
            UpdateExistingEntityProperty(e.NewIndex, e.PropertyDescriptor!.Name);
          } else {
            IsReplacingErrorBindingValueWithOriginal = false;
          }
        }
        break;
    }
  }

  private TBindingItem CreateBindingItemWithEntityList(TEntity entity) {
    var result = CreateBindingItem(entity);
    result.EntityList = this;
    return result;
  }

  private TypedBindingList<TEntity, TBindingItem> CreateBindingList() {
    return CreateBindingList((
      from entity in this
      select CreateBindingItemWithEntityList(entity)
    ).ToList());
  }

  private BindingColumnList CreateColumnsWithSession() {
    var result = CreateColumns();
    foreach (var column in result) {
      column.Session = Session;
    }
    return result;
  }

  private DatabaseUpdateErrorException CreateDatabaseUpdateErrorException(
    Exception exception, int rowIndex) {
    if (!IsDatabaseUpdateError(exception)) {
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

  private IDictionary<string, ReferenceableItemList>
    FetchChildColumnReferenceableItemLists() {
    var dummyChildList = Global.CreateEntityList(ChildListType!);
    foreach (var column in dummyChildList.Columns) {
      column.Session = Session;
    }
    dummyChildList.Columns.FetchReferenceableItems();
    return dummyChildList.Columns.ReferencingColumns.ToDictionary(
      column => column.PropertyName,
      column => column.ReferenceableItems!);
  }

  private void GetReferenceableItemLists() {
    switch (ListRole) {
      case ListRole.Child:
        Columns.SetReferenceableItems(ParentList!.ChildColumnReferenceableItemLists!);
        break;
      case ListRole.Parent:
        ChildColumnReferenceableItemLists = FetchChildColumnReferenceableItemLists();
        break;
      case ListRole.StandAlone:
        Columns.FetchReferenceableItems();
        break;
    }
  }

  private void UpdateExistingEntityProperty(int rowIndex, string propertyName) {
    // Debug.WriteLine(
    //   $"EntityListBase.UpdateExistingEntityProperty: row {rowIndex}; property {propertyName}");
    LastDatabaseChangeAction = StatementType.Update;
    var bindingItem = BindingList[rowIndex];
    var newValue = bindingItem.GetPropertyValue(propertyName);
    var oldValue = BackupItem!.GetPropertyValue(propertyName);
    if (newValue == oldValue) {
      return;
    }
    // Debug.WriteLine(
    //   $"EntityListBase.UpdateExistingEntityProperty: row {rowIndex}; property {propertyName}; new value '{newValue}'");
    var entity = this[rowIndex];
    try {
      bindingItem.ValidatePropertyUpdate(propertyName, entity);
      Session.BeginUpdate();
      bindingItem.UpdateEntityProperty(propertyName, entity);
      Session.Commit();
      BackupItem = CreateBackupItem(bindingItem);
    } catch (Exception exception) {
      Session.Abort();
      ExistingEntityPropertyErrorBackupItem = CreateBackupItem(bindingItem);
      BackupItemToRestoreFrom = BackupItem;
      // This exception will be passed to the grid's DataError event handler.
      throw CreateDatabaseUpdateErrorException(exception, rowIndex);
    }
  }
}