using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void OnRowError([NotNull] RowErrorException e);
    void OnRowUpdated();
    void SetCurrentRowFieldValue([NotNull] string columnName, [NotNull] object newValue);
  }
}