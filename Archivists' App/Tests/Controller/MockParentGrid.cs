using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockParentGrid : MockGridBase, IParentGrid {
    private new TestParentGridController Controller {
      get => (TestParentGridController)base.Controller;
      set => base.Controller = value;
    }

    ParentGridController IParentGrid.Controller => Controller;
    public override int RowCount => Controller.BindingList!.Count;

    public override void SetFocus() {
      base.SetFocus();
      ((MockGridBase)Controller.EditorController.View.MainGrid).Focused = false;
    }

    public void SetController(ParentGridController controller) {
      Controller = (TestParentGridController)controller;
    }
  }
}