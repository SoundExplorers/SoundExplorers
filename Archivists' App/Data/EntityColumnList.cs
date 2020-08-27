using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

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
    ///   Returns the entity column with the specified name (case-insensitive),
    ///   if found, otherwise returns a null reference.
    /// </summary>
    /// <param name="columnName">
    ///   The name of the column (case-insensitive).
    /// </param>
    /// <returns>
    ///   The entity column with the specified name (case-insensitive),
    ///   if found, otherwise a null reference.
    /// </returns>
    [CanBeNull]
    public IEntityColumn this[string columnName] =>
    (
      from IEntityColumn entityColumn in this
      where string.Compare(entityColumn.ColumnName, columnName,
        StringComparison.OrdinalIgnoreCase) == 0
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
          $"The list already contains an entity column named {entityColumn.ColumnName}.",
          nameof(entityColumn));
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
        select entityColumn).Any();
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the
    ///   field properties of the listed <see cref="Entity" />.
    /// </summary>
    /// <returns>
    ///   An <see cref="EntityColumnList" /> representing the columns.
    /// </returns>
    [NotNull]
    public static EntityColumnList Create<T>() where T : Entity<T> {
      var properties = typeof(T).GetProperties();
      var result = new EntityColumnList(properties.Length);
      foreach (var property in properties) {
        var fieldAttributes = GetFieldAttributes(property);
        if (PropertyIsField(fieldAttributes)) {
          result.Add(EntityColumn<T>.Create(fieldAttributes, property));
        }
      }
      result.Sort(new EntityColumnComparer());
      return result;
    }

    [NotNull]
    private static IList<FieldAttribute> GetFieldAttributes(
      [NotNull] PropertyInfo property) {
      return (
        from Attribute attribute in property.GetCustomAttributes(true)
        where attribute.GetType().IsSubclassOf(typeof(FieldAttribute))
              || attribute.GetType() == typeof(FieldAttribute)
        select (FieldAttribute)attribute).ToList();
    }

    private static bool PropertyIsField(
      [NotNull] IEnumerable<FieldAttribute> fieldAttributes) {
      return fieldAttributes.Any();
    }
  } //End of class
} //End of namespace