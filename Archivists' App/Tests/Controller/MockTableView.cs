﻿using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockTableView : ITableView {
    private TableController Controller { get; set; }
    public int FocusMainGridCellCount { get; private set; }
    public int FocusMainGridCellColumnIndex { get; private set; }
    public int FocusMainGridCellRowIndex { get; private set; }
    public int MakeMainGridInsertionRowCurrentCount { get; private set; }
    public int OnRowInsertedOrDeletedCount { get; private set; }
    public int ResumeEditCurrentCellCount { get; private set; }
    public int SelectCurrentRowOnlyCount { get; private set; }
    public int ShowErrorMessageCount { get; private set; }

    public void SetController(TableController controller) {
      Controller = controller;
    }

    public void FocusMainGridCell(int rowIndex, int columnIndex) {
      FocusMainGridCellCount++;
      FocusMainGridCellColumnIndex = columnIndex;
      FocusMainGridCellRowIndex = rowIndex;
    }

    public void MakeMainGridInsertionRowCurrent() {
      MakeMainGridInsertionRowCurrentCount++;
    }

    public void OnRowInsertedOrDeleted() {
      OnRowInsertedOrDeletedCount++;
    }

    public void ResumeEditCurrentCell(object errorValue) {
      ResumeEditCurrentCellCount++;
    }

    public void SelectCurrentRowOnly() {
      SelectCurrentRowOnlyCount++;
    }

    public void ShowErrorMessage(string text) {
      ShowErrorMessageCount++;
    }

    public void StartDatabaseUpdateErrorTimer() {
      Controller.ShowDatabaseUpdateError();
    }
  }
}