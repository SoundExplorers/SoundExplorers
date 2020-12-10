using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal abstract class GridBase : DataGridView {
    private GridContextMenu _contextMenu;
    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }

    public new GridContextMenu ContextMenu =>
      _contextMenu ?? (_contextMenu = new GridContextMenu(this));

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