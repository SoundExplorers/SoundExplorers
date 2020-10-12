using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void BeginEditCurrentCell([CanBeNull]object initialValue);
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridInsertionRowCurrent();

    /// <summary>
    ///   Occurs when an entity corresponding to a row in the main grid
    ///   has been successfully inserted or deleted on the database.
    /// </summary>
    void OnRowInsertedOrDeleted();

    void SelectCurrentRowOnly();
    void StartDatabaseUpdateErrorTimer();
    void ShowErrorMessage([NotNull] string text);
  }
}