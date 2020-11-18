using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  [UsedImplicitly]
  public class ComboBoxCellController : CellControllerBase {

    /// <summary>
    ///   Initialises a new instance of the <see cref="ComboBoxCellController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the combo box cell to be controlled.
    /// </param>
    /// <param name="editorController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    public ComboBoxCellController([NotNull] IView<ComboBoxCellController> view,
      [NotNull] EditorController editorController, [NotNull] string columnName) : base(
      editorController, columnName) {
      view.SetController(this);
    }

    private string CreateNoAvailableReferencesMessage() {
      return $"There are no {Column.ReferencedTableName} " + 
             $"{Column.ReferencedPropertyName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{Column.ReferencedTableName} table before you can select a " +
             $"{Column.ReferencedTableName} for a {TableName}.";
    }

    [NotNull]
    public object[] GetItems() {
      if (Column.ReferenceableItems.Count == 0) {
        EditorController.ShowWarningMessage(CreateNoAvailableReferencesMessage());
      }
      return Column.ReferenceableItems.ToArray();
    }

    [CanBeNull]
    public static string GetKey([CanBeNull] object value) {
      return ReferenceableItemList.ToSimpleKey(value);
    }

    public void OnCellValueChanged(int rowIndex,
      [NotNull] object cellValue) {
      if (!EditorController.IsInsertionRowCurrent) {
        return;
      }
      string simpleKey = ReferenceableItemList.ToSimpleKey(cellValue);
      if (Column.ReferenceableItems.ContainsKey(simpleKey)) {
        return;
      }
      EditorController.OnInsertionRowReferencedEntityNotFound(
        rowIndex, Column.Name, simpleKey);
    }
  }
}