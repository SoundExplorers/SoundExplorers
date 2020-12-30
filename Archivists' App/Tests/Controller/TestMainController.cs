using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  public class TestMainController : MainController {
    public TestMainController(IView<MainController> view,
      QueryHelper queryHelper, SessionBase session) : base(view) {
      QueryHelper = queryHelper;
      Session = session;
    }

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    protected override Option CreateOption(string name, object? defaultValue = null) {
      return new TestOption(QueryHelper, Session, name, defaultValue);
    }
  }
}