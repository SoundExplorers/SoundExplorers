using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   Table editor MDI child window of the main window.
  /// </summary>
  internal partial class EditorView : Form, IEditorView {
    /// <summary>
    ///   Initialises a new instance of the <see cref="EditorView" /> class.
    /// </summary>
    public EditorView() {
      InitializeComponent();
      Move += EditorView_Move;
      Resize += EditorView_Resize;
      // Allow things to be dropped on to the PictureBox.
      FittedPictureBox1.AllowDrop = true;
      GridSplitContainer.GotFocus += SplitContainerOnGotFocus;
      ImageSplitContainer.GotFocus += SplitContainerOnGotFocus;
    }

    private DataGridView FocusedGrid { get; set; }

    private DataGridViewRow MainCurrentRow => MainGrid.CurrentRow ??
                                              throw new NullReferenceException(
                                                nameof(MainGrid.CurrentRow));

    private MainView MainView => (MainView)MdiParent;
    private SizeableFormOptions SizeableFormOptions { get; set; }
    public EditorController Controller { get; private set; }

    public void EditMainGridCurrentCell() {
      MainGrid.BeginEdit(true);
    }

    public void FocusMainGridCell(int rowIndex, int columnIndex) {
      // This triggers MainGrid_RowEnter.
      // Debug.WriteLine("EditorView.FocusMainGridCell");
      try {
        MainGrid.CurrentCell = MainGrid.Rows[rowIndex].Cells[columnIndex];
      } catch {
        // Can happen if insertion row is left before error message is shown.
      }
    }

    public void MakeMainGridRowCurrent(int rowIndex) {
      // This triggers MainGrid_RowEnter.
      // Debug.WriteLine($"EditorView.MakeMainGridRowCurrent: row {rowIndex}");
      MainGrid.CurrentCell = MainGrid.Rows[rowIndex].Cells[0];
    }

    public void OnError() {
      // Debug.WriteLine("EditorView.OnError");
      Cursor = Cursors.WaitCursor;
      BeginInvoke((Action)OnErrorAsync);
    }

    public void OnRowAddedOrDeleted() {
      MainGrid.AutoResizeColumns();
      MainGrid.Focus();
    }

    public void RestoreMainGridCurrentRowCellErrorValue(int columnIndex,
      object errorValue) {
      ((ICanRestoreErrorValue)MainCurrentRow.Cells[columnIndex]).RestoreErrorValue(
        errorValue);
    }

    public void SelectCurrentRowOnly() {
      MainGrid.ClearSelection();
      MainCurrentRow.Selected = true;
    }

    public void ShowErrorMessage(string text) {
      //MeasureProfiler.SaveData();
      ShowMessage(text, MessageBoxIcon.Error);
    }

    public void ShowWarningMessage(string text) {
      ShowMessage(text, MessageBoxIcon.Warning);
    }

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Makes the insertion row of the main grid current.
    /// </summary>
    private void MakeMainGridInsertionRowCurrent() {
      // This triggers MainGrid_RowEnter.
      // Debug.WriteLine("EditorView.MakeMainGridInsertionRowCurrent");
      MakeMainGridRowCurrent(MainGrid.Rows.Count - 1);
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
          if (string.IsNullOrWhiteSpace(textBox.SelectedText)) {
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          }
          Clipboard.SetText(textBox.SelectedText);
          break;
        }
      }
    }

    /// <summary>
    ///   Creates a EditorView and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    ///   The parameter is passed to the controller's constructor.
    /// </summary>
    /// <param name="entityListType">
    ///   The type of entity list whose data is to be displayed.
    /// </param>
    /// <param name="mainController">
    ///   Controller for the main window.
    /// </param>
    [NotNull]
    public static EditorView Create([CanBeNull] Type entityListType,
      [NotNull] MainController mainController) {
      return (EditorView)ViewFactory.Create<EditorView, EditorController>(
        entityListType, mainController);
      // EditorView result;
      // try {
      //   result = new EditorView();
      //   var dummy = new EditorController(result, entityListType); 
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
          case TextBox textBox when string.IsNullOrWhiteSpace(textBox.SelectedText):
            // Clipboard.SetText throws an exception
            // if passed an empty string.
            return;
          case TextBox textBox:
            Clipboard.SetText(textBox.SelectedText);
            textBox.SelectedText = string.Empty;
            break;
        }
      }
    }

    public void DeleteSelectedRows() {
      if (FocusedGrid != MainGrid || Controller.IsInsertionRowCurrent) {
        return;
      }
      if (MainGrid.IsCurrentCellInEditMode) {
        MainGrid.CancelEdit();
      }
      if (MainGrid.SelectedRows.Count == 0) {
        MainCurrentRow.Selected = true;
      }
      foreach (DataGridViewRow row in MainGrid.SelectedRows) {
        MainGrid.Rows.Remove(row);
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

    public void SelectAll() {
      if (FocusedGrid != MainGrid) {
        return;
      }
      if (!MainGrid.IsCurrentCellInEditMode) {
        MainGrid.BeginEdit(true);
      } else { // The cell is already being edited
        if (MainGrid.EditingControl is TextBox textBox) {
          textBox.SelectAll();
        }
      }
    }

    private void AfterPopulateAsync() {
      MakeMainGridInsertionRowCurrent();
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is to be shown
        // above the main grid.
        ParentGrid.Focus();
      } else { // No parent grid
        MainGrid.Focus();
      }
    }

    private void ConfigureMainGridColumn([NotNull] DataGridViewColumn column) {
      // Making every column explicitly not sortable prevents the program
      // from crashing if F3 in pressed while the grid is focused.
      // TODO Check whether F3 crashes program when PARENT grid is focused.
      column.SortMode = DataGridViewColumnSortMode.NotSortable;
      column.HeaderText = Controller.GetColumnDisplayName(column.Name);
      if (column.ValueType == typeof(DateTime)) {
        column.DefaultCellStyle.Format = EditorController.DateFormat;
      }
      if (Controller.DoesColumnReferenceAnotherEntity(column.Name)) {
        column.CellTemplate = ComboBoxCell.Create(Controller, column.Name);
      } else if (column.ValueType == typeof(DateTime)) {
        column.CellTemplate = new CalendarCell();
      } else if (column.ValueType == typeof(string)) {
        column.CellTemplate = new TextBoxCell();
        // Interpret blanking a cell as an empty string, not null.
        // Null is not a problem for the object-oriented database to handle.
        // But this fixes an error where,
        // when a text cell was edited to blank
        // and then Tab was pressed to proceed to the next cell,
        // which happened to be the first cell of the insertion row,
        // if that is relevant,
        // the program would crash with a NullReferenceException.
        column.DefaultCellStyle.DataSourceNullValue = string.Empty;
      }
    }

    /// <summary>
    ///   Enables and focuses the grid
    ///   when the window is activated.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   This is necessary because of the
    ///   workaround implemented in EditorView_Deactivate.
    /// </remarks>
    private void EditorView_Activated(object sender, EventArgs e) {
      //Debug.WriteLine("EditorView_Activated: " + this.Text);
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
    private void EditorView_Deactivate(object sender, EventArgs e) {
      //Debug.WriteLine("EditorView_Deactivate: " + this.Text);
      MainGrid.Enabled = false;
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        ParentGrid.Enabled = false;
      }
    }

    private void EditorView_FormClosed(object sender, FormClosedEventArgs e) {
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

    /// <summary>
    ///   Handles the <see cref="Form" />'s
    ///   <see cref="Control.KeyDown" /> event.
    /// </summary>
    /// <remarks>
    ///   In order for this event handler to be triggered,
    ///   the <see cref="Form" />'s <see cref="Form.KeyPreview" />
    ///   property must be set to <b>True</b>.
    /// </remarks>
    private void EditorView_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.F6:
          if (Controller.IsParentTableToBeShown) {
            FocusGrid(FocusedGrid == ParentGrid ? MainGrid : ParentGrid);
          }
          break;
      } //End of switch
    }

    private void EditorView_Load(object sender, EventArgs e) {
      MainGrid.Controller = new MainGridController(this);
      MainGrid.CutMenuItem.Click += MainView.EditCutMenuItem_Click;
      MainGrid.CopyMenuItem.Click += MainView.EditCopyMenuItem_Click;
      MainGrid.PasteMenuItem.Click += MainView.EditPasteMenuItem_Click;
      MainGrid.SelectAllMenuItem.Click += MainView.EditSelectAllMenuItem_Click;
      MainGrid.DeleteSelectedRowsMenuItem.Click +=
        MainView.EditDeleteSelectedRowsMenuItem_Click;
      // Has to be done here rather than in constructor
      // in order to tell that this is an MDI child form.
      SizeableFormOptions = SizeableFormOptions.Create(this);
      // And better to do this here than in SetController,
      // where any exception would be indirectly reported,
      // due to being thrown in the controller's constructor.
      OpenTable();
    }

    private void EditorView_Move(object sender, EventArgs e) {
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    private void EditorView_Resize(object sender, EventArgs e) {
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    private void EditorView_VisibleChanged(object sender, EventArgs e) {
      if (Visible) {
        //Debug.WriteLine("EditorView_VisibleChanged: " + this.Text);
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
          // Does not work if done in EditorView_Load.
          GridSplitContainer.SplitterDistance = Controller.GridSplitterDistance;
          ParentGrid.AutoResizeColumns();
        } else {
          GridSplitContainer.Panel1Collapsed = true;
        }
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
      // i.e. in EditorView_KeyDown to implement doing it with the F6 key.
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
    ///   For unknown reason,
    ///   when an existing table form is activated,
    ///   the split container gets focused.
    ///   Asynchronously refocusing  the problem.
    /// </summary>
    private void FocusAsync() {
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
    ///   the cell will become the current cell and
    ///   if the main grid was clicked, the context menu will be shown.
    ///   <para>
    ///     I tried to initiate a drag-and-drop operation
    ///     when a cell is clicked with the mouse button 1.
    ///     But it did not work, possibly because it
    ///     puts the cell into edit mode and also,
    ///     when dragged, selects multiple rows.
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
          if (grid == MainGrid) {
            MainGrid.ContextMenuStrip.Show(MainGrid, e.Location);
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
    private void MainGrid_CellBeginEdit(
      object sender, DataGridViewCellCancelEventArgs e) {
      MissingImageLabel.Visible = false;
    }

    /// <summary>
    ///   Occurs when an external data-parsing or validation operation
    ///   in an existing row (not the insertion row) throws an exception.
    /// </summary>
    /// <remarks>
    ///   The event has to be handled for error handling to work.
    ///   Overriding the corresponding protected method in <see cref="MainGrid" />
    ///   does not work.
    /// </remarks>
    private void MainGrid_DataError(object sender, DataGridViewDataErrorEventArgs e) {
      // Debug.WriteLine("MainGrid_DataError");
      // Debug.WriteLine("Context = " + e.Context);
      // Debug.WriteLine("ColumnIndex = " + e.ColumnIndex);
      // Debug.WriteLine("RowIndex = " + e.RowIndex);
      string columnName = MainGrid.Columns[e.ColumnIndex].Name;
      MainGrid.Controller.OnExistingRowCellUpdateError(e.RowIndex, columnName,
        e.Exception);
    }

    /// <summary>
    ///   Actually called once for each row removed,
    ///   even when multiple selected rows are removed at once.
    /// </summary>
    /// <remarks>
    ///   For unknown reason, the RowsRemoved event is raised 2 or 3 times
    ///   while data is being loaded into the grid.
    /// </remarks>
    /// <remarks>
    ///   Overriding the corresponding protected method in <see cref="MainGrid" />
    ///   does not work. The event has to be handled, otherwise the program will crash
    ///   in some circumstances.
    /// </remarks>
    private void MainGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
      MainGrid.Controller.OnRowValidated(e.RowIndex);
    }

    /// <summary>
    ///   Handles the main grid's RowValidated event,
    ///   which is raised when the user exits a row on the grid,
    ///   even when nothing has changed.
    /// </summary>
    /// <remarks>
    ///   Overriding the corresponding protected method in <see cref="MainGrid" />
    ///   does not work. The event has to be handled, otherwise the program will crash
    ///   in some circumstances.
    /// </remarks>
    private void MainGrid_RowValidated(object sender, DataGridViewCellEventArgs e) {
      MainGrid.Controller.OnRowValidated(e.RowIndex);
    }

    private void OnErrorAsync() {
      // Debug.WriteLine("OnErrorAsync");
      MainGrid.CancelEdit();
      Controller.ShowError();
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
      Controller.OnParentGridRowEnter(e.RowIndex);
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
          }
        }
      }
    }

    private void PopulateGrid() {
      MainGrid.CellBeginEdit -= MainGrid_CellBeginEdit;
      MainGrid.Click -= Grid_Click;
      MainGrid.DataError -= MainGrid_DataError;
      MainGrid.MouseDown -= Grid_MouseDown;
      MainGrid.RowsRemoved -= MainGrid_RowsRemoved;
      MainGrid.RowValidated -= MainGrid_RowValidated;
      Controller.FetchData();
      Text = Controller.MainTableName;
      if (Controller.IsParentTableToBeShown) {
        PopulateParentGrid();
      }
      MainGrid.DataSource = Controller.MainBindingList;
      foreach (DataGridViewColumn column in MainGrid.Columns) {
        ConfigureMainGridColumn(column);
      } // End of foreach
      MainGrid.CellBeginEdit += MainGrid_CellBeginEdit;
      MainGrid.Click += Grid_Click;
      MainGrid.DataError += MainGrid_DataError;
      MainGrid.MouseDown += Grid_MouseDown;
      MainGrid.RowsRemoved += MainGrid_RowsRemoved;
      MainGrid.RowValidated += MainGrid_RowValidated;
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        MainGrid.AutoResizeColumns();
        BeginInvoke((Action)AfterPopulateAsync);
      }
    }

    private void PopulateParentGrid() {
      ParentGrid.Click -= Grid_Click;
      ParentGrid.CurrentCellChanged -= ParentGrid_CurrentCellChanged;
      //ParentGrid.GotFocus -= new EventHandler(ControlOnGotFocus);
      //ParentGrid.LostFocus -= new EventHandler(ControlOnLostFocus);
      ParentGrid.MouseDown -= Grid_MouseDown;
      ParentGrid.RowEnter -= ParentGrid_RowEnter;
      ParentGrid.DataSource = Controller.ParentBindingList;
      foreach (DataGridViewColumn column in ParentGrid.Columns) {
        if (column.ValueType == typeof(DateTime)) {
          column.DefaultCellStyle.Format = EditorController.DateFormat;
        }
      } // End of foreach
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        ParentGrid.AutoResizeColumns();
      }
      ParentGrid.Click += Grid_Click;
      ParentGrid.CurrentCellChanged += ParentGrid_CurrentCellChanged;
      //ParentGrid.GotFocus += new EventHandler(ControlOnGotFocus);
      //ParentGrid.LostFocus += new EventHandler(ControlOnLostFocus);
      ParentGrid.MouseDown += Grid_MouseDown;
      ParentGrid.RowEnter += ParentGrid_RowEnter;
      if (ParentGrid.RowCount > 0) {
        ParentGrid.CurrentCell =
          ParentGrid.Rows[0].Cells[0]; // Triggers ParentGrid_RowEnter
      }
    }

    private void ShowMessage([NotNull] string text, MessageBoxIcon icon) {
      Cursor = Cursors.Default;
      MessageBox.Show(
        this, text, Application.ProductName, MessageBoxButtons.OK, icon);
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
    private void SplitContainerOnGotFocus(object sender, EventArgs e) {
      BeginInvoke((Action)FocusAsync);
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

    protected override void WndProc(ref Message m) {
      // ReSharper disable once InconsistentNaming
      const int WM_CLOSE = 0x0010;
      if (m.Msg == WM_CLOSE) {
        // Attempting to close Form
        Controller.IsClosing = true;
      }
      base.WndProc(ref m);
    }
  } //End of class
} //End of namespace