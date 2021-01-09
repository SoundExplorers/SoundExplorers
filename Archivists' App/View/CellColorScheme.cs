using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  internal class CellColorScheme {
    public CellColorScheme(DataGridView grid) {
      Grid = grid;
      SaveDefault();
    }

    private Color DefaultBackColor { get; set; }
    private Color DefaultForeColor { get; set; }
    private Color DefaultSelectionForeColor { get; set; }
    private Color DefaultSelectionBackColor { get; set; }

    private DataGridView Grid { get; }
    
    /// <summary>
    ///   Inverts the foreground and background colours
    ///   of both selected and unselected cells
    ///   in the grid, relative to their defaults.
    /// </summary>
    public void Invert() {
      Grid.DefaultCellStyle.BackColor = DefaultForeColor;
      Grid.DefaultCellStyle.ForeColor = DefaultBackColor;
      Grid.DefaultCellStyle.SelectionBackColor = DefaultSelectionForeColor;
      Grid.DefaultCellStyle.SelectionForeColor = DefaultSelectionBackColor;
    }
    
    /// <summary>
    ///   Restores the foreground and background colours
    ///   of both selected and unselected cells
    ///   in the grid to their defaults.
    /// </summary>
    public void RestoreToDefault() {
      Grid.DefaultCellStyle.BackColor = DefaultBackColor;
      Grid.DefaultCellStyle.ForeColor = DefaultForeColor;
      Grid.DefaultCellStyle.SelectionBackColor = DefaultSelectionBackColor;
      Grid.DefaultCellStyle.SelectionForeColor = DefaultSelectionForeColor;
    }

    private void SaveDefault() {
      DefaultBackColor = Grid.DefaultCellStyle.BackColor;
      DefaultForeColor = Grid.DefaultCellStyle.ForeColor;
      DefaultSelectionBackColor = Grid.DefaultCellStyle.SelectionBackColor;
      DefaultSelectionForeColor = Grid.DefaultCellStyle.SelectionForeColor;
    }
  }
}