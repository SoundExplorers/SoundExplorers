using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity list base class.
  /// </summary>
  public abstract class EntityList<TEntity> : List<IEntity>, IEntityListOld
    where TEntity : EntityBase, new() {
    /// <summary>
    ///   Initialises a new instance of the EntityList class,
    ///   optionally specifying SQL commands.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent entity list is not required.
    /// </param>
    /// <param name="empty">
    ///   Whether an empty list is to be created.
    ///   Default False.
    /// </param>
    /// <exception cref="DataException">
    ///   Error on preparing one of the SQL commands.
    /// </exception>
    protected EntityList(
      Type parentListType = null,
      bool empty = false) {
      Entity = new TEntity();
      if (empty) {
        return;
      }
      if (parentListType != null) {
        ParentList = CreateParentList(parentListType);
      }
    }

    private IEntity Entity { get; }
    private IEntity UnchangedEntity { get; set; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the listed Entity's
    ///   field properties.
    /// </summary>
    public IEntityColumnList Columns => Entity.Columns;

    /// <summary>
    ///   Gets the name of the database table represented by the entity list.
    /// </summary>
    /// <remarks>
    ///   The table name is the same as the type name of the listed Entities.
    /// </remarks>
    public string EntityTypeName => Table?.TableName ??
                                    throw new NullReferenceException(
                                      nameof(EntityTypeName));

    /// <summary>
    ///   Gets the data set containing the main <see cref="Table" />
    ///   and, if specified, the parent table.
    /// </summary>
    public DataSet DataSet { get; private set; }

    /// <summary>
    ///   Fetches the required records of the represented table.
    /// </summary>
    public void Fetch() {
      Table = CreateFilledTable();
      if (ParentList == null) {
        DataSet = new DataSet(EntityTypeName);
        DataSet.Tables.Add(Table);
      } else { // A parent entity list needs to be added.
        ParentList.Fetch();
        DataSet = ParentList.DataSet;
        DataSet.Tables.Add(Table);
        DataSet.Relations.Add(CreateRelationBetweenMainAndParentTables());
      }
      //Refresh();
    }

    /// <summary>
    ///   Gets the list of entities representing the main table's
    ///   parent table, if specified.
    /// </summary>
    /// <remarks>
    ///   If the derived class does not specify a parent table,
    ///   a null reference will be returned.
    /// </remarks>
    public IEntityListOld ParentList { get; }

    /// <summary>
    ///   An event that is raised when there is an error on
    ///   attempting to insert, update or delete a <see cref="DataRow" />.
    /// </summary>
    /// <remarks>
    ///   When the event is raised,
    ///   the database update will have been rejected
    ///   and the values in the <see cref="DataRow" />'s fields
    ///   will have reverted to what they were before the update attempt.
    ///   The rejected values are conserved in
    ///   <see cref="RowErrorEventArgs.RejectedValues" />.
    /// </remarks>
    public event EventHandler<RowErrorEventArgs> RowError;

    /// <summary>
    ///   An event that is raised when a <see cref="DataRow" />
    ///   has been successfully inserted or updated on the database.
    /// </summary>
    public event EventHandler<RowUpdatedEventArgs> RowUpdated;

    /// <summary>
    ///   Gets the data table containing the database records
    ///   represented by the list of entities.
    /// </summary>
    public DataTable Table { get; private set; }

    /// <summary>
    ///   Updates the database table with any changes that have been input
    ///   and refreshes the list of Entities.
    /// </summary>
    /// <param name="oldKeyFieldValues">
    ///   The names and original values of the primary key fields.
    /// </param>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    public void Update(IDictionary<string, object> oldKeyFieldValues = null) {
      //Adapter.Update(Table, oldKeyFieldValues);
    }

    // private void Adapter_RowUpdated(object sender, PgSqlRowUpdatedEventArgs e) {
    //   //Debug.WriteLine("Adapter_RowUpdated");
    //   //Debug.WriteLine("Table.Rows.Count = " + Table.Rows.Count);
    //   //Debug.WriteLine("this.Count = " + Count);
    //   int rowIndex = Table.Rows.IndexOf(e.Row);
    //   //Debug.WriteLine("rowIndex = " + rowIndex);
    //   if (e.Status != UpdateStatus.ErrorsOccurred) {
    //     var message = new StringWriter();
    //     message.Write(TableName + " " + e.StatementType.ToString().ToLower());
    //     // "d" e.g. "Credit updated".  "ed" e.g. "Credit inserted"
    //     message.Write(e.StatementType.ToString().EndsWith("e") ? "d" : "ed");
    //     IEntity entity;
    //     switch (e.StatementType) {
    //       case StatementType.Insert:
    //         entity = (Entity<TEntity>)Factory<IEntity>.Create(typeof(TEntity));
    //         PopulateEntityFromRow(entity, e.Row);
    //         break;
    //       case StatementType.Update:
    //         entity = (Entity<TEntity>)this[rowIndex];
    //         PopulateEntityFromRow(entity, e.Row);
    //         break;
    //       case StatementType.Delete:
    //         entity = UnchangedEntity;
    //         break;
    //       default:
    //         throw new NotSupportedException(
    //           "SQL statement type " + e.StatementType + " is not supported.");
    //     } //End of switch
    //     if (e.StatementType == StatementType.Insert) {
    //       Add(entity);
    //     } else if (e.StatementType == StatementType.Delete) {
    //       RemoveAt(rowIndex);
    //     }
    //     RowUpdated?.Invoke(
    //       this,
    //       new RowUpdatedEventArgs(
    //         rowIndex,
    //         entity,
    //         e.StatementType,
    //         message.ToString()));
    //     return;
    //   }
    //   // e.Status == UpdateStatus.ErrorsOccurred
    //   var rejectedValues = new object[Columns.Count];
    //   if (!(e.Command is DeleteCommand<TEntity>)) {
    //     for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
    //       rejectedValues[columnIndex] = e.Row[columnIndex];
    //     }
    //   } else { // row deleted
    //     for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
    //       rejectedValues[columnIndex] = DBNull.Value;
    //     }
    //   }
    //   Exception exception;
    //   var errorColumnIndex = 0;
    //   if (e.Errors is PgSqlException errors) {
    //     try {
    //       if (errors.Message.ToLower().Contains("duplicate key")) {
    //         exception = CreateDuplicateKeyException(
    //           errors,
    //           rejectedValues,
    //           ref errorColumnIndex);
    //       } else if (errors.Message.ToLower().Contains("foreign key")) {
    //         exception = CreateForeignKeyException(
    //           errors,
    //           rejectedValues,
    //           ref errorColumnIndex);
    //       } else if (errors.Message.ToLower().Contains("null value")) {
    //         exception = CreateNullValueException(
    //           errors,
    //           ref errorColumnIndex);
    //       } else { // Other PgSqlException
    //         exception = new DataException(
    //           errors.Message + Environment.NewLine + Environment.NewLine
    //           + "SQL command text:" + Environment.NewLine + Environment.NewLine
    //           + e.Command.CommandText,
    //           errors);
    //       }
    //     } catch (NotSupportedException ex) {
    //       exception = ex;
    //     }
    //   } else { // Not a PgSqlException, unlikely.
    //     exception = e.Errors;
    //   }
    //   RowError?.Invoke(
    //     this,
    //     new RowErrorEventArgs(
    //       rowIndex,
    //       errorColumnIndex,
    //       rejectedValues,
    //       exception));
    //   e.Row.RejectChanges();
    //   e.Status = UpdateStatus.SkipAllRemainingRows;
    // }

    // private void
    //   Adapter_RowUpdating(object sender, PgSqlRowUpdatingEventArgs e) {
    //   switch (e.StatementType) {
    //     case StatementType.Update:
    //     case StatementType.Delete:
    //       int rowIndex = Table.Rows.IndexOf(e.Row);
    //       UnchangedEntity = (Entity<TEntity>)this[rowIndex].Clone();
    //       break;
    //   } //End of switch
    // }

    /// <summary>
    ///   Adds an Entity to the end of the list.
    /// </summary>
    /// <param name="entity">The Entity to added</param>
    public void Add(TEntity entity) {
      base.Add(entity);
    }

    /// <summary>
    ///   Returns a <see cref="DataTable" />
    ///   filled with all the data in the represented
    ///   database table.
    /// </summary>
    /// <returns>
    ///   A <see cref="DataTable" />
    ///   filled with all the data in the represented
    ///   database table.
    /// </returns>
    /// <remarks>
    ///   The table's column names will be set to the names
    ///   of the corresponding entity fields properties.
    ///   This ensures that the column headers will be displayed
    ///   in Pascal case.
    ///   This is necessary because
    ///   the column names are lower case when
    ///   the PostgreSQL data provider fills the table,
    ///   even if the column names (or aliases)
    ///   in the SELECT SQL have been specified in Pascal case.
    /// </remarks>
    [NotNull]
    private DataTable CreateFilledTable() {
      var table = new DataTable(typeof(TEntity).Name);
      //Adapter.Fill(table);
      for (int i = table.Columns.Count - 1; i >= 0; i--) {
        var tableColumn = table.Columns[i];
        var entityColumn = Entity.Columns[tableColumn.ColumnName];
        tableColumn.ColumnName = entityColumn?.DisplayName ??
                                 (typeof(TEntity) == typeof(Credit) &&
                                  tableColumn.ColumnName == "pieceno"
                                   ? "Piece"
                                   : null);
      }
      return table;
    }

    [NotNull]
    private static IEntityListOld CreateParentList([NotNull] Type parentListType) {
      IEntityListOld result;
      try {
        // if (parentListType == typeof(PieceList)) {
        //   result = new PieceList(null);
        // } else {
        result = EntityListFactory<IEntityListOld>.Create(
          parentListType,
          // Indicate that the parent list does not itself require a parent list.
          new object[] {null});
        // }
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
      return result;
    }

    private DataRelation CreateRelationBetweenMainAndParentTables() {
      var parentColumns = GetParentKeyDataColumns();
      var childColumns = GetForeignKeyDataColumns();
      return new DataRelation(
        EntityTypeName + "_" + ParentList?.EntityTypeName,
        parentColumns,
        childColumns);
    }
    protected abstract DataColumn[] GetForeignKeyDataColumns();
    protected abstract DataColumn[] GetParentKeyDataColumns();

    private void PopulateEntityFromRow(IEntity entity, DataRow row) {
      // foreach (var column in Entity.Columns) {
      //   if (!column.IsHidden) {
      //     // Set the field property of the entity to the corresponding value
      //     // in the table row.
      //     try {
      //       column.SetValue(
      //         entity,
      //         Convert.ChangeType(
      //           row[column.ColumnName],
      //           column.DataType));
      //     } catch (FormatException ex) {
      //       throw new ApplicationException(
      //         "Entity class " + TableName
      //                         + " field property " + column.ColumnName
      //                         + " data type " + column.DataType
      //                         + " is incompatible with value \""
      //                         + row[column.ColumnName] + "\".",
      //         ex);
      //     } catch (InvalidCastException ex) {
      //       throw new ApplicationException(
      //         "Entity class " + TableName
      //                         + " field property " + column.ColumnName
      //                         + " data type " + column.DataType
      //                         + " is incompatible with value \""
      //                         + row[column.ColumnName] + "\".",
      //         ex);
      //     }
      //   }
      // } // End of foreach
    }

    /// <summary>
    ///   Refreshes the list of Entities.
    /// </summary>
    private void Refresh() {
      // Clear();
      // for (var i = 0; i < Table.Rows.Count; i++) {
      //   var row = Table.Rows[i];
      //   IEntity entity = (TEntity)Factory<IEntity>.Create(typeof(TEntity));
      //   PopulateEntityFromRow(entity, row);
      //   Add(entity);
      // } // End of for
    }
  } //End of class
} //End of namespace