using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Common;

namespace SoundExplorers.Controller {
  public interface ITableView : IView<TableController> {
    bool IsEditing { get; }
    bool IsThereACurrentMainEntity { get; }
    int MainCurrentIndex { get; }
    int ParentCurrentIndex { get; }

    [NotNull]
    object GetCurrentRowFieldValue([NotNull] string columnName);

    [NotNull]
    object GetFieldValue([NotNull] string columnName, int rowIndex);

    [NotNull]
    IDictionary<string, object> GetFieldValues(int rowIndex);

    void OnDatabaseUpdated();
    void OnDatabaseUpdateError([NotNull] Exception exception);
    void OnRowError([NotNull] RowErrorEventArgs e);

    void OnRowUpdated([NotNull] string databaseUpdateMessage,
      string mediaTagsUpdateErrorMessage = null);

    void SetCurrentRowFieldValue([NotNull] string columnName, [NotNull] object newValue);

    void SetFieldValue([NotNull] string columnName, int rowIndex,
      [NotNull] object newValue);
  }
}