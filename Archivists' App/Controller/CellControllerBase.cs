using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class CellControllerBase {
    protected CellControllerBase([NotNull] EditorController editorController,
      [NotNull] string columnName) {
      EditorController = editorController;
      Column = EditorController.Columns[columnName];
    }

    [NotNull] internal BindingColumn Column { get; }
    [NotNull] protected EditorController EditorController { get; }
    [CanBeNull] public string TableName => EditorController.MainTableName;
  }
}