using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Controller {
  public interface IGrid {
    int CurrentRowIndex { get; }
    IGridCellColorScheme CellColorScheme { get; }
    bool Enabled { get; set; }
    bool Focused { get; }

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    string Name { get; }

    int RowCount { get; }
    void Focus();

    /// <summary>
    ///   Makes the specified row current, which will set focus and raise OnRowEnter.
    /// </summary>
    void MakeRowCurrent(int rowIndex, bool async = false);

    void OnPopulated();

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    void Populate();
  }
}