using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  public class EntityColumnListFactory {
    public EntityColumnListFactory() {
      EntityColumnFactory = new EntityColumnFactory();
    }

    private EntityColumnFactory EntityColumnFactory { get; }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the field properties of the listed Entity />.
    /// </summary>
    /// <returns>
    ///   An <see cref="EntityColumnList" /> representing the columns.
    /// </returns>
    [NotNull]
    internal EntityColumnList Create<TEntity>() where TEntity : Entity<TEntity> {
      var properties = typeof(TEntity).GetProperties();
      var result = new EntityColumnList();
      foreach (var property in properties) {
        var fieldAttributes = GetFieldAttributes(property);
        if (PropertyIsField(fieldAttributes)) {
          result.Add(EntityColumnFactory.Create<TEntity>(fieldAttributes, property));
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
  }
}