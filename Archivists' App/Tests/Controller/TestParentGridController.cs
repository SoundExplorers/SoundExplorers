using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class TestParentGridController : ParentGridController {
    public TestParentGridController(EditorController editorController) :
      base(editorController) { }

    internal new EditorController EditorController => base.EditorController;
  }
}