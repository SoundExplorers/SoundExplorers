using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   TextBox cell of a DataGridView.
  /// </summary>
  /// <remarks>
  ///   The cell's entire contents are selected on beginning edit
  ///   or, when editing, Ctrl+A or F2.
  ///   For unknown reason, the initial text selection works if the cell edit was started
  ///   with a mouse click but not if started with the F2 key.
  ///   This is fixed by handling F2 in <see cref="EditorView.MainGridOnKeyDown" />.
  /// </remarks>
  public class TextBoxCell : DataGridViewTextBoxCell, ICanRestoreErrorValue {
    public override Type EditType => typeof(DataGridViewTextBoxEditingControl);

    public void RestoreErrorValue(object errorValue) {
      var textBox = (TextBox)DataGridView.EditingControl;
      textBox.Text = errorValue?.ToString();
      textBox.SelectAll();
    }

    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      // Debug.WriteLine(
      //   $"{nameof(TextBoxCell)}.{nameof(InitializeEditingControl)}");
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      var textBox = (TextBox)DataGridView.EditingControl;
      textBox.KeyUp += TextBoxOnKeyUp;
      textBox.SelectAll();
      // I tried a timer to fix the failure of the initial SelectAll when
      // the cell edit was started with the F2 key.  It does not work.
      // var timer = new Timer();
      // timer.Tick += (sender, args) => {
      //   timer.Stop();
      //   textBox.SelectAll();
      // };
    }

    private static void TextBoxOnKeyUp(object sender, KeyEventArgs e) {
      // Debug.WriteLine(
      //   $"{nameof(TextBoxCell)}.{nameof(TextBoxOnKeyUp)}: KeyCode = {e.KeyCode}");
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A ||
          e.Modifiers == Keys.None && e.KeyCode == Keys.F2) {
        ((TextBox)sender).SelectAll();
      }
    }
  }
}