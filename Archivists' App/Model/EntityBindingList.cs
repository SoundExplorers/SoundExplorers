using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class EntityBindingList<TEntity, TBindingItem> : BindingList<TBindingItem>
    where TEntity : IEntity
    where TBindingItem : EntityBindingItemBase<TEntity> {
    public EntityBindingList([NotNull] IList<TBindingItem> list,
      [NotNull] EntityListBase<TEntity, TBindingItem> owner) : base(list) {
      Owner = owner;
    }

    private bool IsFixing { get; set; }
    [NotNull] private EntityListBase<TEntity, TBindingItem> Owner { get; }
    [NotNull] private SessionBase Session => Owner.Session;

    protected override void OnListChanged(ListChangedEventArgs e) {
      AllowNew = true;
      base.OnListChanged(e);
      if (IsFixing) {
        IsFixing = false;
        return;
      }
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded:
          var newEntity = this[e.NewIndex].CreateEntity();
          Session.BeginUpdate();
          try {
            Session.Persist(newEntity);
            Session.Commit();
          } catch (Exception exception) {
            Session.Abort();
            IsFixing = false;
            RemoveItem(e.NewIndex);
            throw CreateDatabaseUpdateErrorException(exception, e.NewIndex);
          }
          Owner.Insert(e.NewIndex, newEntity);
          break;
        case ListChangedType.ItemChanged:
          // TODO: ListChangedType.ItemChanged
          break;
        case ListChangedType.ItemDeleted:
          var deletedEntity = Owner[e.NewIndex];
          Session.BeginUpdate();
          try {
            Session.Unpersist(deletedEntity);
            Session.Commit();
          } catch (Exception exception) {
            Session.Abort();
            IsFixing = true;
            InsertItem(e.NewIndex, Owner.CreateBindingItem(e.NewIndex));
            throw CreateDatabaseUpdateErrorException(exception, e.NewIndex);
          }
          Owner.RemoveAt(e.NewIndex);
          break;
        default:
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
  }
}