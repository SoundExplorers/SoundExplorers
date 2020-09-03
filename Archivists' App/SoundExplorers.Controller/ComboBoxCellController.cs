using System.Data;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  [UsedImplicitly]
  public class ComboBoxCellController {
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
      [NotNull] TableController tableController, [NotNull] string columnName) {
      view.SetController(this);
      ColumnName = columnName;
      TableController = tableController;
    }

    [NotNull] private string ColumnName { get; }

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

    [NotNull] public TableController TableController { get; }

    [NotNull]
    public DataTable GetReferencedTable() {
      return Factory<IEntityList>.Create(ReferencedTableName).Table;
    }
  }
}