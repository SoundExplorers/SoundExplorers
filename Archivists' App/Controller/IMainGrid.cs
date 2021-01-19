namespace SoundExplorers.Controller {
  public interface IMainGrid : IGrid, IView<MainGridController> {
    MainGridController Controller { get; }
    void EditCurrentCell();
    void MakeCellCurrent(int rowIndex, int columnIndex);
    void OnRowAddedOrDeleted();
    void RestoreCurrentRowCellErrorValue(int columnIndex, object? errorValue);
    void SelectCurrentRowOnly();
  }
}