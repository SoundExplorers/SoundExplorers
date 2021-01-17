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
      return List.GetChildrenForMainList(rowIndex)!;
    }

    public override void OnRowEnter(int rowIndex) {
      Debug.WriteLine($"ParentGridController.OnRowEnter: row {rowIndex}");
      if (!EditorController.IsPopulating && rowIndex != PreviousRowIndex) {
        Debug.WriteLine("    Populating main grid");
        EditorController.View.PopulateMainGridOnParentRowChanged(rowIndex);
      }
      base.OnRowEnter(rowIndex);
    }

    public override void Populate(IList? list = null) {
      Debug.WriteLine("ParentGridController.Populate");
      base.Populate(list);
      Debug.WriteLine("ParentGridController.Populate END");
    }
  }
}