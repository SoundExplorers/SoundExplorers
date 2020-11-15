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
      // A known Visual Studio error is that PictureBox's AllowDrop property
      // appears neither in the designer nor in intellisense.
      // But is does exist and is required in order to 
      // allow anything to be dropped on to the PictureBox.
      FittedPictureBox1.AllowDrop = true;
      GridSplitContainer.GotFocus += SplitContainerOnGotFocus;
      ImageSplitContainer.GotFocus += SplitContainerOnGotFocus;
    }

    public EditorController Controller { get; private set; }
    private DataGridView FocusedGrid { get; set; }

    private DataGridViewRow MainCurrentRow => MainGrid.CurrentRow ??
                                              throw new NullReferenceException(
                                                nameof(MainGrid.CurrentRow));

    private bool ParentRowChanged { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; }

    public void EditMainGridCurrentCell() {
      MainGrid.BeginEdit(true);
    }

    public void FocusMainGridCell(int rowIndex, int columnIndex) {
      // This triggers MainGridOnRowEnter.
      MainGrid.CurrentCell = MainGrid.Rows[rowIndex].Cells[columnIndex];
    }

    /// <summary>
    ///   Makes the insertion row of the main grid current.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void MakeMainGridInsertionRowCurrent() {
      // This triggers MainGridOnRowEnter.
      MainGrid.CurrentCell = MainGrid.Rows[MainGrid.Rows.Count - 1].Cells[0];
    }

    /// <summary>
    ///   Occurs when an entity corresponding to a row in the main grid
    ///   has been successfully inserted or deleted on the database.
    /// </summary>
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
      ShowMessage(text, MessageBoxIcon.Error);
    }

    public void ShowWarningMessage(string text) {
      ShowMessage(text, MessageBoxIcon.Warning);
    }

    public void StartDatabaseUpdateErrorTimer() {
      DatabaseUpdateErrorTimer.Start();
    }

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    private void ShowMessage([NotNull] string text, MessageBoxIcon icon) {
      MessageBox.Show(
        this, text, Application.ProductName, MessageBoxButtons.OK, icon);
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
    [NotNull]
    public static EditorView Create([CanBeNull] Type entityListType) {
      return (EditorView)ViewFactory.Create<EditorView, EditorController>(entityListType);
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

    private void AfterPopulateTimerOnTick(object sender, EventArgs e) {
      AfterPopulateTimer.Stop();
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
        column.DefaultCellStyle.Format = "dd MMM yyyy";
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
    ///   Handle's the row error Timer's Tick event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   Have to use a Timer in order for focusing the error row and cell to work.
    /// </remarks>
    private void DatabaseUpdateErrorTimerOnTick(object sender, EventArgs e) {
      DatabaseUpdateErrorTimer.Stop();
      //Debug.WriteLine("DatabaseUpdateErrorTimerOnTick");
      MainGrid.CancelEdit();
      Controller.ShowDatabaseUpdateError();
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
    private void FittedPictureBox1OnDragDrop(object sender, DragEventArgs e) {
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
    private void FittedPictureBox1OnDragOver(object sender, DragEventArgs e) {
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
    private void FittedPictureBox1OnMouseDown(object sender, MouseEventArgs e) {
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
      // i.e. in EditorViewOnKeyDown to implement doing it with the F6 key.
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
    private void FocusTimerOnTick(object sender, EventArgs e) {
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

    private void GridOnClick(object sender, EventArgs e) {
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
    ///     Perhaps we could initiate a drag-and-drop operation
    ///     on Control + mouse button 1.
    ///   </para>
    /// </remarks>
    private void GridOnMouseDown(object sender, MouseEventArgs e) {
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
    private void MainGridOnCellBeginEdit(
      object sender, DataGridViewCellCancelEventArgs e) {
      MissingImageLabel.Visible = false;
    }

    private void MainGridOnCellValueChanged(object sender, DataGridViewCellEventArgs e) {
      if (MainGrid.CurrentCell is ComboBoxCell comboBoxCell) {
        //var actualCellValueType = MainGrid.CurrentCell.Value.GetType();
        var cellValue = MainGrid.CurrentCell.Value;
        string columnName = MainGrid.CurrentCell.OwningColumn.Name;
        string comboBoxText = comboBoxCell.ComboBox.Text;
        //var expectedCellValueType = MainGrid.CurrentCell.OwningColumn.ValueType;
        string format = MainGrid.CurrentCell.OwningColumn.DefaultCellStyle.Format;
        int rowIndex = MainCurrentRow.Index;
        // Debug.WriteLine("MainGridOnCellValueChanged, ComboBoxCell:");
        // Debug.WriteLine(
        //   $"  Cell = '{cellValue}'; combo box = '{comboBoxText}'");
        Controller.OnMainGridComboBoxCellValueChanged(
          rowIndex, columnName, cellValue, comboBoxText, format);
      }
    }

    /// <summary>
    ///   Emulates the ComboBox's SelectedIndexChanged event.
    /// </summary>
    /// <remarks>
    ///   A known problem with MainGrids is that,
    ///   where there are multiple ComboBox columns,
    ///   ComboBox events can get spuriously raised against the ComboBoxes
    ///   in multiple cells of the row that is being edited.
    ///   So this event handler provides a workaround by
    ///   emulating a cell ComboBox's SelectedIndexChange event
    ///   but without the spurious occurrences.
    ///   The fix is based on the second answer here:
    ///   https://stackoverflow.com/questions/11141872/event-that-fires-during-MainGridcomboboxcolumn-selectedindexchanged
    /// </remarks>
    private void MainGridOnCurrentCellDirtyStateChanged(object sender, EventArgs e) {
      // Debug.WriteLine($"MainGridOnCurrentCellDirtyStateChanged: IsCurrentCellDirty = {MainGrid.IsCurrentCellDirty}");
      if (MainGrid.CurrentCell is ComboBoxCell && MainGrid.IsCurrentCellDirty) {
        // Debug.WriteLine(
        //   "MainGridOnCurrentCellDirtyStateChanged: ComboBoxCell, IsCurrentCellDirty");
        // This fires the cell value changed handler MainGridOnCellValueChanged.
        MainGrid.CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
      }
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="DataGridView.DataError" /> event,
    ///   which occurs when an external data-parsing or validation operation throws an
    ///   exception.
    /// </summary>
    /// <remarks>
    ///   The event is raised in two anticipated scenarios:
    ///   1) When there is an error on attempting to update a main grid cell
    ///   representing a property of an existing entity.
    ///   2) When invalidly formatted data is pasted into a cell of
    ///   either a new or existing row, e.g. text into a date.
    /// </remarks>
    private void MainGridOnDataError(object sender, DataGridViewDataErrorEventArgs e) {
      // Debug.WriteLine("MainGridOnDataError");
      // Debug.WriteLine("Context = " + e.Context);
      // Debug.WriteLine("ColumnIndex = " + e.ColumnIndex);
      // Debug.WriteLine("RowIndex = " + e.RowIndex);
      string columnName = MainGrid.Columns[e.ColumnIndex].Name;
      Controller.OnMainGridDataError(e.RowIndex, columnName, e.Exception);
    }

    /// <summary>
    ///   Handles the main grid's
    ///   <see cref="Control.KeyDown" /> event to:
    ///   begin editing the current cell with, if a text cell, all contents selected.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   When a text cell edit is started with a mouse click,
    ///   selecting all contents of the cell is done by
    ///   <see cref="TextBoxCell.InitializeEditingControl" />.
    /// </remarks>
    private void MainGridOnKeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.F2:
          if (MainGrid.CurrentCell != null) {
            MainGrid.BeginEdit(true);
          }
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
    private void MainGridOnRowEnter(object sender, DataGridViewCellEventArgs e) {
      // This is the safe way of checking whether we have entered the insertion (new) row:
      //if (e.RowIndex == MainGrid.RowCount - 1) {
      //   // Controller.OnEnteringInsertionRow();
      //   // // if (Entities is ImageList) {
      //   // //   ShowImageOrMessage(null);
      //   // // }
      //}
      Controller.OnMainGridRowEnter(e.RowIndex);
    }

    private void
      MainGridOnRowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
      //Debug.WriteLine("MainGridOnRowsRemoved");
      //Debug.WriteLine(MainGrid.Rows[e.RowIndex].Cells[0].Value);
      Controller.OnMainGridRowRemoved(e.RowIndex);
    }

    private void MainGridOnRowValidated(object sender, DataGridViewCellEventArgs e) {
      //Debug.WriteLine("MainGridOnRowValidated");
      if (ParentRowChanged) {
        ParentRowChanged = false;
      }
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
    private void ParentGridOnCurrentCellChanged(object sender, EventArgs e) {
      MainGrid.AutoResizeColumns();
    }

    /// <summary>
    ///   Handles the parent grid's
    ///   <see cref="DataGridView.RowEnter" /> event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void ParentGridOnRowEnter(object sender, DataGridViewCellEventArgs e) {
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
          }
        }
      }
    }

    private void PopulateGrid() {
      // Controller.FetchData();
      // Text = Controller.MainTable?.TableName;
      MainGrid.CellBeginEdit -= MainGridOnCellBeginEdit;
      //MainGrid.CellEndEdit -= MainGridOnCellEndEdit;
      //MainGrid.CellEnter -= new DataGridViewCellEventHandler(MainGridOnCellEnter);
      //MainGrid.CellStateChanged -= new DataGridViewCellStateChangedEventHandler(GridOnCellStateChanged);
      //MainGrid.CellValidated -= new DataGridViewCellEventHandler(MainGridOnCellValidated);
      MainGrid.CellValueChanged -= MainGridOnCellValueChanged;
      MainGrid.Click -= GridOnClick;
      MainGrid.CurrentCellDirtyStateChanged -= MainGridOnCurrentCellDirtyStateChanged;
      MainGrid.DataError -= MainGridOnDataError;
      //MainGrid.GotFocus -= new EventHandler(ControlOnGotFocus);
      MainGrid.KeyDown -= MainGridOnKeyDown;
      //MainGrid.LostFocus -= new EventHandler(ControlOnLostFocus);
      MainGrid.MouseDown -= GridOnMouseDown;
      //MainGrid.RowStateChanged
      MainGrid.RowEnter -= MainGridOnRowEnter;
      //MainGrid.RowLeave -= MainGridOnRowLeave;
      MainGrid.RowsRemoved -= MainGridOnRowsRemoved;
      MainGrid.RowValidated -= MainGridOnRowValidated;
      Controller.FetchData();
      Text = Controller.MainTableName;
      if (Controller.IsParentTableToBeShown) {
        PopulateParentGrid();
      }
      MainGrid.DataSource = Controller.MainBindingList;
      foreach (DataGridViewColumn column in MainGrid.Columns) {
        ConfigureMainGridColumn(column);
      } // End of foreach
      MainGrid.CellBeginEdit += MainGridOnCellBeginEdit;
      //MainGrid.CellEndEdit += MainGridOnCellEndEdit;
      //MainGrid.CellEnter += new DataGridViewCellEventHandler(MainGridOnCellEnter);
      //MainGrid.CellStateChanged += new DataGridViewCellStateChangedEventHandler(GridOnCellStateChanged);
      //MainGrid.CellValidated += new DataGridViewCellEventHandler(MainGridOnCellValidated);
      MainGrid.CellValueChanged += MainGridOnCellValueChanged;
      MainGrid.Click += GridOnClick;
      MainGrid.CurrentCellDirtyStateChanged += MainGridOnCurrentCellDirtyStateChanged;
      MainGrid.DataError += MainGridOnDataError;
      //MainGrid.GotFocus += new EventHandler(ControlOnGotFocus);
      MainGrid.KeyDown += MainGridOnKeyDown;
      //MainGrid.LostFocus += new EventHandler(ControlOnLostFocus);
      MainGrid.MouseDown += GridOnMouseDown;
      MainGrid.RowEnter += MainGridOnRowEnter;
      //MainGrid.RowLeave += MainGridOnRowLeave;
      MainGrid.RowsRemoved += MainGridOnRowsRemoved;
      MainGrid.RowValidated += MainGridOnRowValidated;
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        MainGrid.AutoResizeColumns();
        AfterPopulateTimer.Start();
      }
    }

    private void PopulateParentGrid() {
      ParentGrid.Click -= GridOnClick;
      ParentGrid.CurrentCellChanged -= ParentGridOnCurrentCellChanged;
      //ParentGrid.GotFocus -= new EventHandler(ControlOnGotFocus);
      //ParentGrid.LostFocus -= new EventHandler(ControlOnLostFocus);
      ParentGrid.MouseDown -= GridOnMouseDown;
      ParentGrid.RowEnter -= ParentGridOnRowEnter;
      ParentGrid.DataSource = Controller.ParentBindingList;
      foreach (DataGridViewColumn column in ParentGrid.Columns) {
        if (column.ValueType == typeof(DateTime)) {
          column.DefaultCellStyle.Format = "dd MMM yyyy";
        }
      } // End of foreach
      // Has to be done when visible.
      // So can't be done when called from constructor.
      if (Visible) {
        ParentGrid.AutoResizeColumns();
      }
      ParentGrid.Click += GridOnClick;
      ParentGrid.CurrentCellChanged += ParentGridOnCurrentCellChanged;
      //ParentGrid.GotFocus += new EventHandler(ControlOnGotFocus);
      //ParentGrid.LostFocus += new EventHandler(ControlOnLostFocus);
      ParentGrid.MouseDown += GridOnMouseDown;
      ParentGrid.RowEnter += ParentGridOnRowEnter;
      if (ParentGrid.RowCount > 0) {
        ParentGrid.CurrentCell =
          ParentGrid.Rows[0].Cells[0]; // Triggers ParentGridOnRowEnter
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
    ///   workaround implemented in EditorViewOnDeactivate.
    /// </remarks>
    private void EditorViewOnActivated(object sender, EventArgs e) {
      //Debug.WriteLine("EditorViewOnActivated: " + this.Text);
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
    private void EditorViewOnDeactivate(object sender, EventArgs e) {
      //Debug.WriteLine("EditorViewOnDeactivate: " + this.Text);
      MainGrid.Enabled = false;
      if (Controller.IsParentTableToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        ParentGrid.Enabled = false;
      }
    }

    private void EditorViewOnFormClosed(object sender, FormClosedEventArgs e) {
      //MainGrid.RowValidated -= new DataGridViewCellEventHandler(MainGridOnRowValidated);
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
    private void EditorViewOnKeyDown(object sender, KeyEventArgs e) {
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
      } //End of switch
    }

    private void EditorViewOnLoad(object sender, EventArgs e) {
      // Has to be done here rather than in constructor
      // in order to tell that this is an MDI child form.
      SizeableFormOptions = SizeableFormOptions.Create(this);
      // And better to do this here than in SetController,
      // where any exception would be indirectly reported,
      // due to being thrown in the controller's constructor.
      OpenTable();
    }

    private void EditorViewOnVisibleChanged(object sender, EventArgs e) {
      if (Visible) {
        //Debug.WriteLine("EditorViewOnVisibleChanged: " + this.Text);
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
          // Does not work if done in EditorViewOnLoad.
          GridSplitContainer.SplitterDistance = Controller.GridSplitterDistance;
          ParentGrid.AutoResizeColumns();
        } else {
          GridSplitContainer.Panel1Collapsed = true;
        }
      }
    }
  } //End of class
} //End of namespace