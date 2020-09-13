using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class MainController {
    private DatabaseConnection _databaseConnection;
    private Option _statusBarOption;
    private Option _tableOption;
    private Option _toolBarOption;

    public MainController([NotNull] IView<MainController> view) {
      view.SetController(this);
    }

    /// <summary>
    ///   The database connection,
    ///   whose default should only need to be replaced in tests.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    internal DatabaseConnection DatabaseConnection {
      get => _databaseConnection ?? new DatabaseConnection();
      // ReSharper disable once UnusedMember.Global
      set => _databaseConnection = value;
    }

    public bool IsStatusBarVisible {
      get => StatusBarOption.BooleanValue;
      set => StatusBarOption.BooleanValue = value;
    }

    public bool IsToolBarVisible {
      get => ToolBarOption.BooleanValue;
      set => ToolBarOption.BooleanValue = value;
    }

    private Option StatusBarOption => _statusBarOption ??
                                      (_statusBarOption = new Option("StatusBar", true));

    public string TableName {
      get => TableOption.StringValue;
      set => TableOption.StringValue = value;
    }

    private Option TableOption => _tableOption ??
                                  (_tableOption = new Option("Table"));

    private Option ToolBarOption => _toolBarOption ??
                                    (_toolBarOption = new Option("ToolBar", true));

    public void ConnectToDatabase() {
      DatabaseConnection.Open();
    }
  }
}