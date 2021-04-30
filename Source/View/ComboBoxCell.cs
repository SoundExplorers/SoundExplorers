﻿using System;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   A DataGridView cell that supports editing via an embedded ComboBox control.
  /// </summary>
  internal class ComboBoxCell : DataGridViewTextBoxCell, IView<ComboBoxCellController>,
    ICanRestoreErrorValue {
    /// <summary>
    ///   Gets the cell's combo box.
    /// </summary>
    public ComboBox ComboBox => (ComboBox)DataGridView!.EditingControl;

    /// <summary>
    ///   Gets the type of the editing control that the ComboBoxCell is to use.
    /// </summary>
    public override Type EditType => typeof(DataGridViewComboBoxEditingControl);

    private ComboBoxCellController Controller => (ComboBoxCellController)Tag;

    public void RestoreErrorValue(object errorValue) {
      string key =
        ComboBoxCellController.GetKey(errorValue);
      int foundIndex = ComboBox.FindStringExact(key);
      ComboBox.SelectedIndex = foundIndex;
      if (ComboBox.SelectedIndex != foundIndex) {
        ComboBox.SelectedIndex = foundIndex;
      }
    }

    public void SetController(ComboBoxCellController controller) {
      Tag = controller;
    }

    /// <summary>
    ///   Creates a ComboBoxCell and its associated controller, as per the
    ///   Model-View-Controller design pattern, returning the view instance created. The
    ///   parameters are passed to the controller's constructor.
    /// </summary>
    /// <param name="mainGridController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    public static ComboBoxCell Create(MainGridController mainGridController,
      string columnName) {
      return (ComboBoxCell)ViewFactory.Create<ComboBoxCell, ComboBoxCellController>(
        mainGridController, columnName);
    }

    /// <summary>
    ///   Attaches and initializes the hosted drop-down list control.
    /// </summary>
    /// <param name="rowIndex">
    ///   The index of the row being edited.
    /// </param>
    /// <param name="initialFormattedValue">
    ///   The initial value to be displayed in the hosted drop-down list control.
    /// </param>
    /// <param name="dataGridViewCellStyle">
    ///   A cell style that is used to determine the appearance of the hosted control.
    ///   Not used in this case.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   Throw if the referenced table contains no rows that can be made available for
    ///   selection in the cell's drop-down list.
    /// </exception>
    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      //Debug.WriteLine($"ComboBoxCell.InitializeEditingControl: initialFormattedValue = '{initialFormattedValue}'"); 
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      ComboBox.FlatStyle = FlatStyle.Flat;
      ComboBox.Items.Clear();
      ComboBox.Items.AddRange(Controller.GetItems());
      ComboBox.DisplayMember = "Key";
      ComboBox.ValueMember = "Value";
      ComboBox.SelectedIndex =
        ComboBox.FindStringExact(initialFormattedValue.ToString());
    }
  } //End of class
} //End of namespace