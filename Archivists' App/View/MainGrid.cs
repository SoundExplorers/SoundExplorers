using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  public class MainGrid : GridBase, IMainGrid {
    private RowContextMenu _rowContextMenu;

    private new DataGridViewRow CurrentRow =>
      base.CurrentRow ?? throw new NullReferenceException(nameof(CurrentRow));

    internal MainGridController Controller { get; private set; }

    private RowContextMenu RowContextMenu =>
      _rowContextMenu ?? (_rowContextMenu = CreateRowContextMenu());

    private RowContextMenu CreateRowContextMenu() {
      var result = new RowContextMenu();
      result.CutMenuItem.Click += MainView.EditCutMenuItem_Click;
      result.CopyMenuItem.Click += MainView.EditCopyMenuItem_Click;
      result.PasteMenuItem.Click += MainView.EditPasteMenuItem_Click;
      result.SelectAllMenuItem.Click += MainView.EditSelectAllMenuItem_Click;
      result.DeleteSelectedRowsMenuItem.Click += MainView.EditDeleteSelectedRowsMenuItem_Click;
      return result;
    }

    internal MainView MainView { get; set; }

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

    public override void Copy() {
      base.Copy();
      if (CurrentCell.Value == null || !IsCurrentCellInEditMode) {
        return;
      }
      switch (EditingControl) {
        case TextBox textBox: {
          if (string.IsNullOrWhiteSpace(textBox.SelectedText)) {
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          }
          Clipboard.SetText(textBox.SelectedText);
          break;
        }
      }
    }

    public override void Cut() {
      base.Cut();
      if (CurrentCell.Value == null || CurrentCell.ReadOnly) {
        return;
      }
      if (!IsCurrentCellInEditMode) {
        BeginEdit(true);
        CurrentCell.Value = string.Empty;
        EndEdit();
      } else {
        // The cell is already being edited
        switch (EditingControl) {
          case TextBox textBox when string.IsNullOrWhiteSpace(textBox.SelectedText):
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          case TextBox textBox:
            Clipboard.SetText(textBox.SelectedText);
            textBox.SelectedText = string.Empty;
            break;
        }
      }
    }

    internal void DeleteSelectedRows() {
      if (Controller.IsInsertionRowCurrent) {
        return;
      }
      if (IsCurrentCellInEditMode) {
        CancelEdit();
      }
      if (SelectedRows.Count == 0) {
        CurrentRow.Selected = true;
      }
      foreach (DataGridViewRow row in SelectedRows) {
        Rows.Remove(row);
      }
    }

    internal void Paste() {
      if (!CurrentCell.ReadOnly) {
        if (!IsCurrentCellInEditMode) {
          BeginEdit(true);
          CurrentCell.Value = Clipboard.GetText();
          EndEdit();
        } else { // The cell is already being edited
          if (EditingControl is TextBox textBox) {
            textBox.SelectedText = Clipboard.GetText();
          }
        }
      }
    }

    internal void SelectAllInCurrentCell() {
      if (!IsCurrentCellInEditMode) {
        BeginEdit(true);
      } else { // The cell is already being edited
        if (EditingControl is TextBox textBox) {
          textBox.SelectAll();
        }
      }
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
        column.CellTemplate = new TextBoxCell {Tag = MainView};
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

    internal void ConfigureColumns() {
      foreach (DataGridViewColumn column in Columns) {
        ConfigureColumn(column);
      }
    }

    internal void MakeInsertionRowCurrent() {
      // This triggers OnRowEnter.
      // Debug.WriteLine("EditorView.MakeMainGridInsertionRowCurrent");
      MakeRowCurrent(Rows.Count - 1);
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

    internal void OnError() {
      CancelEdit();
      Controller.ShowError();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.F2:
          if (CurrentCell != null) {
            BeginEdit(true);
          }
          break;
        case Keys.Apps: // Context menu key
        case Keys.Shift | Keys.F10:
          var cellRectangle = GetCellDisplayRectangle(
            CurrentCell.ColumnIndex,
            CurrentRow.Index, true);
          var point = new Point(cellRectangle.Right, cellRectangle.Bottom);
          RowContextMenu.Show(this, point);
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
          RowContextMenu.Show(this, e.Location);
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
  }
}