namespace SoundExplorers.Controller {
  public interface IMainGrid : IGrid, IView<MainGridController> {
    void EditCurrentCell();
    void MakeCellCurrent(int rowIndex, int columnIndex);
    void MakeNewRowCurrent();
    void OnRowAddedOrDeleted();
    void RestoreCurrentRowCellErrorValue(int columnIndex, object? errorValue);
    void SelectCurrentRowOnly();
  }
}