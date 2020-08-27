using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity column metadata.
  /// </summary>
  internal class EntityColumn<T> : IEntityColumn
    where T : Entity<T> {
    private string _nameOnDb;

    /// <summary>
    ///   Gets or sets the name of the column.
    ///   This must be set to the name of the corresponding
    ///   property of the class derived from <see cref="Entity" />.
    /// </summary>
    public virtual string ColumnName { get; set; }

    /// <summary>
    ///   Gets or sets the type of data stored in the column.
    /// </summary>
    /// </remarks>
    public virtual Type DataType { get; set; }

    /// <summary>
    ///   Returns the field value
    ///   of the column in the specified
    ///   instance of the entity.
    /// </summary>
    /// <param name="entity">
    ///   The instance of the entity
    ///   for which the field value
    ///   of the column is required.
    /// </param>
    /// <returns>
    ///   The field value
    ///   of the column in the specified
    ///   instance of the entity.
    /// </returns>
    public object GetValue(IEntity entity) {
      try {
        return entity.GetType().InvokeMember(
          ColumnName,
          BindingFlags.GetProperty,
          null,
          entity,
          null);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
    }

    /// <summary>
    ///   Gets or sets whether the corresponding property of the class
    ///   derived from <see cref="Entity" />
    ///   is flagged with a <see cref="HiddenFieldAttribute" />.
    /// </summary>
    /// <remarks>
    ///   Not necessarily the opposite of the
    ///   <see cref="Visible" /> property.
    /// </remarks>
    public virtual bool IsHidden { get; set; }

    /// <summary>
    ///   Gets or sets whether the column is in
    ///   the primary key required for database access.
    /// </summary>
    /// <remarks>
    ///   Flag the corresponding property of the class
    ///   derived from <see cref="Entity" />
    ///   with a <see cref="PrimaryKeyFieldAttribute" />
    ///   to indicate that the column is in the primary key.
    /// </remarks>
    public virtual bool IsInPrimaryKey { get; set; }

    /// <summary>
    ///   Gets or sets whether the column is in
    ///   the unique key (if there is one).
    /// </summary>
    /// <remarks>
    ///   Flag the corresponding property of the class
    ///   derived from <see cref="Entity" />
    ///   with an <see cref="UniqueKeyFieldAttribute" />
    ///   to indicate that the column is in the primary key.
    /// </remarks>
    public bool IsInUniqueKey { get; set; }

    /// <summary>
    ///   Gets or sets the name of the column as on
    ///   the database table.
    /// </summary>
    /// <remarks>
    ///   The default is the same as <see cref="ColumnName" />,
    ///   except where the column is in the key
    ///   and <see cref="ColumnName" /> is "Date" or "Name",
    ///   in which case the default is "TC",
    ///   where "T" is the entity/table name and C is <see cref="ColumnName" />.
    ///   For example, for the Newsletter Entity's Date primary key column,
    ///   the corresponding column name in the database's
    ///   Newsletter table is NewsletterDate.
    /// </remarks>
    public virtual string NameOnDb {
      get {
        if (_nameOnDb == null) {
          if (IsInPrimaryKey) {
            switch (ColumnName) {
              case "Date":
              case "Name":
                _nameOnDb = TableName + ColumnName;
                break;
              default:
                _nameOnDb = ColumnName;
                break;
            } //End of switch
          } else {
            _nameOnDb = ColumnName;
          }
        }
        return _nameOnDb;
      }
      set => _nameOnDb = value;
    }

    /// <summary>
    ///   Gets or sets the name of the column on
    ///   the referenced database table whose name
    ///   is the name of the corresponding
    ///   property of the class derived from <see cref="Entity" />
    ///   (<see cref="ColumnName" />).
    ///   Null if the column is not from a referenced table.
    /// </summary>
    public virtual string ReferencedColumnName { get; set; }

    /// <summary>
    ///   Gets or sets the name of
    ///   the referenced database table whose name
    ///   is the name of the corresponding referenced <see cref="Entity" />
    ///   (<see cref="ColumnName" />).
    ///   Null if the column is not from a referenced table.
    /// </summary>
    public virtual string ReferencedTableName { get; set; }

    /// <summary>
    ///   Gets the one-based left-to-right sequence number of the column
    ///   in the main grid.  Zero if not to be included in the main grid.
    /// </summary>
    public int SequenceNo { get; set; }

    /// <summary>
    ///   Sets the field value
    ///   of the column in the specified
    ///   instance of the entity.
    /// </summary>
    /// <param name="entity">
    ///   The instance of the entity
    ///   for which the field value
    ///   of the column is to be set.
    /// </param>
    /// <param name="value">
    ///   The value to which
    ///   the field value
    ///   of the column is to be set.
    /// </param>
    public void SetValue(IEntity entity, object value) {
      try {
        entity.GetType().InvokeMember(
          ColumnName,
          BindingFlags.SetProperty,
          null,
          entity,
          new[] {value});
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }

    /// <summary>
    ///   Gets the name of the column's owning table.
    ///   This is the name of the corresponding
    ///   class derived from <see cref="Entity" />.
    /// </summary>
    public string TableName => typeof(T).Name;

    /// <summary>
    ///   Gets or sets whether the column is
    ///   be shown in the table editor.
    /// </summary>
    /// <remarks>
    ///   <b>False</b> if
    ///   the corresponding property of the class
    ///   derived from <see cref="Entity" />
    ///   is flagged with a <see cref="HiddenFieldAttribute" />
    ///   or if the column is the replicated from the parent grid.
    ///   Not necessarily the opposite of the
    ///   <see cref="IsHidden" /> property.
    /// </remarks>
    public virtual bool Visible { get; set; }

    [NotNull]
    public static EntityColumn<T> Create([NotNull] IList<FieldAttribute> fieldAttributes,
      [NotNull] PropertyInfo property) {
      var column = new EntityColumn<T>();
      PopulateColumnReferenceDetailsIfAny(fieldAttributes, property, column);
      column.ColumnName = property.Name;
      column.DataType = property.PropertyType;
      column.IsInPrimaryKey =
        DoesPropertyHaveAttribute<PrimaryKeyFieldAttribute>(fieldAttributes);
      column.IsInUniqueKey =
        DoesPropertyHaveAttribute<UniqueKeyFieldAttribute>(fieldAttributes);
      column.IsHidden =
        DoesPropertyHaveAttribute<HiddenFieldAttribute>(fieldAttributes);
      column.Visible = !column.IsHidden;
      column.SequenceNo = GetPropertySequenceNo(fieldAttributes);
      return column;
    }

    private static bool DoesPropertyHaveAttribute<TFieldAttribute>(
      [NotNull] IEnumerable<FieldAttribute> fieldAttributes)
      where TFieldAttribute : FieldAttribute {
      return (
        from Attribute attribute in fieldAttributes
        where attribute.GetType()
                .IsSubclassOf(typeof(TFieldAttribute))
              || attribute.GetType() == typeof(TFieldAttribute)
        select attribute).Any();
    }

    [CanBeNull]
    private static ReferencedFieldAttribute FindReferencedFieldAttribute(
      [NotNull] IEnumerable<FieldAttribute> fieldAttributes) {
      return (ReferencedFieldAttribute)(
        from Attribute attribute in fieldAttributes
        where attribute.GetType()
                .IsSubclassOf(typeof(ReferencedFieldAttribute))
              || attribute.GetType() == typeof(ReferencedFieldAttribute)
        select attribute).FirstOrDefault();
    }

    private static int GetPropertySequenceNo(
      [NotNull] IEnumerable<FieldAttribute> fieldAttributes) {
      return fieldAttributes.First().SequenceNo;
    }

    private static void PopulateColumnReferenceDetails([NotNull] PropertyInfo property,
      [NotNull] IEntityColumn column, [NotNull] string referencedColumnName,
      [NotNull] string referencedTableName) {
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

    private static void PopulateColumnReferenceDetailsIfAny(
      [NotNull] IEnumerable<FieldAttribute> fieldAttributes,
      [NotNull] PropertyInfo property, [NotNull] IEntityColumn column) {
      var referencedFieldAttribute =
        FindReferencedFieldAttribute(fieldAttributes);
      if (referencedFieldAttribute == null) {
        return;
      }
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
      ValidateReference(property, referencedTableName, referencedFieldAttribute,
        referencedColumnName);
      PopulateColumnReferenceDetails(property, column, referencedColumnName,
        referencedTableName);
    }

    private static void ValidateReference([NotNull] PropertyInfo property,
      [NotNull] string referencedTableName,
      [NotNull] ReferencedFieldAttribute referencedFieldAttribute,
      [NotNull] string referencedColumnName) {
      if (!Factory<IEntityList>.Types.ContainsKey(referencedTableName)) {
        if (referencedFieldAttribute.Name.Contains(".")) {
          throw new ApplicationException(
            "There is no EntityList class for table "
            + referencedTableName
            + " specified for referenced field property "
            + property.Name + " of Entity class " + typeof(T).Name + ".");
        }
        throw new ApplicationException(
          "There is no EntityList class for a table "
          + "with the same name as referenced field property "
          + referencedTableName + " of Entity class " + typeof(T).Name + ".");
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
          + property.Name + " of Entity class " + typeof(T).Name + ".");
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
                       + property.Name + " of entity class " +
                       typeof(T).Name +
                       ".");
      }
    }
  } //End of class
} //End of namespace