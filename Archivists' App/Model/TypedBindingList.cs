using System.Collections.Generic;
using System.ComponentModel;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   A strongly typed binding list to facilitate testing.
  /// </summary>
  public class TypedBindingList<TEntity, TBindingItem> : BindingList<TBindingItem>
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    public TypedBindingList(IList<TBindingItem> bindingItems) : base(
      bindingItems) { }
  }
}