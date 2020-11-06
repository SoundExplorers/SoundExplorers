using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Each derived class must call OnPropertyChanged in each of its property setters,
  ///   in order that the BindingList of which instantiations are items
  ///   will raise its ListChanged event
  ///   (with ListChangedEventArgs.ListChangedType == ListChangedType.ItemChanged)
  ///   when there there is a change to the value of a property of a new or existing item.
  ///   Each derived class should also be marked with
  ///   <see cref="NoReorderAttribute" /> to ensure that the order in which properties
  ///   are declared in the derived class will not change
  ///   when the Resharper code cleanup is run.
  /// </summary>
  /// <remarks>
  ///   The <see cref="NoReorderAttribute" /> is important because the property order
  ///   determines the order in which the corresponding columns will be displayed
  ///   on the grid.
  ///   Derived classes that were developed without <see cref="NoReorderAttribute" />
  ///   did not actually have their property order changed by code cleanup,
  ///   presumably because their structure is so simple and, in particular,
  ///   they don't implement interfaces, which affect the code cleanup order.
  ///   Other derived classes are expected to be just as simple.
  ///   So the <see cref="NoReorderAttribute" /> is a safety feature.
  /// </remarks>
  public abstract class BindingItemBase<TEntity, TBindingItem> : IBindingItem,
    INotifyPropertyChanged
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private IDictionary<string, IEntity> _parents;
    private IDictionary<string, PropertyInfo> _properties;
    private QueryHelper _queryHelper;
    private SessionBase _session;

    private IDictionary<string, IEntity> Parents =>
      _parents ?? (_parents = new Dictionary<string, IEntity>());

    private IDictionary<string, PropertyInfo> Properties =>
      _properties ?? (_properties = CreatePropertyDictionary());

    /// <summary>
    ///   The setter should only be needed for testing.
    /// </summary>
    [NotNull]
    internal QueryHelper QueryHelper {
      get => _queryHelper ?? (_queryHelper = QueryHelper.Instance);
      set => _queryHelper = value;
    }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    public void SetParent(string propertyName, IEntity parent) {
      Parents[propertyName] = parent;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [NotNull]
    internal TBindingItem CreateBackup() {
      var result = new TBindingItem();
      result.RestorePropertyValuesFrom((TBindingItem)this);
      return result;
    }

    private void CopyPropertyValueToEntity(PropertyInfo property, EntityBase entity,
      IEnumerable<PropertyInfo> entityProperties) {
      var entityProperty = GetMatchingEntityProperty(property, entityProperties);
      var propertyValue = property.GetValue(this);
      object entityPropertyValue;
      if (entityProperty.PropertyType == property.PropertyType) {
        entityPropertyValue = propertyValue;
      } else {
        entityPropertyValue = Parents.ContainsKey(property.Name)
          ? Parents[property.Name]
          : null;
      }
      try {
        entityProperty.SetValue(entity, entityPropertyValue);
      } catch (TargetInvocationException exception) {
        throw exception.InnerException ?? exception;
      }
    }

    internal void CopyPropertyValuesToEntity([NotNull] EntityBase entity) {
      var entityProperties = entity.GetType().GetProperties();
      foreach (var property in Properties.Values) {
        CopyPropertyValueToEntity(property, entity, entityProperties);
      }
    }

    [NotNull]
    internal TEntity CreateEntity() {
      SetParentsToDefaultIfNotSpecified();
      var result = new TEntity();
      CopyPropertyValuesToEntity(result);
      return result;
    }

    [NotNull]
    private IDictionary<string, PropertyInfo> CreatePropertyDictionary() {
      var properties = GetType().GetProperties().ToList();
      var result = new Dictionary<string, PropertyInfo>(properties.Count);
      foreach (var property in properties) {
        result.Add(property.Name, property);
      }
      return result;
    }

    /// <summary>
    ///   Derived classes should override this when a parent cell in a new row
    ///   has been defaulted to a specific referenced entity.
    /// </summary>
    /// <returns>
    ///   A dictionary with the parent property name as the key
    ///   and the default parent simple key as the value.
    ///   If not overridden, an empty dictionary.
    /// </returns>
    [NotNull]
    protected virtual IDictionary<string, string> GetDefaultParentSimpleKeys() {
      return new Dictionary<string, string>();
    }

    [NotNull]
    private static PropertyInfo GetMatchingEntityProperty([NotNull] PropertyInfo property,
      [NotNull] IEnumerable<PropertyInfo> entityProperties) {
      return (
        from entityProperty in entityProperties
        where entityProperty.Name == property.Name
        select entityProperty).First();
    }

    [NotNull]
    private object GetPropertyValue([NotNull] PropertyInfo property) {
      return property.GetValue(this);
    }

    [NotNull]
    internal IList<object> GetPropertyValues() {
      return (
        from property in Properties.Values
        select property.GetValue(this)).ToList();
    }

    internal void RestorePropertyValuesFrom([NotNull] TBindingItem backup) {
      foreach (var property in Properties.Values) {
        property.SetValue(this, backup.GetPropertyValue(property));
      }
    }

    private void SetParentsToDefaultIfNotSpecified() {
      var dictionary = GetDefaultParentSimpleKeys();
      foreach (var kvp in dictionary) {
        SetParentToDefaultIfNotSpecified(kvp.Key, kvp.Value);
      }
    }

    private void SetParentToDefaultIfNotSpecified([NotNull] string propertyName,
      [NotNull] string defaultParentSimpleKey) {
      if (!Parents.ContainsKey(propertyName)) {
        SetParent(propertyName,
          QueryHelper.Find<EventType>(defaultParentSimpleKey, Session));
      }
    }

    internal void UpdateEntityProperty(
      [NotNull] string propertyName, [NotNull] TEntity entity) {
      var entityProperties = entity.GetType().GetProperties();
      CopyPropertyValueToEntity(Properties[propertyName], entity, entityProperties);
    }
  }
}