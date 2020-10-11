using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   TextBox cell of a DataGridView.
  /// </summary>
  /// <remarks>
  ///   The cell's entire contents are selected on beginning edit or Ctrl+A.
  /// </remarks>
  public class TextBoxCell : DataGridViewTextBoxCell {
    public override Type EditType => typeof(DataGridViewTextBoxEditingControl);

    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      var textBox = (TextBox)DataGridView.EditingControl;
      textBox.SelectAll();
      textBox.KeyUp += TextBoxOnKeyUp;
    }

    private static void TextBoxOnKeyUp(object sender, KeyEventArgs e) {
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A) {
        ((TextBox)sender).SelectAll();
      }
    }
  }
}