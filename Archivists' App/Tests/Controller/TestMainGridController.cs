using System.Data;
using SoundExplorers.Controller;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Controller {
  public class TestMainGridController : MainGridController {
    public TestMainGridController(IEditorView editorView) :
      base(new MockMainGrid(), editorView) {
      CurrentRowIndex = -1;
    }

    internal bool AutoValidate { get; set; }
    private int CurrentRowIndex { get; set; }

    protected override StatementType LastChangeAction => TestUnsupportedLastChangeAction
      ? StatementType.Select // Not used.
      : base.LastChangeAction;

    internal bool TestUnsupportedLastChangeAction { get; set; }
    public new MockMainGrid Grid => (MockMainGrid)base.Grid;

    public override void OnRowEnter(int rowIndex) {
      CurrentRowIndex = rowIndex;
      // if (!EditorView.IsPopulating) { // Is this condition needed?
      //   CurrentRowIndex = rowIndex;
      // }
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

    public override void OnRowValidated(int rowIndex) {
      CurrentRowIndex = -1;
      base.OnRowValidated(rowIndex);
    }

    internal void CreateAndGoToInsertionRow() {
      List.BindingList.AddNew();
      OnRowEnter(List.BindingList.Count - 1);
    }

    internal void SetComboBoxCellValue(
      int rowIndex, string columnName, object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      ((IBindingItem)List.BindingList[rowIndex]!).SetPropertyValue(columnName, value);
      comboBoxCellController.OnCellValueChanged(0, value);
    }

    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      string columnName) {
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, this, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }
  }
}