using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  public class TestComboBoxCellController : ComboBoxCellController {
    public TestComboBoxCellController([NotNull] IView<ComboBoxCellController> view,
      [NotNull] EditorController editorController, [NotNull] string columnName,
      [NotNull] SessionBase session) : base(
      view, editorController, columnName) {
      Session = session;
    }

    private SessionBase Session { get; }

    protected override IEntityList CreateEntityList() {
      var result = base.CreateEntityList();
      result.Session = Session;
      return result;
    }
  }
}