﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
  public abstract class BindingItemBase<TEntity, TBindingItem> : INotifyPropertyChanged
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private IDictionary<string, PropertyInfo> _entityProperties;
    private IDictionary<string, PropertyInfo> _properties;
    private QueryHelper _queryHelper;
    private SessionBase _session;

    private IDictionary<string, PropertyInfo> EntityProperties =>
      _entityProperties ?? (_entityProperties = CreatePropertyDictionary<TEntity>());

    private IDictionary<string, object> EntityPropertyValues { get; set; }

    private IDictionary<string, PropertyInfo> Properties =>
      _properties ?? (_properties = CreatePropertyDictionary<TBindingItem>());

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

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CopyValuesToEntityProperties([NotNull] TEntity entity) {
      foreach (var property in Properties.Values) {
        CopyValueToEntityProperty(property, entity);
      }
    }

    private void CopyValueToEntityProperty([NotNull] PropertyInfo property,
      [NotNull] TEntity entity) {
      var entityProperty = EntityProperties[property.Name];
      var oldEntityPropertyValue = entityProperty.GetValue(entity);
      var newEntityPropertyValue = EntityPropertyValues[property.Name];
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
      var result = new TBindingItem();
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
    private IEntity FindParent([NotNull] PropertyInfo property,
      [NotNull] Type parentType) {
      var propertyValue = property.GetValue(this);
      if (propertyValue == null) {
        return null;
      }
      string parentSimpleKey;
      if (property.PropertyType == typeof(DateTime)) {
        var date = (DateTime)propertyValue;
        if (date > EntityBase.InitialDate) {
          parentSimpleKey = EntityBase.DateToSimpleKey(date);
        } else {
          return null;
        }
      } else { // string
        parentSimpleKey = propertyValue.ToString();
      }
      return QueryHelper.FindTopLevelEntity(parentType, parentSimpleKey, Session);
    }

    [CanBeNull]
    private object GetEntityPropertyValue([NotNull] PropertyInfo property,
      [NotNull] PropertyInfo entityProperty) {
      return entityProperty.PropertyType == property.PropertyType
        ? property.GetValue(this)
        : FindParent(property, entityProperty.PropertyType);
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
      } catch (Exception exception) {
        throw new PropertyConstraintException(
          $"Failed to set {typeof(TEntity).Name}.{entityProperty.Name} "
          + $"to '{newEntityPropertyValue}':"
          + Environment.NewLine + $"{exception.Message}",
          entityProperty.Name, exception);
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