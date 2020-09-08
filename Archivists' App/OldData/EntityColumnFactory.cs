using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.OldData {
  internal class EntityColumnFactory {
    private IEntityColumn Column { get; set; }
    private Type EntityType { get; set; }
    private IList<FieldAttribute> FieldAttributes { get; set; }
    private PropertyInfo Property { get; set; }

    [NotNull]
    public EntityColumn<TEntity> Create<TEntity>(
      [NotNull] IList<FieldAttribute> fieldAttributes,
      [NotNull] PropertyInfo property) where TEntity : Entity<TEntity> {
      FieldAttributes = fieldAttributes;
      Property = property;
      Column = new EntityColumn<TEntity>();
      EntityType = typeof(TEntity);
      Column.ColumnName = property.Name;
      Column.DataType = property.PropertyType;
      Column.IsInPrimaryKey = FindAttribute<PrimaryKeyFieldAttribute>() != null;
      Column.IsInUniqueKey = FindAttribute<UniqueKeyFieldAttribute>() != null;
      Column.SequenceNo = GetPropertySequenceNo();
      Column.IsHidden = FindAttribute<HiddenFieldAttribute>() != null ||
                        Column.SequenceNo < 1;
      Column.Visible = !Column.IsHidden;
      var referencedFieldAttribute = FindAttribute<ReferencedFieldAttribute>();
      if (referencedFieldAttribute != null) {
        PopulateColumnReferenceDetails(referencedFieldAttribute);
      }
      return (EntityColumn<TEntity>)Column;
    }

    [CanBeNull]
    private TFieldAttribute FindAttribute<TFieldAttribute>()
      where TFieldAttribute : FieldAttribute {
      return (TFieldAttribute)(
        from Attribute attribute in FieldAttributes
        where attribute.GetType()
                .IsSubclassOf(typeof(TFieldAttribute))
              || attribute.GetType() == typeof(TFieldAttribute)
        select attribute).FirstOrDefault();
    }

    private int GetPropertySequenceNo() {
      return FieldAttributes.First().SequenceNo;
    }

    private void PopulateColumnReferenceDetails(
      [NotNull] ReferencedFieldAttribute referencedFieldAttribute) {
      string referencedColumnName;
      string referencedTableName;
      if (referencedFieldAttribute.Name.Contains(".")) {
        var chunks = referencedFieldAttribute.Name.Split('.');
        referencedColumnName = chunks[1];
        referencedTableName = chunks[0];
      } else {
        referencedColumnName = referencedFieldAttribute.Name;
        referencedTableName = Property.Name;
      }
      ValidateReference(referencedFieldAttribute, referencedTableName,
        referencedColumnName);
      SetColumnReferenceProperties(referencedTableName, referencedColumnName);
    }

    private void SetColumnReferenceProperties([NotNull] string referencedTableName,
      [NotNull] string referencedColumnName) {
      Column.ReferencedColumnName = referencedColumnName;
      Column.ReferencedTableName = referencedTableName;
      if (Property.PropertyType == typeof(DateTime)) {
        Column.NameOnDb = referencedTableName + "Date";
      } else if (referencedTableName == nameof(Artist) ||
                 referencedTableName == nameof(Act)) {
        Column.NameOnDb = referencedTableName + "Name";
      } else {
        Column.NameOnDb = referencedTableName + "Id";
      }
    }

    private void ValidateReference(
      [NotNull] ReferencedFieldAttribute referencedFieldAttribute,
      [NotNull] string referencedTableName,
      [NotNull] string referencedColumnName) {
      if (!Factory<IEntityList>.Types.ContainsKey(referencedTableName)) {
        if (referencedFieldAttribute.Name.Contains(".")) {
          throw new ApplicationException(
            "There is no EntityList class for table "
            + referencedTableName
            + " specified for referenced field Property "
            + Property.Name + " of Entity class " + EntityType.Name + ".");
        }
        throw new ApplicationException(
          "There is no EntityList class for a table "
          + "with the same name as referenced field Property "
          + referencedTableName + " of Entity class " + EntityType.Name + ".");
      }
      var referencedEntity = Factory<IEntity>.Create(referencedTableName);
      var referencedColumn =
        referencedEntity.Columns[referencedColumnName];
      if (referencedColumn == null) {
        throw new ApplicationException(
          "Referenced Entity class "
          + referencedTableName
          + " does not contain a field Property named "
          + referencedColumnName
          + " as specified in the ReferencedField attribute of field Property "
          + Property.Name + " of Entity class " + EntityType.Name + ".");
      }
      if (referencedColumn.DataType != Property.PropertyType) {
        throw new ApplicationException(
          "Data type " + referencedColumn.DataType
                       + " of field Property "
                       + referencedColumnName
                       + " of referenced Entity class "
                       + referencedTableName
                       + " is not the same as data type " +
                       Property.PropertyType
                       + " of referencing field Property "
                       + Property.Name + " of entity class " +
                       EntityType.Name +
                       ".");
      }
    }
  }
}