using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using SoundExplorers.Common;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    private IList<IBindingColumn>? _bindingColumns;
    private int _firstVisibleColumnIndex;
    private IGrid? _otherGrid;

    protected GridControllerBase(IGrid grid, EditorController editorController) {
      Grid = grid;
      EditorController = editorController;
      _firstVisibleColumnIndex = -1;
    }

    public IList<IBindingColumn> BindingColumns =>
      _bindingColumns ??= CreateBindingColumns();

    public IBindingList? BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }

    public int FirstVisibleColumnIndex => _firstVisibleColumnIndex >= 0
      ? _firstVisibleColumnIndex
      : _firstVisibleColumnIndex = GetFirstVisibleColumnIndex();

    protected IGrid Grid { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    protected bool IsPopulating { get; private set; }
    private IGrid OtherGrid => _otherGrid ??= GetOtherGrid();

    private IList<IBindingColumn> CreateBindingColumns() {
      return (from column in Columns select (IBindingColumn)column).ToList();
    }

    private int GetFirstVisibleColumnIndex() {
      int result = -1;
      for (int i = 0; i < Columns.Count; i++) {
        if (Columns[i].IsVisible) {
          result = i;
          break;
        }
      }
      return result;
    }

    private IGrid GetOtherGrid() {
      return Grid == EditorController.View.MainGrid
        ? EditorController.View.ParentGrid
        : EditorController.View.MainGrid;
    }

    public virtual void OnGotFocus() {
      Debug.WriteLine($"GridControllerBase.OnGotFocus {Grid.Name}");
      if (!IsPopulating) {
        if (EditorController.IsParentGridToBeShown) {
          OtherGrid.Enabled = true;
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

    public virtual void Populate(
        IdentifyingParentChildren? identifyingParentChildren = null) {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(identifyingParentChildren);
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
      Debug.WriteLine(
        $"GridControllerBase.PrepareToSwitchFocusFromOtherGridToThis {Grid.Name}");
      Grid.CellColorScheme.RestoreToDefault();
      OtherGrid.CellColorScheme.Invert();
      OtherGrid.Enabled = false;
    }
  }
}