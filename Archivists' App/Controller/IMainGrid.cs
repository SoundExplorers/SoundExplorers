using System.Collections;

namespace SoundExplorers.Controller {
  public interface IMainGrid : IView<MainGridController> {
    void EditCurrentCell();
    void MakeCellCurrent(int rowIndex, int columnIndex);
    void MakeRowCurrent(int rowIndex);
    void OnRowAddedOrDeleted();
    void Populate(IList? list = null);
    void RestoreCurrentRowCellErrorValue(int columnIndex, object? errorValue);
    void SelectCurrentRowOnly();
  }
}