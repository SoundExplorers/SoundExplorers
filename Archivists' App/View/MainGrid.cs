using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using SoundExplorers.Common;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class MainGrid : GridBase, IMainGrid {
    public MainGrid() {
      // Fixes a .Net 5 problem where, when the user cancelled (by pressing Esc) an edit
      // of a text cell on the insertion row after typing text, the program would crash.
      ShowCellErrors = false;
    }

    public TextBox TextBox =>
      (TextBox)EditingControl ??
      throw new InvalidOperationException(
        "The current cell is not in edit mode or its editor is not a TextBox.");

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

    public void RestoreCurrentRowCellErrorValue(int columnIndex, object? errorValue) {
      ((ICanRestoreErrorValue)CurrentRow!.Cells[columnIndex]).RestoreErrorValue(
        errorValue);
    }

    public void SelectCurrentRowOnly() {
      ClearSelection();
      CurrentRow!.Selected = true;
    }

    public override void Populate(IList? list = null) {
      Debug.WriteLine("MainGrid.Populate");
      FirstVisibleColumnIndex = -1;
      base.Populate(list);
      Debug.WriteLine("MainGrid.Populate END");
    }

    protected override DataGridViewColumn CreateColumn(IBindingColumn bindingColumn) {
      var result = base.CreateColumn(bindingColumn);
      Visible = bindingColumn.IsVisible;
      if (!Visible) {
        return result;
      }
      // Column will be shown
      if (FirstVisibleColumnIndex == -1) {
        FirstVisibleColumnIndex = result.Index;
      }
      if (bindingColumn.ReferencesAnotherEntity) {
        result.CellTemplate = ComboBoxCell.Create(Controller, result.Name);
      } else if (result.ValueType == typeof(DateTime)) {
        result.CellTemplate = new CalendarCell();
      } else if (result.ValueType == typeof(string)) {
        result.CellTemplate = new TextBoxCell();
        // Interpret blanking a cell as an empty string, not null. Null is not a problem
        // for the object-oriented database to handle. But this fixes an error where,
        // when a text cell was edited to blank and then Tab was pressed to proceed to
        // the next cell, the program would crash with a NullReferenceException. Perhaps 
        // this is no longer necessary, now that ShowCellErrors is set to false. 
        result.DefaultCellStyle.DataSourceNullValue = string.Empty;
      }
      return result;
    }

    // protected override void ConfigureColumn(DataGridViewColumn column) {
    //   if (!Controller.IsColumnToBeShown(column.Name)) {
    //     column.Visible = false;
    //     return;
    //   }
    //   // Column will be shown
    //   base.ConfigureColumn(column);
    //   if (FirstVisibleColumnIndex == -1) {
    //     FirstVisibleColumnIndex = column.Index;
    //   }
    //   if (Controller.DoesColumnReferenceAnotherEntity(column.Name)) {
    //     column.CellTemplate = ComboBoxCell.Create(Controller, column.Name);
    //   } else if (column.ValueType == typeof(DateTime)) {
    //     column.CellTemplate = new CalendarCell();
    //   } else if (column.ValueType == typeof(string)) {
    //     column.CellTemplate = new TextBoxCell();
    //     // Interpret blanking a cell as an empty string, not null. Null is not a problem
    //     // for the object-oriented database to handle. But this fixes an error where,
    //     // when a text cell was edited to blank and then Tab was pressed to proceed to
    //     // the next cell, which happened to be the first cell of the insertion row, if
    //     // that is relevant, the program would crash with a NullReferenceException.
    //     column.DefaultCellStyle.DataSourceNullValue = string.Empty;
    //   }
    // }

    protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e) {
      base.OnCellBeginEdit(e);
      if (IsTextBoxCellCurrent
          // On pasting into or deleting the contents of a cell that is not yet in edit
          // mode, the cell temporarily goes into edit mode but the EditingControl is
          // null instead of a TextBox. So we cannot subscribe to the TextBox's events.
          // However, the cell comes out of edit mode immediately the deletion or paste
          // is done. So in those cases there is no need to subscribe to the TextBox
          // events anyway.
          && EditingControl is TextBox) {
        BeginInvoke((Action)SubscribeToTextBoxEvents);
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
      if (IsTextBoxCellCurrent) {
        // Now that the TextBox cell edit has finished,
        // whether text can be cut or copied reverts to depending on
        // whether there is any text in the cell.
        MainView.CutToolStripButton.Enabled = CanCut;
        MainView.CopyToolStripButton.Enabled = CanCopy;
      }
    }

    protected override void OnCellValueChanged(DataGridViewCellEventArgs e) {
      base.OnCellValueChanged(e);
      if (CurrentCell is ComboBoxCell comboBoxCell) {
        // Debug.WriteLine("MainGrid_CellValueChanged, ComboBoxCell");
        var cellValue = CurrentCell.Value;
        int rowIndex = CurrentRow!.Index;
        comboBoxCell.Controller.OnCellValueChanged(rowIndex, cellValue);
      }
    }

    /// <summary>
    ///   Emulates the ComboBox's SelectedIndexChanged event.
    /// </summary>
    /// <remarks>
    ///   A known problem with <see cref="DataGridView" /> is that,
    ///   where there are multiple ComboBox columns,
    ///   ComboBox events can get spuriously raised against the ComboBoxes
    ///   in multiple cells of the row that is being edited.
    ///   So this event handler provides a workaround by
    ///   emulating a cell ComboBox's SelectedIndexChange event
    ///   but without the spurious occurrences.
    ///   The fix is based on the second answer here:
    ///   https://stackoverflow.com/questions/11141872/event-that-fires-during-MainGridcomboboxcolumn-selectedindexchanged
    /// </remarks>
    protected override void OnCurrentCellDirtyStateChanged(EventArgs e) {
      base.OnCurrentCellDirtyStateChanged(e);
      // Debug.WriteLine(
      //   $"MainGrid.CurrentCellDirtyStateChanged: IsCurrentCellDirty = {IsCurrentCellDirty}");
      if (CurrentCell is ComboBoxCell && IsCurrentCellDirty) {
        // Debug.WriteLine(
        //   "MainGrid.OnCurrentCellDirtyStateChanged: ComboBoxCell, IsCurrentCellDirty");
        // This fires the cell value changed handler MainGrid_CellValueChanged.
        CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
      }
    }

    protected override void OnCurrentCellChanged(EventArgs e) {
      base.OnCurrentCellChanged(e);
      if (CurrentCell != null) {
        MainView.CutToolStripButton.Enabled = CanCut;
        // I don't think it is practicable to continually enable or disable the Paste
        // button depending on the change in whether to clipboard contain text.
        MainView.PasteToolStripButton.Enabled = !CurrentCell.ReadOnly;
      }
    }

    public void OnError() {
      CancelEdit();
      Controller.ShowError();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.F2:
          if (CurrentCell != null) {
            BeginEdit(false);
          }
          break;
        default:
          base.OnKeyDown(e);
          break;
      } //End of switch
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
}