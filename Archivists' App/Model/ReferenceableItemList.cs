using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class ReferenceableItemList : List<object> {
    public ReferenceableItemList([NotNull] BindingColumn referencingColumn) {
      ReferencingColumn = referencingColumn;
    }

    private IDictionary<string, IEntity> EntityDictionary { get; set; }
    [NotNull] private BindingColumn ReferencingColumn { get; }

    public bool ContainsFormattedValue([NotNull] string formattedValue) {
      return EntityDictionary.ContainsKey(formattedValue);
    }

    internal void Fetch() {
      EntityDictionary = FetchEntityDictionary();
      Clear();
      AddRange(
        from KeyValuePair<string, IEntity> pair in EntityDictionary
        select (object)new KeyValuePair<string, object>(pair.Key, pair.Value)
      );
    }

    [NotNull]
    private IDictionary<string, IEntity> FetchEntityDictionary() {
      var entities = CreateEntityList();
      entities.Populate(createBindingList:false);
      return (from IEntity entity in entities
          select new KeyValuePair<string, IEntity>(GetKey(entity), entity)
        ).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    [CanBeNull]
    private static string Format([NotNull] object referencingPropertyValue) {
      if (referencingPropertyValue is DateTime date) {
        return date > EntityBase.InitialDate ? date.ToString(Global.DateFormat) : null;
      } // string
      return referencingPropertyValue.ToString();
    }

    [CanBeNull]
    internal IEntity GetEntity([NotNull] object referencingPropertyValue) {
      string formattedValue = Format(referencingPropertyValue);
      if (formattedValue == null) {
        return null;
      }
      if (ContainsFormattedValue(formattedValue)) {
        return EntityDictionary[formattedValue];
      }
      var message =
        $"{ReferencingColumn.Name} not found: '{formattedValue}'";
      throw new RowNotInTableException(message);
    }

    [NotNull]
    private IEntityList CreateEntityList() {
      var result =
        Global.CreateEntityList(
          ReferencingColumn.ReferencedEntityListType ??
          throw new NullReferenceException(
            nameof(ReferencingColumn.ReferencedEntityListType)));
      result.Session = ReferencingColumn.Session;
      return result;
    }

    [CanBeNull]
    public static string GetKey([CanBeNull] object value) {
      switch (value) {
        case null:
          return null;
        case Newsletter newsletter:
          return newsletter.Date.ToString(Global.DateFormat);
        case IEntity entity:
          return entity.SimpleKey;
        default:
          // After duplicate key error message shown on inserting event.
          // Is this a problem?
          return value.ToString();
      }
    }
  }
}