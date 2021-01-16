using System.Collections;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public abstract class MockGridBase : IGrid {
    internal int FocusCount { get; private set; }
    internal int InvertCellColorSchemeCount { get; private set; }
    internal int MakeRowCurrentCount { get; private set; }
    internal int MakeRowCurrentRowIndex { get; private set; }
    internal int RestoreCellColorSchemeToDefaultCount { get; private set; }
    protected GridControllerBase Controller { get; set; } = null!;
    public bool Focused { get; internal set; }
    public abstract int RowCount { get; }

    public virtual void Focus() {
      Focused = true;
      FocusCount++;
    }

    public void InvertCellColorScheme() {
      InvertCellColorSchemeCount++;
    }

    public void MakeRowCurrent(int rowIndex) {
      //Debug.WriteLine($"MockEditorView.MakeRowCurrent: row {rowIndex}");
      MakeRowCurrentCount++;
      MakeRowCurrentRowIndex = rowIndex;
      Controller.OnRowEnter(rowIndex);
    }

    public void Populate(IList? list = null) {
      Controller.Populate(list);
    }

    public void RestoreCellColorSchemeToDefault() {
      RestoreCellColorSchemeToDefaultCount++;
    }
  }
}