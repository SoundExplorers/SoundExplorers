using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View; 

/// <summary>
///   The colour scheme of a grid's cells, with support for inverting the foreground
///   and background colours of a parent or child grid when the other grid of the pair
///   is focused.
/// </summary>
internal class GridCellColorScheme : IGridCellColorScheme {
  public GridCellColorScheme(DataGridView grid) {
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
  private DataGridView Grid { get; }

  /// <summary>
  ///   Inverts the foreground and background colours of both selected and unselected
  ///   cells in the grid, relative to their defaults. This indicates that the other
  ///   grid is focused.
  /// </summary>
  /// <remarks>
  ///   We use the grid's default colours for all columns, as we do not want URL cells,
  ///   the only ones for which column-specific defaults have actually been defined,
  ///   to appear with an inversion of their special blue-on-white colours when the
  ///   grid they are in is not focused. It could be misleading or distracting if they
  ///   were to stand out in that context.
  /// </remarks>
  public void Invert() {
    for (int i = 0; i < Grid.Columns.Count; i++) {
      DataGridViewColumn column = Grid.Columns[i];
      column.DefaultCellStyle.BackColor = Grid.DefaultCellStyle.ForeColor;
      column.DefaultCellStyle.ForeColor = Grid.DefaultCellStyle.BackColor;
      column.DefaultCellStyle.SelectionBackColor =
        Grid.DefaultCellStyle.SelectionForeColor;
      column.DefaultCellStyle.SelectionForeColor =
        Grid.DefaultCellStyle.SelectionBackColor;
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

  /// <summary>
  ///   Creates a list of the default background colours of all columns.
  /// </summary>
  /// <remarks>
  ///   Currently, a specific background colour is specified only for URL columns. All
  ///   other columns default their background colours to the default for the grid.
  /// </remarks>
  private IList<Color> CreateDefaultBackColors() {
    return (
      from DataGridViewColumn column in Grid.Columns
      select column.DefaultCellStyle.BackColor).ToList();
  }

  /// <summary>
  ///   Creates a list of the default foreground colours of all columns.
  /// </summary>
  /// <remarks>
  ///   Currently, a specific foreground colour is specified only for URL columns. All
  ///   other columns default their foreground colours to the default for the grid.
  /// </remarks>
  private IList<Color> CreateDefaultForeColors() {
    return (
      from DataGridViewColumn column in Grid.Columns
      select column.DefaultCellStyle.ForeColor).ToList();
  }

  /// <summary>
  ///   Creates a list of the default selection background colours of all columns.
  /// </summary>
  /// <remarks>
  ///   Currently, all columns default their selection background colours to the
  ///   default for the grid.
  /// </remarks>
  private IList<Color> CreateDefaultSelectionBackColors() {
    return (
      from DataGridViewColumn column in Grid.Columns
      select column.DefaultCellStyle.SelectionBackColor).ToList();
  }

  /// <summary>
  ///   Creates a list of the default selection foreground colours of all columns.
  /// </summary>
  /// <remarks>
  ///   Currently, all columns default their selection foreground colours to the
  ///   default for the grid.
  /// </remarks>
  private IList<Color> CreateDefaultSelectionForeColors() {
    return (
      from DataGridViewColumn column in Grid.Columns
      select column.DefaultCellStyle.SelectionForeColor).ToList();
  }
}