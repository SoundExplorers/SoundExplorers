using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class MainGridController : GridControllerBase {
    public MainGridController(
      // ReSharper disable once SuggestBaseTypeForParameter
      IMainGrid grid, EditorController editorController) :
      base(grid, editorController) { }

    internal int LastCurrentRowIndex { get; set; }

    protected virtual StatementType LastChangeAction =>
      List.LastDatabaseUpdateErrorException!.ChangeAction;

    /// <summary>
    ///   This flag is used in a workaround for an unwanted feature of
    ///   DataGridView.OnBindingContextChanged".  In some of the multiple times that
    ///   event method occurs per <see cref="EditorController.Populate" />, it makes row
    ///   0 of the main grid current (by calling internal method
    ///   DataGridView.MakeFirstDisplayedCellCurrentCell). In this application, that
    ///   would usually be the wrong row, as we want the new row to be current on
    ///   population of the main grid. And it would also focus the main grid when we want
    ///   the parent grid to be focused. Normally, the unwanted behaviour would trigger
    ///   DataGridView.OnRowEnter for row 0, which MainGrid overrides. The workaround
    ///   stops OnRowEnter from being called in this case.
    /// </summary>
    /// <remarks>
    ///   Another workaround I looked at was to override OnBindingContextChanged to stop
    ///   the base method from being called.  That is not practicable, as it prevents
    ///   editor controls from appearing in grid cells when they are edited!
    /// </remarks>
    protected bool IsFixingFocus { get; private set; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorController.MainList;

    private new IMainGrid Grid => (IMainGrid)base.Grid;
    private bool IsRestoringRowCurrency { get; set; }
    private IParentGrid ParentGrid => EditorController.View.ParentGrid;

    /// <summary>
    ///   Occurs when an exception is thrown on ending a cell edit.
    /// </summary>
    /// <remarks>
    ///   A <see cref="DatabaseUpdateErrorException" />, which is explicitly thrown by
    ///   the application's code, is thrown at end of cell edit on existing rows but on
    ///   row validation for the insertion row, when this event is not raised.
    /// </remarks>
    public void OnCellEditException(int rowIndex, string columnName,
      Exception? exception) {
      Debug.WriteLine(
        $"MainGridController.OnCellEditException: rowIndex = {rowIndex}; columnName = {columnName}, {exception?.GetType().Name}");
      // For unknown reason, the way I've got the error handling set up, the main grid's
      // DataError event, whose handler calls this method, gets raise twice if there's
      // a DatabaseUpdateErrorException, the second time with a null exception. It does
      // not seem to do any harm, so long as it is trapped like this.
      if (exception != null) {
        List.OnCellEditException(rowIndex, columnName, exception);
        EditorController.View.OnError();
      }
    }

    /// <summary>
    ///   If the main grid's current row was the new row before focusing the parent grid,
    ///   then, on focusing the parent grid, the new row was removed, so the main grid's
    ///   last existing row became its current row. So, if focus has now been switched back
    ///   to the main grid, restore currency to the new row.
    /// </summary>
    public override void OnGotFocus() {
      // Debug.WriteLine("MainGridController.OnGotFocus");
      Debug.WriteLine(
        $"MainGridController.OnGotFocus: CurrentRowIndex = {Grid.CurrentRowIndex}; LastCurrentRowIndex = {LastCurrentRowIndex}");
      IsFixingFocus = false;
      if (LastCurrentRowIndex >= 0 && LastCurrentRowIndex != Grid.CurrentRowIndex) {
        IsRestoringRowCurrency = true;
        Grid.MakeRowCurrent(LastCurrentRowIndex, true);
        LastCurrentRowIndex = -1;
      } else {
        LastCurrentRowIndex = -1;
        base.OnGotFocus();
      }
    }

    public override void OnRowEnter(int rowIndex) {
      // Debug.WriteLine(
      //   "MainGridController.OnRowEnter:  Any row entered (after ItemAdded if insertion row)");
      Debug.WriteLine(
        $"MainGridController.OnRowEnter: row {rowIndex} of {BindingList.Count}");
      // We need to guard against out of range rowIndex, which happens if the user
      // presses Tab from the last cell of a child grid.
      if (rowIndex < List.BindingList.Count) {
        base.OnRowEnter(rowIndex);
        if (!IsPopulating) {
          List.OnRowEnter(rowIndex);
        }
        if (IsRestoringRowCurrency) {
          IsRestoringRowCurrency = false;
          base.OnGotFocus();
        }
      }
      // base.OnRowEnter(rowIndex);
      // if (!IsPopulating) {
      //   List.OnRowEnter(rowIndex);
      // }
      // if (IsRestoringRowCurrency) {
      //   IsRestoringRowCurrency = false;
      //   base.OnGotFocus();
      // }
    }

    /// <summary>
    ///   Deletes the entity at the specified row index from the database and removes it
    ///   from the list.
    /// </summary>
    public void OnRowRemoved(int rowIndex) {
      Debug.WriteLine($"MainGridController.OnRowRemoved: row {rowIndex}");
      // Debug.WriteLine(
      //   $"{nameof(OnMainGridRowRemoved)}:  2 or 3 times on opening a table before 1st ItemAdded (insertion row entered); existing row removed");
      // For unknown reason, the grid's RowsRemoved event is raised 2 or 3 times while
      // data is being loaded into the grid. Also, the grid row might have been removed
      // because of an insertion error, in which case the entity will not have been
      // persisted (rowIndex == Count).
      if (!IsPopulating && rowIndex < List.Count) {
        try {
          List.DeleteEntity(rowIndex);
          Grid.OnRowAddedOrDeleted();
        } catch (DatabaseUpdateErrorException) {
          EditorController.View.OnError();
        }
      }
    }

    /// <summary>
    ///   Handles the main grid's RowValidated event, which is raised when the user exits
    ///   a row on the grid. If the specified table row is new, inserts an entity on the
    ///   database with the table row data. For an existing row, there should be nothing
    ///   to do, as any edits will have been committed on a cell by cell basis.
    /// </summary>
    /// <remarks>
    ///   The RowValidated event is also raised when the editor window or main window is
    ///   closed. If the insertion row is being edited when this happens, no insertion to
    ///   the database will take place. An attempt a database insert in this scenario
    ///   would result in an exception at an unpredictable point, due to the closure
    ///   process already being underway. Ideally, the user would instead be given the
    ///   options of committing the insertion or cancelling the close and returning to
    ///   the editor.
    /// </remarks>
    public void OnRowValidated(int rowIndex) {
      Debug.WriteLine($"MainGridController.OnRowValidated: row {rowIndex}");
      //Debug.WriteLine("MainGridController.OnRowValidated:  Any row left, after final ItemChanged, if any");
      if (IsFixingFocus) {
        IsFixingFocus = false;
        // Stops the main grid from being focused when the user changes row on the parent
        // grid, repopulating the main grid.  See also the comments for IsFixingFocus. 
        return;
      }
      if (IsPopulating || EditorController.IsClosing ||
          EditorController.MainController.IsClosing) {
        return;
      }
      try {
        List.OnRowValidated(rowIndex);
        Grid.OnRowAddedOrDeleted();
      } catch (DatabaseUpdateErrorException) {
        EditorController.View.OnError();
      }
    }

    public override void Populate() {
      Debug.WriteLine("MainGridController.Populate");
      base.Populate();
      LastCurrentRowIndex = -1;
      foreach (var column in Columns) {
        column.EditWidth = GetColumnEditWidthInPixels(column);
      }
    }

    public void ShowError() {
      Debug.WriteLine(
        $"MainGridController.ShowError: LastChangeAction == {LastChangeAction}");
      Grid.MakeCellCurrent(List.LastDatabaseUpdateErrorException!.RowIndex,
        List.LastDatabaseUpdateErrorException.ColumnIndex);
      if (LastChangeAction == StatementType.Delete) {
        Grid.SelectCurrentRowOnly();
      }
      EditorController.View.ShowErrorMessage(
        List.LastDatabaseUpdateErrorException.Message);
      // Debug.WriteLine("Error message shown");
      switch (LastChangeAction) {
        case StatementType.Delete:
          break;
        case StatementType.Insert:
          CancelInsertion();
          break;
        case StatementType.Update:
          CancelUpdate();
          break;
        default:
          throw new NotSupportedException(
            $"{nameof(DatabaseUpdateErrorException.ChangeAction)} " +
            $"'{LastChangeAction}' is not supported.");
      }
    }

    public void ShowWarningMessage(string message) {
      EditorController.View.ShowWarningMessage(message);
    }

    internal void SetIdentifyingParentChildrenForList(
      IdentifyingParentAndChildren identifyingParentAndChildrenForList) {
      IsFixingFocus = true;
      IdentifyingParentChildrenForList = identifyingParentAndChildrenForList;
    }

    protected override void OnPopulatedAsync() {
      Debug.WriteLine("MainGridController.OnPopulatedAsync");
      if (List.ListRole == ListRole.Child) {
        IsFixingFocus = true;
        EditorController.View.OnParentAndMainGridsShown();
        Grid.CellColorScheme.Invert();
      }
      base.OnPopulatedAsync();
      // Warning: Profiling blocks code coverage analysis
      // MeasureProfiler.SaveData();
      if (List.ListRole == ListRole.Child &&
          ParentGrid.Controller.LastRowNeedsToBeScrolledIntoView) {
        ParentGrid.Controller.ScrollLastRowIntoView();
      } else {
        // Show that the population process is finished.
        EditorController.View.SetMouseCursorToDefault();
      }
    }

    protected virtual void ReplaceErrorBindingValueWithOriginal() {
      List.ReplaceErrorBindingValueWithOriginal();
    }

    /// <summary>
    ///   Invoked when the user clicks OK on an insert error message box. The insertion
    ///   row is then removed and a new row is created and made current with the error
    ///   data restored for correction or cancellation of the insertion by the user.
    ///   If this were not done, the user could go to a new row below the error row,
    ///   leaving the error row on the grid but not bound to an entity.
    /// </summary>
    private void CancelInsertion() {
      Debug.WriteLine("EditorController.CancelInsertion");
      int insertionRowIndex = List.BindingList.Count - 1;
      // Backs up the error insertion item
      List.BackupAndRemoveInsertionErrorBindingItem();
      // Force a new row to be created with the erroneous data restored to it.
      Grid.MakeCellCurrent(insertionRowIndex,
        List.LastDatabaseUpdateErrorException!.ColumnIndex);
    }

    /// <summary>
    ///   Invoked when the user clicks OK on an existing row cell update error message
    ///   box.
    /// </summary>
    private void CancelUpdate() {
      // Restore the original i.e. valid value. If we are then going to allow the user to
      // correct the error value, this is still useful, as it will allow the user to
      // cancel out of the error correction edit instead of trying to modify the error
      // value to make it valid. If the user does cancel out of the error correction
      // edit, the cell value will then revert to the original i.e. valid value.   
      ReplaceErrorBindingValueWithOriginal();
      var exception = List.LastDatabaseUpdateErrorException!;
      if (exception.ErrorType != ErrorType.Database) {
        // Allowing the user to correct the error value is not possible.
        return;
      }
      // Allow the user to correct the error value.
      Grid.EditCurrentCell();
      Grid.RestoreCurrentRowCellErrorValue(
        exception.ColumnIndex,
        List.GetErrorValues()[exception.ColumnIndex]);
    }

    private int GetColumnEditWidthInPixels(BindingColumn column) {
      const int margin = 21;
      const int textColumnEditWidth = 200;
      if (column.ValueType == typeof(DateTime)) {
        return Grid.GetTextWidthInPixels(Global.DateFormat.ToUpper()) + margin;
      }
      if (column.ReferencesAnotherEntity) {
        var keys = column.ReferenceableItems?.GetKeys();
        if (keys?.Count > 0) {
          return (
            from key in keys
            select Grid.GetTextWidthInPixels(key)).Max() + margin;
        }
        return -1;
      }
      return column.ValueType == typeof(string) &&
             column.PropertyName != nameof(PieceBindingItem.Duration)
        ? textColumnEditWidth
        : -1;
    }
  }
}