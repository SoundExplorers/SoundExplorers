using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Common;
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
    where TEntity : IEntity
    where TBindingItem : BindingItemBase {
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

    private EntityComparer<TEntity> EntityComparer { get; }

    /// <summary>
    ///   Gets the binding list representing the list of entities.
    /// </summary>
    public IBindingList BindingList { get; private set; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    public SessionBase Session {
      get => _session ?? (_session = Global.Session);
      // ReSharper disable once UnusedMember.Global
      set => _session = value;
    }

    /// <summary>
    ///   Gets metadata for the columns of the Table that represents
    ///   the list of entities.
    /// </summary>
    public EntityColumnList Columns => _columns ?? (_columns = CreateColumns());

    /// <summary>
    ///   True if this is a (read-only) parent list.
    ///   False (the default) if this is the (updatable) main (and maybe only) list.
    /// </summary>
    public bool IsParentList { get; set; }

    /// <summary>
    ///   Gets the type of parent list (IEntityList) required when this is the main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </summary>
    public Type ParentListType { get; }

    public string TableName => typeof(TEntity).Name;

    /// <summary>
    ///   Returns a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list if this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    public abstract IList GetChildren(int rowIndex);

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

    private void BindingListOnListChanged(object sender, ListChangedEventArgs e) {
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded:
          Debug.WriteLine("ListChangedType.ItemAdded");
          // var newEntity = this[e.NewIndex].CreateEntity();
          // Session.BeginUpdate();
          // try {
          //   Session.Persist(newEntity);
          //   Session.Commit();
          // } catch (Exception exception) {
          //   Session.Abort();
          //   IsFixing = false;
          //   RemoveItem(e.NewIndex);
          //   throw CreateDatabaseUpdateErrorException(exception, e.NewIndex);
          // }
          // Owner.Insert(e.NewIndex, newEntity);
          break;
        case ListChangedType.ItemChanged:
          // TODO: ListChangedType.ItemChanged
          Debug.WriteLine("ListChangedType.ItemChanged");
          break;
        case ListChangedType.ItemDeleted:
          Debug.WriteLine("ListChangedType.ItemDeleted");
          // var deletedEntity = Owner[e.NewIndex];
          // Session.BeginUpdate();
          // try {
          //   Session.Unpersist(deletedEntity);
          //   Session.Commit();
          // } catch (Exception exception) {
          //   Session.Abort();
          //   IsFixing = true;
          //   InsertItem(e.NewIndex, Owner.CreateBindingItem(e.NewIndex));
          //   throw CreateDatabaseUpdateErrorException(exception, e.NewIndex);
          // }
          // Owner.RemoveAt(e.NewIndex);
          break;
        default:
          Debug.WriteLine("ListChangedType default");
          throw new NotSupportedException(e.ListChangedType.ToString());
      }
    }

    [NotNull]
    private static DatabaseUpdateErrorException CreateDatabaseUpdateErrorException(
      [NotNull] Exception exception, int rowIndex,
      IList<object> rowItemValues = null) {
      if (!IsDatabaseUpdateError(exception)) {
        throw exception; // Terminal error
      }
      return new DatabaseUpdateErrorException(exception.Message, rowIndex, 0,
        rowItemValues,
        exception);
    }

    /// <summary>
    ///   Returns whether the specified exception indicates that,
    ///   for an anticipated reason, a requested database update could not be done,
    ///   in which case the exception's message will need to be shown to the user to
    ///   explain the error that cause the update to be disallowed.
    ///   If false, the exception should be treated as a terminal error.
    /// </summary>
    private static bool IsDatabaseUpdateError(Exception exception) {
      return exception is DataException || exception is DuplicateKeyException;
    }

    [NotNull]
    private BindingList<TBindingItem> CreateBindingList() {
      var list = (
        from entity in this
        select CreateBindingItem(entity)
      ).ToList();
      return new BindingList<TBindingItem>(list);
    }

    [NotNull]
    protected abstract TBindingItem CreateBindingItem([NotNull] TEntity entity);

    [NotNull]
    protected abstract EntityColumnList CreateColumns();
  }
}