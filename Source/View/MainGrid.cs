using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using SoundExplorers.Common;
using SoundExplorers.Controller;

namespace SoundExplorers.View; 

internal class MainGrid : GridBase, IMainGrid {
  private Graphics? _graphics;

  public MainGrid() {
    // Fixes a .Net 5 problem where, when the user cancelled (by pressing Esc) an edit
    // of a text cell on the insertion row after typing text, the program would crash.
    ShowCellErrors = false;
  }

  /// <summary>
  ///   Pasting to a main grid text cell that is not already in edit mode is no longer
  ///   supported. See remarks for
  ///   <see cref="GridContextMenu.BeginCellEditIfRequired" />.
  /// </summary>
  public override bool CanPaste =>
    base.CanPaste && (!IsTextBoxCellCurrent || EditingControl is TextBox);

  /// <summary>
  ///   Cutting or deleting or selecting all the contents of a main grid text cell that
  ///   is not already in edit mode is no longer supported. See remarks for
  ///   <see cref="GridContextMenu.BeginCellEditIfRequired" />.
  /// </summary>
  public override bool CanSelectAll => base.CanSelectAll && EditingControl is TextBox;

  public TextBox TextBox =>
    (TextBox)EditingControl ??
    throw new InvalidOperationException(
      "The current cell is not in edit mode or its editor is not a TextBox.");

  private bool IsCalendarToBeDisplayed { get; set; }
  private Graphics Graphics => _graphics ??= CreateGraphics();

  private bool IsComboBoxCellCurrent =>
    CurrentCell?.OwningColumn.CellTemplate is ComboBoxCell;

  private bool IsDatePickerCellCurrent =>
    CurrentCell?.OwningColumn.CellTemplate is DatePickerCell;

  private bool IsDropDownListToBeDisplayed { get; set; }

  public new MainGridController Controller {
    get => (MainGridController)base.Controller;
    private set => base.Controller = value;
  }

  public void SetController(MainGridController controller) {
    Controller = controller;
  }

  public void EditCurrentCell() {
    BeginEdit(true);
  }

  public int GetTextWidthInPixels(string text) {
    return GetTextWidthInPixels(text, Font, Graphics);
  }

  public void MakeCellCurrent(int rowIndex, int columnIndex) {
    // This triggers OnRowEnter.
    // Debug.WriteLine("MainGrid.MakeCellCurrent");
    try {
      CurrentCell = Rows[rowIndex].Cells[columnIndex];
    } catch {
      // An ignorable exception can happen if the insertion row is left before an error
      // message is shown.
    }
  }

  public void OnRowAddedOrDeleted() {
    AutoResizeColumns();
    Focus();
  }

  public void RestoreCurrentRowCellErrorValue(int columnIndex, object errorValue) {
    Debug.WriteLine("MainGrid.RestoreCurrentRowCellErrorValue");
    ((ICanRestoreErrorValue)CurrentRow!.Cells[columnIndex]).RestoreErrorValue(
      errorValue);
  }

  public void SelectCurrentRowOnly() {
    ClearSelection();
    CurrentRow!.Selected = true;
  }

  public void OnError() {
    Debug.WriteLine("MainGrid.OnError");
    CancelEdit();
    Controller.ShowError();
  }

  protected override DataGridViewColumn AddColumn(IBindingColumn bindingColumn) {
    var result = base.AddColumn(bindingColumn);
    result.Visible = bindingColumn.IsVisible;
    if (!result.Visible) {
      return result;
    }
    // Column will be shown
    if (bindingColumn.ReferencesAnotherEntity) {
      result.CellTemplate = ComboBoxCell.Create(Controller, result.Name);
    } else if (result.ValueType == typeof(DateTime)) {
      result.CellTemplate = new DatePickerCell();
    } else if (result is DataGridViewTextBoxColumn) {
      result.CellTemplate = new TextBoxCell();
      result.DefaultCellStyle.DataSourceNullValue = string.Empty;
    }
    return result;
  }

  protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e) {
    base.OnCellBeginEdit(e);
    // On pasting into or deleting the contents of a cell that is not yet in edit mode,
    // the cell temporarily goes into edit mode but the EditingControl is null and the
    // cell comes out of edit mode immediately the deletion or paste is done. So in
    // those cases we should not need to do any of the following (and we would not be
    // able to subscribe to a TextBox's events).
    if (CurrentCell != null) {
      SetCurrentColumnWidthToEditWidth();
      if (IsTextBoxCellCurrent) {
        BeginInvoke((Action)SubscribeToTextBoxEvents);
        if (Controller.IsUrlColumn(CurrentCell.OwningColumn.Name)) {
          MainView.ToolsLinkMenuItem.Enabled =
            MainView.LinkToolStripButton.Enabled = false;
          // When a URL is not being edited, its text is underlined blue, to encourage
          // linking. But that looks weird if the URL is being edited. So make the text
          // plain-old while the URL is in edit mode.
          BeginInvoke((Action)delegate {
            var textBox = (CurrentCell as TextBoxCell)!.TextBox;
            textBox.BackColor = DefaultCellStyle.BackColor;
            textBox.ForeColor = DefaultCellStyle.ForeColor;
            textBox.Font = DefaultCellStyle.Font;
          });
        }
      } else if (IsDropDownListToBeDisplayed) {
        IsDropDownListToBeDisplayed = false;
        BeginInvoke((Action)delegate {
          ((ComboBoxCell)CurrentCell).ComboBox.DroppedDown = true;
        });
      } else if (IsCalendarToBeDisplayed) {
        IsCalendarToBeDisplayed = false;
        BeginInvoke((Action)DisplayCalendar);
      }
    }
    // THE FOLLOWING IS NOT YET IN USE BUT MAY BE LATER:
    // This is only relevant if the Path cell of an Image row is being edited. If the
    // Missing Image label was visible just before entering edit mode, it will have
    // been because the file was not specified or can't be found or is not an image
    // file. That's presumably about to be rectified. So the message to that effect in
    // the Missing Image label could be misleading. Also, the advice in the Missing
    // Image label that an image file can be dragged onto the label will not apply, as
    // dragging and dropping is disabled while the Path cell is being edited.
    EditorView.MissingImageLabel.Visible = false;
  }

  protected override void OnCellEndEdit(DataGridViewCellEventArgs e) {
    base.OnCellEndEdit(e);
    AutoResizeColumns();
    if (IsTextBoxCellCurrent) {
      // Now that the TextBox cell edit has finished, whether text can be cut or copied
      // reverts to depending on whether there is any text in the cell.
      MainView.CutToolStripButton.Enabled = CanCut;
      MainView.CopyToolStripButton.Enabled = CanCopy;
      if (Controller.IsUrlColumn(CurrentCell.OwningColumn.Name)) {
        MainView.ToolsLinkMenuItem.Enabled =
          MainView.LinkToolStripButton.Enabled =
            !string.IsNullOrWhiteSpace(CurrentCell.Value?.ToString());
      }
    }
  }

  protected override void OnCurrentCellChanged(EventArgs e) {
    base.OnCurrentCellChanged(e);
    if (CurrentCell != null) {
      MainView.CutToolStripButton.Enabled = CanCut;
      // I don't think it is practicable to continually enable or disable the Paste
      // button depending on the change in whether the clipboard contains text.
      MainView.PasteToolStripButton.Enabled = !CurrentCell.ReadOnly;
    }
  }

  protected override void OnKeyDown(KeyEventArgs e) {
    switch (e.KeyData) {
      case Keys.F2:
        if (CurrentCell != null) {
          BeginEdit(true);
        }
        break;
      case Keys.F4:
      case Keys.Alt | Keys.Up:
      case Keys.Alt | Keys.Down:
        if (IsComboBoxCellCurrent) {
          IsDropDownListToBeDisplayed = true;
          BeginEdit(true);
        } else if (IsDatePickerCellCurrent) {
          IsCalendarToBeDisplayed = true;
          BeginEdit(true);
        }
        break;
      default:
        base.OnKeyDown(e);
        break;
    } //End of switch
  }

  /// <summary>
  ///   Returns the width in pixels (rounded up) of the specified text in the specified
  ///   font.
  /// </summary>
  /// <param name="text">
  ///   The text whose width is to be measured.
  /// </param>
  /// <param name="font">
  ///   The font in which the specified text is to be drawn.
  /// </param>
  /// <param name="graphics">
  ///   The <see cref="Graphics" /> object that is to be used
  ///   to do the measurement.
  /// </param>
  /// <remarks>
  ///   The <see cref="Control.CreateGraphics" /> method of any <see cref="Control" />
  ///   can be used to create the <see cref="Graphics" /> object to which the
  ///   <paramref name="graphics" /> parameter is set.
  /// </remarks>
  private static int GetTextWidthInPixels(string text, Font font, Graphics graphics) {
    return (int)Math.Ceiling(graphics.MeasureString(text, font).Width);
  }

  /// <summary>
  /// </summary>
  /// <remarks>
  ///   As <see cref="DateTimePicker" /> does not have a property or method to
  ///   programatically display the calendar, we have to do it by sending the
  ///   date picker the F4 keyboard shortcut as a Windows message.
  /// </remarks>
  [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
  private void DisplayCalendar() {
    int dummy = SafeNativeMethods.SendMessage(
      ((DatePickerCell)CurrentCell).DatePicker.Handle,
      (uint)WindowsMessage.WM_KEYDOWN, (int)Keys.F4, 0);
  }

  /// <summary>
  ///   While a text box cell is being edited, whether text can be cut or copied
  ///   depends on whether any text in the cell is selected.
  /// </summary>
  private void OnTextBoxSelectionMayHaveChanged() {
    //Debug.WriteLine("MainGrid.OnTextBoxSelectionMayHaveChanged");
    MainView.CutToolStripButton.Enabled = CanCut;
    MainView.CopyToolStripButton.Enabled = CanCopy;
  }

  private void SetCurrentColumnWidthToEditWidth() {
    var column = CurrentCell.OwningColumn;
    int editWidth = Controller.BindingColumns[column.Index].EditWidth;
    if (editWidth > column.Width) {
      column.Width = editWidth;
    }
  }

  private void SubscribeToTextBoxEvents() {
    //Debug.WriteLine("MainGrid.SubscribeToTextBoxEvents");
    TextBox.KeyUp -= TextBox_KeyUp;
    TextBox.MouseClick -= TextBox_MouseClick;
    TextBox.TextChanged -= TextBox_TextChanged;
    OnTextBoxSelectionMayHaveChanged();
    TextBox.KeyUp += TextBox_KeyUp;
    TextBox.MouseClick += TextBox_MouseClick;
    TextBox.TextChanged += TextBox_TextChanged;
  }

  private void TextBox_KeyUp(object? sender, KeyEventArgs e) {
    //Debug.WriteLine("MainGrid.TextBox_KeyUp");
    OnTextBoxSelectionMayHaveChanged();
  }

  private void TextBox_MouseClick(object? sender, MouseEventArgs e) {
    //Debug.WriteLine("MainGrid.TextBox_MouseClick");
    OnTextBoxSelectionMayHaveChanged();
  }

  private void TextBox_TextChanged(object? sender, EventArgs e) {
    //Debug.WriteLine("MainGrid.TextBox_TextChanged");
    OnTextBoxSelectionMayHaveChanged();
  }
}