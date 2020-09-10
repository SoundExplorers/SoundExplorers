using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Reflection;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public abstract class EntityListBase<TEntity> : List<TEntity>, IEntityList
    where TEntity : EntityBase {
    private EntityColumnList _columns;
    private IEntityList _parentList;
    private DataColumn[] _primaryKeyDataColumns;
    private QueryHelper _queryHelper;
    private SessionBase _session;
    private DataTable _table;

    /// <summary>
    ///   Base constructor for derived entity lists.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent entity list is not required.
    /// </param>
    protected EntityListBase(Type parentListType = null) {
      ParentListType = parentListType;
    }

    private Type ParentListType { get; }

    [NotNull]
    internal QueryHelper QueryHelper {
      get => _queryHelper ?? (_queryHelper = QueryHelper.Instance);
      set => _queryHelper = value;
    }

    [NotNull]
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Gets metadata for the Table's columns.
    /// </summary>
    public EntityColumnList Columns => _columns ?? (_columns = CreateColumns());

    public DataSet DataSet { get; private set; }

    /// <summary>
    ///   Gets the list of entities represented in the main table's
    ///   parent table, or null if a parent list is not required.
    /// </summary>
    public IEntityList ParentList => ParentListType == null
      ? _parentList ?? (_parentList = CreateParentList())
      : null;

    /// <summary>
    ///   Gets the data columns that uniquely identify the a row in the table.
    /// </summary>
    public DataColumn[] PrimaryKeyDataColumns => _primaryKeyDataColumns ??
                                                 (_primaryKeyDataColumns =
                                                   GetPrimaryKeyDataColumns());

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    public DataTable Table => _table ?? (_table = CreateEmptyTableWithColumns());

    /// <summary>
    ///   Fetches the required entities from the database
    ///   and populates the list and table with them.
    /// </summary>
    public void Fetch() {
      Clear();
      AddRange(Session.AllObjects<TEntity>());
      Table.Clear();
      AddRowsToTable();
      if (ParentList == null) {
        DataSet = new DataSet(Table.TableName);
        DataSet.Tables.Add(Table);
      } else { // A parent entity list needs to be added.
        ParentList.Fetch();
        DataSet = ParentList.DataSet;
        DataSet?.Tables.Add(Table);
        DataSet?.Relations.Add(CreateRelationBetweenMainAndParentTables());
      }
    }

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
    ///   Updates the entity at the specified row index.
    /// </summary>
    /// <param name="rowIndex">
    ///   Zero-based row index.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   A database update error occured.
    /// </exception>
    public void UpdateEntity(int rowIndex) {
      try { } catch (Exception exception) {
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

    [NotNull]
    private DataRelation CreateRelationBetweenMainAndParentTables() {
      return new DataRelation(
        Table.TableName + "_" + ParentList?.Table.TableName,
        ParentList?.PrimaryKeyDataColumns ??
        throw new NullReferenceException("ParentList.PrimaryKeyDataColumns"),
        GetAndHideForeignKeyDataColumnsReferencingIdentifyingParent());
    }

    [NotNull]
    private IEntityList CreateParentList() {
      IEntityList result;
      try {
        // if (parentListType == typeof(PieceList)) {
        //   result = new PieceList(null);
        // } else {
        result = EntityListFactory<IEntityList>.Create(
          ParentListType,
          // Indicate that the parent list does not itself require a parent list.
          new object[] {null});
        // }
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
      return result;
    }

    /// <summary>
    ///   The foreign key columns that reference the identifying parent entity
    ///   need to be hidden when a parent table is shown, otherwise they would
    ///   duplicate the same columns shown in the parent table.
    /// </summary>
    [NotNull]
    private DataColumn[] GetAndHideForeignKeyDataColumnsReferencingIdentifyingParent() {
      var list = new List<DataColumn>();
      foreach (var column in Columns) {
        if (column.ReferencedTableName == ParentList?.Table.TableName) {
          column.IsVisible = false;
          list.Add(Table.Columns[column.DisplayName]);
        }
      }
      return list.ToArray();
    }

    [NotNull]
    protected abstract DataColumn[] GetPrimaryKeyDataColumns();
  }
}