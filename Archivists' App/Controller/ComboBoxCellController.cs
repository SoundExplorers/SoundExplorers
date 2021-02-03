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

    private string CreateNoAvailableReferencesMessage() {
      return $"There are no {Column.ReferencedTableName} " +
             $"{Column.ReferencedPropertyName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{Column.ReferencedTableName} table before you can select a " +
             $"{Column.ReferencedTableName} for a {TableName}.";
    }

    public object[] GetItems() {
      if (Column.ReferenceableItems.Count == 0) {
        MainGridController.ShowWarningMessage(CreateNoAvailableReferencesMessage());
      }
      return Column.ReferenceableItems.ToArray();
    }

    public static string? GetKey(object? value) {
      return ReferenceableItemList.ToSimpleKey(value);
    }

    /// <summary>
    ///   As the combo boxes in the main grid are all dropdown lists, it is only possible
    ///   to attempt to update one with a value that is not available for selection by
    ///   pasting.  For the insertion row, we need to check for that here.
    /// </summary>
    /// <remarks>
    ///   For existing rows, this validation is done in
    ///   <see cref="MainGridController.OnCellEditException"/>.
    /// </remarks>
    public void OnCellValueChanged(int rowIndex,
      object? cellValue) {
      // It is impossible to paste null, so, when we detect it here, it must be a
      // programming artefact. And a null cell value has only been found when we can
      // ignore it: on pasting an invalid format date into Event.Newsletter on the
      // insertion row when there are no existing rows and Location has not been
      // specified. A format error message will be shown for the invalid Newsletter in
      // MainGridController.OnCellEditException.
      if (!MainGridController.IsInsertionRowCurrent || cellValue == null) {
        return;
      }
      string simpleKey = ReferenceableItemList.ToSimpleKey(cellValue)!;
      if (Column.ReferenceableItems.ContainsKey(simpleKey)) {
        return;
      }
      MainGridController.OnInsertionRowReferencedEntityNotFound(
        rowIndex, Column.PropertyName, simpleKey);
    }
  }
}