using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController([NotNull] IEditorView editorView) : base(editorView) {
    }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorView.Controller.ParentList;

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    public override void OnRowEnter(int rowIndex) {
      base.OnRowEnter(rowIndex);
      if (IsPopulating) {
        return;
      }
      if (EditorView.IsFocusingParentGrid) {
        EditorView.IsFocusingParentGrid = false;
        return;
      }
      if (rowIndex == PreviousRowIndex) {
        return;
      }
      // Debug.WriteLine($"ParentGridController.OnRowEnter: row {rowIndex}, populating main grid.");
      EditorView.MainGrid.Populate(List.GetChildrenForMainList(rowIndex));
      PreviousRowIndex = rowIndex;
    }
  }
}