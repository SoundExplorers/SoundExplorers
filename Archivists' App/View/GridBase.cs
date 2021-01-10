using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal abstract class GridBase : DataGridView {
    private GridContextMenu? _contextMenu;

    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      Margin = new Padding(4);
      RowHeadersWidth = 51;
      // Fixes a .Net 5 problem where, when the user cancelled (by pressing Esc) an edit
      // of a text cell on the insertion row after typing text, the program would crash.
      ShowCellErrors = false;
      ShowCellToolTips = false; // Before .Net 5, tooltips were off by default.
      CellColorScheme = new CellColorScheme(this);
    }

    public bool CanCut => CanSelectAll && CopyableText.Length > 0;
    public bool CanCopy => CopyableText.Length > 0;
    public bool CanPaste => !CurrentCell.ReadOnly && Clipboard.ContainsText();
    public bool CanDelete => CanCut;
    public bool CanSelectAll => !CurrentCell.ReadOnly && IsTextBoxCellCurrent;
    public bool CanSelectRow => CurrentRow != null;

    public bool CanDeleteSelectedRows =>
      !ReadOnly && !IsCurrentCellInEditMode && SelectedRows.Count > 0 &&
      !SelectedRows.Contains(NewRow);

    public CellColorScheme CellColorScheme { get; }

    protected GridControllerBase Controller { get; set; } = null!;

    public GridContextMenu ContextMenu =>
      _contextMenu ??= new GridContextMenu(this);

    protected EditorView EditorView =>
      Controller.EditorView as EditorView ??
      throw new InvalidOperationException(
        "Cannot cast Controller.EditorView to EditorView");
    
    protected int FirstVisibleColumnIndex { get; set; }

    public bool IsTextBoxCellCurrent =>
      CurrentCell?.OwningColumn.CellTemplate is TextBoxCell;

    public MainView MainView { get; set; } = null!;

    /// <summary>
    ///   Gets the new (i.e. empty) row at the bottom of an editable grid.
    /// </summary>
    private DataGridViewRow NewRow => !ReadOnly
      ? Rows[^1]
      : throw new InvalidOperationException(
        "A read-only grid does not contain a new row.");

    /// <summary>
    ///   Gets the cell text that can be copied to the clipboard or, if there is none,
    ///   an empty string.
    /// </summary>
    public string CopyableText {
      get {
        if (IsTextBoxCellCurrent && IsCurrentCellInEditMode) {
          return ((TextBox)EditingControl).SelectedText;
        }
        return CurrentCell.Value?.ToString() ?? string.Empty;
      }
    }

    /// <summary>
    ///   Enables or disables the menu items of the grid's context menu
    ///   or the corresponding items of the Edit menu on the main window menu bar.
    /// </summary>
    public void EnableOrDisableMenuItems(
      ToolStripMenuItem cutMenuItem,
      ToolStripMenuItem copyMenuItem,
      ToolStripMenuItem pasteMenuItem,
      ToolStripMenuItem deleteMenuItem,
      ToolStripMenuItem selectAllMenuItem,
      ToolStripMenuItem selectRowMenuItem,
      ToolStripMenuItem deleteSelectedRowsMenuItem) {
      selectAllMenuItem.Enabled = CanSelectAll;
      cutMenuItem.Enabled = CanCut;
      deleteMenuItem.Enabled = CanDelete;
      copyMenuItem.Enabled = CanCopy;
      pasteMenuItem.Enabled = CanPaste;
      selectRowMenuItem.Enabled = CanSelectRow;
      deleteSelectedRowsMenuItem.Enabled = CanDeleteSelectedRows;
    }

    protected virtual void ConfigureColumn(DataGridViewColumn column) {
      // Making every column explicitly not sortable prevents the program
      // from crashing if F3 in pressed while the grid is focused.
      column.SortMode = DataGridViewColumnSortMode.NotSortable;
      column.HeaderText = Controller.GetColumnDisplayName(column.Name);
      if (column.ValueType == typeof(DateTime)) {
        column.DefaultCellStyle.Format = EditorController.DateFormat;
      }
    }

    private void ConfigureColumns() {
      foreach (DataGridViewColumn column in Columns) {
        ConfigureColumn(column);
      }
    }

    /// <summary>
    ///   Returns the cell that is at the specified client co-ordinates of the main grid.
    ///   Null if there is no cell at the coordinates.
    /// </summary>
    /// <param name="x">
    ///   The x co-ordinate relative to the main grid's client rectangle.
    /// </param>
    /// <param name="y">
    ///   The y co-ordinate relative to the main grid's client rectangle.
    /// </param>
    private DataGridViewCell? GetCellAtClientCoOrdinates(int x, int y) {
      var hitTestInfo = HitTest(x, y);
      if (hitTestInfo.Type == DataGridViewHitTestType.Cell) {
        return Rows[
          hitTestInfo.RowIndex].Cells[
          hitTestInfo.ColumnIndex];
      }
      return null;
    }

    public void MakeRowCurrent(int rowIndex) {
      // This triggers OnRowEnter.
      Debug.WriteLine($"GridBase.MakeRowCurrent: {Name} row {rowIndex}");
      CurrentCell = Rows[rowIndex].Cells[FirstVisibleColumnIndex];
    }

    protected override void OnCurrentCellChanged(EventArgs e) {
      base.OnCurrentCellChanged(e);
      if (CurrentCell != null) {
        MainView.CopyToolStripButton.Enabled = CanCopy;
      }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.Apps: // Context menu key
        case Keys.Shift | Keys.F10: // Alternate context menu keyboard shortcut
          var cellRectangle = GetCellDisplayRectangle(
            CurrentCell.ColumnIndex,
            CurrentRow!.Index, true);
          var point = new Point(cellRectangle.Right, cellRectangle.Bottom);
          ContextMenu.Show(this, point);
          // When base.ContextMenuStrip was set to ContextMenu, neither e.Handled nor
          // e.SuppressKeyPress nor not calling base.OnKeyDown stopped the context menu
          // from being shown a second time immediately afterwards in the default wrong
          // position. The solution is not to set base.ContextMenuStrip to ContextMenu
          // and instead to show the context menu in grid event handlers: here when shown
          // with one of the two standard keyboard shortcuts; EditorView.Grid_MouseDown
          // when shown with a right mouse click.
          e.Handled = e.SuppressKeyPress = true;
          break;
        default:
          base.OnKeyDown(e);
          break;
      } //End of switch
    }

    /// <summary>
    ///   When mouse button 2 is clicked on a cell,
    ///   the cell will become the current cell
    ///   and, unless the cell is already being edited, the context menu will be shown.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        // Find the cell, if any, that mouse button 2 clicked.
        var cell = GetCellAtClientCoOrdinates(e.X, e.Y);
        if (cell != null) { // Cell found
          CurrentCell = cell;
          if (!IsCurrentCellInEditMode) {
            ContextMenu.Show(this, e.Location);
          }
        }
      }
      base.OnMouseDown(e);
    }

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities that will populate the grid.
    ///   If null, the default, all entities of the class's entity type
    ///   will be fetched from the database.
    /// </param>
    public virtual void Populate(IList? list = null) {
      Controller.Populate(list);
      DataSource = Controller.BindingList;
      ConfigureColumns();
      AutoResizeColumns();
    }
  }
}