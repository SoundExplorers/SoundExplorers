using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Controller {
  public class TestMainGridController : MainGridController {
    public TestMainGridController([NotNull] IEditorView editorView) :
      base(new MockMainGrid(), editorView) { }

    internal bool AutoValidate { get; set; }

    protected override ChangeAction LastChangeAction => TestUnsupportedLastChangeAction
      ? ChangeAction.None
      : base.LastChangeAction;

    internal bool TestUnsupportedLastChangeAction { get; set; }
    public new MockMainGrid Grid => (MockMainGrid)base.Grid;

    public override void OnRowEnter(int rowIndex) {
      if (AutoValidate) {
        if (List.HasRowBeenEdited) {
          OnRowValidated(CurrentRowIndex);
        }
        if (rowIndex == List.BindingList.Count) { // New row
          List.BindingList.AddNew();
        }
      }
      base.OnRowEnter(rowIndex);
    }

    internal void CreateAndGoToInsertionRow() {
      List.BindingList.AddNew();
      OnRowEnter(List.BindingList.Count - 1);
    }

    internal void SetComboBoxCellValue(
      int rowIndex, [NotNull] string columnName, [NotNull] object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      ((IBindingItem)List.BindingList[rowIndex]).SetPropertyValue(columnName, value);
      comboBoxCellController.OnCellValueChanged(0, value);
    }

    [NotNull]
    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      [NotNull] string columnName) {
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, this, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }
  }
}