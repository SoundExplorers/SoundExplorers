using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    void EditMainGridCurrentCell();
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridRowCurrent(int rowIndex);

    /// <summary>
    ///   Occurs when an entity corresponding to a row in the main grid
    ///   has been successfully inserted or deleted on the database.
    /// </summary>
    void OnRowAddedOrDeleted();

    void RestoreMainGridCurrentRowCellErrorValue(int columnIndex,
      [CanBeNull] object errorValue);

    void SelectCurrentRowOnly();
    void ShowErrorMessage([NotNull] string text);
    void ShowWarningMessage(string text);
    void StartOnErrorTimer();
  }
}