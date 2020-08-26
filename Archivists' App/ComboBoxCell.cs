using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using SoundExplorers.Data;

namespace SoundExplorers {
    /// <summary>
    ///   ComboBox cell of a DataGridView.
    /// </summary>
    /// <remarks>
    ///   The cell displays dates in ordinary text box cells,
    ///   but when the user edits a cell, a ComboBox control appears.
    /// </remarks>
    internal class ComboBoxCell : DataGridViewTextBoxCell {
        /// <summary>
        ///   Gets or sets the entity column metadata.
        /// </summary>
        /// <remarks>
        ///   For unknown reason,
        ///   non-inherited properties of a ComboBoxCell
        ///   (i.e. that are not inherited from DataGridViewTextBoxCell)
        ///   to which a DataGridViewColumn.CellTemplate
        ///   has been set
        ///   don't persist when a cell is edited.
        ///   So we are going to store this property in the Tag.
        ///   That solves the problem.
        /// </remarks>
        public virtual IEntityColumn Column {
      get => Tag as IEntityColumn;
      set => Tag = value;
    }

        /// <summary>
        ///   Gets the cell's combo box.
        /// </summary>
        private ComboBoxEditingControl ComboBox =>
      DataGridView.EditingControl as ComboBoxEditingControl;

    public override Type EditType =>
      // Return the type of the editing control that ComboBoxCell uses.
      typeof(ComboBoxEditingControl);

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
      IEntityList referencedEntities;
      string parentColunnName = null;
      object parentColunnValue = null;
      if (Column.TableName == "Image"
          && Column.ColumnName == "Date") {
        parentColunnName = "Location";
        parentColunnValue = OwningRow.Cells[parentColunnName].Value; // Location
        referencedEntities = Factory<IEntityList>.Create(
          Column.ReferencedTableName, // Table name:  Performance in this case
          // Constructor arguments
          null, // parentListType
          parentColunnValue); // location
      } else {
        referencedEntities =
          Factory<IEntityList>.Create(Column.ReferencedTableName);
      }
      if (ValueType == typeof(string)) {
        ComboBox.DataSource = referencedEntities.Table.DefaultView;
        ComboBox.DisplayMember = Column.ReferencedColumnName;
        ComboBox.ValueMember = Column.ReferencedColumnName;
        ComboBox.SelectedIndex =
          ComboBox.FindStringExact(initialFormattedValue.ToString());
      } else { // DateTime
        if (referencedEntities.Count > 0) {
          PopulateDateDropDownList(
            initialFormattedValue.ToString(),
            referencedEntities);
        } else {
          ComboBox.DataSource = null;
          ThrowNoAvailableReferencesException(parentColunnName,
            parentColunnValue);
        }
      }
    }

    /// <summary>
    ///   Populates the drop-down list of dates with the
    ///   available values of the date key colum of the referenced table.
    /// </summary>
    /// <param name="initialFormattedDate">
    ///   The formatted date to be initially selected in the drop-down list of dates.
    /// </param>
    /// <param name="referencedEntities">
    ///   An entity list representing those rows of the referenced table
    ///   that are to be used to populate the date list.
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
      string initialFormattedDate, IEntityList referencedEntities) {
      var dictionary =
        new Dictionary<string, DateTime>(referencedEntities.Count);
      foreach (DataRow row in referencedEntities.Table.Rows) {
        dictionary.Add(
          ((DateTime)row[Column.ReferencedColumnName]).ToString(
            OwningColumn.DefaultCellStyle.Format),
          (DateTime)row[Column.ReferencedColumnName]);
      } //End of foreach
      ComboBox.DataSource =
        new BindingSource(dictionary, null);
      ComboBox.DisplayMember = "Key";
      ComboBox.ValueMember = "Value";
      ComboBox.SelectedIndex = ComboBox.FindStringExact(initialFormattedDate);
      if (ComboBox.SelectedIndex == -1) {
        var firstDate =
          (DateTime)referencedEntities.Table.Rows[0][
            Column.ReferencedColumnName];
        if (firstDate == DateTime.Parse("01 Jan 1900")) {
          ComboBox.SelectedIndex = 0;
          if (string.IsNullOrEmpty(Value.ToString())) {
            Value = firstDate;
          }
        } else {
          var lastDate =
            (DateTime)referencedEntities.Table.Rows[
              referencedEntities.Count - 1][Column.ReferencedColumnName];
          ComboBox.SelectedIndex = referencedEntities.Count - 1;
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
    /// <param name="parentColunnName">
    ///   The name of the parent column, if any.
    ///   This is the grid column whose value in the current row
    ///   determines which rows of the current column's referenced table
    ///   can be made available for selection.
    ///   Null if the grid contains no such parent column.
    /// </param>
    /// <param name="parentColunnValue">
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
    ///   which is handled by TableForm.MainGrid_DataError,
    ///   gets raised.
    /// </remarks>
    private void ThrowNoAvailableReferencesException(string parentColunnName,
      object parentColunnValue) {
      string message =
        "There are no " + Column.ReferencedTableName
                        + " " + Column.ReferencedColumnName + "s ";
      if (parentColunnName != null) {
        message +=
          "for " + parentColunnName
                 + " \"" + parentColunnValue + "\" ";
      }
      message +=
        "to choose between.  You need to insert at least one row into the "
        + Column.ReferencedTableName + " table ";
      if (parentColunnName != null) {
        message +=
          "for " + parentColunnName
                 + " \"" + parentColunnValue + "\" ";
      }
      message +=
        "before you can add rows to the "
        + Column.TableName + " table";
      if (parentColunnName != null) {
        message +=
          " for " + parentColunnName
                  + " \"" + parentColunnValue + "\"";
      }
      message += ".";
      throw new ApplicationException(message);
    }
  } //End of class
} //End of namespace