using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Represents an entity table in a form suitable (when converted to an array)
  ///   for populating a ComboBox.
  /// </summary>
  public class ReferenceableItemList : List<object> {
    public ReferenceableItemList([NotNull] BindingColumn referencingColumn) {
      ReferencingColumn = referencingColumn;
    }

    private IDictionary<string, IEntity> EntityDictionary { get; set; }
    [NotNull] private BindingColumn ReferencingColumn { get; }

    public bool ContainsKey([NotNull] string simpleKey) {
      return EntityDictionary.ContainsKey(simpleKey);
    }

    [NotNull]
    private static IDictionary<string, IEntity> CreateEntityDictionary(
      // ReSharper disable once SuggestBaseTypeForParameter
      [NotNull] IEntityList entities) {
      return (
        from IEntity entity in entities
        select new KeyValuePair<string, IEntity>(ToSimpleKey(entity), entity)
      ).ToDictionary(pair => pair.Key, pair => pair.Value);
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

    internal void Fetch() {
      var entities = CreateEntityList();
      entities.Populate(createBindingList: false);
      EntityDictionary = CreateEntityDictionary(entities);
      PopulateItems(entities);
    }

    /// <summary>
    ///   Returns the specified simple key formatted as it appears on the grid.
    /// </summary>
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

    private void PopulateItems(
      // ReSharper disable once SuggestBaseTypeForParameter
      IEntityList entities) {
      Clear();
      // if (entities is NewsletterList) {
      //   Add(new KeyValuePair<object, object>(
      //     Format(EntityBase.DateToSimpleKey(EntityBase.InitialDate)), new Newsletter()));
      // }
      AddRange(
        from IEntity entity in entities
        select (object)new KeyValuePair<object, object>(Format(entity.SimpleKey), entity)
      );
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