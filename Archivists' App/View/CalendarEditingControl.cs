using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   Editing control for a calendar cell of a DataGridView.
  /// </summary>
  /// <remarks>
  ///   Code modified from
  ///   MSDN/"How to: Host Controls in Windows Forms DataGridView Cells"
  ///   http://msdn.microsoft.com/en-us/library/7tas5c80.aspx.
  /// </remarks>
  internal class CalendarEditingControl : DateTimePicker,
    IDataGridViewEditingControl {
    public CalendarEditingControl() {
      //this.Format = DateTimePickerFormat.Short;
      Format = DateTimePickerFormat.Custom;
      CustomFormat = "dd MMM yyyy";
    }

    /// <summary>
    /// Changes the control's user interface (UI) to be consistent with the specified
    //  cell style.
    /// </summary>
    /// <param name="dataGridViewCellStyle">
    ///   The System.Windows.Forms.DataGridViewCellStyle to use as the model for the
    ///   UI.
    /// </param>
    public void ApplyCellStyleToEditingControl(
      DataGridViewCellStyle dataGridViewCellStyle) {
      Font = dataGridViewCellStyle.Font;
      CalendarForeColor = dataGridViewCellStyle.ForeColor;
      CalendarMonthBackground = dataGridViewCellStyle.BackColor;
    }

    /// <summary>
    ///   Gets or sets the System.Windows.Forms.DataGridView that contains the cell.
    /// </summary>
    public DataGridView EditingControlDataGridView { get; set; }

    /// <summary>
    ///   Gets or sets the formatted value of the cell being modified by the editor.
    /// </summary>
    public object EditingControlFormattedValue {
      get => Value.ToString(CustomFormat);
      set {
        if (value is string) {
          try {
            // This will throw an exception of the string is 
            // null, empty, or not in the format of a date.
            Value = DateTime.Parse((string)value);
          } catch {
            // In the case of an exception, just use the 
            // default value so we're not left with a null
            // value.
            Value = DateTime.Now;
          }
        }
      }
    }

    /// <summary>
    ///   Gets or sets the index of the hosting cell's parent row.
    /// </summary>
    public int EditingControlRowIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value of the editing control
    //  differs from the value of the hosting cell.
    /// </summary>
    public bool EditingControlValueChanged { get; set; }

    /// <summary>
    /// Determines whether the specified key is a regular input key that the editing
    //  control should process or a special key that the System.Windows.Forms.DataGridView
    //  should process.
    /// </summary>
    /// <param name="keyData">
    /// A System.Windows.Forms.Keys that represents the key that was pressed.
    /// </param>
    /// <param name="dataGridViewWantsInputKey">
    /// True when the System.Windows.Forms.DataGridView wants to process the System.Windows.Forms.Keys
    //  in keyData; otherwise, False.
    /// </param>
    /// <returns>
    /// True if the specified key is a regular input key that should be handled by
    //  the editing control; otherwise, False.
    /// </returns>
    public bool EditingControlWantsInputKey(
      Keys key, bool dataGridViewWantsInputKey) {
      // Let the DateTimePicker handle the keys listed.
      switch (key & Keys.KeyCode) {
        case Keys.Left:
        case Keys.Up:
        case Keys.Down:
        case Keys.Right:
        case Keys.Home:
        case Keys.End:
        case Keys.PageDown:
        case Keys.PageUp:
          return true;
        default:
          return !dataGridViewWantsInputKey;
      }
    }

    /// <summary>
    /// Gets the cursor used when the mouse pointer is over the System.Windows.Forms.DataGridView.EditingPanel
    //  but not over the editing control.
    /// </summary>
    public Cursor EditingPanelCursor => base.Cursor;

    /// <summary>
    /// Retrieves the formatted value of the cell.
    /// </summary>
    /// <param name="context">
    /// A bitwise combination of System.Windows.Forms.DataGridViewDataErrorContexts
    //  values that specifies the context in which the data is needed.
    /// </param>
    /// <returns>
    ///   An System.Object that represents the formatted version of the cell contents.
    /// </returns>
    public object GetEditingControlFormattedValue(
      DataGridViewDataErrorContexts context) {
      return EditingControlFormattedValue;
    }

    /// <summary>
    ///   Prepares the currently selected cell for editing.
    /// </summary>
    /// <param name="selectAll">
    ///   True to select all of the cell's content; otherwise, False.
    /// </param>
    /// <remarks>
    ///   No preparation needs to be done.
    /// </remarks>
    public void PrepareEditingControlForEdit(bool selectAll) { }

    /// <summary>
    /// Gets or sets a value indicating whether the cell contents need to be repositioned
    //  whenever the value changes.
    /// </summary>
    public bool RepositionEditingControlOnValueChange => false;

    protected override void OnValueChanged(EventArgs eventargs) {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnValueChanged(eventargs);
    }
  } //End of class
} //End of namespace