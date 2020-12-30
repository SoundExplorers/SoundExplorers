using System.Collections;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainGrid : IMainGrid {
    private TestMainGridController Controller { get; set; } = null!;
    public int EditCurrentCellCount { get; private set; }
    public int MakeCellCurrentCount { get; private set; }
    public int MakeCellCurrentColumnIndex { get; private set; }
    public int MakeCellCurrentRowIndex { get; private set; }
    public int MakeRowCurrentCount { get; private set; }
    public int MakeRowCurrentRowIndex { get; private set; }
    public int OnRowAddedOrDeletedCount { get; private set; }
    public int RestoreCurrentRowCellErrorValueCount { get; private set; }
    public int SelectCurrentRowOnlyCount { get; private set; }

    public void SetController(MainGridController controller) {
      Controller = (TestMainGridController)controller;
    }

    public void EditCurrentCell() {
      EditCurrentCellCount++;
    }

    public void MakeCellCurrent(int rowIndex, int columnIndex) {
      MakeCellCurrentCount++;
      MakeCellCurrentColumnIndex = columnIndex;
      MakeCellCurrentRowIndex = rowIndex;
    }

    public void MakeRowCurrent(int rowIndex) {
      //Debug.WriteLine($"MockEditorView.MakeRowCurrent: row {rowIndex}");
      MakeRowCurrentCount++;
      MakeRowCurrentRowIndex = rowIndex;
      Controller.OnRowEnter(rowIndex);
    }

    public void OnRowAddedOrDeleted() {
      OnRowAddedOrDeletedCount++;
    }

    public void Populate(IList? list = null) {
      throw new System.NotImplementedException();
    }

    public void RestoreCurrentRowCellErrorValue(int columnIndex,
      object errorValue) {
      RestoreCurrentRowCellErrorValueCount++;
    }

    public void SelectCurrentRowOnly() {
      SelectCurrentRowOnlyCount++;
    }
  }
}