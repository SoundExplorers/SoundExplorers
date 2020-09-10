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
    public string ReferencedTableName => _referencedTableName ??
                                         (_referencedTableName =
                                           TableController.GetReferencedTableName(
                                             ColumnName));

    [NotNull]
    public DataTable FetchReferencedTable() {
      var entityList = EntityListFactory<IEntityListOld>.Create(ReferencedTableName);
      entityList.Fetch();
      return entityList.Table;
    }
  }
}