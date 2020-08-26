using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundExplorers.Data {
    /// <summary>
    ///   A keyed list of entity columns with
    ///   <see cref="IEntityColumn.ColumnName" /> as the key.
    /// </summary>
    internal class EntityColumnList : List<IEntityColumn> {
        /// <summary>
        ///   Initialises a new instance of the <see cref="EntityColumnList" /> class.
        /// </summary>
        public EntityColumnList() { }

        /// <summary>
        ///   Initialises a new instance of the <see cref="EntityColumnList" /> class,
        ///   specifying the
        ///   number of entity columns that the new list can initially store.
        /// </summary>
        /// <param name="capacity">
        ///   The number of entity columns that the new list can initially store.
        /// </param>
        public EntityColumnList(int capacity) : base(capacity) { }

        /// <summary>
        ///   Returns the entity column with the specified name,
        ///   if found, otherwise returns a null reference.
        /// </summary>
        /// <param name="columnName">
        ///   The name of the column.
        /// </param>
        /// <returns>
        ///   The entity column with the specified name,
        ///   if found, otherwise a null reference.
        /// </returns>
        public IEntityColumn this[string columnName] =>
    (
      from IEntityColumn entityColumn in this
      where entityColumn.ColumnName == columnName
      select entityColumn).FirstOrDefault();

        /// <summary>
        ///   Add the specified entity column to the list,
        ///   provided its name is unique in the list.
        /// </summary>
        /// <param name="entityColumn">
        ///   The entity column to be added.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   The list already contains an entity column
        ///   of the same name.
        /// </exception>
        public new void Add(IEntityColumn entityColumn) {
      if (ContainsKey(entityColumn.ColumnName)) {
        throw new ArgumentException(
          "The list already contains an entity column named \""
          + entityColumn.ColumnName + "\".",
          "entityColumn");
      }
      base.Add(entityColumn);
    }

        /// <summary>
        ///   Returns whether the list contains
        ///   an entity column with the specified name.
        /// </summary>
        /// <param name="columnName">
        ///   The name of the column.
        /// </param>
        /// <returns>
        ///   Whether the list contains
        ///   an entity column with the specified name.
        /// </returns>
        public bool ContainsKey(string columnName) {
      return (
        from IEntityColumn entityColumn in this
        where entityColumn.ColumnName == columnName
        select entityColumn).Count() > 0;
    }
  } //End of class
} //End of namespace