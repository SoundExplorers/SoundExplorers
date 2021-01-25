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

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }
    
    protected IGrid Grid { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }
    
    protected bool IsPopulating { get; private set; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    protected IGrid GetOtherGrid() {
      return Grid == EditorController.View.MainGrid
        ? EditorController.View.ParentGrid
        : EditorController.View.MainGrid;
    }
    
    public virtual void OnGotFocus() {
      Debug.WriteLine("GridControllerBase.OnGotFocus");
      if (!IsPopulating) {
        if (EditorController.IsParentGridToBeShown) {
          GetOtherGrid().Enabled = true;
        }
        EditorController.View.SetMouseCursorToDefault();
      }
    }

    public abstract void OnRowEnter(int rowIndex);

    public virtual void OnPopulatedAsync() {
      Debug.WriteLine($"GridControllerBase.OnPopulatedAsync {Grid.Name}");
      IsPopulating = false;
      if (Grid.RowCount > 1) {
        Grid.MakeRowCurrent(Grid.RowCount - 1);
      }
    }

    public virtual void Populate(IList? list = null) {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(list);
      Grid.OnPopulated();
    }

    public virtual void PrepareForFocus() {
      Debug.WriteLine($"GridControllerBase.PrepareForFocus {Grid.Name}");
      if (!IsPopulating) {
        EditorController.View.SetMouseCursorToWait();
        if (EditorController.IsParentGridToBeShown) {
          PrepareToSwitchFocusFromOtherGridToThis();
        }
      }
    }

    private void PrepareToSwitchFocusFromOtherGridToThis() {
      Debug.WriteLine($"GridControllerBase.PrepareToSwitchFocusFromOtherGridToThis {Grid.Name}");
      Grid.CellColorScheme.RestoreToDefault();
      var otherGrid = GetOtherGrid();
      otherGrid.CellColorScheme.Invert();
      otherGrid.Enabled = false;
    }
  }
}