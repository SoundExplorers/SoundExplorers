using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class MainController {
    private IOpen _databaseConnection;
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
    internal IOpen DatabaseConnection {
      get => _databaseConnection ?? new DatabaseConnection();
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
                                      (_statusBarOption =
                                        CreateOption("StatusBar", true));

    public string TableName {
      get => TableOption.StringValue;
      set => TableOption.StringValue = value;
    }

    private Option TableOption => _tableOption ??
                                  (_tableOption = CreateOption("Table"));

    private Option ToolBarOption => _toolBarOption ??
                                    (_toolBarOption = CreateOption("ToolBar", true));

    public void ConnectToDatabase() {
      DatabaseConnection.Open();
    }

    [ExcludeFromCodeCoverage]
    [NotNull]
    protected virtual Option CreateOption([NotNull] string name,
      object defaultValue = null) {
      return new Option(name, defaultValue);
    }
  }
}