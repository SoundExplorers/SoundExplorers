using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Base class for a list of entities that populates a DataTable.
  /// </summary>
  /// <typeparam name="TEntity">
  ///   Entity type
  /// </typeparam>
  public abstract class EntityListBase<TEntity> : List<TEntity>, IEntityList
    where TEntity : EntityBase {
    private EntityColumnList _columns;
    private SessionBase _session;
    private DataTable _table;

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
    }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    [NotNull]
    internal SessionBase Session {
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

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    public DataTable Table => _table ?? (_table = CreateEmptyTableWithColumns());

    /// <summary>
    ///   Deletes the entity at the specified row index
    ///   from the database and removes it from the list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   A database update error occured.
    /// </exception>
    public void DeleteEntity(int rowIndex) {
      var rowItemValues = BackupRowItemValues(rowIndex);
      try {
        Session.BeginUpdate();
        Session.Unpersist(this[rowIndex]);
        Session.Commit();
      } catch (Exception exception) {
        throw CreateRowErrorException(exception, rowIndex, rowItemValues);
      }
      RemoveAt(rowIndex);
    }

    /// <summary>
    ///   Returns a list of the child entities of the entity at the specified row index
    ///   that are to populate the main list if this is the parent list.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    public abstract IList GetChildren(int rowIndex);

    /// <summary>
    ///   Populates the list and table.
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
        try {
          Session.BeginRead();
          AddRange(Session.AllObjects<TEntity>());
          Session.Commit();
        } catch (Exception exception) {
          throw CreateRowErrorException(exception, rowIndex, rowItemValues);
        }
      }
      Table.Clear();
      AddRowsToTable();
    }

    /// <summary>
    ///   If the specified table row is new or its data has changed,
    ///   inserts (if new) or updates corresponding the entity
    ///   on the database with the table row data.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   A database update error occured.
    /// </exception>
    public void InsertOrUpdateEntityIfRequired(int rowIndex) {
      var rowItemValues = BackupRowItemValues(rowIndex);
      TEntity backupEntity = null;
      TEntity newEntity = null;
      bool isNewRow = rowIndex == Count;
      if (isNewRow) {
        newEntity = CreateEntity();
        Add(newEntity);
      } else {
        backupEntity = CreateBackupEntity(this[rowIndex]);
      }
      try {
        Session.BeginUpdate();
        UpdateEntityAtRow(rowIndex);
        if (isNewRow) {
          Session.Persist(newEntity);
        }
        Session.Commit();
      } catch (Exception exception) {
        if (isNewRow) {
          Remove(newEntity);
        } else {
          RestoreEntityAndRow(backupEntity, rowIndex);
        }
        throw CreateRowErrorException(exception, rowIndex, rowItemValues);
      }
    }

    private void AddRowsToTable() {
      foreach (var entity in this) {
        Table.Rows.Add(CreateRowFromEntity(entity));
      }
    }

    [NotNull]
    private IList<object> BackupRowItemValues(int rowIndex) {
      var row = Table.Rows[rowIndex];
      var result = new List<object>(Columns.Count);
      for (var i = 0; i < Columns.Count; i++) {
        result.Add(row[i]);
      }
      return result;
    }

    [NotNull]
    protected abstract TEntity CreateBackupEntity([NotNull] TEntity entity);

    [NotNull]
    protected abstract EntityColumnList CreateColumns();

    private static TEntity CreateEntity() {
      try {
        return (TEntity)Activator.CreateInstance(typeof(TEntity));
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }

    [NotNull]
    private DataRow CreateRowFromEntity([NotNull] TEntity entity) {
      var result = Table.NewRow();
      var values = GetRowItemValuesFromEntity(entity);
      for (var i = 0; i < values.Count; i++) {
        result[i] = values[i];
      }
      return result;
    }

    [NotNull]
    private static RowErrorException CreateRowErrorException(
      [NotNull] Exception exception, int rowIndex,
      [NotNull] IList<object> rowItemValues) {
      string message =
        exception is ApplicationException || exception is DataException ||
        exception is DuplicateKeyException
          ? exception.Message
          : exception.ToString();
      return new RowErrorException(message, rowIndex, 0, rowItemValues,
        exception);
    }

    [NotNull]
    private DataTable CreateEmptyTableWithColumns() {
      var result = new DataTable(nameof(TEntity));
      foreach (var column in Columns) {
        result.Columns.Add(column.DisplayName, column.DataType);
      }
      return result;
    }

    [NotNull]
    protected abstract IList<object> GetRowItemValuesFromEntity([NotNull] TEntity entity);

    private void RestoreEntityAndRow([NotNull] TEntity backupEntity, int rowIndex) {
      var entity = this[rowIndex];
      RestoreEntityPropertiesFromBackup(backupEntity, entity);
      var values = GetRowItemValuesFromEntity(entity);
      for (var i = 0; i < values.Count; i++) {
        Table.Rows[rowIndex][i] = values[i];
      }
    }

    protected abstract void RestoreEntityPropertiesFromBackup(
      [NotNull] TEntity backupEntity, [NotNull] TEntity entityToRestore);

    protected abstract void UpdateEntityAtRow(int rowIndex);
  }
}