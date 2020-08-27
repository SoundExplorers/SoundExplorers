using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity list base class.
  /// </summary>
  internal abstract class EntityList<T> : List<IEntity>, IEntityList
    where T : Entity<T> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="EntityList" /> class,
    ///   optionally specifying SQL commands,
    ///   fetching all the records of the represented table.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent entity list is not required.
    /// </param>
    /// <param name="selectCommand">
    ///   Optionally specifies the SELECT command.
    ///   Null to to get the SQL from an embedded resource file,
    ///   if found, or generate it
    /// </param>
    /// <param name="insertCommand">
    ///   Optionally specifies the INSERT command.
    ///   Null to to get the SQL from an embedded resource file,
    ///   if found, or generate it
    /// </param>
    /// <param name="updateCommand">
    ///   Optionally specifies the UPDATE command.
    ///   Null to to get the SQL from an embedded resource file,
    ///   if found, or generate it
    /// </param>
    /// <param name="deleteCommand">
    ///   Optionally specifies the DELETE command.
    ///   Null to to get the SQL from an embedded resource file,
    ///   if found, or generate it
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
      SelectCommand<T> selectCommand = null,
      InsertCommand<T> insertCommand = null,
      UpdateCommand<T> updateCommand = null,
      DeleteCommand<T> deleteCommand = null,
      bool empty = false) {
      Entity = new Entity<T>();
      if (empty) {
        return;
      }
      Adapter = new OurSqlDataAdapter<T>(
        selectCommand, insertCommand, updateCommand, deleteCommand);
      Adapter.RowUpdated += Adapter_RowUpdated;
      Adapter.RowUpdating += Adapter_RowUpdating;
      Table = CreateFilledTable();
      if (parentListType == null) {
        DataSet = new DataSet(TableName);
        DataSet.Tables.Add(Table);
      } else { // A parent entity list needs to be added.
        try {
          if (parentListType == typeof(PieceList)) {
            ParentList = new PieceList(null);
          } else {
            ParentList = Factory<IEntityList>.Create(
              parentListType,
              // Indicate that the parent list does not itself require a parent list.
              new Type[] {null});
          }
        } catch (TargetInvocationException ex) {
          throw ex.InnerException;
        }
        DataSet = ParentList.DataSet;
        DataSet.Tables.Add(Table);
        // Create a relation between the main and parent tables.
        var parentColumns = ParentKeyColumnsToDataColumns();
        var childColumns = ForeignKeyColumnsToDataColumns();
        var relation = DataSet.Relations.Add(
          TableName + "_" + ParentList.TableName,
          parentColumns,
          childColumns);
      }
      Refresh();
    }

    /// <summary>
    ///   Gets or sets the data adapter.
    /// </summary>
    protected OurSqlDataAdapter<T> Adapter { get; set; }

    protected IEntity Entity { get; set; }
    private IEntity UnchangedEntity { get; set; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the listed Entity's
    ///   field properties.
    /// </summary>
    public EntityColumnList Columns => Entity.Columns;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the derived class's
    ///   field properties that are not in the primary key.
    /// </summary>
    public EntityColumnList NonPrimaryKeyColumns => Entity.NonPrimaryKeyColumns;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the Entity's
    ///   field properties that are not in the unique key
    ///   (if there is one).
    /// </summary>
    public EntityColumnList NonUniqueKeyColumns => Entity.NonUniqueKeyColumns;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the listed Entity's
    ///   primary key field properties.
    /// </summary>
    public EntityColumnList PrimaryKeyColumns => Entity.PrimaryKeyColumns;
    //public virtual DataTable Table { get; set; }

    /// <summary>
    ///   Gets the name of the database table represented by the entity list.
    /// </summary>
    /// <remarks>
    ///   The table name is the same as the type name of the listed Entities.
    /// </remarks>
    public virtual string TableName => Table.TableName;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's
    ///   unique key field properties.
    /// </summary>
    /// <remarks>
    ///   Empty if there is no unique key.
    /// </remarks>
    public EntityColumnList UniqueKeyColumns => Entity.UniqueKeyColumns;

    /// <summary>
    ///   Gets the data set containing the main <see cref="Table" />
    ///   and, if specified, the parent table.
    /// </summary>
    public virtual DataSet DataSet { get; protected set; }

    /// <summary>
    ///   Gets the list of entities representing the main table's
    ///   parent table, if specified.
    /// </summary>
    /// <remarks>
    ///   If the derived class does not specify a parent table,
    ///   a null reference will be returned.
    /// </remarks>
    public virtual IEntityList ParentList { get; protected set; }

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
    public virtual DataTable Table { get; protected set; }

    /// <summary>
    ///   Updates the database table with any changes that have been input
    ///   and refreshes the list of <see cref="Entity" />s.
    /// </summary>
    /// <param name="oldKeyFields">
    ///   The names and original values of the primary key fields.
    /// </param>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    public virtual void Update(Dictionary<string, object> oldKeyFields = null) {
      Adapter.Update(Table, oldKeyFields);
    }

    private void Adapter_RowUpdated(object sender, PgSqlRowUpdatedEventArgs e) {
      //Debug.WriteLine("Adapter_RowUpdated");
      //Debug.WriteLine("Table.Rows.Count = " + Table.Rows.Count);
      //Debug.WriteLine("this.Count = " + Count);
      int rowIndex = Table.Rows.IndexOf(e.Row);
      //Debug.WriteLine("rowIndex = " + rowIndex);
      if (e.Status != UpdateStatus.ErrorsOccurred) {
        var message = new StringWriter();
        message.Write(TableName + " " + e.StatementType.ToString().ToLower());
        if (e.StatementType.ToString().EndsWith("e")) {
          message.Write("d"); // e.g. Credit updated
        } else {
          message.Write("ed"); // e.g. Credit inserted
        }
        IEntity entity;
        switch (e.StatementType) {
          case StatementType.Insert:
            entity = (Entity<T>)Factory<IEntity>.Create(typeof(T));
            PopulateEntityFromRow(entity, e.Row);
            break;
          case StatementType.Update:
            entity = (Entity<T>)this[rowIndex];
            PopulateEntityFromRow(entity, e.Row);
            break;
          case StatementType.Delete:
            entity = UnchangedEntity;
            break;
          default:
            throw new NotSupportedException(
              "SQL statement type " + e.StatementType + " is not supported.");
        } //End of switch
        if (e.StatementType == StatementType.Insert) {
          Add(entity);
        } else if (e.StatementType == StatementType.Delete) {
          RemoveAt(rowIndex);
        }
        if (RowUpdated != null) {
          RowUpdated(
            this,
            new RowUpdatedEventArgs(
              rowIndex,
              entity,
              e.StatementType,
              message.ToString()));
        }
        return;
      }
      // e.Status == UpdateStatus.ErrorsOccurred
      var rejectedValues = new object[Columns.Count];
      if (!(e.Command is DeleteCommand<T>)) {
        for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
          rejectedValues[columnIndex] = e.Row[columnIndex];
        }
      } else { // row deleted
        for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
          rejectedValues[columnIndex] = DBNull.Value;
        }
      }
      Exception exception;
      var errorColumnIndex = 0;
      if (e.Errors is PgSqlException) {
        try {
          if (e.Errors.Message.ToLower().Contains("duplicate key")) {
            exception = CreateDuplicateKeyException(
              e.Errors as PgSqlException,
              rejectedValues,
              ref errorColumnIndex);
          } else if (e.Errors.Message.ToLower().Contains("foreign key")) {
            exception = CreateForeignKeyException(
              e.Errors as PgSqlException,
              rejectedValues,
              ref errorColumnIndex);
          } else if (e.Errors.Message.ToLower().Contains("null value")) {
            exception = CreateNullValueException(
              e.Errors as PgSqlException,
              ref errorColumnIndex);
          } else { // Other PgSqlException
            exception = new DataException(
              e.Errors.Message + Environment.NewLine + Environment.NewLine
              + "SQL command text:" + Environment.NewLine + Environment.NewLine
              + e.Command.CommandText,
              e.Errors);
          }
        } catch (NotSupportedException ex) {
          exception = ex;
        }
      } else { // Not a PgSqlException, unlikely.
        exception = e.Errors;
      }
      if (RowError != null) {
        RowError(
          this,
          new RowErrorEventArgs(
            rowIndex,
            errorColumnIndex,
            rejectedValues,
            exception));
      }
      e.Row.RejectChanges();
      e.Status = UpdateStatus.SkipAllRemainingRows;
    }

    private void
      Adapter_RowUpdating(object sender, PgSqlRowUpdatingEventArgs e) {
      switch (e.StatementType) {
        case StatementType.Update:
        case StatementType.Delete:
          int rowIndex = Table.Rows.IndexOf(e.Row);
          UnchangedEntity = (Entity<T>)this[rowIndex].Clone();
          break;
      } //End of switch
    }

    /// <summary>
    ///   Adds an Entity to the end of the list.
    /// </summary>
    /// <param name="entity">The Entity to added</param>
    public virtual void Add(Entity<T> entity) {
      base.Add(entity);
    }

    /// <summary>
    ///   Creates an <see cref="ApplicationException" />
    ///   for a duplicate key, providing a more meaningful
    ///   message than is generated by the database engine.
    /// </summary>
    /// <param name="exception">
    ///   The <see cref="PgSqlException" /> containing
    ///   the error message generated by the database engine.
    /// </param>
    /// <param name="rejectedValues">
    ///   An array of the values of the row's fields
    ///   as at just before the change was rejected.
    /// </param>
    /// <param name="errorColumnIndex">
    ///   A reference parameter that returns
    ///   the column index within the referencing entity
    ///   of the first field of the offending duplicate key.
    /// </param>
    /// <returns>
    ///   The <see cref="ApplicationException" /> created.
    /// </returns>
    /// <remarks>
    ///   The error message generated by the database engine
    ///   is expected to be something like this:
    ///   duplicate key value violates unique constraint "KEY"
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   Thrown if a meaningful error message cannot be generated by parsing
    ///   the error message generated by the database engine.
    /// </exception>
    private ApplicationException CreateDuplicateKeyException(
      PgSqlException exception,
      object[] rejectedValues,
      ref int errorColumnIndex) {
      string keyName = exception.Message.Split('"')[1];
      EntityColumnList keyColumns;
      var message = new StringWriter();
      message.Write("A " + TableName + " with ");
      if (keyName.ToLower().StartsWith("pk_")) {
        message.Write("primary");
        keyColumns = PrimaryKeyColumns;
      } else if (keyName.ToLower().StartsWith("uk_")) {
        message.Write("unique");
        keyColumns = UniqueKeyColumns;
      } else {
        throw new NotSupportedException(
          "Key constraint name \"" + keyName
                                   + "\" is not supported. It should start with "
                                   + "\"pk_\" (for a primary key) or \"uk_\" (for a unique key).",
          exception);
      }
      message.WriteLine(" key");
      foreach (var keyColumn in keyColumns) {
        string errorValue =
          keyColumn.DataType == typeof(DateTime)
            ? ((DateTime)rejectedValues[Columns.IndexOf(keyColumn)]).ToString(
              "dd MMM yyyy")
            : rejectedValues[Columns.IndexOf(keyColumn)].ToString();
        message.WriteLine(
          "    " + keyColumn.ColumnName + " = " + errorValue);
      } //End of foreach
      message.WriteLine("already exists.");
      errorColumnIndex = Columns.IndexOf(keyColumns[0]);
      return new ApplicationException(
        message.ToString(),
        exception);
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
    private DataTable CreateFilledTable() {
      var table = new DataTable(typeof(T).Name);
      Adapter.Fill(table);
      for (var i = 0; i < table.Columns.Count; i++) {
        var tableColumn = table.Columns[i];
        var entityColumn = Entity.Columns[tableColumn.ColumnName];
        tableColumn.ColumnName = entityColumn?.ColumnName ??
                                 (typeof(T) == typeof(Credit) &&
                                  tableColumn.ColumnName == "pieceno"
                                   ? "Piece"
                                   : null);
      }
      return table;
    }

    /// <summary>
    ///   Creates an <see cref="ApplicationException" />
    ///   for a foreign key violation, providing a more meaningful
    ///   message than is generated by the database engine.
    /// </summary>
    /// <param name="exception">
    ///   The <see cref="PgSqlException" /> containing
    ///   the error message generated by the database engine.
    /// </param>
    /// <param name="rejectedValues">
    ///   An array of the values of the row's fields
    ///   as at just before the change was rejected.
    /// </param>
    /// <param name="errorColumnIndex">
    ///   A reference parameter that returns
    ///   the columns index within the referencing entity
    ///   of the first field of the offending foreign key.
    /// </param>
    /// <returns>
    ///   The <see cref="ApplicationException" /> created.
    /// </returns>
    /// <remarks>
    ///   The error message generated by the database engine
    ///   is expected to be something like this:
    ///   insert or update on table "TABLE" violates foreign key constraint "FOREIGNKEY"
    ///   or:
    ///   update or delete on table "TABLE" violates foreign key constraint "FOREIGNKEY" on table
    ///   "REFERENCINGTABLE"
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   Thrown if a meaningful error message cannot be generated by parsing
    ///   the error message generated by the database engine.
    /// </exception>
    private ApplicationException CreateForeignKeyException(
      PgSqlException exception,
      object[] rejectedValues,
      ref int errorColumnIndex) {
      var chunks = exception.Message.Split('"');
      string foreignKeyName = chunks[3];
      if (chunks.Count() >= 6) {
        // Referencing table name in lower case, 
        // in the error message that was generated by the database engine.
        string referencingTableName = chunks[5];
        // Referencing table name in mixed case, 
        // as preferred for the error message.
        string referencingEntityName = (
          from string tableName in Factory<IEntity>.Types.Keys
          where tableName.ToLower() == referencingTableName.ToLower()
          select tableName).FirstOrDefault();
        if (referencingEntityName == null) {
          referencingEntityName = referencingTableName;
        }
        return new ApplicationException(
          "The " + TableName
                 + " is referenced by one or more "
                 + referencingEntityName + "s.",
          exception);
      }
      string notSupportedMessage =
        "Foreign key constraint name \"" + foreignKeyName
                                         + "\" is not supported." +
                                         Environment.NewLine
                                         + "It should consist of either" +
                                         Environment.NewLine
                                         + "\"fk_" + TableName.ToLower()
                                         + "_\" followed by the referenced table name" +
                                         Environment.NewLine
                                         + "or" + Environment.NewLine
                                         + "\"fk\" followed by the referencing table name "
                                         + "followed by \"_\" followed by \"" +
                                         TableName.ToLower() + "\"."
                                         + Environment.NewLine +
                                         Environment.NewLine;
      // Referenced table name in lower case, 
      // as embedded in foreign key name.
      string referencedTableName;
      if (foreignKeyName.ToLower().StartsWith("fk_" + TableName.ToLower())) {
        referencedTableName =
          foreignKeyName.ToLower().Replace(
            "fk_" + TableName.ToLower() + "_", string.Empty);
      } else {
        throw new NotSupportedException(
          notSupportedMessage,
          exception);
      }
      // Referenced table name in mixed case, 
      // as required when creating Entity
      // and as preferred for the error message.
      string referencedEntityName = (
        from IEntityColumn column in Columns
        where column.ReferencedTableName != null
              && column.ReferencedTableName.ToLower() ==
              referencedTableName.ToLower()
        select column.ReferencedTableName).FirstOrDefault();
      if (referencedEntityName == null) {
        throw new NotSupportedException(
          notSupportedMessage,
          exception);
      }
      var referencedEntity = Factory<IEntity>.Create(referencedEntityName);
      var foreignKeyColumns = new EntityColumnList();
      foreach (var column in Columns) {
        if (column.ReferencedTableName != null) {
          // Where there's a multiple-column foreign key,
          // not all the columns in the foreign key are 
          // necessarily going to have the table that is referenced
          // by the foreign key specified as the referenced table
          // for the referenced field property of the referencing entity.
          // This is because we generally substitute unique key columns
          // from ancestor tables within foreign keys shown on the grid.
          // So, for example, 
          // in in the Image table's foreign key "fk_image_performance", 
          // which references the Performance table,
          // we need to include referencing entity field Location,
          // for which ReferencedTableName is Location,
          // and referencing entity field Date,
          // for which ReferencedTableName is Performance.
          if (column.ReferencedTableName == referencedEntityName
              || referencedEntity.Columns.ContainsKey(column.ColumnName)) {
            foreignKeyColumns.Add(column);
          }
        }
      } //End of foreach
      var message = new StringWriter();
      message.WriteLine("A " + referencedEntityName + " with");
      foreach (var foreignKeyColumn in foreignKeyColumns) {
        string errorValue =
          foreignKeyColumn.DataType == typeof(DateTime)
            ? ((DateTime)rejectedValues[Columns.IndexOf(foreignKeyColumn)])
            .ToString("dd MMM yyyy")
            : rejectedValues[Columns.IndexOf(foreignKeyColumn)].ToString();
        message.WriteLine(
          "    " + foreignKeyColumn.ColumnName + " = " + errorValue);
      } //End of foreach
      message.WriteLine("cannot be found.");
      errorColumnIndex = Columns.IndexOf(foreignKeyColumns[0]);
      return new ApplicationException(
        message.ToString(),
        exception);
    }

    /// <summary>
    ///   Creates an <see cref="ApplicationException" />
    ///   for a not-null constraint violation,
    ///   providing a more meaningful
    ///   message than is generated by the database engine.
    /// </summary>
    /// <param name="exception">
    ///   The <see cref="PgSqlException" /> containing
    ///   the error message generated by the database engine.
    /// </param>
    /// <param name="errorColumnIndex">
    ///   A reference parameter that returns
    ///   the column index within the referencing entity
    ///   of the field with the offending null value.
    /// </param>
    /// <returns>
    ///   The <see cref="ApplicationException" /> created.
    /// </returns>
    /// <remarks>
    ///   The error message generated by the database engine
    ///   is expected to be something like this:
    ///   null value in column "COLUMN" violates not-null constraint
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   Thrown if a meaningful error message cannot be generated by parsing
    ///   the error message generated by the database engine.
    /// </exception>
    private ApplicationException CreateNullValueException(
      PgSqlException exception,
      ref int errorColumnIndex) {
      string columnNameOnDb = exception.Message.Split('"')[1];
      var errorColumn = (
        from IEntityColumn column in Columns
        where column.NameOnDb.ToLower() == columnNameOnDb.ToLower()
        select column).FirstOrDefault();
      if (errorColumn != null) {
        errorColumnIndex = Columns.IndexOf(errorColumn);
        return new ApplicationException(
          errorColumn.ColumnName + " must be specified.",
          exception);
      }
      throw new NotSupportedException(
        "An entity field property corresponding to database column name \""
        + columnNameOnDb
        + "\" cannot be found for table \"" + TableName
        + "\".",
        exception);
    }

    private DataColumn[] ForeignKeyColumnsToDataColumns() {
      var dataColumns =
        new List<DataColumn>(ParentList.PrimaryKeyColumns.Count());
      foreach (var parentKeyColumn in ParentList.PrimaryKeyColumns) {
        foreach (var mainKeyColumn in PrimaryKeyColumns) {
          if (mainKeyColumn.ColumnName == parentKeyColumn.ColumnName
              || mainKeyColumn.ReferencedColumnName ==
              parentKeyColumn.ColumnName) {
            dataColumns.Add(Table.Columns[mainKeyColumn.ColumnName]);
            break;
          }
        } //End of foreach
      } //End of foreach
      return dataColumns.ToArray();
    }

    private DataColumn[] ParentKeyColumnsToDataColumns() {
      var dataColumns =
        new List<DataColumn>(ParentList.PrimaryKeyColumns.Count());
      foreach (var parentKeyColumn in ParentList.PrimaryKeyColumns) {
        dataColumns.Add(ParentList.Table.Columns[parentKeyColumn.ColumnName]);
      } //End of foreach
      return dataColumns.ToArray();
    }

    private void PopulateEntityFromRow(IEntity entity, DataRow row) {
      foreach (var column in Entity.Columns) {
        if (!column.IsHidden) {
          // Set the field property of the entity to the corresponding value
          // in the table row.
          try {
            column.SetValue(
              entity,
              Convert.ChangeType(
                row[column.ColumnName],
                column.DataType));
          } catch (FormatException ex) {
            throw new ApplicationException(
              "Entity class " + TableName
                              + " field property " + column.ColumnName
                              + " data type " + column.DataType
                              + " is incompatible with value \""
                              + row[column.ColumnName] + "\".",
              ex);
          } catch (InvalidCastException ex) {
            throw new ApplicationException(
              "Entity class " + TableName
                              + " field property " + column.ColumnName
                              + " data type " + column.DataType
                              + " is incompatible with value \""
                              + row[column.ColumnName] + "\".",
              ex);
          }
        }
      } // End of foreach
    }

    /// <summary>
    ///   Refreshes the list of <see cref="Entity" />s.
    /// </summary>
    protected virtual void Refresh() {
      Clear();
      for (var i = 0; i < Table.Rows.Count; i++) {
        var row = Table.Rows[i];
        IEntity entity = (Entity<T>)Factory<IEntity>.Create(typeof(T));
        PopulateEntityFromRow(entity, row);
        Add(entity);
      } // End of for
    }

    ///// <summary>
    ///// Not used at present.
    ///// </summary>
    ///// <remarks>
    ///// Using PgSqlCommandBuilder to generate the 
    ///// INSERT, UPDATE and DELETE commands did not work.
    ///// It caused PgSqlDataAdapter.Update to throw this
    ///// InvalidOperationException:
    ///// "Dynamic SQL generation is not supported against a SelectCommand that does not return any base table information."
    ///// That exception normally means that the primary key columns were not selected in the SELECT command.
    ///// The same thing worked fine in MySql.
    ///// TO DO: Replicate the bug with a PostgreSQL sample database and report if necessary.
    ///// </remarks>
    //protected PgSqlCommandBuilder CommandBuilder { get; private set; }
  } //End of class
} //End of namespace