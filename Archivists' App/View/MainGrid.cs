using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class MainGrid : GridBase, IMainGrid {
    private new DataGridViewRow CurrentRow =>
      base.CurrentRow ?? throw new NullReferenceException(nameof(CurrentRow));

    public MainGridController Controller { get; private set; }

    public TextBox TextBox =>
      (TextBox)EditingControl ??
      throw new InvalidOperationException(
        "The current cell is not in edit mode or its editor is not a TextBox.");

    public void SetController(MainGridController controller) {
      Controller = controller;
    }

    public void EditCurrentCell() {
      BeginEdit(true);
    }

    public void MakeCellCurrent(int rowIndex, int columnIndex) {
      // This triggers OnRowEnter.
      // Debug.WriteLine("EditorView.MakeCellCurrent");
      try {
        CurrentCell = Rows[rowIndex].Cells[columnIndex];
      } catch {
        // Can happen if insertion row is left before error message is shown.
      }
    }

    public void MakeRowCurrent(int rowIndex) {
      // This triggers OnRowEnter.
      // Debug.WriteLine($"EditorView.MakeRowCurrent: row {rowIndex}");
      CurrentCell = Rows[rowIndex].Cells[0];
    }

    public void OnRowAddedOrDeleted() {
      AutoResizeColumns();
      Focus();
    }

    public void RestoreCurrentRowCellErrorValue(int columnIndex, object errorValue) {
      ((ICanRestoreErrorValue)CurrentRow.Cells[columnIndex]).RestoreErrorValue(
        errorValue);
    }

    public void SelectCurrentRowOnly() {
      ClearSelection();
      CurrentRow.Selected = true;
    }

    private void ConfigureColumn([NotNull] DataGridViewColumn column) {
      // Making every column explicitly not sortable prevents the program
      // from crashing if F3 in pressed while the grid is focused.
      // TODO Check whether F3 crashes program when PARENT grid is focused.
      column.SortMode = DataGridViewColumnSortMode.NotSortable;
      column.HeaderText = Controller.GetColumnDisplayName(column.Name);
      if (column.ValueType == typeof(DateTime)) {
        column.DefaultCellStyle.Format = EditorController.DateFormat;
      }
      if (Controller.DoesColumnReferenceAnotherEntity(column.Name)) {
        column.CellTemplate = ComboBoxCell.Create(Controller, column.Name);
      } else if (column.ValueType == typeof(DateTime)) {
        column.CellTemplate = new CalendarCell();
      } else if (column.ValueType == typeof(string)) {
        column.CellTemplate = new TextBoxCell();
        // Interpret blanking a cell as an empty string, not null.
        // Null is not a problem for the object-oriented database to handle.
        // But this fixes an error where,
        // when a text cell was edited to blank
        // and then Tab was pressed to proceed to the next cell,
        // which happened to be the first cell of the insertion row,
        // if that is relevant,
        // the program would crash with a NullReferenceException.
        column.DefaultCellStyle.DataSourceNullValue = string.Empty;
      }
    }

    public void ConfigureColumns() {
      foreach (DataGridViewColumn column in Columns) {
        ConfigureColumn(column);
      }
    }

    public void MakeInsertionRowCurrent() {
      // This triggers OnRowEnter.
      // Debug.WriteLine("EditorView.MakeMainGridInsertionRowCurrent");
      MakeRowCurrent(Rows.Count - 1);
    }

    protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e) {
      base.OnCellBeginEdit(e);
      if (IsTextBoxCellCurrent) {
        //Debug.WriteLine("MainGrid.OnCellBeginEdit: TextBox cell");
        BeginInvoke((Action)delegate {
          TextBox.KeyUp -= TextBox_KeyUp;
          TextBox.MouseClick -= TextBox_MouseClick;
          TextBox.TextChanged -= TextBox_TextChanged;
          OnTextBoxSelectionMayHaveChanged();
          TextBox.KeyUp += TextBox_KeyUp;
          TextBox.MouseClick += TextBox_MouseClick;
          TextBox.TextChanged += TextBox_TextChanged;
        });
      }
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
        int rowIndex = CurrentRow.Index;
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
      //Debug.WriteLine($"MainGrid_CurrentCellDirtyStateChanged: IsCurrentCellDirty = {MainGrid.IsCurrentCellDirty}");
      if (CurrentCell is ComboBoxCell && IsCurrentCellDirty) {
        // Debug.WriteLine(
        //   "MainGrid_CurrentCellDirtyStateChanged: ComboBoxCell, IsCurrentCellDirty");
        // This fires the cell value changed handler MainGrid_CellValueChanged.
        CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
      }
    }

    protected override void OnCurrentCellChanged(EventArgs e) {
      base.OnCurrentCellChanged(e);
      if (CurrentCell != null) {
        MainView.CutToolStripButton.Enabled = CanCut;
        // I don't it is practicable to continually enable or disable the Paste button
        // depending on the change in whether to clipboard contain text.
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
        case Keys.Apps: // Context menu key
        case Keys.Shift | Keys.F10: // Alternate context menu keyboard shortcut
          var cellRectangle = GetCellDisplayRectangle(
            CurrentCell.ColumnIndex,
            CurrentRow.Index, true);
          var point = new Point(cellRectangle.Right, cellRectangle.Bottom);
          ContextMenu.Show(this, point);
          // When base.ContextMenuStrip was set to RowContextMenu,
          // neither e.Handled nor e.SuppressKeyPress nor not calling base.OnKeyDown
          // stopped the context menu from being shown a second time
          // immediately afterwards in the default wrong position.
          // The solution is not to set
          // base.ContextMenuStrip to RowContextMenu and instead to
          // show the context menu in grid event handlers:
          // here when shown with one of the two standard keyboard shortcuts:
          // EditorView.Grid_MouseDown when shown with a right mouse click.
          e.Handled = e.SuppressKeyPress = true;
          break;
        default:
          base.OnKeyDown(e);
          break;
      } //End of switch
    }

    /// <summary>
    ///   When mouse button 2 is clicked on a cell
    ///   and unless the cell is being edited,
    ///   the context menu will be shown:
    ///   the base method will already have made
    ///   that cell the current cell.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        if (!IsCurrentCellInEditMode) {
          ContextMenu.Show(this, e.Location);
        }
      } else {
        base.OnMouseDown(e);
      }
    }

    /// <summary>
    ///   The initial state of the row will be saved to a detached row
    ///   to allow comparison with any changes if the row gets edited.
    /// </summary>
    /// <remarks>
    ///   If the row represents an Image whose Path
    ///   specifies a valid image file,
    ///   the image will be shown below the main grid.
    ///   If the row represents an Image whose Path
    ///   does not specifies a valid image file,
    ///   a Missing Image label containing an appropriate message will be displayed.
    /// </remarks>
    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      base.OnRowEnter(e);
      // This is the safe way of checking whether we have entered the insertion (new) row:
      //if (e.RowIndex == MainGrid.RowCount - 1) {
      //   // Controller.OnEnteringInsertionRow();
      //   // // if (Entities is ImageList) {
      //   // //   ShowImageOrMessage(null);
      //   // // }
      //}
      Controller.OnRowEnter(e.RowIndex);
    }

    private void OnTextBoxSelectionMayHaveChanged() {
      //Debug.WriteLine("MainGrid.OnTextBoxSelectionMayHaveChanged");
      // While a text box cell is being edited,
      // whether text can be cut or copied depends on
      // whether any text in the cell is selected.
      MainView.CutToolStripButton.Enabled = CanCut;
      MainView.CopyToolStripButton.Enabled = CanCopy;
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e) {
      //Debug.WriteLine("MainGrid.TextBox_KeyUp");
      OnTextBoxSelectionMayHaveChanged();
    }

    private void TextBox_MouseClick(object sender, MouseEventArgs e) {
      //Debug.WriteLine("MainGrid.TextBox_MouseClick");
      OnTextBoxSelectionMayHaveChanged();
    }

    private void TextBox_TextChanged(object sender, EventArgs e) {
      //Debug.WriteLine("MainGrid.TextBox_TextChanged");
      OnTextBoxSelectionMayHaveChanged();
    }
  }
}