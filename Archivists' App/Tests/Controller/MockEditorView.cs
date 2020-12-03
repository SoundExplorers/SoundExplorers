﻿using System.Diagnostics;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    private EditorController Controller { get; set; }
    public int EditMainGridCurrentCellCount { get; private set; }
    public int FocusMainGridCellCount { get; private set; }
    public int FocusMainGridCellColumnIndex { get; private set; }
    public int FocusMainGridCellRowIndex { get; private set; }
    public string LastErrorMessage { get; private set; }
    public int MakeMainGridRowCurrentCount { get; private set; }
    public int MakeMainGridRowCurrentRowIndex { get; private set; }
    public int OnRowAddedOrDeletedCount { get; private set; }
    public int RestoreMainGridCurrentRowCellErrorValueCount { get; private set; }
    public int SelectCurrentRowOnlyCount { get; private set; }
    public int ShowErrorMessageCount { get; private set; }
    public int ShowWarningMessageCount { get; private set; }

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    public void EditMainGridCurrentCell() {
      EditMainGridCurrentCellCount++;
    }

    public void FocusMainGridCell(int rowIndex, int columnIndex) {
      FocusMainGridCellCount++;
      FocusMainGridCellColumnIndex = columnIndex;
      FocusMainGridCellRowIndex = rowIndex;
    }

    public void MakeMainGridRowCurrent(int rowIndex) {
      Debug.WriteLine($"MockEditorView.MakeMainGridRowCurrent: row {rowIndex}");
      MakeMainGridRowCurrentCount++;
      MakeMainGridRowCurrentRowIndex = rowIndex;
      //Controller.OnMainGridRowValidated();
      Controller.OnMainGridRowEnter(rowIndex);
    }

    public void OnError() {
      Controller.ShowError();
    }

    public void OnRowAddedOrDeleted() {
      OnRowAddedOrDeletedCount++;
    }

    public void RestoreMainGridCurrentRowCellErrorValue(int columnIndex,
      object errorValue) {
      RestoreMainGridCurrentRowCellErrorValueCount++;
    }

    public void SelectCurrentRowOnly() {
      SelectCurrentRowOnlyCount++;
    }

    public void ShowErrorMessage(string text) {
      Debug.WriteLine("MockEditorView.ShowErrorMessage");
      LastErrorMessage = text;
      ShowErrorMessageCount++;
    }

    public void ShowWarningMessage(string text) {
      ShowWarningMessageCount++;
    }
  }
}