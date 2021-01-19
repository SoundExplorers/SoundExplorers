using System.Collections;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController(
      // ReSharper disable once SuggestBaseTypeForParameter
      IParentGrid grid, EditorController editorController) : base(
      grid, editorController) { }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorController.ParentList!;

    public IList GetChildrenForMainList(int rowIndex) {
      Debug.WriteLine($"ParentGridController.GetChildrenForMainList: row {rowIndex}");
      return List.GetChildrenForMainList(rowIndex)!;
    }
    
    // public void OnGotFocus() {
    //   Debug.WriteLine("ParentGridController.OnGotFocus");
    //   if (EditorController.View.MainGrid.Controller.IsJustPopulated) {
    //     EditorController.View.MainGrid.Controller.IsJustPopulated = false;
    //     EditorController.View.SetCursorToDefault();
    //   }
    // }

    public override void OnRowEnter(int rowIndex) {
      Debug.WriteLine($"ParentGridController.OnRowEnter: row {rowIndex}");
      if (!IsPopulating && rowIndex != PreviousRowIndex) {
        Debug.WriteLine("    Populating main grid");
        EditorController.View.PopulateMainGridOnParentRowChanged(rowIndex);
      }
      base.OnRowEnter(rowIndex);
    }
  }
}