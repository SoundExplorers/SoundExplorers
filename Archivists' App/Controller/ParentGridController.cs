using System.Collections;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController(IEditorView editorView) : base(editorView) { }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorView.Controller.ParentList!;

    private int PreviousRowIndex { get; set; }

    public IList GetChildrenForMainList(int rowIndex) {
      return List.GetChildrenForMainList(rowIndex)!;
    }

    /// <summary>
    ///   An existing row on the parent grid has been entered. So the main grid will be
    ///   populated with the required child entities of the entity at the specified row index.
    /// </summary>
    public void OnRowEnter(int rowIndex) {
      if (EditorView.IsPopulating) {
        return;
      }
      if (rowIndex == PreviousRowIndex) {
        return;
      }
      Debug.WriteLine(
        $"ParentGridController.OnRowEnter: row {rowIndex}, populating main grid.");
      EditorView.MainGrid.Populate(List.GetChildrenForMainList(rowIndex)!);
      PreviousRowIndex = rowIndex;
    }

    public override void Populate(IList? list = null) {
      PreviousRowIndex = -1;
      base.Populate(list);
    }
  }
}