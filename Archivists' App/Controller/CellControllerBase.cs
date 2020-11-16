using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public abstract class CellControllerBase {
    protected CellControllerBase([NotNull] EditorController editorController,
      [NotNull] string columnName) {
      ColumnName = columnName;
      EditorController = editorController;
    }

    [NotNull] internal string ColumnName { get; }
    [NotNull] protected EditorController EditorController { get; }
    [CanBeNull] public string TableName => EditorController.MainTableName;
  }
}