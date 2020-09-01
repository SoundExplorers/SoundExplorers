using System;
using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {

    bool IsEditing { get; }
    bool IsThereACurrentMainEntity { get; }
    int MainCurrentIndex { get; }
    int ParentCurrentIndex { get; }

    [NotNull] object GetCurrentFieldValue([NotNull] string columnName);
    [NotNull] object GetFieldValue([NotNull] string columnName, int rowIndex);

    void OnRowError([NotNull] RowErrorEventArgs e);

    void OnRowUpdated([NotNull] string databaseUpdateMessage,
      string mediaTagsUpdateErrorMessage = null);
  }
}