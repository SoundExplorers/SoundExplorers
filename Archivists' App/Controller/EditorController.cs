using System;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the table editor.
  /// </summary>
  [UsedImplicitly]
  public class EditorController {
    /// <summary>
    ///   Gets the format in which dates are to be shown on the grid.
    /// </summary>
    public const string DateFormat = Global.DateFormat;

    private Option _gridSplitterDistanceOption;
    private Option _imageSplitterDistanceOption;

    /// <summary>
    ///   Initialises a new instance of the <see cref="EditorController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The table editor view to be shown.
    /// </param>
    /// <param name="mainListType">
    ///   The type of entity list whose data is to be displayed.
    /// </param>
    public EditorController([NotNull] IEditorView view,
      [NotNull] Type mainListType) {
      View = view;
      MainListType = mainListType;
      View.SetController(this);
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's field properties.
    /// </summary>
    [NotNull]
    internal BindingColumnList Columns => MainList?.Columns ??
                                          throw new NullReferenceException(
                                            nameof(Columns));

    /// <summary>
    ///   User option for the position of the split between the
    ///   (upper) parent grid, if shown, and the (lower) main grid.
    /// </summary>
    public int GridSplitterDistance {
      get => GridSplitterDistanceOption.Int32Value;
      set => GridSplitterDistanceOption.Int32Value = value;
    }

    private Option GridSplitterDistanceOption =>
      _gridSplitterDistanceOption ?? (_gridSplitterDistanceOption =
        CreateOption($"{MainList?.EntityTypeName}.GridSplitterDistance"));

    protected virtual ChangeAction LastChangeAction =>
      MainList.LastDatabaseUpdateErrorException.ChangeAction;

    /// <summary>
    ///   User option for the position of the split between the
    ///   image data (above) and the image (below) in the image table editor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public int ImageSplitterDistance {
      get => ImageSplitterDistanceOption.Int32Value;
      set => ImageSplitterDistanceOption.Int32Value = value;
    }

    [ExcludeFromCodeCoverage]
    private Option ImageSplitterDistanceOption =>
      _imageSplitterDistanceOption ?? (_imageSplitterDistanceOption =
        new Option($"{MainList?.EntityTypeName}.ImageSplitterDistance"));

    private bool IsFormatException =>
      MainList.LastDatabaseUpdateErrorException?.InnerException?.GetType() ==
      typeof(FormatException);

    /// <summary>
    ///   Gets whether a read-only related grid for a parent table is to be shown
    ///   above the main grid.
    /// </summary>
    public bool IsParentTableToBeShown => ParentList?.BindingList != null;

    private bool IsReferencingValueNotFoundException =>
      MainList.LastDatabaseUpdateErrorException?.InnerException?.GetType() ==
      typeof(RowNotInTableException);

    [CanBeNull] public IBindingList MainBindingList => MainList?.BindingList;

    /// <summary>
    ///   Gets or set the list of entities represented in the main table.
    /// </summary>
    protected IEntityList MainList { get; private set; }

    [NotNull] private Type MainListType { get; }
    [CanBeNull] public string MainTableName => MainList?.EntityTypeName;
    [CanBeNull] public IBindingList ParentBindingList => ParentList?.BindingList;

    /// <summary>
    ///   Gets or sets the list of entities represented in the parent table, if any.
    /// </summary>
    [CanBeNull]
    private IEntityList ParentList { get; set; }

    [NotNull] private IEditorView View { get; }

    /// <summary>
    ///   Returns whether the specified column references another entity.
    /// </summary>
    public bool DoesColumnReferenceAnotherEntity([NotNull] string columnName) {
      return !string.IsNullOrWhiteSpace(Columns[columnName].ReferencedPropertyName);
    }

    /// <summary>
    ///   Edit the tags of the audio file, if found,
    ///   of the current Piece, if any,
    ///   Otherwise shows an informative message box.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   Whatever error might be thrown on attempting to update the tags.
    /// </exception>
    [ExcludeFromCodeCoverage]
    public void EditAudioFileTags() {
      // string path = GetMediumPath(Medium.Audio);
      // var dummy = new AudioFile(path);
    }

    public void FetchData() {
      MainList = CreateEntityList(MainListType);
      if (MainList.ParentListType != null) {
        ParentList = CreateEntityList(MainList.ParentListType);
        ParentList.IsParentList = true;
        ParentList.Populate();
        if (ParentList.BindingList?.Count > 0) {
          MainList.Populate(ParentList.GetChildrenForMainList(0));
        }
      } else {
        MainList.Populate();
      }
    }

    public void ShowDatabaseUpdateError() {
      View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex,
        MainList.LastDatabaseUpdateErrorException.ColumnIndex);
      if (LastChangeAction == ChangeAction.Delete) {
        View.SelectCurrentRowOnly();
      }
      // Known problem:  If IsReferencingValueNotFoundException on the insertion row, 
      // the error value shown in the cell reverts to the default value
      // on showing the message box.
      View.ShowErrorMessage(MainList.LastDatabaseUpdateErrorException.Message);
      if (IsFormatException) {
        // if (LastChangeAction == ChangeAction.Insert) {
        //   // A FormatException was thrown due to an invalidly formatted paste
        //   // into an insertion row cell, e.g. text into a date cell.
        //   // This the only type of editing error
        //   // where the error row remains the insertion row.
        //   //
        //   // Unlike other editing error types,
        //   // including FormatException in an existing row cell,
        //   // in this case we are not returning edited cells to their changed values.
        //   //
        //   // It looks like that would be very tricky in to do in the insertion row.
        //   // It seems to be impossible to change insertion cell values programatically.
        //   // Perhaps the insertion row could to be exited and replaced with
        //   // a new one based on a special binding item pre-populated
        //   // with the changed values (apart from the property corresponding to the
        //   // FormatException, which would be impossible).
        //   // As this should be a rare scenario and the resulting lack of data restoration
        //   // is not expected to be seen as onerous by users,
        //   // I'm putting this in the too-hard basket for now.
        // } else {
        //   // LastChangeAction == ChangeAction.Update
        // }
        return;
      }
      if (IsReferencingValueNotFoundException) {
        if (!MainList.IsInsertionRowCurrent) {
          MainList.RestoreReferencingPropertyOriginalValue(
            MainList.LastDatabaseUpdateErrorException.RowIndex,
            MainList.LastDatabaseUpdateErrorException.ColumnIndex);
        }
        return;
      }
      switch (LastChangeAction) {
        case ChangeAction.Delete:
          break;
        case ChangeAction.Insert:
          MainList.RemoveCurrentBindingItem();
          View.MakeMainGridInsertionRowCurrent();
          RestoreCurrentRowErrorValues();
          MainList.IsFixingNewRow = true;
          break;
        case ChangeAction.Update:
          MainList.RestoreCurrentBindingItemOriginalValues();
          View.EditMainGridCurrentCell();
          View.RestoreMainGridCurrentRowCellErrorValue(
            MainList.LastDatabaseUpdateErrorException.ColumnIndex,
            MainList.GetErrorValues()[
              MainList.LastDatabaseUpdateErrorException.ColumnIndex]);
          break;
        default:
          throw new NotSupportedException(
            $"{nameof(ChangeAction)} '{LastChangeAction}' is not supported.");
      }
    }

    [NotNull]
    protected virtual IEntityList CreateEntityList([NotNull] Type type) {
      return Global.CreateEntityList(type);
    }

    [ExcludeFromCodeCoverage]
    [NotNull]
    protected virtual Option CreateOption([NotNull] string name) {
      return new Option(name);
    }

    [NotNull]
    public string GetColumnDisplayName([NotNull] string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public void OnMainGridDataError(int rowIndex, [NotNull] string columnName,
      [CanBeNull] Exception exception) {
      switch (exception) {
        case DatabaseUpdateErrorException databaseUpdateErrorException:
          MainList.LastDatabaseUpdateErrorException = databaseUpdateErrorException;
          View.StartDatabaseUpdateErrorTimer();
          break;
        case FormatException formatException:
          // Can happen when pasting an invalid value into a cell,
          // e.g. text into a date.
          MainList.OnFormatException(rowIndex, columnName, formatException);
          View.StartDatabaseUpdateErrorTimer();
          break;
        case RowNotInTableException referencedEntityNotFoundException:
          // Can happen when pasting an invalid value into a cell,
          // e.g. text into a date.
          MainList.OnReferencedEntityNotFound(rowIndex, columnName, 
            referencedEntityNotFoundException);
          View.StartDatabaseUpdateErrorTimer();
          break;
        case null:
          // For unknown reason, the way I've got the error handling set up,
          // this event gets raise twice if there's a DatabaseUpdateErrorException,
          // the second time with a null exception.
          // It does not seem to do any harm, so long as it is trapped like this.
          break;
        default:
          // Terminal error.  In the Release compilation,
          // the stack trace will be shown by the terminal error handler in Program.cs.
          throw exception;
      }
    }

    public void OnMainGridRowEnter(int rowIndex) {
      // Debug.WriteLine(
      //   $"{nameof(OnMainGridRowEnter)}:  Any row entered (after ItemAdded if insertion row)");
      MainList?.OnRowEnter(rowIndex);
    }

    /// <summary>
    ///   Deletes the entity at the specified row index
    ///   from the database and removes it from the list.
    /// </summary>
    public void OnMainGridRowRemoved(int rowIndex) {
      // Debug.WriteLine(
      //   $"{nameof(OnMainGridRowRemoved)}:  2 or 3 times on opening a table before 1st ItemAdded (insertion row entered); existing row removed");
      // For unknown reason, the grid's RowRemoved event is raised 2 or 3 times
      // while data is being loaded into the grid.
      // Also, the grid row might have been removed because of an insertion error,
      // in which case the entity will not have been persisted (rowIndex == Count).
      if (MainList.IsDataLoadComplete && rowIndex < MainList.Count) {
        try {
          MainList.DeleteEntity(rowIndex);
          View.OnRowAddedOrDeleted();
        } catch (DatabaseUpdateErrorException) {
          View.StartDatabaseUpdateErrorTimer();
        }
      }
    }

    /// <summary>
    ///   If the specified table row is new,
    ///   inserts an entity on the database with the table row data.
    /// </summary>
    public void OnMainGridRowValidated(int rowIndex) {
      // Debug.WriteLine(
      //   $"{nameof(OnMainGridRowValidated)}:  Any row left, after final ItemChanged, if any");
      try {
        MainList?.OnRowValidated(rowIndex);
        View.OnRowAddedOrDeleted();
      } catch (DatabaseUpdateErrorException) {
        View.StartDatabaseUpdateErrorTimer();
      }
    }

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    public void OnParentGridRowEntered(int rowIndex) {
      MainList?.Populate(ParentList?.GetChildrenForMainList(rowIndex));
    }

    /// <summary>
    ///   A combo box cell value does not match any of it's embedded combo box's items.
    ///   So the combo box's selected index and text could not be updated.
    ///   As the combo boxes are all dropdown lists,
    ///   the only way this can have happened is that
    ///   the invalid value was pasted into the cell.
    ///   If the cell value had been changed
    ///   by selecting an item on the embedded combo box,
    ///   it could only be a matching value.
    /// </summary>
    internal void OnReferencedEntityNotFound(int rowIndex, [NotNull] string columnName,
      [NotNull] RowNotInTableException exception) {
      MainList.OnReferencedEntityNotFound(
        rowIndex, columnName, exception);
      View.StartDatabaseUpdateErrorTimer();
    }

    /// <summary>
    ///   Plays the audio, if found,
    ///   of the current Piece, if any.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   The audio cannot be played.
    /// </exception>
    [ExcludeFromCodeCoverage]
    public void PlayAudio() {
      //Process.Start(GetMediumPath(Medium.Audio));
    }

    /// <summary>
    ///   Plays the video, if found,
    ///   of the current Piece, if any.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   The video cannot be played.
    /// </exception>
    [ExcludeFromCodeCoverage]
    public void PlayVideo() {
      //Process.Start(GetMediumPath(Medium.Video));
    }

    private void RestoreCurrentRowErrorValues() {
      var errorValues = MainList.GetErrorValues();
      for (var i = 0; i < MainList.Columns.Count; i++) {
        View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex, i);
        View.EditMainGridCurrentCell();
        View.RestoreMainGridCurrentRowCellErrorValue(i, errorValues[i]);
      }
      View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex,
        MainList.LastDatabaseUpdateErrorException.ColumnIndex);
      View.EditMainGridCurrentCell();
    }

    /// <summary>
    ///   Shows the newsletter, if any, associated with the current row.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   A newsletter cannot be shown.
    /// </exception>
    [ExcludeFromCodeCoverage]
    public void ShowNewsletter() {
      // var newsletter = GetNewsletterToShow();
      // if (string.IsNullOrWhiteSpace(newsletter.Path)) { } else if (!File.Exists(
      //   newsletter.Path)) {
      //   throw new ApplicationException("Newsletter file \"" + newsletter.Path
      //     + "\", specified by the Path of the "
      //     + newsletter.Date.ToString(BindingColumn.DateFormat)
      //     + " Newsletter, cannot be found.");
      // } else {
      //   Process.Start(newsletter.Path);
      // }
    }

    public void ShowWarningMessage(string message) {
      View.ShowWarningMessage(message);
    }
  }
}