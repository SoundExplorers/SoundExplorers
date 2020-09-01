using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Data;

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
    ///   The view to be shown.
    /// </param>
    /// <param name="tableName">
    ///   The name of the table whose data is to be displayed.
    /// </param>
    public TableController([NotNull] ITableView view,
      [NotNull] string tableName) {
      View = view;
      View.SetController(this);
      TableName = tableName;
    }

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's field properties.
    /// </summary>
    [CanBeNull]
    public IEntityColumnList Columns => Entities?.Columns;

    /// <summary>
    ///   Gets the data set containing the main table
    ///   and, if specified, the parent table.
    /// </summary>
    [CanBeNull]
    public DataSet DataSet => Entities?.DataSet;

    /// <summary>
    ///   The entity list representing the table whose data is shown in the table editor.
    /// </summary>
    [CanBeNull]
    private IEntityList Entities { get; set; }

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
        new Option($"{TableName}.GridSplitterDistance"));

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
        new Option($"{TableName}.ImageSplitterDistance"));

    public bool IsParentTableToBeShown => ParentTableName != null;

    /// <summary>
    ///   Gets the list of entities representing the main table's
    ///   parent table, if specified.
    /// </summary>
    [CanBeNull]
    public IEntityList ParentList => Entities?.ParentList;

    [CanBeNull] public string ParentTableName => ParentList?.TableName;
    [CanBeNull] public DataTable Table => Entities?.Table;
    [NotNull] public string TableName { get; }
    [NotNull] private ITableView View { get; }

    private void Entities_RowError(object sender, RowErrorEventArgs e) {
      View.OnRowError(e);
    }

    private void Entities_RowUpdated(object sender, RowUpdatedEventArgs e) {
      string databaseUpdateMessage = e.Message;
      string mediaTagsUpdateErrorMessage = null;
      if (e.Entity is IMediaEntity mediaEntity) {
        try {
          string updateTagsMessage = mediaEntity.UpdateTags();
          if (!string.IsNullOrEmpty(updateTagsMessage)) {
            databaseUpdateMessage += ". " + updateTagsMessage;
          }
        } catch (ApplicationException ex) {
          mediaTagsUpdateErrorMessage = ex.Message;
        }
      }
      View.OnRowUpdated(databaseUpdateMessage, mediaTagsUpdateErrorMessage);
    }

    public void FetchData() {
      Entities = TableName == nameof(ArtistInImage)
        ? new ArtistInImageList()
        : Factory<IEntityList>.Create(TableName);
      Entities.RowError += Entities_RowError;
      Entities.RowUpdated += Entities_RowUpdated;
    }

    /// <summary>
    ///   Updates the database table with any changes that have been input
    ///   and refreshes the list of Entities.
    /// </summary>
    public void Update(Dictionary<string, object> oldKeyFields = null) {
      Entities?.Update(oldKeyFields);
    }
  }
}