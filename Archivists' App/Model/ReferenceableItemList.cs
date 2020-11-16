using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class ReferenceableItemList : List<object> {
    private Type _referencedEntityListType;
    private string _referencedPropertyName;
    private string _referencedTableName;
    private SessionBase _session;

    public ReferenceableItemList([NotNull] BindingColumn referencingColumn) {
      ReferencingColumn = referencingColumn;
    }

    private IDictionary<string, IEntity> EntityDictionary { get; set; }
    [NotNull] private BindingColumn ReferencingColumn { get; }

    [NotNull]
    private Type ReferencedEntityListType =>
      _referencedEntityListType ?? (_referencedEntityListType =
        ReferencingColumn.ReferencedEntityListType ??
        throw new NullReferenceException(
          nameof(BindingColumn.ReferencedEntityListType)));

    [NotNull]
    public string ReferencedPropertyName =>
      _referencedPropertyName ?? (_referencedPropertyName =
        ReferencingColumn.ReferencedPropertyName ??
        throw new NullReferenceException(nameof(BindingColumn.ReferencedPropertyName)));

    [NotNull]
    public string ReferencedTableName => _referencedTableName ?? (_referencedTableName =
      ReferencingColumn.ReferencedTableName ??
      throw new NullReferenceException(nameof(BindingColumn.ReferencedTableName)));

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    public SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    public bool ContainsKey([NotNull] string formattedValue) {
      return EntityDictionary.ContainsKey(formattedValue);
    }

    [NotNull]
    private IEntityList CreateEntityList() {
      var result = Global.CreateEntityList(ReferencedEntityListType);
      result.Session = Session;
      return result;
    }

    public void Fetch([CanBeNull] string format) {
      EntityDictionary = FetchEntityDictionary(format);
      Clear();
      AddRange(
        from KeyValuePair<string, IEntity> pair in EntityDictionary
        select (object)new KeyValuePair<string, object>(pair.Key, pair.Value)
      );
    }

    [NotNull]
    private IDictionary<string, IEntity> FetchEntityDictionary(
      [CanBeNull] string format) {
      var entityList = CreateEntityList();
      entityList.Populate();
      return (from IEntity entity in entityList
          select new KeyValuePair<string, IEntity>(GetKey(entity, format), entity)
        ).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    [CanBeNull]
    public static string GetKey([CanBeNull] object value, [CanBeNull] string format) {
      if (value == null) {
        return null;
      }
      // The only non-string key expected, which therefore needs to be converted
      // to a formatted string is Newsletter.Date.
      bool isDateKey = !string.IsNullOrWhiteSpace(format);
      if (isDateKey && value is Newsletter newsletter) {
        return newsletter.Date.ToString(format);
      }
      if (value is IEntity entity) {
        return entity.SimpleKey;
      }
      // After duplicate key error message shown on inserting event.
      // Is this a problem?
      return value.ToString();
      // Otherwise this is all that would be necessary:
      // return isDateKey
      //   ? ((Newsletter)value).Date.ToString(format)
      //   : ((IEntity)value).SimpleKey;
    }
  }
}