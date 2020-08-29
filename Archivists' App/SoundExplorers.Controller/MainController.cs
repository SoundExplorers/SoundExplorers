using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  public class MainController {
    private Option _statusBarOption;
    private Option _tableOption;
    private Option _toolBarOption;

    public MainController([NotNull] IMainView view) {
      view.SetController(this);
    }

    public bool IsStatusBarVisible {
      get => StatusBarOption.BooleanValue;
      set => StatusBarOption.BooleanValue = value;
    }

    public bool IsToolBarVisible {
      get => ToolBarOption.BooleanValue;
      set => ToolBarOption.BooleanValue = value;
    }

    public string TableName {
      get => TableOption.StringValue;
      set => TableOption.StringValue = value;
    }

    private Option StatusBarOption => _statusBarOption ??
                                      (_statusBarOption = new Option("StatusBar", true));

    private Option TableOption => _tableOption ??
                                      (_tableOption = new Option("Table"));

    private Option ToolBarOption => _toolBarOption ??
                                  (_toolBarOption = new Option("ToolBar", true));
  }
}