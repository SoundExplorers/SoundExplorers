using System.Collections;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController([NotNull] IEditorView editorView) : base(editorView) { }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorView.Controller.ParentList;

    private bool IsPopulating { get; set; }

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    public void OnRowEnter(int rowIndex) {
      if (EditorView.IsFocusingParentGrid) {
        EditorView.IsFocusingParentGrid = false;
        return;
      }
      // if (List.IsDataLoadComplete) { // Does not work for parent grid
      if (IsPopulating) {
        if (rowIndex < List.Count - 1) {
          return;
        }
        IsPopulating = false;
      }
      EditorView.MainGrid.Populate(List.GetChildrenForMainList(rowIndex));
    }

    public override void Populate(IList list = null) {
      IsPopulating = true;
      base.Populate(list);
    }
  }
}