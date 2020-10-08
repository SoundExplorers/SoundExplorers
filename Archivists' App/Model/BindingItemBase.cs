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
  ///   WARNING: The order in which properties are declared in a derived class
  ///   determines the order in which the corresponding columns will be displayed
  ///   on the grid.  Resharper's code cleanup can sometimes change that order.
  ///   It has not done that so far in derived classes.  If it does, a workaround
  ///   will need to be found.
  ///   Hopefully marking the derived classes with <see cref="NoReorderAttribute"/>
  ///   will ensure the order does not change. 
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