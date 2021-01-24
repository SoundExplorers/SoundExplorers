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
    
    /// <summary>
    ///   Gets or sets whether the grid is being focused programatically, i.e. via
    ///   <see cref="IGrid.SetFocus" />. If false, either the grid is not being focused,
    ///   or it is being focusing by a left mouse button click via a Windows message.
    /// </summary>
    public bool IsFocusingProgramatically { get; set; }
    
    protected bool IsPopulating { get; private set; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public IGrid GetOtherGrid() {
      return Grid == EditorController.View.MainGrid
        ? EditorController.View.ParentGrid
        : EditorController.View.MainGrid;
    }

    public void OnFocusing() {
      Debug.WriteLine($"GridControllerBase.OnFocusing {Grid.Name}");
      if (!IsFocusingProgramatically) {
        PrepareForFocus();
      }
      if (!IsPopulating && EditorController.IsParentGridToBeShown) {
        Grid.CellColorScheme.RestoreToDefault();
        GetOtherGrid().CellColorScheme.Invert();
      }
    }
    
    public virtual void OnGotFocus() {
      Debug.WriteLine("GridControllerBase.OnGotFocus");
      if (!IsPopulating) {
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

    public virtual void PrepareForFocus() {
      Debug.WriteLine($"GridControllerBase.PrepareForFocus {Grid.Name}");
      if (!IsPopulating) {
        EditorController.View.SetMouseCursorToWait();
      }
    }

    public virtual void Populate(IList? list = null) {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(list);
      Grid.OnPopulated();
    }
  }
}