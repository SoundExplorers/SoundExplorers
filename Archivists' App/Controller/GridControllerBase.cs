using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase(IGrid grid, EditorController editorController) {
      Grid = grid;
      EditorController = editorController;
    }

    public IBindingList? BindingList => List.BindingList;
    protected IGrid Grid { get; }
    protected bool IsPopulating { get; private set; }
    protected int PreviousRowIndex { get; private set; }

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public void OnFocusing() {
      Debug.WriteLine($"GridControllerBase.OnFocusing {Grid.Name}");
      if (EditorController.IsParentGridToBeShown && !IsPopulating) {
        Grid.CellColorScheme.RestoreToDefault();
        EditorController.GetOtherGrid(Grid).CellColorScheme.Invert();
      }
    }

    public virtual void OnRowEnter(int rowIndex) {
      Debug.WriteLine($"GridControllerBase.OnRowEnter {Grid.Name}: row {rowIndex}");
      PreviousRowIndex = rowIndex;
    }

    public virtual void OnPopulatedAsync() {
      Debug.WriteLine($"GridControllerBase.OnPopulatedAsync {Grid.Name}");
      IsPopulating = false;
      if (Grid.RowCount > 0) {
        Grid.MakeRowCurrent(Grid.RowCount - 1);
      }
    }

    public void Populate(IList? list = null) {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      PreviousRowIndex = -1;
      List.Populate(list);
      Grid.OnPopulated();
    }
  }
}