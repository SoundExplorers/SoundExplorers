using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  public abstract class GridBase : DataGridView {
    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }

    public virtual void Copy() {
      if (CurrentCell.Value == null) {
        return;
      }
      if (!IsCurrentCellInEditMode) {
        Clipboard.SetText(CurrentCell.Value.ToString());
      }
    }

    public virtual void Cut() {
      if (CurrentCell.Value == null) {
        return;
      }
      if (!IsCurrentCellInEditMode) {
        Clipboard.SetText(CurrentCell.Value.ToString());
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
    public static void SwapColors([NotNull] DataGridView a, [NotNull] DataGridView b) {
      var swapColor = a.DefaultCellStyle.BackColor;
      a.DefaultCellStyle.BackColor = b.DefaultCellStyle.BackColor;
      b.DefaultCellStyle.BackColor = swapColor;
      swapColor = a.DefaultCellStyle.ForeColor;
      a.DefaultCellStyle.ForeColor = b.DefaultCellStyle.ForeColor;
      b.DefaultCellStyle.ForeColor = swapColor;
      swapColor = a.DefaultCellStyle.SelectionBackColor;
      a.DefaultCellStyle.SelectionBackColor =
        b.DefaultCellStyle.SelectionBackColor;
      b.DefaultCellStyle.SelectionBackColor = swapColor;
      swapColor = a.DefaultCellStyle.SelectionForeColor;
      a.DefaultCellStyle.SelectionForeColor =
        b.DefaultCellStyle.SelectionForeColor;
      b.DefaultCellStyle.SelectionForeColor = swapColor;
    }
  }
}