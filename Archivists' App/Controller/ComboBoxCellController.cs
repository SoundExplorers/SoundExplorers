using System;
using System.Collections.Generic;
using System.Linq;
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

    private IEntityList EntityList { get; set; }

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
    public object[] FetchItems([CanBeNull] string format) {
      EntityList = Global.CreateEntityList(ReferencedEntityListType);
      EntityList.Populate();
      if (EntityList.Count == 0) {
        TableController.ShowWarningMessage(CreateNoAvailableReferencesMessage());
      }
      return (from IEntity entity in EntityList
          select (object)new KeyValuePair<string, IEntity>(GetKey(entity, format), entity)
        )
        .ToArray();
    }

    [CanBeNull]
    public static string GetKey([CanBeNull] object value, [CanBeNull] string format) {
      if (value == null) {
        return null;
      }
      // The only non-string key expected, which therefore needs to be converted
      // to a formatted string is Newsletter.Date.
      bool isDateKey = !string.IsNullOrWhiteSpace(format);
      return isDateKey
        ? ((Newsletter)value).Date.ToString(format)
        : ((IEntity)value).SimpleKey;
    }

    private string CreateNoAvailableReferencesMessage() {
      return $"There are no {ReferencedTableName} {ReferencedColumnName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{ReferencedTableName} table before you can select a " +
             $"{ReferencedTableName} for a {TableName}.";
    }
  }
}