using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Represents an entity table in a form suitable (when converted to an array)
  ///   for populating a ComboBox for selecting a referenced entity.
  /// </summary>
  public class ReferenceableItemList : List<object> {
    public ReferenceableItemList(BindingColumn referencingColumn) {
      ReferencingColumn = referencingColumn;
    }

    private IDictionary<string, IEntity?>? EntityDictionary { get; set; }
    private BindingColumn ReferencingColumn { get; }

    public bool ContainsKey(string simpleKey) {
      return EntityDictionary!.ContainsKey(simpleKey);
    }

    private static IDictionary<string, IEntity?> CreateEntityDictionary(
      // ReSharper disable once SuggestBaseTypeForParameter
      IEntityList entities) {
      return entities.Cast<IEntity>().ToDictionary(entity => ToSimpleKey(entity)!)!;
    }

    private IEntityList CreateEntityList() {
      var result = Global.CreateEntityList(ReferencingColumn.ReferencedEntityListType!);
      result.Session = ReferencingColumn.Session;
      return result;
    }

    public static RowNotInTableException CreateReferencedEntityNotFoundException(
      string columnName, string simpleKey) {
      string message = $"{columnName} not found: '{Format(simpleKey)}'";
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
    private static string? Format(string? simpleKey) {
      return DateTime.TryParse(simpleKey, out var date)
        ? date.ToString(Global.DateFormat)
        : simpleKey;
    }

    internal IEntity? GetEntity(object referencingPropertyValue) {
      string simpleKey = ToSimpleKey(referencingPropertyValue)!;
      if (ContainsKey(simpleKey)) {
        return EntityDictionary![simpleKey];
      }
      throw CreateReferencedEntityNotFoundException(
        ReferencingColumn.PropertyName, simpleKey);
    }

    private void PopulateItems(
      // ReSharper disable once SuggestBaseTypeForParameter
      IEntityList entities) {
      Clear();
      AddRange(
        from IEntity entity in entities
        select (object)new KeyValuePair<object?, object>(Format(entity.SimpleKey), entity)
      );
    }

    public static string? ToSimpleKey(object? value) {
      return value switch {
        null => null,
        Newsletter newsletter => EntityBase.DateToSimpleKey(newsletter.Date),
        IEntity entity => entity.SimpleKey,
        DateTime date => EntityBase.DateToSimpleKey(date),
        _ => value.ToString()
      };
    }
  }
}