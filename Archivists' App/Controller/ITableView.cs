using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridInsertionRowCurrent();

    /// <summary>
    ///   Occurs when an entity corresponding to a row in the main grid
    ///   has been successfully inserted or deleted on the database.
    /// </summary>
    void OnRowInsertedOrDeleted();

    /// <summary>
    ///   Resumes editing the current cell,
    ///   with the cell initially repopulated with the specified error value.
    ///   This will allow the user to correct the error value or,
    ///   by cancelling the edit, restore the original value.
    /// </summary>
    void ResumeEditCurrentCell([CanBeNull] object errorValue);

    void SelectCurrentRowOnly();
    void StartDatabaseUpdateErrorTimer();
    void ShowErrorMessage([NotNull] string text);
  }
}