using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using JetBrains.Annotations;
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
    /// <param name="isParentList">
    ///   True if this is a (read-only) parent list.
    ///   False if this is the (updatable) main (and maybe only) list.
    /// </param>
    /// <param name="parentListType">
    ///   The type of (read-only) parent list (IEntityList)
    ///   required when this is the (updatable) main list.
    ///   Null if a parent list is not required when this is the main list.
    /// </param>
    protected EntityListBase(bool isParentList = false, Type parentListType = null) {
      IsParentList = isParentList;
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
    ///   True if this is a (read-only) parent list.
    ///   False if this is the (updatable) main (and maybe only) list.
    /// </summary>
    protected bool IsParentList { get; }

    [NotNull]
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Gets metadata for the columns of the Table that represents
    ///   the list of entities.
    /// </summary>
    public EntityColumnList Columns => _columns ?? (_columns = CreateColumns());

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
      try {
        Session.Unpersist(this[rowIndex]);
      } catch (Exception exception) {
        throw ConvertException(exception);
      }
      RemoveAt(rowIndex);
    }

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
        AddRange(Session.AllObjects<TEntity>());
      }
      Table.Clear();
      AddRowsToTable();
    }

    /// <summary>
    ///   Updates the entity at the specified row index
    ///   with the data in the corresponding table row.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   A database update error occured.
    /// </exception>
    public void UpdateEntity(int rowIndex) {
      var backupEntity = CreateBackupEntity(this[rowIndex]);
      try {
        UpdateEntityAtRow(rowIndex);
      } catch (Exception exception) {
        RestoreEntityAndRow(backupEntity, rowIndex);
        throw ConvertException(exception);
      }
    }

    private void AddRowsToTable() {
      foreach (var entity in this) {
        Table.Rows.Add(CreateRowFromEntity(entity));
      }
    }

    [NotNull]
    protected abstract TEntity CreateBackupEntity([NotNull] TEntity entity);

    [NotNull]
    protected abstract EntityColumnList CreateColumns();

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
    private static Exception ConvertException([NotNull] Exception exception) {
      if (exception is DataException || exception is DuplicateKeyException) {
        return new ApplicationException(exception.Message, exception);
      }
      return exception;
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