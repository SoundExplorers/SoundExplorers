using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   A DataGridView cell that supports editing via an embedded ComboBox control.
  /// </summary>
  /// <remarks>
  ///   The cell displays dates in ordinary text box cells,
  ///   but when the user edits a cell, a ComboBox control appears.
  ///   TODO: Check that DataGridViewComboBoxEditingControl works in place of ComboBoxEditingControl.
  /// </remarks>
  internal class ComboBoxCell : DataGridViewTextBoxCell, IView<ComboBoxCellController>,
    ICanRestoreErrorValue {
    /// <summary>
    ///   Gets the type of the editing control that the ComboBoxCell is to use.
    /// </summary>
    public override Type EditType => typeof(DataGridViewComboBoxEditingControl);
    // public override Type EditType => typeof(ComboBoxEditingControl);

    /// <summary>
    ///   Gets the cell's combo box.
    /// </summary>
    private ComboBox ComboBox => (ComboBox)DataGridView.EditingControl;

    private ComboBoxCellController Controller => (ComboBoxCellController)Tag;

    public void RestoreErrorValue(object errorValue) {
      ComboBox.SelectedIndex = ComboBox.FindStringExact(
        ComboBoxCellController.GetKey(errorValue, OwningColumn.DefaultCellStyle.Format));
    }

    public void SetController(ComboBoxCellController controller) {
      Tag = controller;
    }

    /// <summary>
    ///   Creates a ComboBoxCell and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    ///   The parameters are passed to the controller's constructor.
    /// </summary>
    /// <param name="tableController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    [NotNull]
    public static ComboBoxCell Create([NotNull] TableController tableController,
      [NotNull] string columnName) {
      return (ComboBoxCell)ViewFactory.Create<ComboBoxCell, ComboBoxCellController>(
        tableController, columnName);
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
    ///   Not used in this case
    /// </param>
    /// <exception cref="ApplicationException">
    ///   Throw if the referenced table contains no rows that
    ///   can be made available for selection in the cell's drop-down list.
    /// </exception>
    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      // Debug.WriteLine(
      //   $"ComboBoxCell.InitializeEditingControl: rowIndex = {rowIndex}; initialFormattedValue = {initialFormattedValue}");
      ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      ComboBox.Items.Clear();
      ComboBox.Items.AddRange(
        Controller.FetchItems(OwningColumn.DefaultCellStyle.Format));
      ComboBox.DisplayMember = "Key";
      ComboBox.ValueMember = "Value";
      ComboBox.SelectedIndex =
        ComboBox.FindStringExact(initialFormattedValue.ToString());
      ComboBox.SelectedIndexChanged += ComboBoxOnSelectedIndexChanged;
    }

    private void ComboBoxOnSelectedIndexChanged(object sender, EventArgs e) {
      // Debug.WriteLine(
      //   $"ComboBoxCell.ComboBoxOnSelectedIndexChanged: Text = {ComboBox.Text}; SelectedValue = {ComboBox.SelectedValue}");
      // Makes InitializeEditingControl reenter:
      //DataGridView.CurrentCell.Value = ComboBox.Text;
      Controller.OnSelectedIndexChanged(RowIndex, ComboBox.SelectedItem);
      // For unknown reason, SelectedValue is always null. So this does not work:
      //Controller.OnSelectedIndexChanged(RowIndex, ComboBox.SelectedValue);
    }

    // /// <summary>
    // ///   Populates the drop-down list of dates with the
    // ///   available values of the date key column of the referenced table.
    // /// </summary>
    // /// <param name="initialFormattedDate">
    // ///   The formatted date to be initially selected in the drop-down list of dates.
    // /// </param>
    // /// <param name="referencedBindingList">
    // ///   The referenced table that will be used to populate the date list.
    // /// </param>
    // /// <remarks>
    // ///   If the cell does not already contain a date:
    // ///   01 Jan 1900, the standard default date,
    // ///   will be initially selected in the list if
    // ///   that date is available in the referenced table;
    // ///   otherwise the latest available date
    // ///   will be initially selected.
    // /// </remarks>
    // private void PopulateDateDropDownList(
    //   string initialFormattedDate, IBindingList referencedBindingList) {
    //   // var dictionary =
    //   //   new Dictionary<string, DateTime>(referencedBindingList.Count);
    //   // // Some of this should be in the controller.
    //   // foreach (var row in referencedBindingList) {
    //   //   dictionary.Add(
    //   //     ((DateTime)row[Controller.ReferencedColumnName]).ToString(
    //   //       OwningColumn.DefaultCellStyle.Format),
    //   //     (DateTime)row[Controller.ReferencedColumnName]);
    //   // } //End of foreach
    //   // ComboBox.DataSource =
    //   //   new BindingSource(dictionary, null);
    //   // ComboBox.DisplayMember = "Key";
    //   // ComboBox.ValueMember = "Value";
    //   ComboBox.SelectedIndex = ComboBox.FindStringExact(initialFormattedDate);
    //   if (ComboBox.SelectedIndex == -1) {
    //     // var firstDate =
    //     //   (DateTime)referencedBindingList.Rows[0][
    //     //     Controller.ReferencedColumnName];
    //     // if (firstDate == DateTime.Parse("01 Jan 1900")) {
    //     //   ComboBox.SelectedIndex = 0;
    //     //   if (string.IsNullOrWhiteSpace(Value.ToString())) {
    //     //     Value = firstDate;
    //     //   }
    //     // } else {
    //     //   var lastDate =
    //     //     (DateTime)referencedBindingList.Rows[referencedBindingList.Rows.Count - 1][
    //     //       Controller.ReferencedColumnName];
    //     //   ComboBox.SelectedIndex = referencedBindingList.Count - 1;
    //     //   if (string.IsNullOrWhiteSpace(Value.ToString())) {
    //     //     Value = lastDate;
    //     //   }
    //     // }
    //   }
    // }

    // /// <summary>
    // ///   Throws an <see cref="ApplicationException" /> to indicate
    // ///   that the referenced table contains no rows that
    // ///   can be made available for selection in the cell's drop-down list.
    // /// </summary>
    // /// <param name="parentColumnName">
    // ///   For possible future with images.
    // ///   The name of the parent column, if any.
    // ///   This is the grid column whose value in the current row
    // ///   determines which rows of the current column's referenced table
    // ///   can be made available for selection.
    // ///   Null if the grid contains no such parent column.
    // /// </param>
    // /// <param name="parentColumnValue">
    // ///   For possible future with images.
    // ///   If the current column has a parent column,
    // ///   the value of the parent cell in the current row.
    // ///   Otherwise null.
    // /// </param>
    // /// <exception cref="ApplicationException">
    // ///   The required application exception.
    // /// </exception>
    // /// <remarks>
    // ///   When this exception is thrown,
    // ///   the DataGridView.DataError event,
    // ///   which is handled by TableView.MainGrid_DataError,
    // ///   gets raised.
    // /// </remarks>
    // private void ThrowNoAvailableReferencesException(string parentColumnName = null,
    //   object parentColumnValue = null) {
    //   string message =
    //     "There are no " + Controller.ReferencedTableName
    //                     + " " + Controller.ReferencedColumnName + "s ";
    //   if (parentColumnName != null) {
    //     message +=
    //       "for " + parentColumnName
    //              + " \"" + parentColumnValue + "\" ";
    //   }
    //   message +=
    //     "to choose between.  You need to insert at least one row into the "
    //     + Controller.ReferencedTableName + " table ";
    //   if (parentColumnName != null) {
    //     message +=
    //       "for " + parentColumnName
    //              + " \"" + parentColumnValue + "\" ";
    //   }
    //   message +=
    //     "before you can add rows to the "
    //     + Controller.TableName + " table";
    //   // For possible future with images.
    //   if (parentColumnName != null) {
    //     message +=
    //       " for " + parentColumnName
    //               + " \"" + parentColumnValue + "\"";
    //   }
    //   message += ".";
    //   throw new ApplicationException(message);
    // }
  } //End of class
} //End of namespace