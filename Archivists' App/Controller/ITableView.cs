using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void FocusMainGridCell(int rowIndex, int columnIndex);
    void MakeMainGridInsertionRowCurrent();
    void OnRowUpdated();
    void SelectCurrentRowOnly();
    void StartDatabaseUpdateErrorTimer();
    void ShowErrorMessage([NotNull] string text);
  }
}