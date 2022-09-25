using SoundExplorers.Controller;

namespace SoundExplorers.View; 

internal class ParentGrid : GridBase, IParentGrid {
  public ParentGrid() {
    AllowUserToAddRows = false;
    AllowUserToDeleteRows = false;
    MultiSelect = false;
    ReadOnly = true;
  }

  public new ParentGridController Controller {
    get => (ParentGridController)base.Controller;
    private set => base.Controller = value;
  }

  public void SetController(ParentGridController controller) {
    Controller = controller;
  }
}