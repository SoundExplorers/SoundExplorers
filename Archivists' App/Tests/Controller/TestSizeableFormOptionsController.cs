using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  public class TestSizeableFormOptionsController : SizeableFormOptionsController {
    public TestSizeableFormOptionsController(
      [NotNull] IView<SizeableFormOptionsController> view, [NotNull] string formName,
      [NotNull] QueryHelper queryHelper,
      [NotNull] SessionBase session) : base(view, formName) {
      QueryHelper = queryHelper;
      Session = session;
    }

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }
  }
}