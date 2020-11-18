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

    public bool ContainsKey([NotNull] string simpleKey) {
      return EntityDictionary.ContainsKey(simpleKey);
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
      entities.Populate(createBindingList: false);
      return (from IEntity entity in entities
          select new KeyValuePair<string, IEntity>(ToSimpleKey(entity), entity)
        ).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    [CanBeNull]
    private static string Format([NotNull] string simpleKey) {
      return DateTime.TryParse(simpleKey, out var date)
        ? date.ToString(Global.DateFormat)
        : simpleKey;
    }

    [CanBeNull]
    internal IEntity GetEntity([NotNull] object referencingPropertyValue) {
      string simpleKey = ToSimpleKey(referencingPropertyValue);
      if (string.IsNullOrWhiteSpace(simpleKey)) {
        return null;
      }
      if (ContainsKey(simpleKey)) {
        return EntityDictionary[simpleKey];
      }
      throw CreateReferencedEntityNotFoundException(
        ReferencingColumn.Name, simpleKey);
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

    [NotNull]
    public static RowNotInTableException CreateReferencedEntityNotFoundException(
      [NotNull] string columnName, [NotNull] string simpleKey) {
      var message = $"{columnName} not found: '{Format(simpleKey)}'";
      // Debug.WriteLine("ReferenceableItemList.CreateReferencedEntityNotFoundException");
      // Debug.WriteLine($"    {message}");
      return new RowNotInTableException(message);
    }

    public static string ToSimpleKey([CanBeNull] object value) {
      switch (value) {
        case null:
          return null;
        case Newsletter newsletter:
          return EntityBase.DateToSimpleKey(newsletter.Date);
        case IEntity entity:
          return entity.SimpleKey;
        case DateTime date:
          return EntityBase.DateToSimpleKey(date);
        default:
          return value.ToString();
      }
    }
  }
}