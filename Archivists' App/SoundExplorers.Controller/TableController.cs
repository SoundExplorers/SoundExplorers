using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
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

    private Option GridSplitterDistanceOption => _gridSplitterDistanceOption ??
                                                 (_gridSplitterDistanceOption =
                                                   new Option("Table"));

    //private Option GridSplitterDistanceOption { get; set; }
    private Option ImageSplitterDistanceOption { get; set; }
    [NotNull] public string TableName { get; }
  }
}