using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Controller;

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
    [CanBeNull] public EditorController EditorController { get; set; }

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

    public void SetComboBoxCellValue(
      int rowIndex, [NotNull] string columnName, [NotNull] object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      this[rowIndex].SetPropertyValue(columnName, value);
      comboBoxCellController.OnCellValueChanged(0, value);
    }

    [NotNull]
    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      [NotNull] string columnName) {
      var dummy = EditorController ?? throw new NullReferenceException(
        "In TestEditor.ComboBoxCellController, " 
        + "the TestEditor's EditorController property has not been set.");
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, EditorController, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }
  }
}