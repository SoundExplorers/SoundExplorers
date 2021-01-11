using System;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  internal class TestEditorController : EditorController {
    public TestEditorController(
      Type mainListType, IEditorView view,
      QueryHelper queryHelper, SessionBase session) :
      base(view, mainListType,
        new TestMainController(new MockView<MainController>(), queryHelper, session)) {
      QueryHelper = queryHelper;
      Session = session;
    }

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }

    public override void Populate() {
      base.Populate();
      if (IsParentGridToBeShown) {
        // Emulates the parent grid behaviour where, after it has been populated,
        // it makes its last row current current,
        // which triggers the population of the main grid with that parent's children.
        View.ParentGrid.Controller.OnRowEnter(ParentList!.Count - 1);
      }
    }
  }
}