using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class CellControllerBase {
    protected CellControllerBase([NotNull] MainGridController mainGridController,
      [NotNull] string columnName) {
      MainGridController = mainGridController;
      Column = MainGridController.Columns[columnName];
    }

    [NotNull] internal BindingColumn Column { get; }
    [NotNull] protected MainGridController MainGridController { get; }
    [CanBeNull] public string TableName => MainGridController.TableName;
  }
}