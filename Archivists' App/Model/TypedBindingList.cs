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

    /// <summary>
    ///   A non-nullable replacement for <see cref="IBindingList.AddNew" />,
    ///   for ease of testing, as it should never be null.
    /// </summary>
    public new TBindingItem AddNew() {
      return base.AddNew()!;
    }

    /// <summary>
    ///   Used for restoring error values to the new row for correction or edit
    ///   cancellation after an insertion error message hase been shown.
    /// </summary>
    internal TBindingItem? InsertionErrorItem { get; set; }

    protected override void OnAddingNew(AddingNewEventArgs e) {
      if (InsertionErrorItem != null) {
        e.NewObject = InsertionErrorItem;      
      }
      base.OnAddingNew(e);
      InsertionErrorItem = null;
    }
  }
}