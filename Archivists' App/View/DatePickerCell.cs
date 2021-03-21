using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   Date picker cell of a DataGridView.
  /// </summary>
  /// <remarks>
  ///   The cell displays dates in ordinary text box cells,
  ///   but when the user edits a cell, a DateTimePicker control appears.
  ///   The default date when inserting a new row will be today's date.
  ///   <para>
  ///     Code modified from
  ///     MSDN/"How to: Host Controls in Windows Forms DataGridView Cells"
  ///     http://msdn.microsoft.com/en-us/library/7tas5c80.aspx.
  ///   </para>
  /// </remarks>
  internal class DatePickerCell : DataGridViewTextBoxCell, ICanRestoreErrorValue {
    public override Type EditType =>
      // Return the type of the editing control that DatePickerCell uses.
      typeof(DatePickerEditingControl);

    public DateTimePicker DatePicker =>
      (DateTimePicker)DataGridView!.EditingControl;

    public void RestoreErrorValue(object? errorValue) {
      if (errorValue is DateTime dateTime) {
        DatePicker.Value = dateTime.Date;
      }
    }

    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      // Use the default row value when Value property is null
      // or, more to the point in this case, empty.
      if (Value == null
          || Value == DBNull.Value) {
        DatePicker.Value = DateTime.Now;
        Value = DatePicker.Value;
      } else {
        DatePicker.Value = (DateTime)Value;
      }
    }
  } //End of class
} //End of namespace