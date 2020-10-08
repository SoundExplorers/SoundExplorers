﻿using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestEditor<TBindingItem> where TBindingItem : BindingItemBase {
    public TestEditor(IBindingList bindingList) {
      BindingList = (BindingList<TBindingItem>)bindingList;
    }

    private BindingList<TBindingItem> BindingList { get; }
    [NotNull] public TBindingItem this[int index] => BindingList[index];
    public int Count => BindingList.Count;

    /// <summary>
    ///   Emulates adding a new item to the grid via the insertion row.
    /// </summary>
    [NotNull]
    public TBindingItem AddNew() {
      return BindingList.AddNew() ?? throw new NullReferenceException();
    }
  }
}