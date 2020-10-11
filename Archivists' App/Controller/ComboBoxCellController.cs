using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for a cell that supports an embedded combo box control.
  /// </summary>
  [UsedImplicitly]
  public class ComboBoxCellController : CellControllerBase {
    private string _referencedColumnName;
    private Type _referencedEntityListType;
    private string _referencedTableName;

    /// <summary>
    ///   Initialises a new instance of the <see cref="ComboBoxCellController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view of the combo box cell to be controlled.
    /// </param>
    /// <param name="tableController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the combo box cell.
    /// </param>
    public ComboBoxCellController([NotNull] IView<ComboBoxCellController> view,
      [NotNull] TableController tableController, [NotNull] string columnName) : base(
      tableController, columnName) {
      view.SetController(this);
    }

    [NotNull]
    public string ReferencedColumnName =>
      _referencedColumnName ?? (_referencedColumnName =
        TableController.Columns[ColumnName]?.ReferencedColumnName ??
        throw new NullReferenceException(nameof(EntityColumn.ReferencedColumnName)));

    [NotNull]
    private Type ReferencedEntityListType =>
      _referencedEntityListType ?? (_referencedEntityListType =
        TableController.Columns[ColumnName]?.ReferencedEntityListType ??
        throw new NullReferenceException(nameof(EntityColumn.ReferencedEntityListType)));

    [NotNull]
    public string ReferencedTableName => _referencedTableName ?? (_referencedTableName =
      TableController.Columns[ColumnName]?.ReferencedTableName ??
      throw new NullReferenceException(nameof(EntityColumn.ReferencedTableName)));

    [NotNull]
    public IBindingList FetchReferencedBindingList() {
      var entityList = Global.CreateEntityList(ReferencedEntityListType);
      entityList.Populate();
      return entityList.BindingList ?? throw new NullReferenceException("BindingList");
    }
  }
}