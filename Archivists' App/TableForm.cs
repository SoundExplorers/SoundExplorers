using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Data;
using Image = SoundExplorers.Data.Image;
using ImageList = SoundExplorers.Data.ImageList;

namespace SoundExplorers {
  internal partial class TableForm : Form {
    /// <summary>
    ///   Initialises a new instance of the <see cref="TableForm" /> class.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table whose data is to be displayed.
    /// </param>
    public TableForm(
      string tableName) {
      InitializeComponent();
      // A known Visual Studio bug is that PictureBox's AllowDrop property
      // appears neither in the designer nor in intellisence.
      // But is does exist and is required in order to 
      // allow anything to be dropped on to the PictureBox.
      FittedPictureBox1.AllowDrop = true;
      GridSplitContainer.GotFocus += SplitContainer_GotFocus;
      ImageSplitContainer.GotFocus += SplitContainer_GotFocus;
      TableName = tableName;
      OpenTable(TableName);
    }

    /// <summary>
    ///   The entity list representing the table whose data is shown on the form.
    /// </summary>
    public IEntityList Entities { get; private set; }

    private DataGridView FocusedGrid { get; set; }
    private Option GridSplitterDistanceOption { get; set; }
    private Option ImageSplitterDistanceOption { get; set; }
    private bool ParentRowChanged { get; set; }
    private RowErrorEventArgs RowErrorEventArgs { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; }
    private string TableName { get; }
    private DataGridViewRow UnchangedRow { get; set; }
    private bool UpdateCancelled { get; set; }

    public void Copy() {
      if (FocusedGrid.CurrentCell.Value == null) {
        return;
      }
      if (!FocusedGrid.IsCurrentCellInEditMode) {
        Clipboard.SetText(FocusedGrid.CurrentCell.Value.ToString());
        return;
      }
      switch (MainGrid.EditingControl) {
        // The current cell is in the main grid,
        // (the only grid that can be edited)
        // and is already being edited.
        case TextBox textBox when string.IsNullOrEmpty(textBox.SelectedText):
          // Clipboard.SetText throws an exception
          // if passed an empty string.
          return;
        case TextBox textBox:
          Clipboard.SetText(textBox.SelectedText);
          break;
        case PathEditingControl pathEditingControl:
          pathEditingControl.Copy();
          break;
      }
    }

    public void Cut() {
      if (FocusedGrid.CurrentCell.Value == null) {
        return;
      }
      if (!FocusedGrid.IsCurrentCellInEditMode) {
        Clipboard.SetText(FocusedGrid.CurrentCell.Value.ToString());
      }
      if (FocusedGrid.CurrentCell.ReadOnly) {
        return;
      }
      // The current cell is in the main grid,
      // (the only grid that can be edited).
      if (!MainGrid.IsCurrentCellInEditMode) {
        MainGrid.BeginEdit(true);
        MainGrid.CurrentCell.Value = string.Empty;
        MainGrid.EndEdit();
      } else {
        switch (MainGrid.EditingControl) {
          // The cell is already being edited
          case TextBox textBox: {
            if (string.IsNullOrEmpty(textBox.SelectedText)) {
              // Clipboard.SetText throws an exception
              // if passed an empty string.
              return;
            }
            Clipboard.SetText(textBox.SelectedText);
            textBox.SelectedText = string.Empty;
            break;
          }
          case PathEditingControl pathEditingControl:
            pathEditingControl.Cut();
            break;
        }
      }
    }

    /// <summary>
    ///   Updates the specified path cell with
    ///   the file path in the specified drop data,
    ///   focusing the main grid
    ///   and making the updated path cell the current cell.
    /// </summary>
    /// <param name="fileDropData">
    ///   A drag-and-drop operation's drop data containing a file path.
    /// </param>
    /// <param name="pathCell">The path cell to be updated.</param>
    private void DropPathOnCell(IDataObject fileDropData, PathCell pathCell) {
      var paths = fileDropData.GetData(DataFormats.FileDrop) as string[];
      if (paths == null
          || paths.Length == 0) {
        return;
      }
      if (FocusedGrid != MainGrid) {
        FocusGrid(MainGrid);
      }
      MainGrid.CurrentCell = pathCell;
      // Do the update between BeginEdit and EndEdit
      // to ensure that the appropriate event handlers are invoked.
      pathCell.Value = paths[0];
      MainGrid.BeginEdit(true);
      //pathCell.Value = paths[0];
      //MainGrid.EndEdit();
    }

    /// <summary>
    ///   Edit the tags of the audio file, if found,
    ///   of the current Piece, if any,
    ///   Otherwise shows an informative message box.
    /// </summary>
    public void EditAudioFileTags() {
      try {
        string path = GetMediumPath(Medium.Audio);
        var dummy = new AudioFile(path);
      } catch (ApplicationException ex) {
        MessageBox.Show(
          this,
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }

    /// <summary>
    ///   Handles the entity lists's EntityList.RowError event,
    ///   which occurs when there is an error on
    ///   attempting to insert, update or delete a database table row
    ///   corresponding to a row in the main grid.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void Entities_RowError(object sender, RowErrorEventArgs e) {
      RowErrorEventArgs = e;
      UpdateCancelled = true;
      RowErrorTimer.Start();
    }

    /// <summary>
    ///   Handles the entity lists's
    ///   EntityList.RowError event,
    ///   which occurs when a database table row
    ///   corresponding to a row in the main grid
    ///   has been successfully inserted or updated on the database
    ///   and <see cref="Entities" /> has been updated with the change.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void Entities_RowUpdated(object sender, RowUpdatedEventArgs e) {
      //Debug.WriteLine("Entities_RowUpdated RowIndex = " + e.RowIndex);
      var statusLabel = (ParentForm as MdiParentForm)?.StatusLabel ??
                        throw new NullReferenceException();
      statusLabel.Text = e.Message;
      var mediaEntity = e.Entity as IMediaEntity;
      if (mediaEntity == null) {
        return;
      }
      string updateTagsMessage;
      try {
        updateTagsMessage = mediaEntity.UpdateTags();
      } catch (ApplicationException ex) {
        MessageBox.Show(
          this,
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }
      if (updateTagsMessage != string.Empty) {
        statusLabel.Text += ". " + updateTagsMessage + ".";
      }
    }

    /// <summary>
    ///   Handles both the missing image label's
    ///   and the picture box's
    ///   <see cref="Control.DragDrop" /> event
    ///   to drop a file path on the label or picture box,
    ///   whichever is shown,
    ///   updating the corresponding Image.Path cell,
    ///   focusing the main grid
    ///   and making the updated cell the current cell.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   To save confusion,
    ///   this will not work while the main grid
    ///   is in edit mode.
    /// </remarks>
    private void FittedPictureBox1_DragDrop(object sender, DragEventArgs e) {
      if (Entities is ImageList
          && !MainGrid.IsCurrentCellInEditMode
          && e.Data.GetDataPresent(DataFormats.FileDrop)) {
        var pathCell = (PathCell)MainGrid.CurrentRow?.Cells["Path"] ??
                       throw new NullReferenceException();
        DropPathOnCell(
          e.Data,
          pathCell);
        ShowImageOrMessage(pathCell.Path);
      }
    }

    /// <summary>
    ///   Handles both the missing image label's
    ///   and the picture box's
    ///   <see cref="Control.DragOver" /> event
    ///   to show that a file path can be dropped on the label or picture box,
    ///   whichever is shown,
    ///   if the main grid shows the Image table and is not in edit mode.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   To save confusion,
    ///   path dropping is not supported while the main grid is in edit mode.
    /// </remarks>
    private void FittedPictureBox1_DragOver(object sender, DragEventArgs e) {
      if (Entities is ImageList
          && !MainGrid.IsCurrentCellInEditMode
          && e.Data.GetDataPresent(DataFormats.FileDrop)) {
        e.Effect = DragDropEffects.Copy;
      } else {
        e.Effect = DragDropEffects.None;
      }
    }

    /// <summary>
    ///   Handles the picture box's
    ///   <see cref="Control.MouseDown" /> event to
    ///   initiate a drag-and-drop operation
    ///   to allow the path of the image displayed in the picture box
    ///   to be dropped on another application.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void FittedPictureBox1_MouseDown(object sender, MouseEventArgs e) {
      var data = new DataObject(
        DataFormats.FileDrop,
        new[] {FittedPictureBox1.ImageLocation});
      FittedPictureBox1.DoDragDrop(
        data, DragDropEffects.Copy | DragDropEffects.None);
    }

    /// <summary>
    ///   Focuses the specified grid.
    /// </summary>
    /// <param name="grid">
    ///   The grid to be focused.
    /// </param>
    /// <remarks>
    ///   Where two grids are shown,
    ///   their colour schemes are swapped round,
    ///   indicating which is now the current grid
    ///   by having the usual colour scheme inverted
    ///   on the other grid.
    /// </remarks>
    private void FocusGrid(DataGridView grid) {
      if (Entities.ParentList == null) {
        grid.Focus();
        return;
      }
      // A read-only related grid for the parent table is shown
      // above the main grid.
      if (grid != FocusedGrid) {
        SwapGridColors();
      }
      // By trial an error,
      // I found that this complicated rigmarole was required to
      // properly shift the focus programatically, 
      // i.e. in TableForm_KeyDown to implement doing it with the F6 key.
      var unfocusedGrid =
        grid == MainGrid ? ParentGrid : MainGrid;
      unfocusedGrid.Enabled = false;
      grid.Enabled = true;
      base.Refresh(); // Don't want to repopulate grid, which this.Refresh would do!
      grid.Focus();
      base.Refresh(); // Don't want to repopulate grid, which this.Refresh would do!
      unfocusedGrid.Enabled = true;
      FocusedGrid = grid;
    }

    /// <summary>
    ///   Handle's the focus Timer's Tick event
    ///   to shift the focus to the required grid.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   For unknown reason,
    ///   when an existing table form is activated,
    ///   the split container gets focused.
    ///   Refocusing in this Timer fixes the problem.
    /// </remarks>
    private void FocusTimer_Tick(object sender, EventArgs e) {
      FocusTimer.Stop();
      if (FocusedGrid != null) {
        FocusGrid(FocusedGrid);
      } else {
        FocusGrid(ParentGrid);
      }
    }

    /// <summary>
    ///   Gets the cell that is at the specified client co-ordinates of the main grid.
    ///   Null if there is no cell at the coordinates.
    /// </summary>
    /// <param name="x">
    ///   The x co-ordinate relative to the main grid's client rectangle.
    /// </param>
    /// <param name="y">
    ///   The y co-ordinate relative to the main grid's client rectangle.
    /// </param>
    /// <returns>
    ///   The cell at the co-ordinates if found, otherwise null.
    /// </returns>
    private DataGridViewCell GetCellAtClientCoOrdinates(int x, int y) {
      var hitTestInfo = MainGrid.HitTest(x, y);
      if (hitTestInfo.Type == DataGridViewHitTestType.Cell) {
        return MainGrid.Rows[
          hitTestInfo.RowIndex].Cells[
          hitTestInfo.ColumnIndex];
      }
      return null;
    }

    /// <summary>
    ///   Returns the path of the specified medium file of the current Piece
    ///   if the file exists.
    /// </summary>
    /// <param name="medium">
    ///   The required medium.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   A suitable Piece is not selected
    ///   or the file is not specified or does not exist.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The specified medium is not supported.
    /// </exception>
    private string GetMediumPath(Medium medium) {
      var piece = GetPiece();
      string indefiniteArticle;
      string path;
      switch (medium) {
        case Medium.Audio:
          indefiniteArticle = "An";
          path = piece.AudioPath;
          break;
        case Medium.Video:
          indefiniteArticle = "A";
          path = piece.VideoPath;
          break;
        default:
          throw new ArgumentOutOfRangeException(
            nameof(medium),
            medium,
            "Medium " + medium + " is not supported.");
      } //End of switch (medium)
      if (string.IsNullOrEmpty(path)) {
        throw new ApplicationException(
          indefiniteArticle + " " + medium.ToString().ToLower()
          + " file has not been specified for the Piece.");
      }
      if (!File.Exists(path)) {
        throw new ApplicationException(
          medium + " file \"" + path + "\" cannot be found.");
      }
      return path;
      //Process.Start(path);
    }

    /// <summary>
    ///   Returns the Newsletter to be shown.
    /// </summary>
    /// <returns>
    ///   The Newsletter to be shown.
    /// </returns>
    /// <exception cref="ApplicationException">
    ///   A suitable Newsletter to show is not associated with the selected row.
    /// </exception>
    private Newsletter GetNewsletterToShow() {
      if (Entities is ArtistInImageList) {
        if (Entities.ParentList.Count > 0) {
          if (ParentGrid.CurrentRow != null) {
            var image =
              (Image)Entities.ParentList[ParentGrid.CurrentRow.Index];
            return image.FetchNewsletter();
          }
        }
        throw new ApplicationException(
          "You cannot show the newsletter for an Image's Performance because "
          + "no Images are listed in the ArtistInImageList window.");
      }
      if (Entities is CreditList) {
        if (Entities.ParentList.Count > 0) {
          var piece =
            (Piece)Entities.ParentList[ParentGrid.CurrentRow.Index];
          return piece.FetchNewsletter();
        }
        throw new ApplicationException(
          "You cannot show a Piece's newsletter because "
          + "no Pieces are listed in the Credit window.");
      }
      if (Entities is ImageList) {
        if (MainGrid.CurrentRow.IsNewRow) {
          throw new ApplicationException(
            "You must add the new Image before you can show its Performance's newsletter.");
        }
        if (MainGrid.IsCurrentCellInEditMode) {
          throw new ApplicationException(
            "While you are editing the Image, "
            + "you cannot show its Performance's newsletter.");
        }
        var image =
          (Image)Entities[MainGrid.CurrentRow.Index];
        if (image.Location !=
            MainGrid.CurrentRow.Cells["Location"].Value.ToString()
            || image.Date !=
            (DateTime)MainGrid.CurrentRow.Cells["Date"].Value) {
          throw new ApplicationException(
            "You must save or cancel changes to the Image "
            + "before you can show its Performance's newsletter.");
        }
        return image.FetchNewsletter();
      }
      if (Entities is NewsletterList) {
        if (MainGrid.CurrentRow.IsNewRow) {
          throw new ApplicationException(
            "You must add the new Newsletter before you can show it.");
        }
        if (MainGrid.IsCurrentCellInEditMode) {
          throw new ApplicationException(
            "You cannot show the Newsletter while you are editing it.");
        }
        var newsletter = (Newsletter)Entities[MainGrid.CurrentRow.Index];
        if (newsletter.Path !=
            MainGrid.CurrentRow.Cells["Path"].Value.ToString()) {
          throw new ApplicationException(
            "You must save or cancel changes to the Newsletter before you can show it.");
        }
        return newsletter;
      }
      if (Entities is PieceList) {
        if (Entities.ParentList.Count > 0) {
          var set =
            (Set)Entities.ParentList[ParentGrid.CurrentRow.Index];
          return set.FetchNewsletter();
        }
        throw new ApplicationException(
          "You cannot show a Set's newsletter because "
          + "no Sets are listed in the Piece window.");
      }
      if (Entities is PerformanceList) {
        if (MainGrid.CurrentRow.IsNewRow) {
          throw new ApplicationException(
            "You must add the new Performance before you can show its newsletter.");
        }
        if (MainGrid.IsCurrentCellInEditMode) {
          throw new ApplicationException(
            "You cannot show the Performance's newsletter "
            + "while you are editing the Performance.");
        }
        var performance = (Performance)Entities[MainGrid.CurrentRow.Index];
        if (performance.Newsletter !=
            (DateTime)MainGrid.CurrentRow.Cells["Newsletter"].Value) {
          throw new ApplicationException(
            "You must save or cancel changes to the Performance "
            + "before you can show its newsletter.");
        }
        return performance.FetchNewsletter();
      }
      if (Entities is SetList) {
        if (Entities.ParentList.Count > 0) {
          var performance =
            (Performance)Entities.ParentList[ParentGrid.CurrentRow.Index];
          return performance.FetchNewsletter();
        }
        throw new ApplicationException(
          "You cannot show a Performance's newsletter because "
          + "no Performances are listed in the Set window.");
      }
      throw new ApplicationException(
        "Newsletters are not associated with the " + TableName + " table."
        + Environment.NewLine
        + "To show a newsletter, first select a row in an "
        + "ArtistInImage, Credit, Image, Newsletter, "
        + "Performance, Piece or Set table window.");
    }

    /// <summary>
    ///   Returns the Piece to be played.
    /// </summary>
    /// <returns>
    ///   The Piece to be played.
    /// </returns>
    /// <exception cref="ApplicationException">
    ///   A suitable Piece to play is not selected.
    /// </exception>
    private Piece GetPiece() {
      if (Entities is CreditList) {
        if (Entities.ParentList.Count > 0) {
          return (Piece)Entities.ParentList[ParentGrid.CurrentRow.Index];
        }
        throw new ApplicationException(
          "You cannot play a Piece because no Pieces are listed in the Credit window.");
      }
      if (Entities is PieceList) {
        if (MainGrid.CurrentRow.IsNewRow) {
          throw new ApplicationException(
            "You must add the new Piece before you can play it.");
        }
        if (MainGrid.IsCurrentCellInEditMode) {
          throw new ApplicationException(
            "You cannot play the Piece while you are editing it.");
        }
        // Because this is the detail grid in a master-detail relationship
        // the index of the Piece in the grid is probably different
        // from its index in the entitly list,
        // which will contain all the Pieces of all the Performances in the parent grid.
        // So we need to find it the hard way.
        var piece = (
          from Piece p in (PieceList)Entities
          where p.Date == (DateTime)MainGrid.CurrentRow.Cells["Date"].Value
                && p.Location == MainGrid.CurrentRow.Cells["Location"].Value
                  .ToString()
                && p.Set == (int)MainGrid.CurrentRow.Cells["Set"].Value
                && p.PieceNo == (int)MainGrid.CurrentRow.Cells["PieceNo"].Value
                && p.AudioPath == MainGrid.CurrentRow.Cells["AudioPath"].Value
                  .ToString()
                && p.VideoPath == MainGrid.CurrentRow.Cells["VideoPath"].Value
                  .ToString()
          select p).FirstOrDefault();
        if (piece == null) {
          // Piece not found.
          // As Date, Location and Set are invisible
          // (because common to the parent Set row),
          // PieceNo, AudioPath or VideoPath must have been 
          // changed without yet updating the database.
          throw new ApplicationException(
            "You must save or cancel changes to the Piece before you can play it.");
        }
        return piece;
      }
      throw new ApplicationException(
        "Media files are not associated with the " + TableName + " table."
        + Environment.NewLine
        + "To play a piece, first select a row in a Credit or Piece table window.");
    }

    private void Grid_Click(object sender, EventArgs e) {
      var grid = sender as DataGridView;
      if (grid != FocusedGrid) {
        FocusGrid(grid);
      }
    }

    /// <summary>
    ///   Handles the
    ///   <see cref="Control.MouseDown" /> event
    ///   of either of the two <see cref="DataGridView" />s.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   To save confusion,
    ///   none of the following will work while the main grid
    ///   is in edit mode.
    ///   When mouse button 2 is clicked,
    ///   the grid will be focused if it is not already.
    ///   When mouse button 2 is clicked on a cell,
    ///   the cell will become the current cell.
    ///   When mouse button 2 is clicked on a path cell,
    ///   a drag-and-drop operation to allow the path to be
    ///   dropped on another application will be initiated.
    ///   <para>
    ///     I tried to initiate a drag-and-drop operation
    ///     when a cell is clicked with the mouse button 1.
    ///     But it did not work, possibly because it
    ///     puts the cell into edit mode and also,
    ///     when dragged, selects multiple rows.
    ///     But what if Dan's Mac mouse does not have two buttons?
    ///     Perhaps we could initiate a drag-and-drop operation
    ///     on Control + mouse button 1.
    ///   </para>
    /// </remarks>
    private void Grid_MouseDown(object sender, MouseEventArgs e) {
      var grid = sender as DataGridView;
      if (MainGrid.IsCurrentCellInEditMode) {
        return;
      }
      if (e.Button == MouseButtons.Right) {
        if (grid != FocusedGrid) {
          FocusGrid(grid);
        }
        // Find the cell, if any, that mouse button 2 clicked.
        var cell = GetCellAtClientCoOrdinates(e.X, e.Y);
        if (cell != null) { // Cell found
          grid.CurrentCell = cell;
          var pathCell = cell as PathCell;
          if (pathCell != null && pathCell.FileExists) {
            // This is a path cell that contains the path of an existing file
            var data =
              new DataObject(DataFormats.FileDrop, new[] {pathCell.Path});
            grid.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.None);
          }
        }
      }
    }

    /// <summary>
    ///   Inverts the foreground and background colours
    ///   of both selected and unselected cells
    ///   in the specified grid.
    /// </summary>
    /// <param name="grid">
    ///   The grid whose colours are to be inverted.
    /// </param>
    private void InvertGridColors(DataGridView grid) {
      var swapColor = grid.DefaultCellStyle.BackColor;
      grid.DefaultCellStyle.BackColor = grid.DefaultCellStyle.ForeColor;
      grid.DefaultCellStyle.ForeColor = swapColor;
      swapColor = grid.DefaultCellStyle.SelectionBackColor;
      grid.DefaultCellStyle.SelectionBackColor =
        grid.DefaultCellStyle.SelectionForeColor;
      grid.DefaultCellStyle.SelectionForeColor = swapColor;
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="DataGridView.CellBeginEdit" /> event,
    ///   which occurs when edit mode starts for the currently selected cell,
    ///   to hide the Missing Image label (if visible).
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   This is only relevent if the Path cell of an Image row is being edited.
    ///   If the Missing Image label was visible just before entering edit mode,
    ///   it will have been because the file was not specified or can't be found or is not an image file.
    ///   That's presumably about to be rectified.
    ///   So the message to that effect in the Missing Image label could be misleading.
    ///   Also, the advice in the Missing Image label that an image file can
    ///   be dragged onto the label will not apply, as dragging and dropping is disabled
    ///   while the Path cell is being edited.
    /// </remarks>
    private void MainGrid_CellBeginEdit(object sender,
      DataGridViewCellCancelEventArgs e) {
      MissingImageLabel.Visible = false;
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="DataGridView.CellEndEdit" /> event,
    ///   which occurs when edit mode stops for the currently selected cell.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   If a path cell has been changed
    ///   and the new path specifies a file that exists,
    ///   the default for the initial folder of the Open dialogue that may be
    ///   shown for subsequent updates of cells in the same column
    ///   will be set to the folder of the new path.
    ///   (When the Open dialogue is subsequently shown,
    ///   if the cell being edited initially contains
    ///   a path that includes a specific folder that exists,
    ///   the initial folder for the Open dialogue
    ///   will be set to that folder.
    ///   Otherwise the initial folder for the Open dialogue
    ///   will be set to this column-specific default folder.)
    ///   <para>
    ///   </para>
    ///   If an image path specifies a valid image,
    ///   the image will be shown below the main grid.
    ///   </para>
    ///   <para>
    ///     If an image path does not specifies a valid image,
    ///     a Missing Image label containing an appropriate message will be displayed.
    ///   </para>
    /// </remarks>
    private void MainGrid_CellEndEdit(object sender,
      DataGridViewCellEventArgs e) {
      //Debug.WriteLine("MainGrid_CellEndEdit");
      var pathCell = MainGrid.CurrentCell as PathCell;
      if (pathCell == null) { // Not a path cell.
        return;
      }
      if (pathCell.Path == null) {
        return;
      }
      var column = pathCell.OwningColumn;
      //Debug.WriteLine("MainGrid_CellEndEdit " + column.Name);
      if (Entities is ImageList) { // Image.Path
        // We need to have a go at showing the image or message
        // even if nothing has changed.
        // This allows for:
        //
        // a) the message being temporarily hidden while editing
        //    (see MainGrid_CellBeginEdit)
        //
        // and
        //
        // b) a new image being shown as a result of
        //    a path being dropped on the cell
        //    but the user then cancelling the update of the row.
        ShowImageOrMessage(pathCell.Path);
      }
      if (pathCell.Path != string.Empty
          && (UnchangedRow == null
              || UnchangedRow.Cells[e.ColumnIndex].Value == null
              || pathCell.Path !=
              UnchangedRow.Cells[e.ColumnIndex].Value.ToString())) {
        FileInfo file;
        try {
          file = new FileInfo(pathCell.Path);
        } catch (ArgumentException ex) {
          MessageBox.Show(
            this,
            ex.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
          return;
        }
        if (pathCell.FileExists) {
          switch (column.Name) {
            case "AudioPath": // Piece.AudioPath
              Piece.DefaultAudioFolder = file.Directory;
              break;
            case "Path":
              if (Entities is ImageList) { // Image.Path
                Image.DefaultFolder = file.Directory;
              } else if (Entities is NewsletterList) { // Newsletter.Path
                Newsletter.DefaultFolder = file.Directory;
              } else {
                throw new NotSupportedException(
                  TableName + ".Path is not supported.");
              }
              break;
            case "VideoPath": // Piece.VideoPath
              Piece.DefaultVideoFolder = file.Directory;
              break;
          } //End of switch
        }
      }
    }

    //private void MainGrid_CellEnter(object sender, DataGridViewCellEventArgs e) {
    //    // This debug proves that, when the grid is bound to a second or subsequent list,
    //    // the type of each cell, which determines the cell editor to be used,
    //    // is always DataGridViewTextBoxCell,
    //    // irrespective of what the colunm's CellTemplate,
    //    // as shown by the column's CellType property,
    //    // has been set.
    //    //var cell = MainGrid.CurrentCell;
    //DataGridViewColumn column = cell.OwningColumn;
    //    //Debug.WriteLine("MainGrid_CellEnter  " + column.Name);
    //    //Debug.WriteLine("cell.ValueType = " + cell.ValueType.Name);
    //    //Debug.WriteLine("column.CellType = " + column.CellType.Name);
    //    //Debug.WriteLine("cell.GetType = " + cell.GetType().Name);
    //    //Debug.WriteLine(string.Empty);
    //}

    //private void MainGrid_CellValidated(object sender, DataGridViewCellEventArgs e) {
    //}

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="DataGridView.DataError" /> event,
    ///   which occurs when an external data-parsing or validation operation throws an exception.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   So far, this event has only been raised when
    ///   a referenced table contains no rows that
    ///   can be made available for selection in a ComboBox cell's drop-down list:
    ///   see ComboBoxCell.ThrowNoAvailableReferencesException.
    ///   In this application at least, this is NOT
    ///   the event that is raised when there is an error on
    ///   attempting to insert, update or delete a database table row
    ///   corresponding to a row in the main grid
    ///   (even though the DataGridView.DataError documentation says that might happen):
    ///   that event is EntityList.RowError and is handled by Entities_RowError.
    /// </remarks>
    private void MainGrid_DataError(object sender,
      DataGridViewDataErrorEventArgs e) {
      //Debug.WriteLine("MainGrid_DataError");
      //Debug.WriteLine("Context = " + e.Context.ToString());
      //Debug.WriteLine("RowIndex = " + e.ColumnIndex.ToString());
      //Debug.WriteLine("RowIndex = " + e.RowIndex.ToString());
      //MainGrid.CurrentCell = MainGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //Refresh();
      MainGrid.CancelEdit();
      if (e.Exception is ApplicationException) {
        MessageBox.Show(
          this,
          e.Exception.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      } else {
        MessageBox.Show(
          this,
          e.Exception.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
      e.Cancel = true; // This does not seem to make any difference.
      //e.Cancel = false;
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="Control.DragDrop" /> event
    ///   to drop a file path on a path cell,
    ///   focusing the main grid
    ///   and making the updated path cell the current cell.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   To save confusion,
    ///   this will not work while the main grid
    ///   is in edit mode.
    /// </remarks>
    private void MainGrid_DragDrop(object sender, DragEventArgs e) {
      if (MainGrid.IsCurrentCellInEditMode) {
        return;
      }
      if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
        return;
      }
      // Find the path cell, if any, that is being dropped onto.
      var clientCoOrdinates = MainGrid.PointToClient(new Point(e.X, e.Y));
      var pathCell =
        GetCellAtClientCoOrdinates(
            clientCoOrdinates.X, clientCoOrdinates.Y)
          as PathCell;
      if (pathCell != null) { // Dropping onto a path cell
        DropPathOnCell(
          e.Data,
          pathCell);
      }
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="Control.DragOver" /> event
    ///   to show that a file path can be dropped on a path cell.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   To save confusion,
    ///   path dropping is not supported while the main grid is in edit mode.
    /// </remarks>
    private void MainGrid_DragOver(object sender, DragEventArgs e) {
      if (MainGrid.IsCurrentCellInEditMode) {
        e.Effect = DragDropEffects.None;
        return;
      }
      if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
        e.Effect = DragDropEffects.None;
        return;
      }
      // Find the cell, if any, that the mouse is over.
      var clientCoOrdinates = MainGrid.PointToClient(new Point(e.X, e.Y));
      var pathCell =
        GetCellAtClientCoOrdinates(
            clientCoOrdinates.X, clientCoOrdinates.Y)
          as PathCell;
      if (pathCell != null) { // The mouse is over a path cell
        e.Effect = DragDropEffects.Copy;
      } else { // The mouse is not over a path cell
        e.Effect = DragDropEffects.None;
      }
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="Control.KeyDown" /> event to:
    ///   delete any selected rows on Backspace (Delete on Mac keyboard).
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void MainGrid_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.Back:
          foreach (DataGridViewRow selectedRow in MainGrid.SelectedRows) {
            MainGrid.Rows.Remove(selectedRow);
          } //End of foreach
          break;
      } //End of switch
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="DataGridView.RowEnter" /> event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   <para>
    ///     The initial state of the row will be saved to a dettached row
    ///     to allow comparison with any changes if the row gets edited.
    ///   </para>
    ///   <para>
    ///     If the row represents an Image whose Path
    ///     specifies a valid image file,
    ///     the image will be shown below the main grid.
    ///     If the row represents an Image whose Path
    ///     does not specifies a valid image file,
    ///     a Missing Image label containing an appropriate message will be displayed.
    ///   </para>
    /// </remarks>
    private void MainGrid_RowEnter(object sender, DataGridViewCellEventArgs e) {
      //Debug.WriteLine("MainGrid_RowEnter");
      //
      // e.RowIndex is not necessarily the same as 
      // MainGrid.CurrentRow.Index.
      // In any case MainGrid.CurrentRow will be null 
      // while the grid is being built
      // and possibly at other times.
      // The following Debugs prove those two points:
      //
      //Debug.WriteLine("e.RowIndex = " + e.RowIndex);
      //Debug.WriteLine("MainGrid.RowCount = " + MainGrid.RowCount);
      //Debug.WriteLine("(e.RowIndex == MainGrid.RowCount - 1) = " + (e.RowIndex == MainGrid.RowCount - 1));
      //Debug.WriteLine("MainGrid.CurrentRow == null? " + (MainGrid.CurrentRow == null).ToString());
      //try {
      //    Debug.WriteLine("MainGrid.CurrentRow.Index = " + MainGrid.CurrentRow.Index);
      //    Debug.WriteLine("(MainGrid.CurrentRow.Index == MainGrid.RowCount - 1) = " + (MainGrid.CurrentRow.Index == MainGrid.RowCount - 1));
      //    Debug.WriteLine("MainGrid.CurrentRow.IsNewRow = " + MainGrid.CurrentRow.IsNewRow);
      //} catch {
      //}
      //
      // So this is the safe way of checking whether we have entered the new row:
      if (e.RowIndex == MainGrid.RowCount - 1) {
        // Not this:
        //if (MainGrid.CurrentRow == null
        //||  MainGrid.CurrentRow.Index == MainGrid.RowCount - 1) {
        // Nor this:
        //if (MainGrid.CurrentRow.IsNewRow) {
        // New row
        //Debug.WriteLine("New row");
        UnchangedRow = null;
        if (Entities is ImageList) {
          ShowImageOrMessage(null);
        }
        return;
      }
      // Not new row
      //Debug.WriteLine("Not new row");
      // The inserted/updated row is not necessarily the current row.
      var row = MainGrid.Rows[e.RowIndex];
      UnchangedRow = (DataGridViewRow)row.Clone();
      for (var columnIndex = 0;
        columnIndex < MainGrid.ColumnCount;
        columnIndex++) {
        //Debug.WriteLine(
        //    MainGrid.Columns[columnIndex].Name + " = "
        //    + row.Cells[columnIndex].Value);
        UnchangedRow.Cells[columnIndex].Value = row.Cells[columnIndex].Value;
      } // End of for
      //Debug.WriteLine(UnchangedRow.Cells[0].Value);
      // If the main grid represents Pieces or Credits,
      // we need to conserve the current state of the
      // Piece and its credits.
      // This information will be required when
      // saving any changes to the current Piece or Credit
      // to the metadata tags of the Piece's audio file.
      if (Entities is CreditList) {
        var parentPiece = GetPiece();
        if (parentPiece != null) {
          var credits = (
            from Credit credit in (CreditList)Entities
            where credit.Date == parentPiece.Date
                  && credit.Location == parentPiece.Location
                  && credit.Set == parentPiece.Set
                  && credit.Piece == parentPiece.PieceNo
            select credit).ToList();
          parentPiece.Credits = new CreditList(true);
          foreach (var credit in credits) {
            parentPiece.Credits.Add(credit);
          }
          parentPiece.Original = parentPiece.Clone();
        }
      } else if (Entities is ImageList) {
        var image = Entities[e.RowIndex] as Image;
        ShowImageOrMessage(image.Path);
      } else if (Entities is PieceList) {
        var piece = (
          from Piece p in (PieceList)Entities
          where p.Date == (DateTime)row.Cells["Date"].Value
                && p.Location == row.Cells["Location"].Value.ToString()
                && p.Set == (int)row.Cells["Set"].Value
                && p.PieceNo == (int)row.Cells["PieceNo"].Value
          select p).FirstOrDefault();
        if (piece != null) {
          piece.Credits = piece.FetchCredits();
          piece.Original = piece.Clone();
        }
      }
    }

    private void MainGrid_RowsRemoved(object sender,
      DataGridViewRowsRemovedEventArgs e) {
      //Debug.WriteLine("MainGrid_RowsRemoved");
      //Debug.WriteLine(MainGrid.Rows[e.RowIndex].Cells[0].Value);
      if (UpdateCancelled) {
        return;
      }
      try {
        UpdateDatabase();
      } catch (DataException ex) {
        MessageBox.Show(
          ex.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }

    private void MainGrid_RowValidated(object sender,
      DataGridViewCellEventArgs e) {
      //Debug.WriteLine("MainGrid_RowValidated");
      //Debug.WriteLine(MainGrid.Rows[e.RowIndex].Cells[0].Value);
      if (ParentRowChanged) {
        ParentRowChanged = false;
        // Fairly sure this is no longer required to prevent unnecessary updates.
        // It can definitely erroneously prevent a NECESSARY update, e.g. for Set.
        //return;
      }
      if (UpdateCancelled) {
        return;
      }
      if (MainGrid.RowCount == 1) {
        // There's only the uncommitted new row,
        // which can be discarded.
        return;
      }
      //Debug.WriteLine("UnchangedRow.Index = " + UnchangedRow.Index);
      var isUpdateRequired = false;
      var oldKeyFields = new Dictionary<string, object>();
      try {
        foreach (DataGridViewColumn column in MainGrid.Columns) {
          // When object values are compared here,
          // they always seem to be unequal,
          // leading to a false positive that an update is required.
          // Nasty!
          // So we convert the new and old values to strings before comparing them.
          // That fixes the problem.
          var newValue = MainGrid.Rows[e.RowIndex].Cells[column.Name].Value;
          string newString =
            newValue != null ? newValue.ToString() : string.Empty;
          var oldValue = UnchangedRow != null
            ? UnchangedRow.Cells[column.Index].Value
            : null;
          string oldString =
            oldValue != null ? oldValue.ToString() : string.Empty;
          if (column.Visible
              && newString != oldString) {
            if (column.ValueType != typeof(DateTime)
                || (DateTime)newValue != DateTime.Parse("01 Jan 1900")) {
              isUpdateRequired = true;
            }
          }
          if (Entities.Columns[column.Name].IsInPrimaryKey) {
            oldKeyFields.Add(column.Name, oldValue);
          }
          if (newValue == DBNull.Value) {
            if (column.ValueType == typeof(DateTime)) {
              MainGrid.Rows[e.RowIndex].Cells[column.Name].Value =
                DateTime.Parse("01 Jan 1900");
            } else if (column.Name == "ImageId") {
              // Set ImageId to the next value in the ImageId column's sequence.
              // ImageId is a SERIAL column.
              // So it could be defaulted to get the same effect.
              // But, without setting ImageId in the grid and its bound DataTable,
              // it would not be possible for EntityList.Refresh
              // to refresh the Image entities in the ImageList.
              MainGrid.Rows[e.RowIndex].Cells[column.Name].Value =
                Image.GetNextImageId();
            }
          }
        } // End of foreach
        if (isUpdateRequired) {
          UpdateDatabase(oldKeyFields);
        }
      } catch (DataException ex) {
        MessageBox.Show(
          ex.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }

    /// <summary>
    ///   Opens the specified table.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table whose
    ///   data is to be displayed.
    /// </param>
    protected void OpenTable(string tableName) {
      InvertGridColors(ParentGrid); // Will revert when focused.
      PopulateGrid(tableName);
    }

    /// <summary>
    ///   Handles the parent grid's CurrentCellChanged event
    ///   to resize the main grid when its contents are automatically
    ///   kept in step with the parent grid row change.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   This really only needs to be done when the current row changes.
    ///   But there's no event for that.  The RowEnter event is raised
    ///   just before the row becomes current.  So it is too early
    ///   to work:  I tried.
    /// </remarks>
    private void ParentGrid_CurrentCellChanged(object sender, EventArgs e) {
      MainGrid.AutoResizeColumns();
    }

    /// <summary>
    ///   Handles the parent grid's
    ///   <see cref="DataGridView.RowEnter" /> event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   If the row represents an Image whose Path
    ///   specifies a valid image file,
    ///   the image will be shown below the main grid.
    ///   If the row represents an Image whose Path
    ///   does not specifies a valid image file,
    ///   a Missing Image label containing an appropriate message will be displayed.
    /// </remarks>
    private void
      ParentGrid_RowEnter(object sender, DataGridViewCellEventArgs e) {
      ParentRowChanged = true;
      if (Entities.ParentList is ImageList
          && e.RowIndex < Entities.ParentList.Count) {
        var image = Entities.ParentList[e.RowIndex] as Image;
        ShowImageOrMessage(image.Path);
      }
    }

    public void Paste() {
      if (FocusedGrid != MainGrid) {
        return;
      }
      if (!MainGrid.CurrentCell.ReadOnly) {
        if (!MainGrid.IsCurrentCellInEditMode) {
          MainGrid.BeginEdit(true);
          MainGrid.CurrentCell.Value = Clipboard.GetText();
          MainGrid.EndEdit();
        } else { // The cell is already being edited
          if (MainGrid.EditingControl is TextBox) {
            (MainGrid.EditingControl as TextBox).SelectedText =
              Clipboard.GetText();
          } else if (MainGrid.EditingControl is PathEditingControl) {
            (MainGrid.EditingControl as PathEditingControl).Paste();
          }
        }
      }
    }

    /// <summary>
    ///   Plays the audio, if found,
    ///   of the current Piece, if any.
    ///   Otherwise shows an informative message box.
    /// </summary>
    public void PlayAudio() {
      try {
        Process.Start(GetMediumPath(Medium.Audio));
      } catch (ApplicationException ex) {
        MessageBox.Show(
          this,
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    /// <summary>
    ///   Plays the video, if found,
    ///   of the current Piece, if any.
    ///   Otherwise shows an informative message box.
    /// </summary>
    public void PlayVideo() {
      try {
        Process.Start(GetMediumPath(Medium.Video));
      } catch (ApplicationException ex) {
        MessageBox.Show(
          this,
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    /// <summary>
    ///   Populates the grid.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table whose data is to be displayed.
    /// </param>
    private void PopulateGrid(string tableName) {
      if (tableName == "ArtistInImage") {
        Entities = new ArtistInImageList();
        //if (tableName == "Credit") {
        //    Entities = new CreditList();
        //if (tableName == "Image") {
        //    Entities = new SoundExplorers.Data.ImageList();
        //} else if (tableName == "Piece") {
        //    Entities = new PieceList();
        //} else if (tableName == "Set") {
        //    Entities = new SetList();
      } else {
        Entities = Factory<IEntityList>.Create(tableName);
      }
      Entities.RowError += Entities_RowError;
      Entities.RowUpdated += Entities_RowUpdated;
      Text = Entities.TableName;
      MainGrid.CellBeginEdit -= MainGrid_CellBeginEdit;
      MainGrid.CellEndEdit -= MainGrid_CellEndEdit;
      //MainGrid.CellEnter -= new DataGridViewCellEventHandler(MainGrid_CellEnter);
      //MainGrid.CellStateChanged -= new DataGridViewCellStateChangedEventHandler(Grid_CellStateChanged);
      //MainGrid.CellValidated -= new DataGridViewCellEventHandler(MainGrid_CellValidated);
      MainGrid.Click -= Grid_Click;
      MainGrid.DataError -= MainGrid_DataError;
      MainGrid.DragDrop -= MainGrid_DragDrop;
      MainGrid.DragOver -= MainGrid_DragOver;
      //MainGrid.GotFocus -= new EventHandler(Control_GotFocus);
      MainGrid.KeyDown -= MainGrid_KeyDown;
      //MainGrid.LostFocus -= new EventHandler(Control_LostFocus);
      MainGrid.MouseDown -= Grid_MouseDown;
      //MainGrid.RowStateChanged
      MainGrid.RowEnter -= MainGrid_RowEnter;
      MainGrid.RowsRemoved -= MainGrid_RowsRemoved;
      MainGrid.RowValidated -= MainGrid_RowValidated;
      if (Entities.ParentList != null) {
        // A read-only related grid for the parent table is to be shown
        // above the main grid.
        PopulateParentGrid();
        MainGrid.DataSource = new BindingSource(
          ParentGrid.DataSource,
          Entities.DataSet.Relations[0].RelationName);
        foreach (var entityColumn in Entities.Columns) {
          if (entityColumn.ColumnName != "Comments") {
            // Comments is on multiple tables
            if (Entities.ParentList.Columns.ContainsKey(entityColumn.ColumnName)
                || Entities.ParentList.Columns.ContainsKey(entityColumn
                  .ReferencedColumnName)) {
              entityColumn.Visible = false;
            }
          }
        } //End of foreach
      } else { // No parent grid
        MainGrid.DataSource = Entities.Table.DefaultView;
      }
      foreach (DataGridViewColumn column in MainGrid.Columns) {
        var entityColumn = Entities.Columns[column.Index];
        column.Visible = entityColumn.Visible;
        if (column.Visible) {
          if (column.ValueType == typeof(string)) {
            // Interpret blanking a cell as an empty string, not NULL.
            // This only works when updating, not inserting.
            // When inserting, do something like this in the SQL:
            //  coalesce(@Comments, '')
            column.DefaultCellStyle.DataSourceNullValue = string.Empty;
          } else if (column.ValueType == typeof(DateTime)) {
            column.DefaultCellStyle.Format = "dd MMM yyyy";
          }
          if (!string.IsNullOrEmpty(entityColumn.ReferencedColumnName)) {
            var comboBoxCell = new ComboBoxCell();
            comboBoxCell.Column = entityColumn;
            column.CellTemplate = comboBoxCell;
          } else if (column.ValueType == typeof(DateTime)) {
            column.CellTemplate = new CalendarCell();
          } else if (column.Name.EndsWith("Path")) {
            var pathCell = new PathCell();
            pathCell.Column = entityColumn;
            column.CellTemplate = pathCell;
          }
        }
      } // End of foreach
      MainGrid.CellBeginEdit += MainGrid_CellBeginEdit;
      MainGrid.CellEndEdit += MainGrid_CellEndEdit;
      //MainGrid.CellEnter += new DataGridViewCellEventHandler(MainGrid_CellEnter);
      //MainGrid.CellStateChanged += new DataGridViewCellStateChangedEventHandler(Grid_CellStateChanged);
      //MainGrid.CellValidated += new DataGridViewCellEventHandler(MainGrid_CellValidated);
      MainGrid.Click += Grid_Click;
      MainGrid.DataError += MainGrid_DataError;
      MainGrid.DragDrop += MainGrid_DragDrop;
      MainGrid.DragOver += MainGrid_DragOver;
      //MainGrid.GotFocus += new EventHandler(Control_GotFocus);
      MainGrid.KeyDown += MainGrid_KeyDown;
      //MainGrid.LostFocus += new EventHandler(Control_LostFocus);
      MainGrid.MouseDown += Grid_MouseDown;
      MainGrid.RowEnter += MainGrid_RowEnter;
      MainGrid.RowsRemoved += MainGrid_RowsRemoved;
      MainGrid.RowValidated += MainGrid_RowValidated;
      if (MainGrid.RowCount > 0) {
        // Not reliable because top left cell could be hidden.
        //MainGrid.CurrentCell = MainGrid.Rows[0].Cells[0]; // Triggers MainGrid_RowEnter
        MainGrid_RowEnter(this, new DataGridViewCellEventArgs(0, 0));
      }
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        MainGrid.AutoResizeColumns();
        if (Entities.ParentList != null) {
          // A read-only related grid for the parent table is to be shown
          // above the main grid.
          ParentGrid.Focus();
        } else { // No parent grid
          MainGrid.Focus();
        }
      }
    }

    private void PopulateParentGrid() {
      ParentGrid.Click -= Grid_Click;
      ParentGrid.CurrentCellChanged -= ParentGrid_CurrentCellChanged;
      //ParentGrid.GotFocus -= new EventHandler(Control_GotFocus);
      //ParentGrid.LostFocus -= new EventHandler(Control_LostFocus);
      ParentGrid.MouseDown -= Grid_MouseDown;
      ParentGrid.RowEnter -= ParentGrid_RowEnter;
      ParentGrid.DataSource = new BindingSource(
        Entities.DataSet,
        Entities.ParentList.TableName);
      foreach (DataGridViewColumn column in ParentGrid.Columns) {
        var entityColumn = Entities.ParentList.Columns[column.Index];
        column.Visible = entityColumn.Visible;
        if (column.Visible) {
          if (column.ValueType == typeof(DateTime)) {
            column.DefaultCellStyle.Format = "dd MMM yyyy";
          }
          if (column.Name.EndsWith("Path")) {
            // Although we don't edit cells in the parent grid,
            // we still need to make the cell a PathCell,
            // as this is expected when playing media etc.
            var pathCell = new PathCell();
            pathCell.Column = entityColumn;
            column.CellTemplate = pathCell;
          }
        }
      } // End of foreach
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        ParentGrid.AutoResizeColumns();
      }
      ParentGrid.Click += Grid_Click;
      ParentGrid.CurrentCellChanged += ParentGrid_CurrentCellChanged;
      //ParentGrid.GotFocus += new EventHandler(Control_GotFocus);
      //ParentGrid.LostFocus += new EventHandler(Control_LostFocus);
      ParentGrid.MouseDown += Grid_MouseDown;
      ParentGrid.RowEnter += ParentGrid_RowEnter;
      if (ParentGrid.RowCount > 0) {
        ParentGrid.CurrentCell =
          ParentGrid.Rows[0].Cells[0]; // Triggers ParentGrid_RowEnter
      }
    }

    /// <summary>
    ///   Refreshes the contents of the grid from the database and
    ///   forces the form to invalidate its client area and immediately redraw itself
    ///   and any child controls.
    /// </summary>
    public override void Refresh() {
      PopulateGrid(Entities.TableName);
      if (Entities.ParentList != null) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        FocusGrid(ParentGrid);
      } else {
        MainGrid.Focus();
        FocusedGrid = MainGrid;
      }
      base.Refresh();
    }

    /// <summary>
    ///   Handle's the row error Timer's Tick event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   Having to use a Timer in order for
    ///   making the error row the current row to work.
    /// </remarks>
    private void RowErrorTimer_Tick(object sender, EventArgs e) {
      RowErrorTimer.Stop();
      //Debug.WriteLine("RowErrorTimer_Tick");
      MainGrid.CancelEdit();
      // Focus the error row and cell.
      try {
        if (MainGrid.Columns[RowErrorEventArgs.ColumnIndex].Visible) {
          MainGrid.CurrentCell = MainGrid.Rows[
            RowErrorEventArgs.RowIndex].Cells[
            RowErrorEventArgs.ColumnIndex];
        } else {
          MainGrid.CurrentCell = MainGrid.Rows[
            RowErrorEventArgs.RowIndex].Cells[
            MainGrid.CurrentCell.ColumnIndex];
        }
      } catch (ArgumentOutOfRangeException) {
        // Hopefully this is fixed now
        // (by comparing strings instead of objects in MainGrid_RowValidated)
        // but we shall see.
        // I got this to happen once and can't replicate it.
        // Better to just leave the focus where it is
        // with no error message
        // than let the program annoy the users with a weird message.
        // They can complain if they observe the problem.
        Debug.WriteLine("RowErrorTimer_Tick ArgumentOutOfRangeException");
        try {
          Debug.WriteLine("TableName = " + TableName);
          Debug.WriteLine("RowErrorEventArgs.ColumnIndex = " +
                          RowErrorEventArgs.ColumnIndex);
          Debug.WriteLine("RowErrorEventArgs.RowIndex = " +
                          RowErrorEventArgs.RowIndex);
          Debug.WriteLine("MainGrid.CurrentCell.ColumnIndex = " +
                          MainGrid.CurrentCell.ColumnIndex);
          Debug.WriteLine("MainGrid.CurrentRow.Index = " +
                          MainGrid.CurrentRow.Index);
          Debug.WriteLine("MainGrid.ColumnCount = " + MainGrid.ColumnCount);
          Debug.WriteLine("MainGrid.RowCount = " + MainGrid.RowCount);
          Debug.WriteLine("Entities.Count = " + Entities.Count);
        } catch { }
        // Leave the breakpoint on Debug.Assert.
        // That way, if I hit the problem again,
        // I'll see the diagnostics.
        Debug.Assert(true);
        // ???
        // Fairly sure this should work.
        // Otherwise it just seems to show an unneeded error message.
        UpdateCancelled = false;
        return;
      }
      UpdateCancelled = false;
      // The error row's values will have been restored to their
      // originals when the change was rejected.
      // So put the new values back into the grid row.
      // The user can then either modify or cancel the change.
      //bool rejectsRestored = false;
      for (var columnIndex = 0;
        columnIndex < Entities.Columns.Count;
        columnIndex++) {
        var rejectedValue = RowErrorEventArgs.RejectedValues[columnIndex];
        // All the rejected values will be DBNull if the user had tried to delete the row.
        if (rejectedValue != DBNull.Value) {
          MainGrid.CurrentRow.Cells[columnIndex].Value = rejectedValue;
        }
      } //End of for
      if (RowErrorEventArgs.Exception is ApplicationException
          || RowErrorEventArgs.Exception is DataException) {
        MessageBox.Show(
          RowErrorEventArgs.Exception.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      } else {
        MessageBox.Show(
          RowErrorEventArgs.Exception.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }

    /// <summary>
    ///   Shows the specified image below the main grid
    ///   if the specified path is that of an existing image file.
    ///   Otherwise shows an appropriate message.
    /// </summary>
    /// <param name="path">
    ///   The path of the image to show.
    ///   Null to show an invitation to drag instead of an image.
    /// </param>
    /// <remarks>
    ///   For efficiency,
    ///   nothing already shown will be changed
    ///   unless it does not correspond to what is required.
    /// </remarks>
    private void ShowImageOrMessage(string path) {
      const string NOT_SPECIFIED_MESSAGE =
        "You may drag an image file here.";
      string InvalidImageMessage =
        "The file is not an image." + Environment.NewLine
                                    + NOT_SPECIFIED_MESSAGE;
      string NotFoundMessage =
        "The image file cannot be found." + Environment.NewLine
                                          + NOT_SPECIFIED_MESSAGE;
      var isRefreshRequired = false;
      if (!string.IsNullOrEmpty(path)
          && File.Exists(path)) {
        if (FittedPictureBox1.Visible) {
          if (FittedPictureBox1.ImageLocation != path) {
            isRefreshRequired = true;
          }
        } else { // File exists but picture not shown
          isRefreshRequired = true;
        }
      } else { // Not an existing file
        if (MissingImageLabel.Visible) {
          if (string.IsNullOrEmpty(path)
              && MissingImageLabel.Text != NOT_SPECIFIED_MESSAGE) {
            isRefreshRequired = true;
          } else if (MissingImageLabel.Text != NotFoundMessage) {
            isRefreshRequired = true;
          }
        } else { // Not an existing file but picture shown
          isRefreshRequired = true;
        }
      }
      if (!isRefreshRequired) {
        return;
      }
      if (!string.IsNullOrEmpty(path)
          && File.Exists(path)) {
        try {
          FittedPictureBox1.Load(path);
          FittedPictureBox1.Visible = true;
        } catch (ApplicationException) {
          MissingImageLabel.Text = InvalidImageMessage;
          FittedPictureBox1.Visible = false;
        }
      } else { // Not an existing file
        if (string.IsNullOrEmpty(path)) {
          MissingImageLabel.Text = NOT_SPECIFIED_MESSAGE;
        } else {
          MissingImageLabel.Text = NotFoundMessage;
        }
        FittedPictureBox1.Visible = false;
      }
      MissingImageLabel.Visible = !FittedPictureBox1.Visible;
    }

    /// <summary>
    ///   Shows the newsletter, if any, associated with the current row.
    ///   Otherwise shows an informative message box.
    /// </summary>
    public void ShowNewsletter() {
      Newsletter newsletter;
      try {
        newsletter = GetNewsletterToShow();
      } catch (ApplicationException ex) {
        MessageBox.Show(
          this,
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        return;
      }
      if (string.IsNullOrEmpty(newsletter.Path)) {
        MessageBox.Show(
          this,
          "The Path of the "
          + newsletter.Date.ToString("dd MMM yyyy")
          + " Newsletter has not been specified.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      } else if (!File.Exists(newsletter.Path)) {
        MessageBox.Show(
          this,
          "Newsletter file \"" + newsletter.Path
                               + "\", specified by the Path of the "
                               + newsletter.Date.ToString("dd MMM yyyy")
                               + " Newsletter, cannot be found.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      } else {
        Process.Start(newsletter.Path);
      }
    }

    /// <summary>
    ///   Handle's a SplitContainer's GotFocus event
    ///   to shift the focus to the current grid
    ///   when the SplitContainer gets the focus.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   For unknown reason,
    ///   when an existing table form is activated,
    ///   the grid SplitContainer gets focused.
    ///   In any case, we want focus to return to the current
    ///   grid after the user has grabbed the splitter.
    /// </remarks>
    private void SplitContainer_GotFocus(object sender, EventArgs e) {
      FocusTimer.Start();
    }

    /// <summary>
    ///   Swaps the colour schemes of the two grids.
    /// </summary>
    private void SwapGridColors() {
      var swapColor = MainGrid.DefaultCellStyle.BackColor;
      MainGrid.DefaultCellStyle.BackColor =
        ParentGrid.DefaultCellStyle.BackColor;
      ParentGrid.DefaultCellStyle.BackColor = swapColor;
      swapColor = MainGrid.DefaultCellStyle.ForeColor;
      MainGrid.DefaultCellStyle.ForeColor =
        ParentGrid.DefaultCellStyle.ForeColor;
      ParentGrid.DefaultCellStyle.ForeColor = swapColor;
      swapColor = MainGrid.DefaultCellStyle.SelectionBackColor;
      MainGrid.DefaultCellStyle.SelectionBackColor =
        ParentGrid.DefaultCellStyle.SelectionBackColor;
      ParentGrid.DefaultCellStyle.SelectionBackColor = swapColor;
      swapColor = MainGrid.DefaultCellStyle.SelectionForeColor;
      MainGrid.DefaultCellStyle.SelectionForeColor =
        ParentGrid.DefaultCellStyle.SelectionForeColor;
      ParentGrid.DefaultCellStyle.SelectionForeColor = swapColor;
    }

    /// <summary>
    ///   Enables and focuses the grid
    ///   when the window is activated.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   This is necessary because of the
    ///   workaround implemented in TableForm_Deactivate.
    /// </remarks>
    private void TableForm_Activated(object sender, EventArgs e) {
      //Debug.WriteLine("TableForm_Activated: " + this.Text);
      MainGrid.Enabled = true;
      if (Entities.ParentList != null) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        FocusGrid(ParentGrid);
      } else {
        MainGrid.Focus();
        FocusedGrid = MainGrid;
      }
    }

    /// <summary>
    ///   Disable the grid when another table window
    ///   is activated.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   For unknown reason,
    ///   without this workaround,
    ///   when a table window with date columns is
    ///   deactivated and another table window is activated,
    ///   it is impossible to navigate or edit the grid
    ///   on the active window.
    ///   To be safe, disable the grid even if there aren't date columns:
    ///   maybe there are other data types that would cause similar problems.
    /// </remarks>
    private void TableForm_Deactivate(object sender, EventArgs e) {
      //Debug.WriteLine("TableForm_Deactivate: " + this.Text);
      MainGrid.Enabled = false;
      if (Entities.ParentList != null) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        ParentGrid.Enabled = false;
      }
    }

    private void TableForm_FormClosed(object sender, FormClosedEventArgs e) {
      //MainGrid.RowValidated -= new DataGridViewCellEventHandler(MainGrid_RowValidated);
      //MainGrid.ReadOnly = true;
      //Refresh();
      SizeableFormOptions.Save();
      if (Entities is ArtistInImageList
          || Entities is ImageList) {
        ImageSplitterDistanceOption.Int32Value =
          ImageSplitContainer.SplitterDistance;
      }
      if (Entities.ParentList != null) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        GridSplitterDistanceOption.Int32Value =
          GridSplitContainer.SplitterDistance;
      }
    }

    //private void TableForm_FormClosing(object sender, FormClosingEventArgs e) {
    //    //if (MainGrid.CurrentRow.IsNewRow) {
    //    //    Debug.WriteLine("New row");
    //    //}
    //    //MainGrid.RowValidated -= new DataGridViewCellEventHandler(MainGrid_RowValidated);
    //}

    /// <summary>
    ///   Handles the <see cref="Form" />'s
    ///   <see cref="Control.KeyDown" /> event to:
    ///   switch focus from one grid to the other
    ///   if two grids are shown and one is in focus.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   In order for this event handler to be triggered,
    ///   the <see cref="Form" />'s <see cref="Form.KeyPreview" />
    ///   property must be set to <b>True</b>.
    /// </remarks>
    private void TableForm_KeyDown(object sender, KeyEventArgs e) {
      //switch (e.KeyCode) {
      //case Keys.Enter:
      //    Debug.WriteLine(e.KeyCode);
      //    break;
      //}
      switch (e.KeyData) {
        case Keys.F6:
          if (Entities.ParentList != null) {
            // A read-only related grid for the parent table is shown
            // above the main grid.
            if (FocusedGrid == ParentGrid) {
              FocusGrid(MainGrid);
            } else {
              FocusGrid(ParentGrid);
            }
          }
          break;
        // Tried command (⌘) key on a Mac keyboard + Enter
        // to do same as Control+Enter,
        // i.e to complete the edit of the current cell if being edited.
        // But the Enter key cannot be trapped.
        // ⌘+Enter ends the edit and goes to next row anyway,
        // which is OK.
        // (Command (⌘) key on a Mac keyboard does the same 
        // as Windows/Start key on a Windows keyboard.)
        //case Keys.LWin:
        //    Debug.WriteLine(e.KeyCode);
        //    LWinIsDown = true;
        //    break;
        //case Keys.RWin:
        //    Debug.WriteLine(e.KeyCode);
        //    RWinIsDown = true;
        //    break;
        //case Keys.Enter:
        //    Debug.WriteLine(e.KeyCode);
        //    if (LWinIsDown
        //    || RWinIsDown) {
        //        if (MainGrid.CurrentCell.IsInEditMode) {
        //            e.SuppressKeyPress = true;
        //            MainGrid.EndEdit();
        //        }
        //    }
        //    break;
      } //End of switch
    }

    ///// <summary>
    ///// Handles the <see cref="Form"/>'s 
    ///// <see cref="Control.KeyUp"/> event.
    ///// </summary>
    ///// <param name="sender">Event sender.</param>
    ///// <param name="e">Event arguments.</param>
    ///// <remarks>
    ///// In order for this event handler to be triggered,
    ///// the <see cref="Form"/>'s <see cref="Form.KeyPreview"/> 
    ///// property must be set to <b>True</b>.
    ///// </remarks>
    //private void TableForm_KeyUp(object sender, KeyEventArgs e) {
    //    switch (e.KeyCode) {
    //    case Keys.LWin:
    //        LWinIsDown = false;
    //        break;
    //    case Keys.RWin:
    //        RWinIsDown = false;
    //        break;
    //    }//End of switch
    //}

    private void TableForm_Load(object sender, EventArgs e) {
      // Has to be done here rather than in constructor
      // in order to tell that this is an MDI child form.
      SizeableFormOptions = new SizeableFormOptions(this);
    }

    private void TableForm_VisibleChanged(object sender, EventArgs e) {
      if (Visible) {
        //Debug.WriteLine("TableForm_VisibleChanged: " + this.Text);
        MainGrid.AutoResizeColumns();
        // We need to work out whether we need the image panel
        // before we position the grid splitter.
        // Otherwise the grid splitter gets out of kilter.
        if (Entities is ArtistInImageList
            || Entities is ImageList) {
          ImageSplitContainer.Panel2Collapsed = false;
          ImageSplitterDistanceOption = new Option(
            TableName + ".ImageSplitterDistance",
            ImageSplitContainer.SplitterDistance);
          ImageSplitContainer.SplitterDistance =
            ImageSplitterDistanceOption.Int32Value;
          ShowImageOrMessage(null); // Force image refresh
          if (Entities is ImageList) {
            if (Entities.Count > 0) {
              ShowImageOrMessage(
                MainGrid.Rows[0].Cells["Path"].Value.ToString());
            }
          } else { // ArtistInImageList
            if (Entities.ParentList.Count > 0) {
              ShowImageOrMessage(
                ParentGrid.Rows[0].Cells["Path"].Value.ToString());
            }
          }
        } else {
          ImageSplitContainer.Panel2Collapsed = true;
        }
        if (Entities.ParentList != null) {
          // A read-only related grid for the parent table is shown
          // above the main grid.
          GridSplitContainer.Panel1Collapsed = false;
          GridSplitterDistanceOption = new Option(
            TableName + ".GridSplitterDistance",
            GridSplitContainer.SplitterDistance);
          // Does not work if done in TableForm_Load.
          GridSplitContainer.SplitterDistance =
            GridSplitterDistanceOption.Int32Value;
          ParentGrid.AutoResizeColumns();
        } else {
          GridSplitContainer.Panel1Collapsed = true;
        }
      }
    }

    private void
      UpdateDatabase(Dictionary<string, object> oldKeyFields = null) {
      Entities.Update(oldKeyFields);
      MainGrid.AutoResizeColumns();
      MainGrid.Focus();
    }

    private enum Medium {
      Audio,
      Video
    }
  } //End of class
} //End of namespace