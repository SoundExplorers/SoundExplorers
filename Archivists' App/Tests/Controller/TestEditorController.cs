using System;
using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  internal class TestEditorController : EditorController {
    public TestEditorController([NotNull] IEditorView view, [NotNull] Type mainListType,
      [NotNull] QueryHelper queryHelper, [NotNull] SessionBase session) :
      base(view, mainListType) {
      QueryHelper = queryHelper;
      Session = session;
    }

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    protected override ChangeAction LastChangeAction => TestUnsupportedLastChangeAction
      ? ChangeAction.None
      : base.LastChangeAction;

    public bool TestUnsupportedLastChangeAction { get; set; }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }

    // public void CreateEntityListData([NotNull] Type entityListType,
    //   [NotNull] IList list) {
    //   var entityList = CreateEntityList(entityListType);
    //   entityList.Populate(list);
    // }

    public IEntityList GetMainList() {
      return MainList;
    }
  }
}