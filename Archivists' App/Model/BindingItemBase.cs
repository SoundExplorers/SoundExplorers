using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Each derived class must call <see cref="OnPropertyChanged" />
  ///   in each of its property setters,
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
  public abstract class BindingItemBase<TEntity, TBindingItem> : INotifyPropertyChanged
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private IDictionary<string, PropertyInfo> _entityProperties;
    private IDictionary<string, PropertyInfo> _properties;

    internal BindingColumnList Columns { get; set; }

    private IDictionary<string, PropertyInfo> EntityProperties =>
      _entityProperties ?? (_entityProperties = CreatePropertyDictionary<TEntity>());

    private IDictionary<string, object> EntityPropertyValues { get; set; }

    internal IDictionary<string, PropertyInfo> Properties =>
      _properties ?? (_properties = CreatePropertyDictionary<TBindingItem>());

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CopyValuesToEntityProperties([NotNull] TEntity entity) {
      foreach (var property in Properties.Values) {
        CopyValueToEntityProperty(property.Name, entity);
      }
    }

    private void CopyValueToEntityProperty([NotNull] string propertyName,
      [NotNull] TEntity entity) {
      var entityProperty = EntityProperties[propertyName];
      var oldEntityPropertyValue = entityProperty.GetValue(entity);
      var newEntityPropertyValue = EntityPropertyValues[propertyName];
      if (oldEntityPropertyValue == null && newEntityPropertyValue == null) {
        return;
      }
      if (oldEntityPropertyValue == null ||
          !oldEntityPropertyValue.Equals(newEntityPropertyValue)) {
        SetEntityPropertyValue(entity, entityProperty, newEntityPropertyValue);
      }
    }

    [NotNull]
    internal TBindingItem CreateBackup() {
      var result = new TBindingItem {Columns = Columns};
      result.RestorePropertyValuesFrom((TBindingItem)this);
      return result;
    }

    [NotNull]
    internal TEntity CreateEntity() {
      // Fetch any parent reference values from the database
      // before instantiating the entity.
      // After the entity has been instantiated,
      // set its properties to the corresponding values that have
      // been fetched in advance.
      // Otherwise, i.e. if we were to attempt to
      // fetch the parent reference values from the database
      // after the entity had been instantiated, 
      // in QueryHelper, SessionBase.AllObjects would invoke SessionBase.FlushUpdates,
      // which prematurely attempts to persist the new entity before all its properties 
      // have been set.  For Event and probably some other entity types,
      // that causes a persistence failure.
      EntityPropertyValues = CreateEntityPropertyValueDictionary();
      var result = new TEntity();
      CopyValuesToEntityProperties(result);
      return result;
    }

    [NotNull]
    private IDictionary<string, object> CreateEntityPropertyValueDictionary() {
      var result = new Dictionary<string, object>();
      foreach (var property in Properties.Values) {
        result[property.Name] =
          GetEntityPropertyValue(property, EntityProperties[property.Name]);
      }
      return result;
    }

    [NotNull]
    private static IDictionary<string, PropertyInfo> CreatePropertyDictionary<T>() {
      var properties = typeof(T).GetProperties().ToList();
      var result = new Dictionary<string, PropertyInfo>(properties.Count);
      foreach (var property in properties) {
        result.Add(property.Name, property);
      }
      return result;
    }

    [CanBeNull]
    private IEntity FindParent([NotNull] PropertyInfo property) {
      var propertyValue = property.GetValue(this);
      return propertyValue != null
        ? Columns[property.Name].ReferenceableItems.GetEntity(propertyValue)
        : null;
    }

    [CanBeNull]
    private object GetEntityPropertyValue([NotNull] PropertyInfo property,
      [NotNull] PropertyInfo entityProperty) {
      return entityProperty.PropertyType == property.PropertyType
        ? property.GetValue(this)
        : FindParent(property);
    }

    [NotNull]
    internal IList<object> GetPropertyValues() {
      return (
        from property in Properties.Values
        select property.GetValue(this)).ToList();
    }

    internal void RestorePropertyValuesFrom([NotNull] TBindingItem backup) {
      foreach (var property in Properties.Values) {
        property.SetValue(this, property.GetValue(backup));
      }
    }

    internal void RestoreToEntity([NotNull] TEntity entity) {
      EntityPropertyValues = CreateEntityPropertyValueDictionary();
      CopyValuesToEntityProperties(entity);
    }

    private static void SetEntityPropertyValue([NotNull] TEntity entity,
      [NotNull] PropertyInfo entityProperty, [CanBeNull] object newEntityPropertyValue) {
      try {
        entityProperty.SetValue(entity, newEntityPropertyValue);
      } catch (TargetInvocationException exception) {
        throw exception.InnerException ?? exception;
      }
    }

    internal void UpdateEntityProperty(
      [NotNull] string propertyName, [NotNull] TEntity entity) {
      var entityProperty = EntityProperties[propertyName];
      var newEntityPropertyValue =
        GetEntityPropertyValue(Properties[propertyName], entityProperty);
      SetEntityPropertyValue(entity, entityProperty, newEntityPropertyValue);
    }
  }
}