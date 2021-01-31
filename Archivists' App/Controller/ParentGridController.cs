using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController(
      // ReSharper disable once SuggestBaseTypeForParameter
      IParentGrid grid, EditorController editorController) : base(
      grid, editorController) { }

    private new IParentGrid Grid => (IParentGrid)base.Grid;
    private IMainGrid MainGrid => EditorController.View.MainGrid;
    private bool IsJustPopulated { get; set; }
    private bool IsScrollingLastRowIntoView { get; set; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorController.ParentList!;

    internal bool LastRowNeedsToBeScrolledIntoView {
      get {
        bool result = IsJustPopulated && List!.Count > 1;
        IsJustPopulated = result;
        return result;
      }
    }

    private int PreviousRowIndex { get; set; }

    public IdentifyingParentChildren
      GetIdentifyingParentChildrenForMainList(int rowIndex) {
      Debug.WriteLine(
        $"ParentGridController.GetIdentifyingParentChildrenForMainList: row {rowIndex}");
      return List.GetIdentifyingParentChildrenForMainList(rowIndex)!;
    }

    public override void OnPopulatedAsync() {
      IsJustPopulated = true;
      base.OnPopulatedAsync();
    }

    public override void OnRowEnter(int rowIndex) {
      Debug.WriteLine($"ParentGridController.OnRowEnter: row {rowIndex}");
      if (IsScrollingLastRowIntoView) {
        if (rowIndex == List.Count - 1) {
          IsScrollingLastRowIntoView = false;
          IsJustPopulated = false;
          // Show that the population process for the two grids is finished.
          EditorController.View.SetMouseCursorToDefault();
        } else {
          Grid.MakeRowCurrent(List.Count - 1, true);
        }
        PreviousRowIndex = rowIndex;
        return;
      }
      if (!IsPopulating && rowIndex != PreviousRowIndex) {
        Debug.WriteLine("    Populating main grid");
        EditorController.View.PopulateMainGridOnParentRowChanged(rowIndex);
      }
      PreviousRowIndex = rowIndex;
    }

    public override void PrepareForFocus() {
      Debug.WriteLine(
        $"ParentGridController.PrepareForFocus: MainGrid.CurrentRowIndex = {MainGrid.CurrentRowIndex}");
      base.PrepareForFocus();
      if (!IsPopulating && MainGrid.Controller.LastCurrentRowIndex < 0) {
        MainGrid.Controller.LastCurrentRowIndex = MainGrid.CurrentRowIndex;
      }
    }

    public override void Populate() {
      PreviousRowIndex = -1;
      base.Populate();
    }

    /// <summary>
    ///   Scrolls the last row into view. This should only need to be done if the
    ///   population process for the two grids is otherwise finished and there are at
    ///   least two rows.
    /// </summary>
    internal void ScrollLastRowIntoView() {
      Debug.WriteLine("ParentGridController.ScrollLastRowIntoView");
      IsScrollingLastRowIntoView = true;
      // As we are finishing the population process for the two grids, the last row is
      // expected to be already current though not necessarily scrolled into view. So the
      // first step is to make a different row current. In OnRowEnter for that row, the
      // last row will be made current again, which will scroll it into view. If there
      // were only one row, it would necessarily be scrolled into view already. So we
      // shall assume that, as we are being asked to scroll the last row into view, there
      // must be at least two rows.
      Grid.MakeRowCurrent(List.Count - 2);
    }
  }
}