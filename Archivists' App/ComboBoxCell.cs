using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers {
  /// <summary>
  ///   A DataGridView cell that supports editing via an embedded ComboBox control.
  /// </summary>
  /// <remarks>
  ///   The cell displays dates in ordinary text box cells,
  ///   but when the user edits a cell, a ComboBox control appears.
  /// </remarks>
  internal class ComboBoxCell : DataGridViewTextBoxCell, IView<ComboBoxCellController> {
    /// <summary>
    ///   Gets the cell's combo box.
    /// </summary>
    private ComboBoxEditingControl ComboBox =>
      DataGridView.EditingControl as ComboBoxEditingControl;

    private ComboBoxCellController Controller => (ComboBoxCellController)Tag; 

    public override Type EditType =>
      // Return the type of the editing control that ComboBoxCell uses.
      typeof(ComboBoxEditingControl);

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
      FetchListData(initialFormattedValue);
    }

    private void FetchListData(object initialFormattedValue) {
      var referencedTable = Controller.FetchReferencedTable();
      // string parentColumnName = null;
      // object parentColumnValue = null;
      // if (Column.TableName == "Image"
      //     && Column.ColumnName == "Date") {
      //   parentColumnName = "Location";
      //   parentColumnValue = OwningRow.Cells[parentColumnName].Value; // Location
      //   referencedEntities = Factory<IEntityList>.Create(
      //     Column.ReferencedTableName, // Table name:  Performance in this case
      //     // Constructor arguments
      //     null, // parentListType
      //     parentColumnValue); // location
      // } else {
      //   referencedEntities =
      //     Factory<IEntityList>.Create(Column.ReferencedTableName);
      // }
      if (ValueType == typeof(string)) {
        ComboBox.DataSource = referencedTable.DefaultView;
        ComboBox.DisplayMember = Controller.ReferencedColumnName;
        ComboBox.ValueMember = Controller.ReferencedColumnName;
        ComboBox.SelectedIndex =
          ComboBox.FindStringExact(initialFormattedValue.ToString());
      } else { // DateTime
        if (referencedTable.Rows.Count > 0) {
          PopulateDateDropDownList(
            initialFormattedValue.ToString(),
            referencedTable);
        } else {
          ComboBox.DataSource = null;
          ThrowNoAvailableReferencesException();
          //ThrowNoAvailableReferencesException(parentColumnName, parentColumnValue);
        }
      }
    }

    /// <summary>
    ///   Populates the drop-down list of dates with the
    ///   available values of the date key column of the referenced table.
    /// </summary>
    /// <param name="initialFormattedDate">
    ///   The formatted date to be initially selected in the drop-down list of dates.
    /// </param>
    /// <param name="referencedTable">
    ///   The referenced table that will be used to populate the date list.
    /// </param>
    /// <remarks>
    ///   If the cell does not already contain a date:
    ///   01 Jan 1900, the standard default date,
    ///   will be initially selected in the list if
    ///   that date is available in the referenced table;
    ///   otherwise the latest available date
    ///   will be initially selected.
    /// </remarks>
    private void PopulateDateDropDownList(
      string initialFormattedDate, DataTable referencedTable) {
      var dictionary =
        new Dictionary<string, DateTime>(referencedTable.Rows.Count);
      foreach (DataRow row in referencedTable.Rows) {
        dictionary.Add(
          ((DateTime)row[Controller.ReferencedColumnName]).ToString(
            OwningColumn.DefaultCellStyle.Format),
          (DateTime)row[Controller.ReferencedColumnName]);
      } //End of foreach
      ComboBox.DataSource =
        new BindingSource(dictionary, null);
      ComboBox.DisplayMember = "Key";
      ComboBox.ValueMember = "Value";
      ComboBox.SelectedIndex = ComboBox.FindStringExact(initialFormattedDate);
      if (ComboBox.SelectedIndex == -1) {
        var firstDate =
          (DateTime)referencedTable.Rows[0][
            Controller.ReferencedColumnName];
        if (firstDate == DateTime.Parse("01 Jan 1900")) {
          ComboBox.SelectedIndex = 0;
          if (string.IsNullOrEmpty(Value.ToString())) {
            Value = firstDate;
          }
        } else {
          var lastDate =
            (DateTime)referencedTable.Rows[referencedTable.Rows.Count - 1][
              Controller.ReferencedColumnName];
          ComboBox.SelectedIndex = referencedTable.Rows.Count - 1;
          if (string.IsNullOrEmpty(Value.ToString())) {
            Value = lastDate;
          }
        }
      }
    }

    /// <summary>
    ///   Throws an <see cref="ApplicationException" /> to indicate
    ///   that the referenced table contains no rows that
    ///   can be made available for selection in the cell's drop-down list.
    /// </summary>
    /// <param name="parentColumnName">
    ///   For possible future with images.
    ///   The name of the parent column, if any.
    ///   This is the grid column whose value in the current row
    ///   determines which rows of the current column's referenced table
    ///   can be made available for selection.
    ///   Null if the grid contains no such parent column.
    /// </param>
    /// <param name="parentColumnValue">
    ///   For possible future with images.
    ///   If the current column has a parent column,
    ///   the value of the parent cell in the current row.
    ///   Otherwise null.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   The required application exception.
    /// </exception>
    /// <remarks>
    ///   When this exception is thrown,
    ///   the DataGridView.DataError event,
    ///   which is handled by TableView.MainGrid_DataError,
    ///   gets raised.
    /// </remarks>
    private void ThrowNoAvailableReferencesException(string parentColumnName = null,
      object parentColumnValue = null) {
      string message =
        "There are no " + Controller.ReferencedTableName
                        + " " + Controller.ReferencedColumnName + "s ";
      if (parentColumnName != null) {
        message +=
          "for " + parentColumnName
                 + " \"" + parentColumnValue + "\" ";
      }
      message +=
        "to choose between.  You need to insert at least one row into the "
        + Controller.ReferencedTableName + " table ";
      if (parentColumnName != null) {
        message +=
          "for " + parentColumnName
                 + " \"" + parentColumnValue + "\" ";
      }
      message +=
        "before you can add rows to the "
        + Controller.TableName + " table";
      // For possible future with images.
      if (parentColumnName != null) {
        message +=
          " for " + parentColumnName
                  + " \"" + parentColumnValue + "\"";
      }
      message += ".";
      throw new ApplicationException(message);
    }
  } //End of class
} //End of namespace