using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class MainController {
    private IOpen? _databaseConnection;
    private Option? _tableOption;
    private Option? _toolBarOption;

    public MainController(IView<MainController> view) {
      view.SetController(this);
    }

    /// <summary>
    ///   The database connection,
    ///   whose default should only need to be replaced in tests.
    /// </summary>
    internal IOpen DatabaseConnection {
      get => _databaseConnection ?? new DatabaseConnection();
      set => _databaseConnection = value;
    }

    public bool IsClosing { get; set; }

    public bool IsToolBarVisible {
      get => ToolBarOption.BooleanValue;
      set => ToolBarOption.BooleanValue = value;
    }

    public string TableName {
      get => TableOption.StringValue;
      set => TableOption.StringValue = value;
    }

    private Option TableOption => _tableOption ??= CreateOption("Table");
    private Option ToolBarOption => _toolBarOption ??= CreateOption("ToolBar", true);

    public void ConnectToDatabase() {
      DatabaseConnection.Open();
    }

    [ExcludeFromCodeCoverage]
    protected virtual Option CreateOption(string name,
      object? defaultValue = null) {
      return new Option(name, defaultValue);
    }
  }
}