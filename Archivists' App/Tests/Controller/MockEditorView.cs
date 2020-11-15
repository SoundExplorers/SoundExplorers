using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    private EditorController Controller { get; set; }
    public int EditMainGridCurrentCellCount { get; private set; }
    public int FocusMainGridCellCount { get; private set; }
    public int FocusMainGridCellColumnIndex { get; private set; }
    public int FocusMainGridCellRowIndex { get; private set; }

    public string LastErrorMessage { get; private set; }
    public int MakeMainGridInsertionRowCurrentCount { get; private set; }
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

    public void MakeMainGridInsertionRowCurrent() {
      MakeMainGridInsertionRowCurrentCount++;
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
      LastErrorMessage = text;
      ShowErrorMessageCount++;
    }

    public void ShowWarningMessage(string text) {
      ShowWarningMessageCount++;
    }

    public void StartDatabaseUpdateErrorTimer() {
      Controller.ShowDatabaseUpdateError();
    }
  }
}