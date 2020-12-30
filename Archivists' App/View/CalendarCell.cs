using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  /// <summary>
  ///   Calendar cell of a DataGridView.
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
  internal class CalendarCell : DataGridViewTextBoxCell, ICanRestoreErrorValue {
    public override Type EditType =>
      // Return the type of the editing control that CalendarCell uses.
      typeof(CalendarEditingControl);

    [NotNull]
    private DateTimePicker DateTimePicker => (DateTimePicker)Grid.EditingControl;

    [NotNull]
    private DataGridView Grid =>
      DataGridView
      ?? throw new NullReferenceException(
        "In CalendarCell.Grid, DataGridView is null.");

    public void RestoreErrorValue(object? errorValue) {
      if (errorValue is DateTime dateTime) {
        DateTimePicker.Value = dateTime.Date;
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
        DateTimePicker.Value = DateTime.Now;
        Value = DateTimePicker.Value;
      } else {
        DateTimePicker.Value = (DateTime)Value;
      }
    }
  } //End of class
} //End of namespace