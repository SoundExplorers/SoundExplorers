using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    void EditMainGridCurrentCell();
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridRowCurrent(int rowIndex);
    void OnError();
    void OnRowAddedOrDeleted();

    void RestoreMainGridCurrentRowCellErrorValue(int columnIndex,
      [CanBeNull] object errorValue);

    void SelectCurrentRowOnly();
    void ShowErrorMessage([NotNull] string text);
    void ShowWarningMessage(string text);
  }
}