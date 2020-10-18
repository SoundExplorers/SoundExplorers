using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void EditMainGridCurrentCell();
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridInsertionRowCurrent();

    /// <summary>
    ///   Occurs when an entity corresponding to a row in the main grid
    ///   has been successfully inserted or deleted on the database.
    /// </summary>
    void OnRowAddedOrDeleted();

    void RestoreMainGridCurrentRowCellErrorValue(int columnIndex,
      [CanBeNull] object errorValue);

    void SelectCurrentRowOnly();
    void StartDatabaseUpdateErrorTimer();
    void ShowErrorMessage([NotNull] string text);
  }
}