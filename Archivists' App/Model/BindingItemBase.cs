using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   A derived class must call OnPropertyChanged in each of its property setters,
  ///   in order that the BindingList of which instantiations are items
  ///   will raise its ListChanged event
  ///   (with ListChangedEventArgs.ListChangedType == ListChangedType.ItemChanged)
  ///   when there there is a change to the value of a property of a new or existing item.
  /// </summary>
  /// <remarks>
  ///   IMPORTANT:  Each derived classes should be marked with
  ///   <see cref="NoReorderAttribute"/>.
  ///   This ought to ensure that the order in which properties are declared in
  ///   the derived class will not change when the Resharper code cleanup is run.
  ///   This is important because the property order
  ///   determines the order in which the corresponding columns will be displayed
  ///   on the grid.
  ///   Derived classes that were developed without <see cref="NoReorderAttribute"/>
  ///   did not actually have their property order changed by code cleanup,
  ///   presumably because their structure is so simple and, in particular,
  ///   they don't implement interfaces, which affect the code cleanup order.
  ///   Other derived classes are expected to be just as simple.
  ///   So the <see cref="NoReorderAttribute"/> is a safety feature.
  /// </remarks>
  public abstract class BindingItemBase : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(
      [CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}