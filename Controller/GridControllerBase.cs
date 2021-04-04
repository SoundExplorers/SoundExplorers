using System;
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

    public IBindingList BindingList => List.BindingList;

    public int FirstVisibleColumnIndex => _firstVisibleColumnIndex >= 0
      ? _firstVisibleColumnIndex
      : _firstVisibleColumnIndex = GetFirstVisibleColumnIndex();

    public string TableName => List.EntityTypeName;

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }
    protected IGrid Grid { get; }

    /// <summary>
    ///   Only applicable to a main grid that is a child of a parent grid row, this
    ///   specifies the grid's identifying parent entity and, for populating the grid,
    ///   its child entities.
    /// </summary>
    protected IdentifyingParentAndChildren? IdentifyingParentAndChildrenForList {
      get;
      set;
    }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    protected bool IsPopulating { get; private set; }
    private IGrid OtherGrid => _otherGrid ??= GetOtherGrid();
    private string? RowText { get; set; }

    /// <summary>
    ///   Follows the URL link in the current cell by opening it in the default browser.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public void FollowLink() {
      string link = Grid.CurrentCellValue!.ToString()!;
      try {
        MainController.OpenFile(link);
      } catch (Win32Exception) {
        // Invalid link, like the comment in the URL of cell of the default dummy
        // Newsletter.
        EditorController.View.ShowErrorMessage(
          $"An application cannot be found for opening '{link}'.");
      }
    }

    public bool IsUrlColumn(string columnName) {
      return Columns[columnName].ValueType == typeof(Uri);
    }

    public virtual void OnGotFocus() {
      Debug.WriteLine($"GridControllerBase.OnGotFocus {Grid.Name}");
      if (!IsPopulating) {
        if (EditorController.IsParentGridToBeShown) {
          OtherGrid.Enabled = true;
        }
        SetStatusBarTextToRowText();
        EditorController.View.SetMouseCursorToDefault();
      }
    }

    public virtual void OnRowEnter(int rowIndex) {
      Debug.WriteLine(
        $"GridControllerBase.OnRowEnter {Grid.Name}: row {rowIndex} of {BindingList.Count}");
      if (!IsPopulating) {
        RowText = $"{List.EntityTypeName} {rowIndex + 1:#,0} of {BindingList.Count:#,0}";
        if (Grid.Focused) {
          EditorController.View.SetStatusBarText(RowText);
        }
      }
    }

    protected virtual void OnPopulatedAsync() {
      Debug.WriteLine($"GridControllerBase.OnPopulatedAsync {Grid.Name}");
      IsPopulating = false;
      if (Grid.RowCount > 1) {
        Grid.MakeRowCurrent(Grid.RowCount - 1);
      }
    }

    public void OnWindowActivated() {
      SetStatusBarTextToRowText();
    }

    public virtual void Populate() {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(IdentifyingParentAndChildrenForList);
      IdentifyingParentAndChildrenForList = null;
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

    private void PrepareToSwitchFocusFromOtherGridToThis() {
      Debug.WriteLine(
        $"GridControllerBase.PrepareToSwitchFocusFromOtherGridToThis {Grid.Name}");
      Grid.CellColorScheme.RestoreToDefault();
      OtherGrid.CellColorScheme.Invert();
      OtherGrid.Enabled = false;
    }

    private void SetStatusBarTextToRowText() {
      if (RowText != null) {
        EditorController.View.SetStatusBarText(RowText);
      }
    }
  }
}