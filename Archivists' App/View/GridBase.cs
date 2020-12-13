using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal abstract class GridBase : DataGridView {
    private GridContextMenu _contextMenu;

    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }

    [NotNull] public new GridContextMenu ContextMenu =>
      _contextMenu ?? (_contextMenu = new GridContextMenu(this));

    public bool IsTextBoxCellCurrent =>
      CurrentCell.OwningColumn.CellTemplate is TextBoxCell; 

    /// <summary>
    ///   Gets the new (i.e. empty) row at the bottom of an editable grid.
    /// </summary>
    [NotNull] private DataGridViewRow NewRow => !ReadOnly
      ? Rows[Rows.Count - 1]
      : throw new InvalidOperationException(
        "A read-only grid does not contain a new row.");

    /// <summary>
    ///   Gets the cell text that can be copied to the clipboard or, if there is none,
    ///   an empty string.
    /// </summary>
    [NotNull]
    public string CopyableText {
      get {
        if (IsTextBoxCellCurrent && IsCurrentCellInEditMode) {
          return ((TextBox)EditingControl).SelectedText;
        }
        return CurrentCell.Value?.ToString() ?? string.Empty;
      }
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

    /// <summary>
    ///   Enables or disables the menu items of the grid's context menu
    ///   or the corresponding items of the Edit menu on the main window menu bar.  
    /// </summary>
    public void EnableMenuItems(
      [NotNull] ToolStripMenuItem cutMenuItem,
      [NotNull] ToolStripMenuItem copyMenuItem,
      [NotNull] ToolStripMenuItem pasteMenuItem,
      [NotNull] ToolStripMenuItem deleteMenuItem,
      [NotNull] ToolStripMenuItem selectAllMenuItem,
      [NotNull] ToolStripMenuItem selectRowMenuItem,
      [NotNull] ToolStripMenuItem deleteSelectedRowsMenuItem) {
      selectAllMenuItem.Enabled = CanSelectAll;
      cutMenuItem.Enabled = CanCut;
      deleteMenuItem.Enabled = CanDelete;
      copyMenuItem.Enabled = CanCopy;
      pasteMenuItem.Enabled = CanPaste;
      selectRowMenuItem.Enabled = CanSelectRow;
      deleteSelectedRowsMenuItem.Enabled = CanDeleteSelectedRows;
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
    private DataGridViewCell GetCellAtClientCoOrdinates(int x, int y) {
      var hitTestInfo = HitTest(x, y);
      if (hitTestInfo.Type == DataGridViewHitTestType.Cell) {
        return Rows[
          hitTestInfo.RowIndex].Cells[
          hitTestInfo.ColumnIndex];
      }
      return null;
    }

    /// <summary>
    ///   When mouse button 2 is clicked on a cell,
    ///   the cell will become the current cell.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        // Find the cell, if any, that mouse button 2 clicked.
        var cell = GetCellAtClientCoOrdinates(e.X, e.Y);
        if (cell != null) { // Cell found
          CurrentCell = cell;
        }
      }
      base.OnMouseDown(e);
    }

    /// <summary>
    ///   Swaps the colour schemes of the two grids.
    /// </summary>
    public void SwapColorsWith([NotNull] DataGridView otherGrid) {
      var swapColor = DefaultCellStyle.BackColor;
      DefaultCellStyle.BackColor = otherGrid.DefaultCellStyle.BackColor;
      otherGrid.DefaultCellStyle.BackColor = swapColor;
      swapColor = DefaultCellStyle.ForeColor;
      DefaultCellStyle.ForeColor = otherGrid.DefaultCellStyle.ForeColor;
      otherGrid.DefaultCellStyle.ForeColor = swapColor;
      swapColor = DefaultCellStyle.SelectionBackColor;
      DefaultCellStyle.SelectionBackColor =
        otherGrid.DefaultCellStyle.SelectionBackColor;
      otherGrid.DefaultCellStyle.SelectionBackColor = swapColor;
      swapColor = DefaultCellStyle.SelectionForeColor;
      DefaultCellStyle.SelectionForeColor =
        otherGrid.DefaultCellStyle.SelectionForeColor;
      otherGrid.DefaultCellStyle.SelectionForeColor = swapColor;
    }
  }
}