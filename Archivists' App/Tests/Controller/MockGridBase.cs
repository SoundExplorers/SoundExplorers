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
    protected GridControllerBase Controller { get; set; } = null!;
    public int CurrentRowIndex { get; protected set; }
    IGridCellColorScheme IGrid.CellColorScheme => CellColorScheme;
    public bool Focused { get; internal set; }

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [ExcludeFromCodeCoverage]
    public string Name { get; internal set; } = null!;

    public abstract int RowCount { get; }

    public void MakeRowCurrent(int rowIndex, bool async = false) {
      //Debug.WriteLine($"MockGridBase.MakeRowCurrent: row {rowIndex}");
      MakeRowCurrentCount++;
      CurrentRowIndex = rowIndex;
      Controller.OnRowEnter(rowIndex);
    }

    public void OnPopulated() {
      Controller.OnPopulatedAsync();
    }

    public void Populate(IList? list = null) {
      Controller.Populate(list);
    }

    public virtual void SetFocus() {
      Focused = true;
      Controller.OnFocusing();
    }

    internal void SetCurrentRowIndex(int rowIndex) {
      CurrentRowIndex = rowIndex;
    }
  }
}