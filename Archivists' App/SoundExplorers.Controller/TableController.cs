using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class TableController {
    private Option _gridSplitterDistanceOption;
    private Option _imageSplitterDistanceOption;

    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    /// <param name="tableName">
    ///   The name of the table whose data is to be displayed.
    /// </param>
    public TableController([NotNull] IView<TableController> view,
      [NotNull] string tableName) {
      view.SetController(this);
      TableName = tableName;
    }

    /// <summary>
    ///   User option for the position of the split between the
    ///   (upper) parent grid, if shown, and the (lower) main grid.
    /// </summary>
    public int GridSplitterDistance {
      get => GridSplitterDistanceOption.Int32Value;
      set => GridSplitterDistanceOption.Int32Value = value;
    }

    private Option GridSplitterDistanceOption =>
      _gridSplitterDistanceOption ?? (_gridSplitterDistanceOption =
        new Option($"{TableName}.GridSplitterDistance"));

    /// <summary>
    ///   User option for the position of the split between the
    ///   image data (above) and the image (below) in the image table editor.
    /// </summary>
    public int ImageSplitterDistance {
      get => ImageSplitterDistanceOption.Int32Value;
      set => ImageSplitterDistanceOption.Int32Value = value;
    }

    private Option ImageSplitterDistanceOption =>
      _imageSplitterDistanceOption ?? (_imageSplitterDistanceOption =
        new Option($"{TableName}.ImageSplitterDistance"));

    [NotNull] public string TableName { get; }
  }
}