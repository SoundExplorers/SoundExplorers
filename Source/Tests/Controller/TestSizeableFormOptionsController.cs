using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller; 

public class TestSizeableFormOptionsController : SizeableFormOptionsController {
  public TestSizeableFormOptionsController(
    IView<SizeableFormOptionsController> view, string formName,
    QueryHelper queryHelper,
    SessionBase session) : base(view, formName) {
    QueryHelper = queryHelper;
    Session = session;
  }

  private QueryHelper QueryHelper { get; }
  private SessionBase Session { get; }

  protected override Option CreateOption(string name) {
    return new TestOption(QueryHelper, Session, name);
  }
}