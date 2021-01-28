using System.Data;
using System.Linq;
using SoundExplorers.Common;
using SoundExplorers.Controller;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Controller {
  public class TestMainGridController : MainGridController {
    public TestMainGridController(IMainGrid grid, EditorController editorController) :
      base(grid, editorController) { }

    protected override StatementType LastChangeAction => TestUnsupportedLastChangeAction
      ? StatementType.Select // Not used.
      : base.LastChangeAction;

    internal new EditorController EditorController => base.EditorController;
    internal bool TestUnsupportedLastChangeAction { get; set; }

    internal void CreateAndGoToNewRow() {
      List.BindingList!.AddNew();
      ((MockMainGrid)Grid).SetCurrentRowIndex(List.BindingList.Count - 1);
      OnRowEnter(Grid.CurrentRowIndex);
    }

    internal IBindingColumn GetBindingColumn(string propertyName) {
      return (from bindingColumn in BindingColumns
        where bindingColumn.PropertyName == propertyName
        select bindingColumn).First();
    }

    internal void SetComboBoxCellValue(
      int rowIndex, string columnName, object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      ((IBindingItem)List.BindingList![rowIndex]!).SetPropertyValue(columnName, value);
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