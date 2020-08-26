using System;

namespace SoundExplorers.Data {
    /// <summary>
    ///   Entity column metadata interface.
    /// </summary>
    internal interface IEntityColumn {
        /// <summary>
        ///   Gets or sets the name of the column.
        ///   This must be set to the name of the corresponding
        ///   property of the class derived from <see cref="Entity" />.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        ///   Gets or sets the type of data stored in the column.
        /// </summary>
        Type DataType { get; set; }

        /// <summary>
        ///   Gets or sets whether the corresponding property of the class
        ///   derived from <see cref="Entity" />
        ///   is flagged with a <see cref="HiddenFieldAttribute" />.
        /// </summary>
        bool IsHidden { get; set; }

        /// <summary>
        ///   Gets or sets whether the column is in
        ///   the primary key required for database access.
        /// </summary>
        bool IsInPrimaryKey { get; set; }

        /// <summary>
        ///   Gets or sets whether the column is in
        ///   the unique key (if there is one).
        /// </summary>
        bool IsInUniqueKey { get; set; }

        /// <summary>
        ///   Gets or sets the name of the column as on
        ///   the database table.
        /// </summary>
        string NameOnDb { get; set; }

        /// <summary>
        ///   Gets or sets the name of the column on
        ///   the referenced database table whose name
        ///   is the name of the corresponding
        ///   property of the class derived from <see cref="Entity" />.
        ///   Null if the column is not from a referenced table.
        /// </summary>
        string ReferencedColumnName { get; set; }

        /// <summary>
        ///   Gets or sets the name of
        ///   the referenced database table whose name
        ///   is the name of the corresponding referenced <see cref="Entity" />.
        ///   Null if the column is not from a referenced table.
        /// </summary>
        string ReferencedTableName { get; set; }

        /// <summary>
        ///   Gets the name of the column's owning table.
        ///   This is the name of the corresponding
        ///   class derived from <see cref="Entity" />.
        /// </summary>
        string TableName { get; }

        /// <summary>
        ///   Gets or sets whether the column is
        ///   be shown in the table editor.
        /// </summary>
        bool Visible { get; set; }

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
        object GetValue(IEntity entity);

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
        void SetValue(IEntity entity, object value);
  } //End of class
} //End of namespace