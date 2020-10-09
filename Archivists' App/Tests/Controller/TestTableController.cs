using System;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  internal class TestTableController : TableController {
    public TestTableController([NotNull] ITableView view, [NotNull] Type mainListType,
      [NotNull] SessionBase session) :
      base(view, mainListType) {
      Session = session;
    }

    private SessionBase Session { get; }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }
  }
}