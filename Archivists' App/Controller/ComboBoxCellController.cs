using System;
using System.Data;
using JetBrains.Annotations;
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
    public string ReferencedColumnName => _referencedColumnName ??
                                          (_referencedColumnName =
                                            TableController.GetReferencedColumnName(
                                              ColumnName));

    [NotNull]
    private Type ReferencedEntityListType => _referencedEntityListType ??
                                             (_referencedEntityListType =
                                               TableController
                                                 .GetReferencedEntityListType(
                                                   ColumnName));

    [NotNull]
    public string ReferencedTableName => _referencedTableName ??
                                         (_referencedTableName =
                                           TableController.GetReferencedTableName(
                                             ColumnName));

    [NotNull]
    public DataTable FetchReferencedTable() {
      var entityList = EntityListFactory.Create(ReferencedEntityListType);
      entityList.Populate();
      return entityList.Table;
    }
  }
}