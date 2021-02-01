using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
  public abstract class BindingItemBase<TEntity, TBindingItem> :
    IBindingItem, INotifyPropertyChanged
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private IDictionary<string, PropertyInfo>? _entityProperties;
    private IDictionary<string, PropertyInfo>? _properties;
    private IEnumerable<PropertyInfo>? _visibleProperties;
    
    internal EntityListBase<TEntity, TBindingItem> EntityList { get; set; } = null!;

    private IDictionary<string, PropertyInfo> EntityProperties =>
      _entityProperties ??= CreatePropertyDictionary<TEntity>();

    private IDictionary<string, object>? EntityPropertyValues { get; set; }

    protected IDictionary<string, PropertyInfo> Properties =>
      _properties ??= CreatePropertyDictionary<TBindingItem>();

    private IEnumerable<PropertyInfo> VisibleProperties =>
      _visibleProperties ??= CreateVisibleProperties();

    public void SetPropertyValue(string propertyName, object? value) {
      try {
        Properties[propertyName].SetValue(this, value);
      } catch (TargetInvocationException exception) {
        throw exception.InnerException ?? exception;
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [ExcludeFromCodeCoverage]
    private void CheckIdentifyingParent(TEntity entity) {
      if (EntityList.IdentifyingParent != null && entity.IdentifyingParent == null) {
        throw new InvalidOperationException(
          "For a row of a main grid that has a parent grid, the derived class " +
          "must override BindingItemBase.CopyValuesToEntityProperties to set the " +
          "entity's identifying parent to the entity list's identifying parent before " +
          "calling the base method.");
      }
    }

    /// <summary>
    ///   Derived classes representing rows of main grids that have parent grids must
    ///   override this to set the entity's identifying parent to the entity list's
    ///   identifying parent before calling this base method. 
    /// </summary>
    protected virtual void CopyValuesToEntityProperties(TEntity entity) {
      CheckIdentifyingParent(entity);
      foreach (var property in VisibleProperties) {
        CopyValueToEntityProperty(property.Name, entity);
      }
    }

    private void CopyValueToEntityProperty(string propertyName,
      TEntity entity) {
      var entityProperty = EntityProperties[propertyName];
      var oldEntityPropertyValue = entityProperty.GetValue(entity);
      var newEntityPropertyValue = EntityPropertyValues![propertyName];
      if (oldEntityPropertyValue == null && newEntityPropertyValue == null) {
        return;
      }
      if (oldEntityPropertyValue == null ||
          !oldEntityPropertyValue.Equals(newEntityPropertyValue)) {
        SetEntityPropertyValue(entity, entityProperty, newEntityPropertyValue);
      }
    }

    internal TBindingItem CreateBackup() {
      var result = new TBindingItem {EntityList = EntityList};
      result.RestorePropertyValuesFrom((TBindingItem)this);
      return result;
    }

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

    private IDictionary<string, object> CreateEntityPropertyValueDictionary() {
      // Debug.WriteLine("BindingItemBase.CreateEntityPropertyValueDictionary");
      var result = new Dictionary<string, object>();
      foreach (var property in VisibleProperties) {
        result[property.Name] =
          GetEntityPropertyValue(property, EntityProperties[property.Name])!;
      }
      return result;
    }

    internal virtual Key CreateKey() {
      return new Key(GetSimpleKey(), EntityList.IdentifyingParent);
    }

    private static IDictionary<string, PropertyInfo> CreatePropertyDictionary<T>() {
      var properties = typeof(T).GetProperties().ToList();
      var result = new Dictionary<string, PropertyInfo>(properties.Count);
      foreach (var property in properties) {
        result.Add(property.Name, property);
      }
      return result;
    }

    private IEnumerable<PropertyInfo> CreateVisibleProperties() {
      return from property in Properties.Values
        where EntityList.Columns[property.Name].IsVisible
        select property;
    }

    protected IEntity? FindParent(PropertyInfo property) {
      var propertyValue = property.GetValue(this);
      return propertyValue != null
        ? EntityList.Columns[property.Name].ReferenceableItems.GetEntity(propertyValue)
        : null;
    }

    private object? GetEntityPropertyValue(PropertyInfo property,
      PropertyInfo entityProperty) {
      return entityProperty.PropertyType == property.PropertyType
        ? property.GetValue(this)
        : FindParent(property);
    }

    internal object? GetPropertyValue(string propertyName) {
      return Properties[propertyName].GetValue(this);
    }

    internal IList<object> GetPropertyValues() {
      return (
        from property in Properties.Values
        select property.GetValue(this)).ToList()!;
    }

    protected abstract string GetSimpleKey();

    internal void RestorePropertyValuesFrom(TBindingItem backup) {
      foreach (var property in Properties.Values) {
        SetPropertyValue(property.Name, backup.GetPropertyValue(property.Name)!);
      }
    }

    internal void RestoreToEntity(TEntity entity) {
      EntityPropertyValues = CreateEntityPropertyValueDictionary();
      CopyValuesToEntityProperties(entity);
    }

    private static void SetEntityPropertyValue(TEntity entity,
      PropertyInfo entityProperty, object? newEntityPropertyValue) {
      try {
        entityProperty.SetValue(entity, newEntityPropertyValue);
      } catch (TargetInvocationException exception) {
        throw exception.InnerException ?? exception;
      }
    }

    internal void UpdateEntityProperty(string propertyName, TEntity entity) {
      var entityProperty = EntityProperties[propertyName];
      var newEntityPropertyValue =
        GetEntityPropertyValue(Properties[propertyName], entityProperty);
      SetEntityPropertyValue(entity, entityProperty, newEntityPropertyValue);
    }
  }
}