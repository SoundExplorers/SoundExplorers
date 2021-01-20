using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public abstract class MockGridBase : IGrid {
    protected MockGridBase() {
      CellColorScheme = new MockGridCellColorScheme();
    }

    internal MockGridCellColorScheme CellColorScheme { get; }
    internal int MakeRowCurrentCount { get; private set; }
    internal int MakeRowCurrentRowIndex { get; private set; }
    protected GridControllerBase Controller { get; set; } = null!;
    IGridCellColorScheme IGrid.CellColorScheme => CellColorScheme;
    public bool Focused { get; internal set; }

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [ExcludeFromCodeCoverage]
    public string Name { get; internal set; } = null!;

    public abstract int RowCount { get; }

    public virtual void Focus() {
      Focused = true;
      Controller.OnFocusing();
    }

    public void MakeRowCurrent(int rowIndex) {
      //Debug.WriteLine($"MockGridBase.MakeRowCurrent: row {rowIndex}");
      MakeRowCurrentCount++;
      MakeRowCurrentRowIndex = rowIndex;
      Controller.OnRowEnter(rowIndex);
    }

    public void OnPopulated() {
      Controller.OnPopulatedAsync();
    }

    public void Populate(IList? list = null) {
      Controller.Populate(list);
    }
  }
}