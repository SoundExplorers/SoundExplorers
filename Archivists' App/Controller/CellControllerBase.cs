using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public abstract class CellControllerBase {
    protected CellControllerBase([NotNull] TableController tableController,
      [NotNull] string columnName) {
      ColumnName = columnName;
      TableController = tableController;
    }

    [NotNull] protected string ColumnName { get; }
    [NotNull] protected TableController TableController { get; }
    [CanBeNull] public string TableName => TableController.MainTable?.TableName;
  }
}