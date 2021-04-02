using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Each derived class must call <see cref="OnPropertyChanged" />
  ///   in each of its property setters, in order that the BindingList of which
  ///   instantiations are items will raise its ListChanged event (with
  ///   ListChangedEventArgs.ListChangedType == ListChangedType.ItemChanged) when there
  ///   is a change to the value of a property of a new or existing item. Each derived
  ///   class should also be marked with <see cref="NoReorderAttribute" /> to ensure that
  ///   the order in which properties are declared in the derived class will not change
  ///   when the Resharper code cleanup is run.
  /// </summary>
  /// <remarks>
  ///   The <see cref="NoReorderAttribute" /> is important because the property order
  ///   determines the order in which the corresponding columns will be displayed
  ///   on the grid. Derived classes that were developed without
  ///   <see cref="NoReorderAttribute" />  did not actually have their property order
  ///   changed by code cleanup, presumably because their structure is so simple and, in
  ///   particular, they don't implement interfaces, which affect the code cleanup order.
  ///   Other derived classes are expected to be just as simple. So the
  ///   <see cref="NoReorderAttribute" /> is a safety feature.
  /// </remarks>
  public abstract class BindingItemBase<TEntity, TBindingItem> :
    IBindingItem, INotifyPropertyChanged
    where TEntity : EntityBase
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    private IDictionary<string, PropertyInfo>? _entityProperties;
    private Key? _key;
    private IDictionary<string, PropertyInfo>? _properties;
    internal EntityListBase<TEntity, TBindingItem> EntityList { get; set; } = null!;

    private IDictionary<string, PropertyInfo> EntityProperties =>
      _entityProperties ??= new PropertyDictionary(typeof(TEntity));

    private IDictionary<string, object?>? EntityPropertyValues { get; set; }

    internal Key Key {
      get => _key ??= CreateKey();
      private set => _key = value;
    }

    protected IDictionary<string, PropertyInfo> Properties =>
      _properties ??= new PropertyDictionary(typeof(TBindingItem));

    // Must be explicit interface implementation rather than public. Otherwise it would
    // appear as a column on the grid!
    IDictionary<string, PropertyInfo> IBindingItem.Properties => Properties;

    public object? GetPropertyValue(string propertyName) {
      return Properties[propertyName].GetValue(this);
    }

    public void SetPropertyValue(string propertyName, object? value) {
      DoSetPropertyValue(() => {
        var property = Properties[propertyName];
        property.SetValue(this, value);
      });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal TEntity CreateEntity() {
      EntityPropertyValues = CreateEntityPropertyValueDictionary();
      var result =
        (Activator.CreateInstance(typeof(TEntity), EntityList.Root) as TEntity)!;
      CopyValuesToEntityProperties(result);
      return result;
    }

    internal static string GetDefaultIntegerSimpleKey(
      IList<string> existingSimpleKeys) {
      return existingSimpleKeys.Count > 0
        ? ((from existingSimpleKey in existingSimpleKeys
          select SimpleKeyToInteger(existingSimpleKey,
            string.Empty, true)).Max() + 1).ToString()
        : "1";
    }

    internal static int SimpleKeyToInteger(string simpleKey, string simpleKeyName,
      bool minusOneIfError = false) {
      // Validate format
      int result = ToIntegerOrIfErrorMinusOne(simpleKey);
      bool isFormatValid = result != -1;
      if (!isFormatValid) {
        if (minusOneIfError) {
          return -1;
        }
        throw new PropertyConstraintException(
          EntityBase.GetIntegerSimpleKeyErrorMessage(simpleKeyName), simpleKeyName);
      }
      // Validate range
      bool emptyIfError = minusOneIfError;
      string checkedSimpleKey = EntityBase.IntegerToSimpleKey(
        result, simpleKeyName, emptyIfError);
      if (minusOneIfError && string.IsNullOrWhiteSpace(checkedSimpleKey)) {
        return -1;
      }
      return result;
    }

    internal void UpdateEntityProperty(string propertyName, TEntity entity) {
      var entityProperty = EntityProperties[propertyName];
      var newEntityPropertyValue =
        GetEntityPropertyValue(Properties[propertyName], entityProperty);
      SetEntityPropertyValue(entity, entityProperty, newEntityPropertyValue);
    }

    /// <summary>
    ///   Derived classes that represent items on child lists must override this method
    ///   to implement any entity type-specific validation that needs to be done at
    ///   insertion time against other entities in the entity list or related entities.
    ///   This prevents an InvalidOperationException from being thrown in
    ///   EntityBase.UpdateNonIndexField, which happens if other entities are repeatedly
    ///   retrieved from the database while an insertion is in progress.
    /// </summary>
    /// <remarks>
    ///   Though we are duplicating entity-level validation here, the entity-level
    ///   validation should be retained as a last-resort defence against corrupting the
    ///   database.
    /// </remarks>
    internal virtual void ValidateInsertion() {
      CheckForDuplicateKey();
      foreach (var column in EntityList.Columns.Where(column =>
        column.ReferencesAnotherEntity)) {
        CheckForReferencedEntityNotFound(column);
      }
    }

    /// <summary>
    ///   Derived classes that represent items on child lists must override this method
    ///   to implement any entity type-specific validation that needs to be done at
    ///   existing entity property update time against other entities on the entity list
    ///   or related entities. This prevents an InvalidOperationException from being
    ///   thrown in EntityBase.UpdateNonIndexField, which happens if other entities are
    ///   repeatedly retrieved from the database while an update is in progress.
    /// </summary>
    /// <remarks>
    ///   Though we are duplicating entity-level validation here, the entity-level
    ///   validation should be retained as a last-resort defence against corrupting the
    ///   database.
    /// </remarks>
    internal virtual void ValidatePropertyUpdate(
      string propertyName, TEntity entity) {
      var column = EntityList.Columns[propertyName];
      if (column.IsInKey) {
        CheckForDuplicateKey();
      }
      if (column.ReferencesAnotherEntity) {
        CheckForReferencedEntityNotFound(column);
      }
    }

    protected virtual Key CreateKey() {
      return new Key(GetSimpleKey(), EntityList.IdentifyingParent);
    }

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///   A derived class representing a row of a main grid that is a child of a parent
    ///   grid row must override this method.
    /// </summary>
    /// <remarks>
    ///   I'M NOT SURE HOW ACCURATE THIS IS, AS I NOW CANNOT REVERT CODE TO REPRODUCE THE
    ///   EXACT EXCEPTION
    ///   If referencing properties are not set in the right order in the overriding
    ///   methods, Session.Commit will eventually throw an
    ///   <see cref="InvalidOperationException" /> as a result of handling a
    ///   <see cref="NullReferenceException" /> thrown by VelocityDB's internal
    ///   SessionBase.FlushUpdates() method within SessionBase.AllObjects().
    /// </remarks>
    protected virtual void CopyValuesToEntityProperties(TEntity entity) {
      foreach (var property in Properties.Values) {
        CopyValueToEntityProperty(property.Name, entity);
      }
    }

    protected virtual IDictionary<string, object?> CreateEntityPropertyValueDictionary() {
      // Debug.WriteLine("BindingItemBase.CreateEntityPropertyValueDictionary");
      var result = new Dictionary<string, object?>();
      foreach (var property in Properties.Values) {
        result[property.Name] =
          GetEntityPropertyValue(property, EntityProperties[property.Name])!;
      }
      return result;
    }

    protected IEntity? FindParent(PropertyInfo property) {
      var propertyValue = property.GetValue(this);
      if (propertyValue == null) {
        return null;
      }
      var column = EntityList.Columns[property.Name];
      var referenceableItems = column.ReferenceableItems!;
      var entity = referenceableItems.GetEntity(propertyValue);
      return entity;
    }

    protected virtual object? GetEntityPropertyValue(PropertyInfo property,
      PropertyInfo entityProperty) {
      return entityProperty.PropertyType == property.PropertyType
        ? property.GetValue(this)
        : FindParent(property);
    }

    protected abstract string GetSimpleKey();

    [ExcludeFromCodeCoverage]
    private static void DoSetPropertyValue(Action action) {
      try {
        action.Invoke();
        // The only way to get a detailed diagnostic trace for a crash on
        // PropertyInfo.SetValue is to explicitly set the property's value rather than
        // via SetValue, e.g. by overriding BackupItem.RestoreTo.
      } catch (TargetInvocationException exception) {
        throw exception.InnerException ?? exception;
      }
    }

    private static void SetEntityPropertyValue(TEntity entity,
      PropertyInfo entityProperty, object? newEntityPropertyValue) {
      try {
        entityProperty.SetValue(entity, newEntityPropertyValue);
      } catch (TargetInvocationException exception) {
        // The only way to get a detailed diagnostic trace for a crash on
        // PropertyInfo.SetValue is to explicitly set the property's value rather than
        // via SetValue, e.g. by overriding CopyValuesToEntityProperties. 
        throw exception.InnerException ?? exception;
      }
    }

    private static int ToIntegerOrIfErrorMinusOne(string text) {
      bool isValid = int.TryParse(text.Trim(), out int result);
      return isValid ? result : -1;
    }

    /// <summary>
    ///   There are also checks for duplicate keys in <see cref="EntityBase" /> in the
    ///   Data layer. This method preempts those by searching application data, on hand
    ///   in memory, rather than the database. So it is hoped that it will turn out to be
    ///   quicker with large volumes of data.
    /// </summary>
    /// <remarks>
    ///   This duplicate key check gives a standard error message, while there are more
    ///   varied duplicate key error message in the Data layer, reflecting top-level vs
    ///   child and insertion vs update. I did try to replicate those more varied error
    ///   messages here. But the extra complexity caused problems. And I think the more
    ///   general error message generated here is fine as it is. So I don't think it is
    ///   worth the hassle of making that change.
    /// </remarks>
    private void CheckForDuplicateKey() {
      Key = CreateKey();
      // Entity list could be a sorted list. Duplicate check might be faster. But it
      // would be a big job to do and I don't think there will be a performance problem.
      if ((from otherEntity in EntityList
        where otherEntity.Key == Key
        select otherEntity).Any()) {
        string message =
          $"Another {EntityList.EntityTypeName} with key '{Key}' already exists.";
        throw new DuplicateNameException(message);
      }
    }

    /// <summary>
    ///   Check that combo box cell value on the main grid matches one of its embedded
    ///   combo box's items. If not, the combo box's selected index and text cannot not
    ///   be updated. As the combo boxes are all dropdown lists, the only way there can
    ///   be a mismatch is when an unmatched value is pasted into the cell. If the cell
    ///   value is changed by selecting an item on the embedded combo box, it can only be
    ///   a matching value.
    /// </summary>
    private void CheckForReferencedEntityNotFound(BindingColumn column) {
      var value = GetPropertyValue(column.PropertyName);
      // It is impossible to paste null, so, when we detect it here, it must be a
      // programming artefact. And a null cell value has only been found when we can
      // ignore it: on pasting an invalid format date into Event.Newsletter on the
      // insertion row when there are no existing rows and Location has not been
      // specified. A format error message will be shown for the invalid Newsletter in
      // MainGridController.OnCellEditException.
      if (value != null) {
        string simpleKey = ReferenceableItemList.ToSimpleKey(value)!;
        if (!column.ReferenceableItems!.ContainsKey(simpleKey)) {
          string message =
            $"{column.PropertyName} not found: " +
            $"'{ReferenceableItemList.Format(simpleKey)}'";
          throw new RowNotInTableException(message);
        }
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
  }
}