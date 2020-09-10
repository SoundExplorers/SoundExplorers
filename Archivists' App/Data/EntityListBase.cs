using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public abstract class EntityListBase<TEntity> : List<TEntity>, IEntityList
    where TEntity : EntityBase {
    private IEntityColumnList _columns;
    private IEntityList _parentList;
    private DataColumn[] _primaryKeyDataColumns;
    private SessionBase _session;
    private DataTable _table;

    /// <summary>
    ///   Base constructor for derived entity lists.
    /// </summary>
    /// <param name="isParentListRequired">
    ///   Specifies whether an identifying parent entity list
    ///   is to be included.
    /// </param>
    protected EntityListBase(bool isParentListRequired) {
      IsParentListRequired = isParentListRequired;
    }

    private bool IsParentListRequired { get; }

    [NotNull]
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Gets metadata for the Table's columns.
    /// </summary>
    public IEntityColumnList Columns => _columns ?? (_columns = CreateColumns());

    public DataSet DataSet { get; private set; }

    /// <summary>
    ///   Gets the list of entities represented in the main table's
    ///   parent table, or null if a parent list is not required.
    /// </summary>
    public IEntityList ParentList => IsParentListRequired
      ? _parentList ?? (_parentList = CreateParentList())
      : null;

    /// <summary>
    ///   Gets the name of the identifying parent table.
    ///   Null if this list does not have a parent list.
    /// </summary>
    public string ParentTableName => ParentList?.Table.TableName;

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

    protected abstract void AddRowsToTable();

    [NotNull]
    protected abstract IEntityColumnList CreateColumns();

    [NotNull]
    private DataTable CreateEmptyTableWithColumns() {
      var result = new DataTable(nameof(TEntity));
      foreach (var column in Columns) {
        result.Columns.Add(column.DisplayName, column.DataType);
      }
      return result;
    }

    /// <summary>
    ///   Creates the parent list, indicating that the parent list
    ///   does not itself require a parent list.
    /// </summary>
    [NotNull]
    protected abstract IEntityList CreateParentList();

    [NotNull]
    private DataRelation CreateRelationBetweenMainAndParentTables() {
      return new DataRelation(
        Table.TableName + "_" + ParentList?.Table.TableName,
        ParentList?.PrimaryKeyDataColumns ??
        throw new NullReferenceException("ParentList.PrimaryKeyDataColumns"),
        GetAndHideForeignKeyDataColumnsReferencingIdentifyingParent());
    }

    [NotNull]
    private DataColumn[] GetAndHideForeignKeyDataColumnsReferencingIdentifyingParent() {
      var list = new List<DataColumn>();
      foreach (var column in Columns) {
        if (column.ReferencedTableName == ParentTableName) {
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