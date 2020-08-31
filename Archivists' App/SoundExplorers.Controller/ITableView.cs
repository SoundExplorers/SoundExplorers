using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    void OnRowError([NotNull] RowErrorEventArgs e);

    void OnRowUpdated([NotNull] string databaseUpdateMessage,
      string mediaTagsUpdateErrorMessage = null);
  }
}