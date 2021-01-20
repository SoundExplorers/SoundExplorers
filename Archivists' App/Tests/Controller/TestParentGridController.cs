using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class TestParentGridController : ParentGridController {
    public TestParentGridController(IParentGrid grid, EditorController editorController) :
      base(grid, editorController) { }

    internal new EditorController EditorController => base.EditorController;
  }
}