using System;
using System.IO;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a path cell.
  /// </summary>
  [UsedImplicitly]
  public class PathCellController {
    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the path cell to be controlled.
    /// </param>
    /// <param name="tableName">
    ///   The name of the table whose data is displayed.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the path cell.
    /// </param>
    public PathCellController([NotNull] IView<PathCellController> view,
      [NotNull] string tableName, [NotNull] string columnName) {
      view.SetController(this);
      ColumnName = columnName;
      TableName = tableName;
    }

    [NotNull] private string ColumnName { get; }
    [NotNull] private string TableName { get; }

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
          if (TableName == "Newsletter") { // Newsletter.Path
            result = Newsletter.DefaultFolder;
          } else {
            throw new NotSupportedException(
              TableName + ".Path is not supported.");
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