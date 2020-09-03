using System;
using System.IO;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports the editing of a file path.
  /// </summary>
  [UsedImplicitly]
  public class PathCellController : CellControllerBase {
    /// <summary>
    ///   Initialises a new instance of the <see cref="PathCellController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the path cell to be controlled.
    /// </param>
    /// <param name="tableController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the path cell.
    /// </param>
    public PathCellController([NotNull] IView<PathCellController> view,
      [NotNull] TableController tableController, [NotNull] string columnName) : base(
      tableController, columnName) {
      view.SetController(this);
    }

    [NotNull]
    public DirectoryInfo GetDefaultFolder() {
      DirectoryInfo result = null;
      switch (ColumnName) {
        case "AudioPath": // Piece.AudioPath
          result = Piece.DefaultAudioFolder;
          break;
        case "Path":
          // if (TableName == "Image") {
          //   result = Image.DefaultFolder; // Image.Path
          // } else if (TableName == "Newsletter") {
          if (TableController.TableName == "Newsletter") { // Newsletter.Path
            result = Newsletter.DefaultFolder;
          } else {
            throw new NotSupportedException(
              TableController + ".Path is not supported.");
          }
          break;
        case "VideoPath": // Piece.VideoPath
          result = Piece.DefaultVideoFolder;
          break;
      } //End of switch
      return result ?? throw new InvalidOperationException("Not a path.");
    }
  }
}