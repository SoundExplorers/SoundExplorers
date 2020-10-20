using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  [UsedImplicitly]
  public class ComboBoxCellController : CellControllerBase {
    private string _referencedColumnName;
    private Type _referencedEntityListType;
    private string _referencedTableName;

    /// <summary>
    ///   Initialises a new instance of the <see cref="ComboBoxCellController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the combo box cell to be controlled.
    /// </param>
    /// <param name="tableController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    public ComboBoxCellController([NotNull] IView<ComboBoxCellController> view,
      [NotNull] TableController tableController, [NotNull] string columnName) : base(
      tableController, columnName) {
      view.SetController(this);
    }

    [NotNull]
    private string ReferencedColumnName =>
      _referencedColumnName ?? (_referencedColumnName =
        TableController.Columns[ColumnName]?.ReferencedColumnName ??
        throw new NullReferenceException(nameof(EntityColumn.ReferencedColumnName)));

    [NotNull]
    private Type ReferencedEntityListType =>
      _referencedEntityListType ?? (_referencedEntityListType =
        TableController.Columns[ColumnName]?.ReferencedEntityListType ??
        throw new NullReferenceException(nameof(EntityColumn.ReferencedEntityListType)));

    [NotNull]
    private string ReferencedTableName => _referencedTableName ?? (_referencedTableName =
      TableController.Columns[ColumnName]?.ReferencedTableName ??
      throw new NullReferenceException(nameof(EntityColumn.ReferencedTableName)));

    [NotNull]
    public IBindingList FetchBindingList([CanBeNull] string format) {
      var entityList = Global.CreateEntityList(ReferencedEntityListType);
      entityList.Populate();
      if (entityList.Count == 0) {
        throw new ObjectNotFoundException(CreateNoAvailableReferencesErrorMessage());
      }
      var result = new BindingList<KeyValuePair<string, object>>();
      // The only non-string key expected, which therefore needs to be converted
      // to a formatted string is Newsletter.Date.
      bool isDateKey = !string.IsNullOrWhiteSpace(format);
      foreach (IEntity entity in entityList) {
        string key = isDateKey
          ? ((Newsletter)entity).Date.ToString(format)
          : entity.SimpleKey;
        result.Add(new KeyValuePair<string, object>(key, entity));
      }
      return result;
    }

    private string CreateNoAvailableReferencesErrorMessage() {
      return $"There are no {ReferencedTableName} {ReferencedColumnName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{ReferencedTableName} table before you can select a " + 
             $"{ReferencedTableName} for a {TableName}.";
    }
  }
}