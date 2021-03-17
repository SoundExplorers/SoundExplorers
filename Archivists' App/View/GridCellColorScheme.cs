using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class GridCellColorScheme : IGridCellColorScheme {
    private IList<Color>? _defaultBackColors;
    private IList<Color>? _defaultForeColors;
    private IList<Color>? _defaultSelectionBackColors;
    private IList<Color>? _defaultSelectionForeColors;

    public GridCellColorScheme(GridBase grid) {
      Grid = grid;
    }

    private IList<Color> DefaultBackColors => _defaultBackColors ??=
      (from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.BackColor
          : Grid.DefaultCellStyle.BackColor).ToList();

    private IList<Color> DefaultForeColors => _defaultForeColors ??=
      (from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.ForeColor
          : Grid.DefaultCellStyle.ForeColor).ToList();

    private IList<Color> DefaultSelectionBackColors => _defaultSelectionBackColors ??=
      (from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.SelectionBackColor
          : Grid.DefaultCellStyle.SelectionBackColor).ToList();

    private IList<Color> DefaultSelectionForeColors => _defaultSelectionForeColors ??=
      (from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.SelectionForeColor
          : Grid.DefaultCellStyle.SelectionForeColor).ToList();

    private GridBase Grid { get; }

    /// <summary>
    ///   Inverts the foreground and background colours of both selected and unselected
    ///   cells in the grid, relative to their defaults.
    /// </summary>
    public void Invert() {
      for (int i = 0; i < Grid.Columns.Count; i++) {
        DataGridViewColumn column = Grid.Columns[i];
        // column.DefaultCellStyle.BackColor = Grid.DefaultCellStyle.ForeColor;
        // column.DefaultCellStyle.ForeColor = Grid.DefaultCellStyle.BackColor;
        // column.DefaultCellStyle.SelectionBackColor = Grid.DefaultCellStyle.SelectionForeColor;
        // column.DefaultCellStyle.SelectionForeColor = Grid.DefaultCellStyle.SelectionBackColor;
        column.DefaultCellStyle.BackColor = DefaultForeColors[i];
        // column.DefaultCellStyle.BackColor = Color.Black;
        column.DefaultCellStyle.ForeColor = DefaultBackColors[i];
        column.DefaultCellStyle.SelectionBackColor = DefaultSelectionForeColors[i];
        column.DefaultCellStyle.SelectionForeColor = DefaultSelectionBackColors[i];
      }
    }

    /// <summary>
    ///   Restores the foreground and background colours of both selected and unselected
    ///   cells in the grid to their defaults.
    /// </summary>
    public void RestoreToDefault() {
      for (int i = 0; i < Grid.Columns.Count; i++) {
        DataGridViewColumn column = Grid.Columns[i];
        column.DefaultCellStyle.BackColor = DefaultBackColors[i];
        column.DefaultCellStyle.ForeColor = DefaultForeColors[i];
        column.DefaultCellStyle.SelectionBackColor = DefaultSelectionBackColors[i];
        column.DefaultCellStyle.SelectionForeColor = DefaultSelectionForeColors[i];
      }
    }
  }
}