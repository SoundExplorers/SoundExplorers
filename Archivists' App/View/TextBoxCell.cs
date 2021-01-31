using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   TextBox cell of a DataGridView.
  /// </summary>
  internal class TextBoxCell : DataGridViewTextBoxCell, ICanRestoreErrorValue {
    private TextBoxContextMenu? _cellTextBoxContextMenu;
    private TextBox TextBox => (TextBox)DataGridView!.EditingControl;

    private TextBoxContextMenu TextBoxContextMenu =>
      _cellTextBoxContextMenu ??= new TextBoxContextMenu(TextBox);

    public override Type EditType => typeof(DataGridViewTextBoxEditingControl);

    public void RestoreErrorValue(object? errorValue) {
      TextBox.Text = errorValue?.ToString();
      TextBox.SelectAll();
    }

    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      // Debug.WriteLine(
      //   $"{nameof(TextBoxCell)}.{nameof(InitializeEditingControl)}");
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      TextBox.ContextMenuStrip = TextBoxContextMenu;
      TextBox.KeyUp += TextBox_KeyUp;
    }

    private void TextBox_KeyUp(object? sender, KeyEventArgs e) {
      // Debug.WriteLine(
      //   $"{nameof(TextBoxCell)}.{nameof(TextBoxOnKeyUp)}: KeyCode = {e.KeyCode}");
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A) {
        TextBox.SelectAll();
      }
    }
  }
}