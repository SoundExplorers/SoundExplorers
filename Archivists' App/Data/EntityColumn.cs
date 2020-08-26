using System;
using System.Reflection;

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
        throw ex.InnerException;
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
  } //End of class
} //End of namespace