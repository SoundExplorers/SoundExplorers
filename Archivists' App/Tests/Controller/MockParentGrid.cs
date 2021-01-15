using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockParentGrid : MockGridBase, IParentGrid {
    public override int RowCount => Controller.BindingList!.Count;

    public new ParentGridController Controller {
      get => (ParentGridController)base.Controller;
      set => base.Controller = value;
    }
  }
}