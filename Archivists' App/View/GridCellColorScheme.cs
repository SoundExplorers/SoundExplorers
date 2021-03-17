using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class GridCellColorScheme : IGridCellColorScheme {
    public GridCellColorScheme(GridBase grid) {
      Grid = grid;
      DefaultBackColors = CreateDefaultBackColors(); 
      DefaultForeColors = CreateDefaultForeColors(); 
      DefaultSelectionBackColors = CreateDefaultSelectionBackColors(); 
      DefaultSelectionForeColors = CreateDefaultSelectionForeColors(); 
    }

    private IList<Color> DefaultBackColors { get; }
    private IList<Color> DefaultForeColors { get; }
    private IList<Color> DefaultSelectionBackColors { get; }
    private IList<Color> DefaultSelectionForeColors { get; }
    private GridBase Grid { get; }

    /// <summary>
    ///   Inverts the foreground and background colours of both selected and unselected
    ///   cells in the grid, relative to their defaults. This indicates that the other
    ///   grid is focused.
    /// </summary>
    /// <remarks>
    ///   We use the grid's default colours for all columns, as we do not want URL cells
    ///   to appear with an inversion of their special blue-on-white colours when the
    ///   grid they are in is not focused.  It is less distracting if they don't stand
    ///   out in that context.
    /// </remarks>
    public void Invert() {
      for (int i = 0; i < Grid.Columns.Count; i++) {
        DataGridViewColumn column = Grid.Columns[i];
        column.DefaultCellStyle.BackColor = Grid.DefaultCellStyle.ForeColor;
        column.DefaultCellStyle.ForeColor = Grid.DefaultCellStyle.BackColor;
        column.DefaultCellStyle.SelectionBackColor = Grid.DefaultCellStyle.SelectionForeColor;
        column.DefaultCellStyle.SelectionForeColor = Grid.DefaultCellStyle.SelectionBackColor;
      }
    }

    /// <summary>
    ///   Restores the foreground and background colours of both selected and unselected
    ///   cells in the grid to their defaults.
    /// </summary>
    public void RestoreToDefault() {
      // Debug.WriteLine("GridCellColorScheme.RestoreToDefault");
      for (int i = 0; i < Grid.Columns.Count; i++) {
        DataGridViewColumn column = Grid.Columns[i];
        // Debug.WriteLine($"    {column.Name}");
        column.DefaultCellStyle.BackColor = DefaultBackColors[i];
        column.DefaultCellStyle.ForeColor = DefaultForeColors[i];
        column.DefaultCellStyle.SelectionBackColor = DefaultSelectionBackColors[i];
        column.DefaultCellStyle.SelectionForeColor = DefaultSelectionForeColors[i];
      }
    }

    private IList<Color> CreateDefaultBackColors() {
      return (
        from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.BackColor
          : Grid.DefaultCellStyle.BackColor).ToList();
    }

    private IList<Color> CreateDefaultForeColors() {
      return (
        from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.ForeColor
          : Grid.DefaultCellStyle.ForeColor).ToList();
    }

    private IList<Color> CreateDefaultSelectionBackColors() {
      return (
        from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.SelectionBackColor
          : Grid.DefaultCellStyle.SelectionBackColor).ToList();
    }

    private IList<Color> CreateDefaultSelectionForeColors() {
      return (
        from DataGridViewColumn column in Grid.Columns
        select Grid.Controller.IsUrlColumn(column.Name)
          ? column.DefaultCellStyle.SelectionForeColor
          : Grid.DefaultCellStyle.SelectionForeColor).ToList();
    }
  }
}