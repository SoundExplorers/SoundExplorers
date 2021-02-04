using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
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
    ///   Only applicable to a main grid that is a child of a parent grid row, this
    ///   specifies the grid's identifying parent entity and, for populating the grid,
    ///   its child entities.  
    /// </summary>
    protected IdentifyingParentChildren? IdentifyingParentChildrenForList { get; set; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    protected bool IsPopulating { get; private set; }
    private IGrid OtherGrid => _otherGrid ??= GetOtherGrid();
    public string TableName => List.EntityTypeName;

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

    protected virtual void OnPopulatedAsync() {
      Debug.WriteLine($"GridControllerBase.OnPopulatedAsync {Grid.Name}");
      IsPopulating = false;
      if (Grid.RowCount > 1) {
        Grid.MakeRowCurrent(Grid.RowCount - 1);
      }
    }

    public virtual void Populate() {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(IdentifyingParentChildrenForList);
      IdentifyingParentChildrenForList = null;
      Grid.BeginInvoke(OnPopulatedAsync);
    }

    public virtual void PrepareForFocus() {
      Debug.WriteLine($"GridControllerBase.PrepareForFocus {Grid.Name}");
      if (!IsPopulating) {
        EditorController.View.SetMouseCursorToWait();
        if (EditorController.IsParentGridToBeShown 
            && EditorController.View.CurrentGrid != Grid) {
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