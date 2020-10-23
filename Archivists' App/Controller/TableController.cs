using System;
using System.ComponentModel;
using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the table editor.
  /// </summary>
  [UsedImplicitly]
  public class TableController {
    private Option _gridSplitterDistanceOption;
    private Option _imageSplitterDistanceOption;

    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The table editor view to be shown.
    /// </param>
    /// <param name="mainListType">
    ///   The type of entity list whose data is to be displayed.
    /// </param>
    public TableController([NotNull] ITableView view,
      [NotNull] Type mainListType) {
      View = view;
      MainListType = mainListType;
      View.SetController(this);
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's field properties.
    /// </summary>
    internal EntityColumnList Columns => MainList?.Columns ??
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
        new Option($"{MainList?.TableName}.GridSplitterDistance"));

    protected virtual ChangeAction LastChangeAction =>
      MainList.LastDatabaseUpdateErrorException.ChangeAction;

    /// <summary>
    ///   User option for the position of the split between the
    ///   image data (above) and the image (below) in the image table editor.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public int ImageSplitterDistance {
      get => ImageSplitterDistanceOption.Int32Value;
      set => ImageSplitterDistanceOption.Int32Value = value;
    }

    private Option ImageSplitterDistanceOption =>
      _imageSplitterDistanceOption ?? (_imageSplitterDistanceOption =
        new Option($"{MainList?.TableName}.ImageSplitterDistance"));

    /// <summary>
    ///   Gets whether a read-only related grid for a parent table is to be shown
    ///   above the main grid.
    /// </summary>
    public bool IsParentTableToBeShown => ParentList?.BindingList != null;

    [CanBeNull] public IBindingList MainBindingList => MainList?.BindingList;

    /// <summary>
    ///   Gets or set the list of entities represented in the main table.
    /// </summary>
    private IEntityList MainList { get; set; }

    [NotNull] private Type MainListType { get; }
    [CanBeNull] public string MainTableName => MainList?.TableName;
    [CanBeNull] public IBindingList ParentBindingList => ParentList?.BindingList;

    /// <summary>
    ///   Gets or sets the list of entities represented in the parent table, if any.
    /// </summary>
    [CanBeNull]
    private IEntityList ParentList { get; set; }

    [NotNull] private ITableView View { get; }

    /// <summary>
    ///   Returns whether the specified column references another entity.
    /// </summary>
    public bool DoesColumnReferenceAnotherEntity([NotNull] string columnName) {
      return !string.IsNullOrWhiteSpace(Columns[columnName]?.ReferencedColumnName);
    }

    /// <summary>
    ///   Edit the tags of the audio file, if found,
    ///   of the current Piece, if any,
    ///   Otherwise shows an informative message box.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   Whatever error might be thrown on attempting to update the tags.
    /// </exception>
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
          MainList.Populate(ParentList.GetChildren(0));
        }
      } else {
        MainList.Populate();
      }
    }

    internal void SetParent(int rowIndex, [NotNull] string columnName,
      [CanBeNull] IEntity entity) {
      ((IBindingItem)MainList.BindingList[rowIndex]).SetParent(columnName, entity);
    }

    public void ShowDatabaseUpdateError() {
      View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex,
        MainList.LastDatabaseUpdateErrorException.ColumnIndex);
      if (LastChangeAction == ChangeAction.Delete) {
        View.SelectCurrentRowOnly();
      }
      View.ShowErrorMessage(MainList.LastDatabaseUpdateErrorException.Message);
      switch (LastChangeAction) {
        case ChangeAction.Delete:
          break;
        case ChangeAction.Insert:
          MainList.RemoveCurrentBindingItem();
          View.MakeMainGridInsertionRowCurrent();
          var insertionRowErrorValues = MainList.GetErrorValues();
          for (var i = 0; i < MainList.Columns.Count; i++) {
            View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex, i);
            View.EditMainGridCurrentCell();
            View.RestoreMainGridCurrentRowCellErrorValue(i, insertionRowErrorValues[i]);
          }
          View.FocusMainGridCell(MainList.LastDatabaseUpdateErrorException.RowIndex,
            MainList.LastDatabaseUpdateErrorException.ColumnIndex);
          View.EditMainGridCurrentCell();
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

    [CanBeNull]
    public string GetColumnDisplayName([NotNull] string columnName) {
      return Columns[columnName]?.DisplayName;
    }

    public void OnMainGridDataError([CanBeNull] Exception exception) {
      switch (exception) {
        case DatabaseUpdateErrorException databaseUpdateErrorException:
          MainList.LastDatabaseUpdateErrorException = databaseUpdateErrorException;
          View.StartDatabaseUpdateErrorTimer();
          break;
        case null:
          // For unknown reason, the way I've got the error handling set up,
          // this event gets raise twice if there's a cell edit error,
          // the second time with a null exception.
          // It does not seem to do any harm, so long as it is trapped like this.
          break;
        default:
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
      MainList?.Populate(ParentList?.GetChildren(rowIndex));
    }

    /// <summary>
    ///   Plays the audio, if found,
    ///   of the current Piece, if any.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   The audio cannot be played.
    /// </exception>
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
    public void PlayVideo() {
      //Process.Start(GetMediumPath(Medium.Video));
    }

    /// <summary>
    ///   Shows the newsletter, if any, associated with the current row.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   A newsletter cannot be shown.
    /// </exception>
    public void ShowNewsletter() {
      // var newsletter = GetNewsletterToShow();
      // if (string.IsNullOrWhiteSpace(newsletter.Path)) { } else if (!File.Exists(
      //   newsletter.Path)) {
      //   throw new ApplicationException("Newsletter file \"" + newsletter.Path
      //     + "\", specified by the Path of the "
      //     + newsletter.Date.ToString("dd MMM yyyy")
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