using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestEditor<TEntity, TBindingItem>
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    public TestEditor(IBindingList bindingList = null) {
      SetBindingList(bindingList);
    }

    private BindingList<TBindingItem> BindingList { get; set; }
    [NotNull] public TBindingItem this[int index] => BindingList[index];
    public int Count => BindingList.Count;

    /// <summary>
    ///   Emulates adding a new item to the grid via the insertion row.
    /// </summary>
    [NotNull]
    public TBindingItem AddNew() {
      return BindingList.AddNew() ?? throw new NullReferenceException();
    }

    public void SetBindingList(IBindingList bindingList) {
      BindingList = (BindingList<TBindingItem>)bindingList;
    }
  }
}