using System.Data;
using System.Linq;
using SoundExplorers.Common;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller; 

public class TestMainGridController : MainGridController {
  public TestMainGridController(IMainGrid grid, EditorController editorController) :
    base(grid, editorController) { }

  protected override StatementType LastChangeAction => TestUnsupportedLastChangeAction
    ? StatementType.Select // Not used.
    : base.LastChangeAction;

  internal new EditorController EditorController => base.EditorController;
  internal new bool IsFixingFocus => base.IsFixingFocus;
  internal int ReplaceErrorBindingValueWithOriginalCount { get; private set; }
  internal bool TestUnsupportedLastChangeAction { get; set; }

  public override void OnRowEnter(int rowIndex) {
    if (!IsPopulating && rowIndex != Grid.CurrentRowIndex &&
        Grid.CurrentRowIndex == BindingList.Count - 1) {
      OnRowValidated(Grid.CurrentRowIndex);
    }
    base.OnRowEnter(rowIndex);
  }

  protected override void ReplaceErrorBindingValueWithOriginal() {
    ReplaceErrorBindingValueWithOriginalCount++;
    base.ReplaceErrorBindingValueWithOriginal();
  }

  internal void CreateAndGoToNewRow() {
    List.BindingList.AddNew();
    ((MockMainGrid)Grid).SetCurrentRowIndex(List.BindingList.Count - 1);
    OnRowEnter(Grid.CurrentRowIndex);
  }

  internal IBindingColumn GetBindingColumn(string propertyName) {
    return (from bindingColumn in BindingColumns
      where bindingColumn.PropertyName == propertyName
      select bindingColumn).First();
  }
}