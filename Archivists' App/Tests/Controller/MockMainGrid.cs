using System;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainGrid : MockGridBase, IMainGrid {
    public new TestMainGridController Controller {
      get => (TestMainGridController)base.Controller;
      set => base.Controller = value;
    }

    public int EditCurrentCellCount { get; private set; }
    public int MakeCellCurrentCount { get; private set; }
    public int MakeCellCurrentColumnIndex { get; private set; }
    public int MakeCellCurrentRowIndex { get; private set; }
    public int OnRowAddedOrDeletedCount { get; private set; }
    public int RestoreCurrentRowCellErrorValueCount { get; private set; }
    public int SelectCurrentRowOnlyCount { get; private set; }

    /// <summary>
    ///   Gets the grid row count including the new row.
    /// </summary>
    public override int RowCount => Controller.BindingList!.Count + 1;

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

    public void MakeNewRowCurrent() {
      throw new NotImplementedException();
    }

    public void OnRowAddedOrDeleted() {
      OnRowAddedOrDeletedCount++;
    }

    public void RestoreCurrentRowCellErrorValue(int columnIndex,
      object? errorValue) {
      RestoreCurrentRowCellErrorValueCount++;
    }

    public void SelectCurrentRowOnly() {
      SelectCurrentRowOnlyCount++;
    }
  }
}