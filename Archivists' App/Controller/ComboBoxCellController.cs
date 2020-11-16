using System;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  [UsedImplicitly]
  public class ComboBoxCellController : CellControllerBase {
    private ReferenceableItemList _referenceableItems;

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

    [CanBeNull] private string Format { get; set; }

    [NotNull]
    private ReferenceableItemList ReferenceableItems =>
      _referenceableItems ?? (_referenceableItems = CreateReferenceableItemList());

    private string CreateNoAvailableReferencesMessage() {
      return $"There are no {ReferenceableItems.ReferencedTableName} " + 
             $"{ReferenceableItems.ReferencedPropertyName}s " +
             "to choose between. You need to add at least one row to the " +
             $"{ReferenceableItems.ReferencedTableName} table before you can select a " +
             $"{ReferenceableItems.ReferencedTableName} for a {TableName}.";
    }

    [NotNull]
    protected virtual ReferenceableItemList CreateReferenceableItemList() {
      return new ReferenceableItemList(EditorController.Columns[ColumnName]);
    } 

    [NotNull]
    public object[] FetchItems([CanBeNull] string format) {
      Format = format;
      ReferenceableItems.Fetch(format);
      if (ReferenceableItems.Count == 0) {
        EditorController.ShowWarningMessage(CreateNoAvailableReferencesMessage());
      }
      return ReferenceableItems.ToArray();
    }

    [CanBeNull]
    public static string GetKey([CanBeNull] object value, [CanBeNull] string format) {
      return ReferenceableItemList.GetKey(value, format);
    }

    public void OnCellValueChanged(int rowIndex,
      [NotNull] object cellValue) {
      string formattedCellValue;
      if (cellValue is DateTime date) {
        formattedCellValue = date.ToString(Format);
      } else { // string
        formattedCellValue = cellValue.ToString();
      }
      if (ReferenceableItems.ContainsKey(formattedCellValue)) {
        return;
      }
      EditorController.OnReferencedEntityNotFound(
        rowIndex, ColumnName, formattedCellValue);
    }
  }
}