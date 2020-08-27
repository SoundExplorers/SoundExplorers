using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity base class.
  /// </summary>
  /// <remarks>
  ///   For SQL generation to work,
  ///   the name of a derived class should be
  ///   the name of the corresponding database table.
  /// </remarks>
  internal class Entity<T> : IEntity
    // This recursion of the generic type
    // allows reflection to examine the properties 
    // of derived classes.
    where T : Entity<T> {
    private OurSqlDataAdapter<T> _adapter;
    private EntityColumnList _columns;

    /// <summary>
    ///   Gets the data adapter.
    /// </summary>
    protected virtual OurSqlDataAdapter<T> Adapter {
      get {
        if (_adapter == null) {
          _adapter = new OurSqlDataAdapter<T>(
            new SelectCommand<T>(
              false));
        }
        return _adapter;
      }
    }

    /// <summary>
    ///   Gets or sets the data table containing the database record
    ///   represented by the entity.
    /// </summary>
    protected virtual DataTable Table { get; set; }

    /// <summary>
    ///   Creates a deep copy of the entity.
    /// </summary>
    /// <returns>
    ///   The deep copy created.
    /// </returns>
    /// <remarks>
    ///   This explicit ICloneable.Clone implementation
    ///   satisfies the ICloneable interface
    ///   while allowing the Entity's type-safe Clone method to be invoked
    ///   directly to avoid having to cast the result.
    /// </remarks>
    object ICloneable.Clone() {
      return Clone();
    }

    /// <summary>
    ///   Deletes the row of the table whose primary key field values
    ///   match the values of the corresponding primary key column properties.
    /// </summary>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    public virtual void Delete() {
      foreach (EntityColumn<T> primaryKeyColumn in PrimaryKeyColumns) {
        Adapter.DeleteCommand.Parameters["@" + primaryKeyColumn.ColumnName]
            .Value =
          primaryKeyColumn.GetValue(this);
      } // End of foreach
      Adapter.DeleteCommand.Connection.Open();
      try {
        Adapter.DeleteCommand.ExecuteNonQuery();
      } catch (PgSqlException ex) {
        throw new DataException(
          "Error on executing SQL command to delete " + TableName + ":" +
          Environment.NewLine
          + ex.Message + Environment.NewLine + Environment.NewLine
          + "SQL command text:" + Environment.NewLine + Environment.NewLine
          + Adapter.DeleteCommand.CommandText
          + Environment.NewLine + Environment.NewLine,
          ex);
      } finally {
        Adapter.DeleteCommand.Connection.Close();
      }
    }

    /// <summary>
    ///   Populates those field properties that are not in the primary key
    ///   to the corresponding field values
    ///   on the row, if found, of the table whose primary key field values
    ///   match the values of the corresponding primary key column properties.
    /// </summary>
    /// <returns>
    ///   Whether a matching row on the table was found.
    /// </returns>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    public virtual bool Fetch() {
      foreach (EntityColumn<T> primaryKeyColumn in PrimaryKeyColumns) {
        Adapter.SelectCommand.Parameters["@" + primaryKeyColumn.ColumnName]
            .Value =
          primaryKeyColumn.GetValue(this);
      } // End of foreach
      Table = new DataTable(TableName);
      try {
        Adapter.Fill(Table);
      } catch (PgSqlException ex) {
        throw new DataException(
          "Error on executing SQL command to fetch " + TableName + ":" +
          Environment.NewLine
          + ex.Message + Environment.NewLine + Environment.NewLine
          + "SQL command text:" + Environment.NewLine + Environment.NewLine
          + Adapter.SelectCommand.CommandText
          + Environment.NewLine + Environment.NewLine,
          ex);
      }
      if (Table.Rows.Count == 0) {
        return false;
      }
      foreach (EntityColumn<T> nonKeycolumn in NonPrimaryKeyColumns) {
        // Set the field property of the entity to the corresponding value
        // in the table row.
        nonKeycolumn.SetValue(
          this,
          Convert.ChangeType(
            Table.Rows[0][nonKeycolumn.ColumnName],
            nonKeycolumn.DataType));
      } // End of foreach
      return true;
    }

    /// <summary>
    ///   If the record exists,
    ///   updates the field values of the
    ///   record with the corresponding
    ///   column property values.
    ///   Otherwise inserts a new record.
    /// </summary>
    /// <remarks>
    ///   If the record has already been fetched,
    ///   the primary key field values as at when the
    ///   record was fetched will be used
    ///   to specify the record to be updated.
    ///   So primary key field values can be changed.
    /// </remarks>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    public virtual void Save() {
      bool hasbeenFetched =
        Table != null
        && Table.Rows.Count != 0
        || Fetch();
      if (hasbeenFetched) {
        foreach (EntityColumn<T> column in Columns) {
          Adapter.UpdateCommand.Parameters["@" + column.ColumnName].Value =
            column.GetValue(this);
          if (column.IsInPrimaryKey) {
            Adapter.UpdateCommand.Parameters["@OLD_" + column.ColumnName]
                .Value =
              Adapter.SelectCommand.Parameters["@" + column.ColumnName].Value;
          }
        } // End of foreach
        Adapter.UpdateCommand.Connection.Open();
        try {
          Adapter.UpdateCommand.ExecuteNonQuery();
        } catch (PgSqlException ex) {
          throw new DataException(
            "Error on executing SQL command to save " + TableName + ":" +
            Environment.NewLine
            + ex.Message + Environment.NewLine + Environment.NewLine
            + "SQL command text:" + Environment.NewLine + Environment.NewLine
            + Adapter.UpdateCommand.CommandText
            + Environment.NewLine + Environment.NewLine,
            ex);
        } finally {
          Adapter.UpdateCommand.Connection.Close();
        }
      } else {
        foreach (EntityColumn<T> column in Columns) {
          Adapter.InsertCommand.Parameters["@" + column.ColumnName].Value =
            column.GetValue(this);
        } // End of foreach
        Adapter.InsertCommand.Connection.Open();
        try {
          Adapter.InsertCommand.ExecuteNonQuery();
        } catch (PgSqlException ex) {
          throw new DataException(
            "Error on executing SQL command to save " + TableName + ":" +
            Environment.NewLine
            + ex.Message + Environment.NewLine + Environment.NewLine
            + "SQL command text:" + Environment.NewLine + Environment.NewLine
            + Adapter.InsertCommand.CommandText
            + Environment.NewLine + Environment.NewLine,
            ex);
        } finally {
          Adapter.InsertCommand.Connection.Close();
        }
      }
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the derived class's
    ///   field properties.
    /// </summary>
    /// <remarks>
    ///   Each <see cref="EntityColumn" />
    ///   will correspond to a property of the derived class
    ///   that is flagged with a <see cref="FieldAttribute" />
    ///   or an attribute that is derived from <see cref="FieldAttribute" />.
    /// </remarks>
    public EntityColumnList Columns {
      get {
        if (_columns == null) {
          _columns = CreateColumns();
        }
        return _columns;
      }
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the derived class's
    ///   field properties that are not in the primary key.
    /// </summary>
    /// <remarks>
    ///   Each <see cref="EntityColumn" />
    ///   will correspond to a property of the derived class
    ///   that is flagged with a <see cref="FieldAttribute" />
    ///   or an attribute that is derived from <see cref="FieldAttribute" />
    ///   but is not a <see cref="PrimaryKeyFieldAttribute" />.
    /// </remarks>
    public EntityColumnList NonPrimaryKeyColumns {
      get {
        var nonPrimaryKeyColumns = new EntityColumnList();
        foreach (var column in Columns) {
          if (!column.IsInPrimaryKey) {
            nonPrimaryKeyColumns.Add(column);
          }
        }
        return nonPrimaryKeyColumns;
      }
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by those of the derived class's
    ///   field properties that are not in the unique key
    ///   (if there is one).
    /// </summary>
    /// <remarks>
    ///   Each <see cref="EntityColumn" />
    ///   will correspond to a property of the derived class
    ///   that is flagged with a <see cref="FieldAttribute" />
    ///   or an attribute that is derived from <see cref="FieldAttribute" />
    ///   but is not a <see cref="UniqueFieldAttribute" />.
    /// </remarks>
    public EntityColumnList NonUniqueKeyColumns {
      get {
        var nonUniqueKeyColumns = new EntityColumnList();
        foreach (var column in Columns) {
          if (!column.IsInUniqueKey) {
            nonUniqueKeyColumns.Add(column);
          }
        }
        return nonUniqueKeyColumns;
      }
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the derived class's
    ///   primary key field properties.
    /// </summary>
    /// <remarks>
    ///   Each <see cref="EntityColumn" />
    ///   will correspond to a property of the derived class
    ///   that is flagged with a <see cref="PrimaryKeyFieldAttribute" />.
    /// </remarks>
    public EntityColumnList PrimaryKeyColumns {
      get {
        var primaryKeyColumns = new EntityColumnList();
        foreach (var column in Columns) {
          if (column.IsInPrimaryKey) {
            primaryKeyColumns.Add(column);
          }
        }
        return primaryKeyColumns;
      }
    }

    /// <summary>
    ///   Gets the name of the database table
    ///   containing the row represented by the entity.
    /// </summary>
    public string TableName => typeof(T).Name;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the derived class's
    ///   unique key field properties.
    /// </summary>
    /// <remarks>
    ///   Empty if there is no unique key.
    ///   Each <see cref="EntityColumn" />
    ///   will correspond to a property of the derived class
    ///   that is flagged with a <see cref="UniqueKeyFieldAttribute" />.
    /// </remarks>
    public EntityColumnList UniqueKeyColumns {
      get {
        var uniqueKeyColumns = new EntityColumnList();
        foreach (var column in Columns) {
          if (column.IsInUniqueKey) {
            uniqueKeyColumns.Add(column);
          }
        }
        return uniqueKeyColumns;
      }
    }

    /// <summary>
    ///   Creates a deep copy of the entity.
    /// </summary>
    /// <returns>
    ///   The deep copy created.
    /// </returns>
    public virtual T Clone() {
      var that = Factory<T>.Create(typeof(T));
      DeeplyCopyEntityProperties(this, that);
      return that;
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the
    ///   field properties of the listed <see cref="Entity" />.
    /// </summary>
    /// <returns>
    ///   An <see cref="EntityColumnList" /> representing the columns.
    /// </returns>
    private EntityColumnList CreateColumns() {
      var properties = typeof(T).GetProperties();
      var columns = new EntityColumnList(properties.Count());
      foreach (var property in properties) {
        var customAttributes = property.GetCustomAttributes(true);
        bool propertyIsField = (
          from Attribute attribute in customAttributes
          where attribute.GetType().IsSubclassOf(typeof(FieldAttribute))
                || attribute.GetType() == typeof(FieldAttribute)
          select attribute).Any();
        if (propertyIsField) {
          var column = new EntityColumn<T>();
          var referencedFieldAttribute = (ReferencedFieldAttribute)(
            from Attribute attribute in customAttributes
            where attribute.GetType()
                    .IsSubclassOf(typeof(ReferencedFieldAttribute))
                  || attribute.GetType() == typeof(ReferencedFieldAttribute)
            select attribute).FirstOrDefault();
          if (referencedFieldAttribute != null) {
            string referencedColumnName;
            string referencedTableName;
            if (referencedFieldAttribute.Name.Contains(".")) {
              var chunks = referencedFieldAttribute.Name.Split('.');
              referencedColumnName = chunks[1];
              referencedTableName = chunks[0];
            } else {
              referencedColumnName = referencedFieldAttribute.Name;
              referencedTableName = property.Name;
            }
            if (!Factory<IEntityList>.Types.ContainsKey(referencedTableName)) {
              if (referencedFieldAttribute.Name.Contains(".")) {
                throw new ApplicationException(
                  "There is no EntityList class for table "
                  + referencedTableName
                  + " specified for referenced field property "
                  + property.Name + " of Entity class " + TableName + ".");
              }
              throw new ApplicationException(
                "There is no EntityList class for a table "
                + "with the same name as referenced field property "
                + referencedTableName + " of Entity class " + TableName + ".");
            }
            var referencedEntity = Factory<IEntity>.Create(referencedTableName);
            var referencedColumn =
              referencedEntity.Columns[referencedColumnName];
            if (referencedColumn == null) {
              throw new ApplicationException(
                "Referenced Entity class "
                + referencedTableName
                + " does not contain a field property named "
                + referencedColumnName
                + " as specified in the ReferencedField attribute of field property "
                + property.Name + " of Entity class " + TableName + ".");
            }
            if (referencedColumn.DataType != property.PropertyType) {
              throw new ApplicationException(
                "Data type " + referencedColumn.DataType
                             + " of field property "
                             + referencedColumnName
                             + " of referenced Entity class "
                             + referencedTableName
                             + " is not the same as data type " +
                             property.PropertyType
                             + " of referencing field property "
                             + property.Name + " of entity class " + TableName +
                             ".");
            }
            column.ReferencedColumnName = referencedColumnName;
            column.ReferencedTableName = referencedTableName;
            if (property.PropertyType == typeof(DateTime)) {
              column.NameOnDb = referencedTableName + "Date";
            } else if (referencedTableName == "Artist") {
              column.NameOnDb = referencedTableName + "Name";
            } else {
              column.NameOnDb = referencedTableName + "Id";
            }
          }
          column.ColumnName = property.Name;
          column.DataType = property.PropertyType;
          column.IsInPrimaryKey = (
            from Attribute attribute in customAttributes
            where attribute.GetType()
                    .IsSubclassOf(typeof(PrimaryKeyFieldAttribute))
                  || attribute.GetType() == typeof(PrimaryKeyFieldAttribute)
            select attribute).Any();
          column.IsInUniqueKey = (
            from Attribute attribute in customAttributes
            where attribute.GetType()
                    .IsSubclassOf(typeof(UniqueKeyFieldAttribute))
                  || attribute.GetType() == typeof(UniqueKeyFieldAttribute)
            select attribute).Any();
          column.IsHidden = (
            from Attribute attribute in customAttributes
            where attribute.GetType().IsSubclassOf(typeof(HiddenFieldAttribute))
                  || attribute.GetType() == typeof(HiddenFieldAttribute)
            select attribute).Any();
          column.Visible = !column.IsHidden;
          columns.Add(column);
        }
      } //End of foreach
      return columns;
    }

    /// <summary>
    ///   Deeply copies the properties of a source Entity to a target Entity.
    /// </summary>
    /// <param name="sourceEntity">
    ///   The source Entity.
    /// </param>
    /// <param name="targetEntity">
    ///   The target Entity.
    /// </param>
    private void DeeplyCopyEntityProperties(IEntity sourceEntity,
      IEntity targetEntity) {
      var properties = sourceEntity.GetType().GetProperties(
        BindingFlags.Instance | BindingFlags.Public);
      foreach (var property in properties) {
        if (property.CanWrite) {
          //Debug.WriteLine(property.Name + " " + property.PropertyType.Name);
          if (property.PropertyType.IsValueType
              || property.PropertyType == typeof(string)) {
            var value = property.GetValue(sourceEntity, null);
            property.SetValue(targetEntity, value, null);
            //Debug.WriteLine(value);
          } else if (property.PropertyType.GetInterfaces()
            .Contains(typeof(IEntityList))) {
            // So far, the only entity list property this has been implemented for is
            // Piece.Credits.
            // For any new ones, 
            // the child entity must have a (bool empty) constructor
            // like CreditList does.
            // There are now several Entities 
            // (the ones that derive from PieceOwningEntity) 
            // that have Pieces properties.
            // For now, I think I can get away with not implementing a clone
            // of these properties because,
            // unless and until I implement unforced tagging,
            // I'm only cloning entitys to keep a record of deleted Credits
            // so that they can be removed from tags.
            // Without unforced tagging,
            // even cloning Piece.Credits is overkill.
            //Debug.WriteLine(property.Name + " IEntityList");
            if (property.Name == "Credits") {
              var sourceChildEntities =
                (IEntityList)property.GetValue(sourceEntity, null);
              var targetChildEntities = Factory<IEntityList>.Create(
                property.PropertyType,
                // Constructor arguments
                true); // empty
              foreach (IEntity sourceChildEntity in sourceChildEntities) {
                var targetChildEntity =
                  Factory<IEntity>.Create(sourceChildEntity.GetType());
                // Recursively copy the properties of the source child Entity to the target child Entity.
                DeeplyCopyEntityProperties(sourceChildEntity,
                  targetChildEntity);
                targetChildEntities.Add(targetChildEntity);
              }
              property.SetValue(targetEntity, targetChildEntities, null);
            }
          } else if (property.PropertyType.GetInterfaces()
            .Contains(typeof(IEntity))) {
            //Debug.WriteLine(property.Name + " IEntity");
            // So far, the only entity property this applies to is Piece.Original.
            // Because Piece.Original is itself a Piece,
            // recursively copying its properties would lead to an infinite loop and thus
            // a StackOverflowException.
            // That's OK, because we don't need the Original of the Original.
            if (property.PropertyType != sourceEntity.GetType()) {
              // This has never been tested without causing an infinite loop.
              // See comment above.
              var sourceSubEntity =
                (IEntity)property.GetValue(sourceEntity, null);
              var targetSubEntity =
                Factory<IEntity>.Create(property.PropertyType);
              // Recursively copy the properties of the source sub-Entity to the target sub-Entity.
              DeeplyCopyEntityProperties(sourceSubEntity, targetSubEntity);
              property.SetValue(targetEntity, targetSubEntity, null);
            }
          } else if (property.PropertyType == typeof(PieceAudioTags)) {
            /// Piece.AudioTags
            var audioTags = Factory<PieceAudioTags>.Create(
              typeof(PieceAudioTags),
              // Constructor arguments
              targetEntity as Piece); // piece
            // There's no need to interate through the properties
            // of the new PieceAudioTags to see which ones need setting,
            // as all its properties are read-only 
            // and derived from the target Piece, 
            // as specified in the constructor.
            property.SetValue(targetEntity, audioTags, null);
          } else {
            // A good way to support properties of a new type
            // might be to have that type implement ICloneable
            // and invoke the Clone method here if it does.
            throw new NotSupportedException(
              "The " + property.Name
                     + " property cannot be cloned because cloning properties of type "
                     + property.PropertyType.Name + " is not supported.");
          }
        }
      } //End of foreach
    }
  } //End of class
} //End of namespace