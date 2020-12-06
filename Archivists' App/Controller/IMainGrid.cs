using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IMainGrid : IView<MainGridController> {
    void EditCurrentCell();
    void MakeCellCurrent(int rowIndex, int columnIndex);
    void MakeRowCurrent(int rowIndex);
    void OnRowAddedOrDeleted();
    void RestoreCurrentRowCellErrorValue(int columnIndex, [CanBeNull] object errorValue);
    void SelectCurrentRowOnly();
  }
}