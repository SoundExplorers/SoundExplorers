using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  public class TestMainController : MainController {
    public TestMainController(IMainView view,
      QueryHelper queryHelper, SessionBase session) : base(view) {
      QueryHelper = queryHelper;
      Session = session;
      MockBackupManager = new MockBackupManager();
    }

    internal MockBackupManager MockBackupManager { get; }

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }
    
    protected override IBackupManager CreateBackupManager() {
      return MockBackupManager;
    }

    protected override Option CreateOption(string name, object? defaultValue = null) {
      return new TestOption(QueryHelper, Session, name, defaultValue);
    }

    protected override string GetProductName() {
      return "Sound Explorers Audio Archive";
    }
  }
}