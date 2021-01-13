using System.Collections;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController(EditorController editorController) : base(editorController) { }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorController.ParentList!;

    private int PreviousRowIndex { get; set; }

    public IList GetChildrenForMainList(int rowIndex) {
      return List.GetChildrenForMainList(rowIndex)!;
    }

    public override void OnRowEnter(int rowIndex) {
      if (EditorController.IsPopulating || rowIndex == PreviousRowIndex) {
        PreviousRowIndex = rowIndex;
        return;
      }
      Debug.WriteLine(
        $"ParentGridController.OnRowEnter: row {rowIndex}, populating main grid.");
      EditorController.MainGrid.Populate(List.GetChildrenForMainList(rowIndex)!);
      PreviousRowIndex = rowIndex;
    }

    public override void Populate(IList? list = null) {
      PreviousRowIndex = -1;
      base.Populate(list);
    }
  }
}