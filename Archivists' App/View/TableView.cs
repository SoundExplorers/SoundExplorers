using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   Table editor MDI child window of the main window.
  /// </summary>
  internal partial class TableView : Form, ITableView {
    /// <summary>
    ///   Initialises a new instance of the <see cref="TableView" /> class.
    /// </summary>
    public TableView() {
      InitializeComponent();
      // A known Visual Studio error is that PictureBox's AllowDrop property
      // appears neither in the designer nor in intellisense.
      // But is does exist and is required in order to 
      // allow anything to be dropped on to the PictureBox.
      FittedPictureBox1.AllowDrop = true;
      GridSplitContainer.GotFocus += SplitContainer_GotFocus;
      ImageSplitContainer.GotFocus += SplitContainer_GotFocus;
    }

    public TableController Controller { get; private set; }
    private DataGridView FocusedGrid { get; set; }

    private DataGridViewRow MainCurrentRow => MainGrid.CurrentRow ??
                                              throw new NullReferenceException(
                                                nameof(MainGrid.CurrentRow));

    private bool ParentRowChanged { get; set; }
    private DatabaseUpdateErrorException DatabaseUpdateErrorException { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; }
    private bool UpdateCancelled { get; set; }

    /// <summary>
    ///   Occurs when there is an error on
    ///   attempting to insert, update or delete an entity
    ///   corresponding to a row in the main grid.
    /// </summary>
    /// <param name="e">Error details.</param>
    public void OnDatabaseUpdateError(DatabaseUpdateErrorException e) {
      DatabaseUpdateErrorException = e;
      UpdateCancelled = true;
      RowErrorTimer.Start();
    }

    /// <summary>
    ///   Occurs when an entity
    ///   corresponding to a row in the main grid
    ///   has been successfully inserted, updated or deleted on the database.
    /// </summary>
    public void OnRowUpdated() {
      MainGrid.AutoResizeColumns();
      MainGrid.Focus();
    }

    public void SetCurrentRowFieldValue(string columnName, object newValue) {
      MainCurrentRow.Cells[columnName].Value = newValue;
    }

    public void SetController(TableController controller) {
      Controller = controller;
    }

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
        case TextBox textBox: {
          if (string.IsNullOrEmpty(textBox.SelectedText)) {
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          }
          Clipboard.SetText(textBox.SelectedText);
          break;
        }
        case PathEditingControl pathEditingControl:
          pathEditingControl.Copy();
          break;
      }
    }

    /// <summary>
    ///   Creates a TableView and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    ///   The parameter is passed to the controller's constructor.
    /// </summary>
    /// <param name="entityListType">
    ///   The type of entity list whose data is to be displayed.
    /// </param>
    [NotNull]
    public static TableView Create([NotNull] Type entityListType) {
      return (TableView)ViewFactory.Create<TableView, TableController>(entityListType);
      // TableView result;
      // try {
      //   result = new TableView();
      //   var dummy = new TableController(result, entityListType); 
      // } catch (TargetInvocationException ex) {
      //   throw ex.InnerException ?? ex;
      // }
      // return result;
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
          case TextBox textBox when string.IsNullOrEmpty(textBox.SelectedText):
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          case TextBox textBox:
            Clipboard.SetText(textBox.SelectedText);
            textBox.SelectedText = string.Empty;
            break;
          case PathEditingControl pathEditingControl:
            pathEditingControl.Cut();
            break;
        }
      }
    }

    private void AfterPopulateGridTimer_Tick(object sender, EventArgs e) {
      AfterPopulateGridTimer.Stop();
      // This makes the insertion row current initially. It triggers MainGrid_RowEnter
      MainGrid.CurrentCell = MainGrid.Rows[MainGrid.Rows.Count -1].Cells[0];
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is to be shown
        // above the main grid.
        ParentGrid.Focus();
      } else { // No parent grid
        MainGrid.Focus();
      }
    }

    private void ConfigureCellStyle([NotNull] DataGridViewColumn column) {
      if (column.ValueType == typeof(string)) {
        // Interpret blanking a cell as an empty string, not NULL.
        // This only works when updating, not inserting.
        // When inserting, do something like this in the SQL:
        //  coalesce(@Comments, '')
        column.DefaultCellStyle.DataSourceNullValue = string.Empty;
      } else if (column.ValueType == typeof(DateTime)) {
        column.DefaultCellStyle.Format = "dd MMM yyyy";
      }
      if (Controller.DoesColumnReferenceAnotherEntity(column.Name)) {
        column.CellTemplate = ComboBoxCell.Create(Controller, column.Name);
      } else if (column.ValueType == typeof(DateTime)) {
        column.CellTemplate = new CalendarCell();
      } else if (column.Name.EndsWith("Path")) {
        column.CellTemplate = PathCell.Create(Controller, column.Name);
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
      // if (Controller.Entities is ImageList
      //     && !MainGrid.IsCurrentCellInEditMode
      //     && e.Data.GetDataPresent(DataFormats.FileDrop)) {
      //   var pathCell = (PathCell)MainCurrentRow.Cells["Path"];
      //   DropPathOnCell(
      //     e.Data,
      //     pathCell);
      //   ShowImageOrMessage(pathCell.Path);
      // }
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
      // if (Entities is ImageList
      //     && !MainGrid.IsCurrentCellInEditMode
      //     && e.Data.GetDataPresent(DataFormats.FileDrop)) {
      //   e.Effect = DragDropEffects.Copy;
      // } else {
      //   e.Effect = DragDropEffects.None;
      // }
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
      if (!Controller.IsParentTableToBeShown) {
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
      // i.e. in TableView_KeyDown to implement doing it with the F6 key.
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
      FocusGrid(FocusedGrid ?? ParentGrid);
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
      var grid = (DataGridView)sender;
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
          if (cell is PathCell pathCell && pathCell.FileExists) {
            // This is a path cell that contains the path of an existing file
            var data = new DataObject(DataFormats.FileDrop, new[] {pathCell.Path});
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
      grid.DefaultCellStyle.SelectionBackColor = grid.DefaultCellStyle.SelectionForeColor;
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
    ///   This is only relevant if the Path cell of an Image row is being edited.
    ///   If the Missing Image label was visible just before entering edit mode,
    ///   it will have been because the file was not specified or can't be found or is not an image file.
    ///   That's presumably about to be rectified.
    ///   So the message to that effect in the Missing Image label could be misleading.
    ///   Also, the advice in the Missing Image label that an image file can
    ///   be dragged onto the label will not apply, as dragging and dropping is disabled
    ///   while the Path cell is being edited.
    /// </remarks>
    private void
      MainGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
      MissingImageLabel.Visible = false;
    }

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
    private void MainGrid_DataError(object sender, DataGridViewDataErrorEventArgs e) {
      //Debug.WriteLine("MainGrid_DataError");
      //Debug.WriteLine("Context = " + e.Context.ToString());
      //Debug.WriteLine("RowIndex = " + e.ColumnIndex.ToString());
      //Debug.WriteLine("RowIndex = " + e.RowIndex.ToString());
      //MainGrid.CurrentCell = MainGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //Refresh();
      MainGrid.CancelEdit();
      MessageBox.Show(
        this,
        e.Exception is ApplicationException
          ? e.Exception.Message
          : e.Exception.ToString(),
        Application.ProductName,
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
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
      if (GetCellAtClientCoOrdinates(
        clientCoOrdinates.X, clientCoOrdinates.Y) is PathCell pathCell) {
        // Dropping onto a path cell
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
      e.Effect = GetCellAtClientCoOrdinates(
        clientCoOrdinates.X, clientCoOrdinates.Y) is PathCell
        ? DragDropEffects.Copy
        : DragDropEffects.None;
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
    ///     The initial state of the row will be saved to a detached row
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
      // This is the safe way of checking whether we have entered the insertion (new) row:
      // if (e.RowIndex == MainGrid.RowCount - 1) {
      //   // Controller.OnEnteringInsertionRow();
      //   // // if (Entities is ImageList) {
      //   // //   ShowImageOrMessage(null);
      //   // // }
      //   return;
      // }
      // // Not new row
      Controller.OnMainGridRowEntered(e.RowIndex);
    }

    private void MainGrid_RowLeave(object sender, DataGridViewCellEventArgs e) {
      Controller.OnMainGridRowLeft(e.RowIndex);
    }

    private void MainGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
      //Debug.WriteLine("MainGrid_RowsRemoved");
      //Debug.WriteLine(MainGrid.Rows[e.RowIndex].Cells[0].Value);
      if (UpdateCancelled) {
        return;
      }
      Controller.OnMainGridRowRemoved(e.RowIndex);
    }

    private void MainGrid_RowValidated(object sender, DataGridViewCellEventArgs e) {
      //Debug.WriteLine("MainGrid_RowValidated");
      //Debug.WriteLine(MainGrid.Rows[e.RowIndex].Cells[0].Value);
      if (ParentRowChanged) {
        ParentRowChanged = false;
      }
      if (UpdateCancelled) {
        return;
      }
      // if (MainGrid.RowCount == 1) {
      //   // There's only the uncommitted new row, which can be discarded.
      //   return;
      // }
      Controller.OnMainGridRowValidated(e.RowIndex);
    }

    private void OpenTable() {
      InvertGridColors(ParentGrid); // Will revert when focused.
      PopulateGrid();
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
    private void ParentGrid_RowEnter(object sender, DataGridViewCellEventArgs e) {
      ParentRowChanged = true;
      Controller.OnParentGridRowEntered(e.RowIndex);
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
          if (MainGrid.EditingControl is TextBox textBox) {
            textBox.SelectedText = Clipboard.GetText();
          } else if (MainGrid.EditingControl is PathEditingControl pathEditingControl) {
            pathEditingControl.Paste();
          }
        }
      }
    }

    private void PopulateGrid() {
      // Controller.FetchData();
      // Text = Controller.MainTable?.TableName;
      MainGrid.CellBeginEdit -= MainGrid_CellBeginEdit;
      //MainGrid.CellEndEdit -= MainGrid_CellEndEdit;
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
      MainGrid.RowLeave -= MainGrid_RowLeave;
      MainGrid.RowsRemoved -= MainGrid_RowsRemoved;
      MainGrid.RowValidated -= MainGrid_RowValidated;
      Controller.FetchData();
      Text = Controller.MainTableName;
      if (Controller.IsParentTableToBeShown) {
        PopulateParentGrid();
      }
      MainGrid.DataSource = Controller.MainBindingList;
      foreach (DataGridViewColumn column in MainGrid.Columns) {
        ConfigureCellStyle(column);
      } // End of foreach
      MainGrid.CellBeginEdit += MainGrid_CellBeginEdit;
      //MainGrid.CellEndEdit += MainGrid_CellEndEdit;
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
      MainGrid.RowLeave += MainGrid_RowLeave;
      MainGrid.RowsRemoved += MainGrid_RowsRemoved;
      MainGrid.RowValidated += MainGrid_RowValidated;
      // //if (MainGrid.RowCount > 0) {
      //   // Not reliable because top left cell could be hidden.
      //   //MainGrid.CurrentCell = MainGrid.Rows[0].Cells[0]; // Triggers MainGrid_RowEnter
      //   MainGrid_RowEnter(this, new DataGridViewCellEventArgs(0, 0));
      // //}
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        MainGrid.AutoResizeColumns();
        AfterPopulateGridTimer.Start();
      }
    }

    private void PopulateParentGrid() {
      ParentGrid.Click -= Grid_Click;
      ParentGrid.CurrentCellChanged -= ParentGrid_CurrentCellChanged;
      //ParentGrid.GotFocus -= new EventHandler(Control_GotFocus);
      //ParentGrid.LostFocus -= new EventHandler(Control_LostFocus);
      ParentGrid.MouseDown -= Grid_MouseDown;
      ParentGrid.RowEnter -= ParentGrid_RowEnter;
      ParentGrid.DataSource = Controller.ParentBindingList;
      foreach (DataGridViewColumn column in ParentGrid.Columns) {
        if (column.ValueType == typeof(DateTime)) {
          column.DefaultCellStyle.Format = "dd MMM yyyy";
        }
        if (column.Name.EndsWith("Path")) {
          // Although we don't edit cells in the parent grid,
          // we still need to make the cell a PathCell,
          // as this is expected when playing media etc.
          //var entityColumn = Controller.ParentList?.Columns[column.Index];
          //var pathCell = new PathCell {Column = entityColumn};
          column.CellTemplate = PathCell.Create(Controller, column.Name);
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
      PopulateGrid();
      if (Controller.IsParentTableToBeShown) {
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
        MainGrid.CurrentCell = MainGrid.Rows[
          DatabaseUpdateErrorException.RowIndex].Cells[
          DatabaseUpdateErrorException.ColumnIndex];
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
          Debug.WriteLine("TableName = " + Controller.MainTableName);
          Debug.WriteLine("RowErrorEventArgs.ColumnIndex = " +
                          DatabaseUpdateErrorException.ColumnIndex);
          Debug.WriteLine("RowErrorEventArgs.RowIndex = " + DatabaseUpdateErrorException.RowIndex);
          Debug.WriteLine("MainGrid.CurrentCell.ColumnIndex = " +
                          MainGrid.CurrentCell.ColumnIndex);
          Debug.WriteLine("MainCurrentRow.Index = " + MainCurrentRow.Index);
          Debug.WriteLine("MainGrid.ColumnCount = " + MainGrid.ColumnCount);
          Debug.WriteLine("MainGrid.RowCount = " + MainGrid.RowCount);
          //Debug.WriteLine("Entities.Count = " + Entities.Count);
          // ReSharper disable once EmptyGeneralCatchClause
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
      Controller.RestoreRejectedValues(DatabaseUpdateErrorException.RejectedValues);
      MessageBox.Show(
        this,
        DatabaseUpdateErrorException.Message,
        Application.ProductName,
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
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
      MainGrid.DefaultCellStyle.BackColor = ParentGrid.DefaultCellStyle.BackColor;
      ParentGrid.DefaultCellStyle.BackColor = swapColor;
      swapColor = MainGrid.DefaultCellStyle.ForeColor;
      MainGrid.DefaultCellStyle.ForeColor = ParentGrid.DefaultCellStyle.ForeColor;
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
    ///   workaround implemented in TableView_Deactivate.
    /// </remarks>
    private void TableView_Activated(object sender, EventArgs e) {
      //Debug.WriteLine("TableView_Activated: " + this.Text);
      MainGrid.Enabled = true;
      if (Controller.IsParentTableToBeShown) {
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
    private void TableView_Deactivate(object sender, EventArgs e) {
      //Debug.WriteLine("TableView_Deactivate: " + this.Text);
      MainGrid.Enabled = false;
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        ParentGrid.Enabled = false;
      }
    }

    private void TableView_FormClosed(object sender, FormClosedEventArgs e) {
      //MainGrid.RowValidated -= new DataGridViewCellEventHandler(MainGrid_RowValidated);
      //MainGrid.ReadOnly = true;
      //Refresh();
      SizeableFormOptions.Save();
      // if (Entities is ArtistInImageList
      //     || Entities is ImageList) {
      //   Controller.ImageSplitterDistance = ImageSplitContainer.SplitterDistance;
      // }
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        Controller.GridSplitterDistance = GridSplitContainer.SplitterDistance;
      }
    }

    //private void TableView_FormClosing(object sender, FormClosingEventArgs e) {
    //    //if (MainCurrentRow.IsNewRow) {
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
    private void TableView_KeyDown(object sender, KeyEventArgs e) {
      //switch (e.KeyCode) {
      //case Keys.Enter:
      //    Debug.WriteLine(e.KeyCode);
      //    break;
      //}
      switch (e.KeyData) {
        case Keys.F6:
          if (Controller.IsParentTableToBeShown) {
            // A read-only related grid for the parent table is shown
            // above the main grid.
            FocusGrid(FocusedGrid == ParentGrid ? MainGrid : ParentGrid);
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
    //private void TableView_KeyUp(object sender, KeyEventArgs e) {
    //    switch (e.KeyCode) {
    //    case Keys.LWin:
    //        LWinIsDown = false;
    //        break;
    //    case Keys.RWin:
    //        RWinIsDown = false;
    //        break;
    //    }//End of switch
    //}

    private void TableView_Load(object sender, EventArgs e) {
      // Has to be done here rather than in constructor
      // in order to tell that this is an MDI child form.
      SizeableFormOptions = SizeableFormOptions.Create(this);
      // And better to do this here than in SetController,
      // where any exception would be indirectly reported,
      // due to being thrown in the controller's constructor.
      OpenTable();
    }

    private void TableView_VisibleChanged(object sender, EventArgs e) {
      if (Visible) {
        //Debug.WriteLine("TableView_VisibleChanged: " + this.Text);
        MainGrid.AutoResizeColumns();
        ImageSplitContainer.Panel2Collapsed = true;
        // We need to work out whether we need the image panel
        // before we position the grid splitter.
        // Otherwise the grid splitter gets out of kilter.
        // if (Entities is ArtistInImageList
        //     || Entities is ImageList) {
        //   ImageSplitContainer.Panel2Collapsed = false;
        //   ImageSplitContainer.SplitterDistance = Controller.ImageSplitterDistance;
        //   ShowImageOrMessage(null); // Force image refresh
        //   if (Entities is ImageList) {
        //     if (Entities.Count > 0) {
        //       ShowImageOrMessage(
        //         MainGrid.Rows[0].Cells["Path"].Value.ToString());
        //     }
        //   } else { // ArtistInImageList
        //     if (Controller.ParentList.Count > 0) {
        //       ShowImageOrMessage(
        //         ParentGrid.Rows[0].Cells["Path"].Value.ToString());
        //     }
        //   }
        // } else {
        //   ImageSplitContainer.Panel2Collapsed = true;
        // }
        if (Controller.IsParentTableToBeShown) {
          // A read-only related grid for the parent table is shown
          // above the main grid.
          GridSplitContainer.Panel1Collapsed = false;
          // Does not work if done in TableView_Load.
          GridSplitContainer.SplitterDistance = Controller.GridSplitterDistance;
          ParentGrid.AutoResizeColumns();
        } else {
          GridSplitContainer.Panel1Collapsed = true;
        }
      }
    }
  } //End of class
} //End of namespace