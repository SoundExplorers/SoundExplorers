using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  public class ComboBoxCellController : CellControllerBase {
    /// <summary>
    ///   Initialises a new instance of the <see cref="ComboBoxCellController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the combo box cell to be controlled.
    /// </param>
    /// <param name="mainGridController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    public ComboBoxCellController(IView<ComboBoxCellController> view,
      MainGridController mainGridController,
      string columnName) : base(
      mainGridController, columnName) {
      view.SetController(this);
    }

    public object[] GetItems() {
      if (Column.ReferenceableItems!.Count == 0) {
        MainGridController.ShowWarningMessage(CreateNoAvailableReferencesMessage());
      }
      return Column.ReferenceableItems.ToArray();
    }

    public static string GetKey(object value) {
      return ReferenceableItemList.ToSimpleKey(value);
    }

    private string CreateNoAvailableReferencesMessage() {
      return $"There are no {Column.ReferencedTableName} " +
             $"{Column.ReferencedPropertyName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{Column.ReferencedTableName} table before you can select a " +
             $"{Column.ReferencedTableName} for a {TableName}.";
    }
  }
}