using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    /// <summary>
    ///   Makes the insertion row of the main grid current.
    /// </summary>
    void MakeInsertionRowCurrent();

    void OnDatabaseUpdateError([NotNull] DatabaseUpdateErrorException e);
    void OnRowUpdated();
    void SelectCurrentRowOnly();
    void SetCurrentRowFieldValue([NotNull] string columnName, [NotNull] object newValue);
  }
}