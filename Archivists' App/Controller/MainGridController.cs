using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class MainGridController : GridControllerBase {

    public MainGridController(
      // ReSharper disable once SuggestBaseTypeForParameter
      IMainGrid grid, EditorController editorController) :
      base(grid, editorController) { }

    private new IMainGrid Grid => (IMainGrid)base.Grid;

    private bool IsDuplicateKeyException =>
      List.LastDatabaseUpdateErrorException?.InnerException is DuplicateNameException;

    private bool IsFormatException =>
      List.LastDatabaseUpdateErrorException?.InnerException is FormatException;

    private bool IsReferencingValueNotFoundException =>
      List.LastDatabaseUpdateErrorException?.InnerException
        is RowNotInTableException;

    private bool IsRestoringRowCurrency { get; set; }

    protected virtual StatementType LastChangeAction =>
      List.LastDatabaseUpdateErrorException!.ChangeAction;

    internal int LastCurrentRowIndex { get; set; }

    /// <summary>
    ///   Gets whether the current main grid row is the insertion row,
    ///   which is for adding new entities and is located at the bottom of the grid.
    /// </summary>
    public bool IsInsertionRowCurrent => List.IsInsertionRowCurrent;

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorController.MainList;

    private IParentGrid ParentGrid => EditorController.View.ParentGrid;
    public string TableName => List.EntityTypeName;

    public void OnExistingRowCellEditError(int rowIndex, string columnName,
      Exception? exception) {
      switch (exception) {
        case DatabaseUpdateErrorException databaseUpdateErrorException:
          List.LastDatabaseUpdateErrorException = databaseUpdateErrorException;
          EditorController.View.OnError();
          break;
        case DuplicateNameException duplicateKeyException:
          List.OnValidationError(rowIndex, columnName, duplicateKeyException);
          EditorController.View.OnError();
          break;
        case FormatException formatException:
          // An invalid value was pasted into a cell, e.g. text into a date.
          List.OnValidationError(rowIndex, columnName, formatException);
          EditorController.View.OnError();
          break;
        case RowNotInTableException referencedEntityNotFoundException:
          // A combo box cell value does not match any of it's embedded combo box's
          // items. So the combo box's selected index and text could not be updated. As
          // the combo boxes are all dropdown lists, the only way this can have happened
          // is that the unmatched value was pasted into the cell. If the cell value had
          // been changed by selecting an item on the embedded combo box, it could only
          // be a matching value.
          List.OnValidationError(rowIndex, columnName,
            referencedEntityNotFoundException);
          EditorController.View.OnError();
          break;
        case null:
          // For unknown reason, the way I've got the error handling set up, this event
          // gets raise twice if there's a DatabaseUpdateErrorException, the second time
          // with a null exception. It does not seem to do any harm, so long as it is
          // trapped like this.
          break;
        default:
          // Terminal error. In the Release build, the stack trace will be shown by
          // the terminal error handler in Program.cs.
          throw exception;
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
      if (LastCurrentRowIndex >= 0 && LastCurrentRowIndex != Grid.CurrentRowIndex) {
        IsRestoringRowCurrency = true;
        Grid.MakeRowCurrent(LastCurrentRowIndex, true);
        LastCurrentRowIndex = -1;
      } else {
        LastCurrentRowIndex = -1;
        base.OnGotFocus();
      }
    }

    public override void OnPopulatedAsync() {
      Debug.WriteLine("MainGridController.OnPopulatedAsync");
      if (EditorController.IsParentGridToBeShown) {
        EditorController.View.OnParentAndMainGridsShownAsync();
        Grid.CellColorScheme.Invert();
      }
      base.OnPopulatedAsync();
      if (EditorController.IsParentGridToBeShown &&
          ParentGrid.Controller.LastRowNeedsToBeScrolledIntoView) {
        ParentGrid.Controller.ScrollLastRowIntoView();
      } else {
        // Show that the population process is finished.
        EditorController.View.SetMouseCursorToDefault();
      }
    }

    public override void OnRowEnter(int rowIndex) {
      // Debug.WriteLine(
      //   "MainGridController.OnRowEnter:  Any row entered (after ItemAdded if insertion row)");
      Debug.WriteLine($"MainGridController.OnRowEnter: row {rowIndex} of {List.Count}");
      if (!IsPopulating) {
        List.OnRowEnter(rowIndex);
      }
      if (IsRestoringRowCurrency) {
        IsRestoringRowCurrency = false;
        base.OnGotFocus();
      }
    }

    /// <summary>
    ///   Deletes the entity at the specified row index from the database and removes it
    ///   from the list.
    /// </summary>
    public void OnRowRemoved(int rowIndex) {
      //Debug.WriteLine("MainGridController.OnRowRemoved");
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
    ///   the editor. But see the XML comments for <see cref="CancelInsertion" /> for the
    ///   difficulties that would be involved in reopening a 'validated' insertion.
    /// </remarks>
    public void OnRowValidated(int rowIndex) {
      Debug.WriteLine($"MainGridController.OnRowValidated: row {rowIndex}");
      //Debug.WriteLine("MainGridController.OnRowValidated:  Any row left, after final ItemChanged, if any");
      // Debug.WriteLine(
      //   $"MainGridController.OnRowValidated: row {rowIndex}, IsRemovingInvalidInsertionRow == {MainList.IsRemovingInvalidInsertionRow}");
      if (List.IsRemovingInvalidInsertionRow ||
          IsPopulating || EditorController.IsClosing ||
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

    public override void Populate(IList? list = null) {
      Debug.WriteLine("MainGridController.Populate");
      base.Populate(list);
      LastCurrentRowIndex = -1;
    }

    public void ShowError() {
      // Debug.WriteLine(
      //   $"MainGridController.ShowError: LastChangeAction == {LastChangeAction}");
      Grid.MakeCellCurrent(List.LastDatabaseUpdateErrorException!.RowIndex,
        List.LastDatabaseUpdateErrorException.ColumnIndex);
      if (LastChangeAction == StatementType.Delete) {
        Grid.SelectCurrentRowOnly();
      }
      EditorController.View.ShowErrorMessage(
        List.LastDatabaseUpdateErrorException.Message);
      // Debug.WriteLine("Error message shown");
      if (IsFormatException) {
        return;
      }
      if (IsDuplicateKeyException || IsReferencingValueNotFoundException) {
        if (LastChangeAction == StatementType.Update) {
          List.RestoreReferencingPropertyOriginalValue(
            List.LastDatabaseUpdateErrorException.RowIndex,
            List.LastDatabaseUpdateErrorException.ColumnIndex);
          return;
        }
      }
      switch (LastChangeAction) {
        case StatementType.Delete:
          break;
        case StatementType.Insert:
          CancelInsertion();
          break;
        case StatementType.Update:
          List.RestoreCurrentBindingItemOriginalValues();
          Grid.EditCurrentCell();
          Grid.RestoreCurrentRowCellErrorValue(
            List.LastDatabaseUpdateErrorException.ColumnIndex,
            List.GetErrorValues()[
              List.LastDatabaseUpdateErrorException.ColumnIndex]);
          break;
        default:
          throw new NotSupportedException(
            $"{nameof(DatabaseUpdateErrorException.ChangeAction)} '{LastChangeAction}' is not supported.");
      }
    }

    public void ShowWarningMessage(string message) {
      EditorController.View.ShowWarningMessage(message);
    }

    /// <summary>
    ///   A combo box cell value on the main grid's insertion row does not match any of
    ///   it's embedded combo box's items. So the combo box's selected index and text
    ///   could not be updated. As the combo boxes are all dropdown lists, the only way
    ///   this can have happened is that the unmatched value was pasted into the cell. If
    ///   the cell value had been changed by selecting an item on the embedded combo box,
    ///   it could only be a matching value.
    /// </summary>
    internal void OnInsertionRowReferencedEntityNotFound(
      int rowIndex, string columnName,
      string simpleKey) {
      var referencedEntityNotFoundException =
        ReferenceableItemList.CreateReferencedEntityNotFoundException(
          columnName, simpleKey);
      List.OnValidationError(
        rowIndex, columnName, referencedEntityNotFoundException);
      EditorController.View.OnError();
    }

    /// <summary>
    ///   Invoked when the user clicks OK on an insert error message box. If the
    ///   insertion row is not the only row, the row above the insertion row is
    ///   temporarily made current, which allows the insertion row to be removed. The
    ///   insertion row is then removed, forcing a new empty insertion row to be created.
    ///   Finally the new empty insertion row is made current. The net effect is that,
    ///   after the error message has been shown, the insertion row remains current, from
    ///   the perspective of the user, but all its cells have been blanked out or, where
    ///   applicable, reverted to default values.
    /// </summary>
    /// <remarks>
    ///   The disadvantage is that the user's work on the insertion row is lost and, if
    ///   still needed, has to be restarted from scratch. So why is this done?
    ///   <para>
    ///     If the insertion row were to be left with the error values, the user would
    ///     have no way of cancelling out of the insertion short of closing the
    ///     application. And, as the most probable or only error type is a duplicate key,
    ///     the user would quite likely want to cancel the insertion and edit the
    ///     existing row with that key instead.
    ///   </para>
    ///   <para>
    ///     There is no way of selectively reverting just the erroneous cell values of an
    ///     insertion row to blank or default. So I spent an inordinate amount of effort
    ///     trying to get another option to work. The idea was to convert the insertion
    ///     row into an 'existing' row without updating the database, resetting just the
    ///     erroneous cell values to blank or default. While I still think that would be
    ///     possible, it turned out to add a great deal of complexity just to get it not
    ///     working reliably! So I gave up and went for this relatively simple and
    ///     hopefully robust solution instead.
    ///   </para>
    ///   <para>
    ///     If in future we want to allow the insertion row to be re-edited, perhaps the
    ///     insertion row could to be replaced with a new one based on a special binding
    ///     item pre-populated with the changed values (apart from the property
    ///     corresponding to a FormatException, which would be impossible, or a reference
    ///     not found paste error, which would be pointless).
    ///   </para>
    /// </remarks>
    private void CancelInsertion() {
      // Debug.WriteLine("EditorController.CancelInsertion");
      int insertionRowIndex = List.BindingList!.Count - 1;
      if (insertionRowIndex > 0) {
        // Currently, it is not anticipated that there can be an insertion row error at
        // this point when the insertion row is the only row on the table, as the only
        // anticipated error type that should get to this point is a duplicate key.
        // Format errors are handled differently and should not get here.
        List.IsRemovingInvalidInsertionRow = true;
        Grid.MakeRowCurrent(insertionRowIndex - 1);
        List.RemoveInsertionBindingItem();
        Grid.MakeRowCurrent(insertionRowIndex);
      }
    }
  }
}