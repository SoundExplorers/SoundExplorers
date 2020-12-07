using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   A strongly typed binding list to facilitate testing.
  /// </summary>
  public class TypedBindingList<TEntity, TBindingItem> : BindingList<TBindingItem>
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    public TypedBindingList([NotNull] IList<TBindingItem> bindingItems) : base(
      bindingItems) { }

    /// <summary>
    ///   A non-nullable replacement for <see cref="IBindingList.AddNew" />,
    ///   for ease of testing, as it should never be null.
    /// </summary>
    [NotNull]
    public new TBindingItem AddNew() {
      return base.AddNew() ?? throw new NullReferenceException();
    }
  }
}