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
  ///   This is fixed by handling F2 in <see cref="MainGrid.OnKeyDown" />.
  /// </remarks>
  public class TextBoxCell : DataGridViewTextBoxCell, ICanRestoreErrorValue {
    private TextBoxContextMenu _textBoxContextMenu;

    private MainView MainView => (MainView)Tag;
    private TextBox TextBox => (TextBox)DataGridView.EditingControl;
    
    private TextBoxContextMenu TextBoxContextMenu =>
      _textBoxContextMenu ?? (_textBoxContextMenu = new TextBoxContextMenu(MainView, TextBox));

    public override Type EditType => typeof(DataGridViewTextBoxEditingControl);

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
      TextBox.SelectAll();
    }

    public void RestoreErrorValue(object errorValue) {
      TextBox.Text = errorValue?.ToString();
      TextBox.SelectAll();
    }

    private static void TextBox_KeyUp(object sender, KeyEventArgs e) {
      // Debug.WriteLine(
      //   $"{nameof(TextBoxCell)}.{nameof(TextBoxOnKeyUp)}: KeyCode = {e.KeyCode}");
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A ||
          e.Modifiers == Keys.None && e.KeyCode == Keys.F2) {
        ((TextBox)sender).SelectAll();
      }
    }
  }
}