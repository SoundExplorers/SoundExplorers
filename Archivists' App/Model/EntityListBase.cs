using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
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

    [NotNull]
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Gets metadata for the Table's columns.
    /// </summary>
    public EntityColumnList Columns => _columns ?? (_columns = CreateColumns());

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
      try {
        UpdateEntityAtRow(rowIndex);
      } catch (Exception exception) {
        throw ConvertException(exception);
      }
    }

    protected abstract void UpdateEntityAtRow(int rowIndex);
    protected abstract void AddRowsToTable();

    [NotNull]
    protected abstract EntityColumnList CreateColumns();

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
  }
}