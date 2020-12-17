using System;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  internal class TestEditorController : EditorController {
    public TestEditorController(
      [NotNull] Type mainListType, IEditorView view,
      [NotNull] QueryHelper queryHelper, [NotNull] SessionBase session) :
      base(view, mainListType,
        new TestMainController(new MockView<MainController>(), queryHelper, session)) {
      QueryHelper = queryHelper;
      Session = session;
    }

    [NotNull] private QueryHelper QueryHelper { get; }
    [NotNull] private SessionBase Session { get; }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }
  }
}