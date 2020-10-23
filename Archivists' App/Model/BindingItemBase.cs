using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SoundExplorers.Data;

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
  public abstract class BindingItemBase<TBindingItem> : IBindingItem,
    INotifyPropertyChanged
    where TBindingItem : BindingItemBase<TBindingItem>, new() {
    protected BindingItemBase() {
      Parents = new Dictionary<string, IEntity>();
    }

    private IDictionary<string, IEntity> Parents { get; }

    public void SetParent(string propertyName, IEntity parent) {
      Parents[propertyName] = parent;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [CanBeNull]
    internal TParent GetParent<TParent>()
      where TParent : EntityBase {
      string key = typeof(TParent).Name;
      return Parents.ContainsKey(key) ? Parents[key] as TParent : null;
    }

    [NotNull]
    internal TBindingItem CreateBackup() {
      var result = new TBindingItem();
      result.RestorePropertyValuesFrom((TBindingItem)this);
      return result;
    }

    [NotNull]
    internal IList<object> GetPropertyValues() {
      return (
        from property in GetType().GetProperties()
        select property.GetValue(this)).ToList();
    }

    internal void RestorePropertyValuesFrom([NotNull] TBindingItem backup) {
      var backupPropertyValues = backup.GetPropertyValues();
      var properties = GetType().GetProperties().ToList();
      for (var i = 0; i < properties.Count; i++) {
        properties[i].SetValue(this, backupPropertyValues[i]);
      }
    }
  }
}