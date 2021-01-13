using System;
using System.Diagnostics;
using System.Windows.Forms;
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
      // Allow things to be dropped on to the PictureBox.
      FittedPictureBox1.AllowDrop = true;
      GridSplitContainer.GotFocus += SplitContainer_GotFocus;
      ImageSplitContainer.GotFocus += SplitContainer_GotFocus;
    }

    public GridBase? FocusedGrid { get; private set; }
    private MainView MainView => (MainView)MdiParent;
    private bool IsClosed { get; set; }

    /// <summary>
    ///   This flag is used in a workaround for an unwanted feature of
    ///   <see cref="DataGridView.OnBindingContextChanged" />.  In some of the multiple
    ///   times that event method occurs per <see cref="Populate" />, it makes row 0 of
    ///   the main grid current (by calling internal method
    ///   DataGridView.MakeFirstDisplayedCellCurrentCell). In this application, that
    ///   would usually be the wrong row, as we want the new row to be current on
    ///   population of the main grid. Normally, the unwanted behaviour would trigger
    ///   <see cref="DataGridView.OnRowEnter" /> for row 0, which <see cref="MainGrid" />
    ///   overrides. The workaround that stops OnRowEnter from being called in this case.
    /// </summary>
    /// <remarks>
    ///   Another workaround I looked at was to override OnBindingContextChanged to stop
    ///   the base method from being called.  That is not practicable, as it prevents
    ///   editor controls from appearing in grid cells when they are edited!
    /// </remarks>
    private bool IsFixingFocus { get; set; }

    private bool IsRefreshingData { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; } = null!;
    public EditorController Controller { get; private set; } = null!;
    public bool IsPopulating { get; private set; }
    IMainGrid IEditorView.MainGrid => MainGrid;
    IParentGrid IEditorView.ParentGrid => ParentGrid;

    public void OnError() {
      // Debug.WriteLine("EditorView.OnError");
      MainView.Cursor = Cursors.WaitCursor;
      BeginInvoke((Action)MainGrid.OnError);
    }

    public void OnMainGridPopulated() {
      MainGrid.MakeNewRowCurrent();
      BeginInvoke((Action)OnMainGridPopulatedAsync);
    }

    public void ShowErrorMessage(string text) {
      //MeasureProfiler.SaveData();
      ShowMessage(text, MessageBoxIcon.Error);
    }

    public void ShowWarningMessage(string text) {
      ShowMessage(text, MessageBoxIcon.Warning);
    }

    /// <summary>
    ///   Sets the view's controller.
    /// </summary>
    /// <remarks>
    ///   Avoid putting any additional code here, as any exception would be indirectly
    ///   reported, due to being thrown in the controller's constructor, where
    ///   <see cref="SetController" /> is called. Any code that can be executed once the
    ///   controller has been set should instead be in <see cref="OnLoad" />.
    /// </remarks>
    public void SetController(EditorController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Creates a EditorView and its associated controller, as per the
    ///   Model-View-Controller design pattern, returning the view instance created. The
    ///   parameter is passed to the controller's constructor.
    /// </summary>
    /// <param name="entityListType">
    ///   The type of entity list whose data is to be displayed.
    /// </param>
    /// <param name="mainController">
    ///   Controller for the main window.
    /// </param>
    public static EditorView Create(Type entityListType,
      MainController mainController) {
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

    /// <summary>
    ///   Refreshes the contents of the existing grid or grids from the database.
    /// </summary>
    public void RefreshData() {
      IsRefreshingData = true;
      Populate();
      if (Controller.IsParentGridToBeShown) {
        FocusGrid(ParentGrid);
      } else {
        MainGrid.Focus();
        FocusedGrid = MainGrid;
      }
      Refresh();
      IsRefreshingData = false;
    }

    /// <summary>
    ///   Handles both the missing image label's and the picture box's
    ///   <see cref="Control.DragDrop" /> event to drop a file path on the label or
    ///   picture box, whichever is shown, updating the corresponding Image.Path cell,
    ///   focusing the main grid and making the updated cell the current cell.
    /// </summary>
    /// <remarks>
    ///   To save confusion, this will not work while the main grid is in edit mode.
    /// </remarks>
    private void FittedPictureBox1_DragDrop(object? sender, DragEventArgs e) {
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
    ///   Handles both the missing image label's and the picture box's
    ///   <see cref="Control.DragOver" /> event to show that a file path can be dropped
    ///   on the label or picture box, whichever is shown, if the main grid shows the
    ///   Image table and is not in edit mode.
    /// </summary>
    /// <remarks>
    ///   To save confusion, path dropping is not supported while the main grid is in edit mode.
    /// </remarks>
    private void FittedPictureBox1_DragOver(object? sender, DragEventArgs e) {
      // if (Entities is ImageList
      //     && !MainGrid.IsCurrentCellInEditMode
      //     && e.Data.GetDataPresent(DataFormats.FileDrop)) {
      //   e.Effect = DragDropEffects.Copy;
      // } else {
      //   e.Effect = DragDropEffects.None;
      // }
    }

    /// <summary>
    ///   Handles the picture box's <see cref="Control.MouseDown" /> event to initiate a
    ///   drag-and-drop operation to allow the path of the image displayed in the picture
    ///   box to be dropped on another application.
    /// </summary>
    private void FittedPictureBox1_MouseDown(object? sender, MouseEventArgs e) {
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
    ///   Where two grids are shown, their colour schemes are swapped round, indicating
    ///   which is now the current grid by having the usual colour scheme inverted on the
    ///   other grid.
    /// </remarks>
    private void FocusGrid(GridBase grid) {
      if (!Controller.IsParentGridToBeShown) {
        grid.Focus();
        return;
      }
      // A read-only related grid for the parent table is shown above the main grid.
      grid.CellColorScheme.RestoreToDefault();
      var unfocusedGrid =
        grid == MainGrid ? (GridBase)ParentGrid : MainGrid;
      unfocusedGrid.CellColorScheme.Invert();
      // By trial an error, I found that this complicated rigmarole was required to
      // properly shift the focus programatically, i.e. in EditorView_KeyDown to
      // implement doing it with the F6 key.
      unfocusedGrid.Enabled = false;
      grid.Enabled = true;
      Refresh();
      grid.Focus();
      Refresh();
      unfocusedGrid.Enabled = true;
      FocusedGrid = grid;
    }

    /// <summary>
    ///   Handles the <see cref="Control.MouseDown" /> event of either of the two grids.
    /// </summary>
    /// <remarks>
    ///   When either mouse button is clicked, the grid will be focused if it is not
    ///   already.
    ///   <para>
    ///     I tried to initiate a drag-and-drop operation when a cell is clicked with the
    ///     mouse button 1. But it did not work, possibly because it puts the cell into
    ///     edit mode and also, when dragged, selects multiple rows. Perhaps we could
    ///     initiate a drag-and-drop operation on Control + mouse button 1.
    ///   </para>
    /// </remarks>
    private void Grid_MouseDown(object? sender, MouseEventArgs e) {
      var grid = (GridBase)sender!;
      if (grid != FocusedGrid) {
        FocusGrid(grid);
      }
    }

    /// <summary>
    ///   Handles the main grid's <see cref="DataGridView.CellBeginEdit" /> event, which
    ///   occurs when edit mode starts for the currently selected cell, to hide the
    ///   Missing Image label (if visible).
    /// </summary>
    /// <remarks>
    ///   This is only relevant if the Path cell of an Image row is being edited. If the
    ///   Missing Image label was visible just before entering edit mode, it will have
    ///   been because the file was not specified or can't be found or is not an image
    ///   file. That's presumably about to be rectified. So the message to that effect in
    ///   the Missing Image label could be misleading. Also, the advice in the Missing
    ///   Image label that an image file can be dragged onto the label will not apply, as
    ///   dragging and dropping is disabled while the Path cell is being edited.
    /// </remarks>
    private void MainGrid_CellBeginEdit(
      object sender, DataGridViewCellCancelEventArgs e) {
      MissingImageLabel.Visible = false;
    }

    /// <summary>
    ///   Occurs when an external data-parsing or validation operation in an existing row
    ///   (not the insertion row) throws an exception.
    /// </summary>
    /// <remarks>
    ///   The event has to be handled for error handling to work. Overriding the
    ///   corresponding protected method in <see cref="MainGrid" /> does not work.
    /// </remarks>
    private void MainGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e) {
      // Debug.WriteLine("MainGrid_DataError");
      // Debug.WriteLine("Context = " + e.Context);
      // Debug.WriteLine("ColumnIndex = " + e.ColumnIndex);
      // Debug.WriteLine("RowIndex = " + e.RowIndex);
      string columnName = MainGrid.Columns[e.ColumnIndex].Name;
      MainGrid.Controller.OnExistingRowCellEditError(e.RowIndex, columnName,
        e.Exception);
    }

    /// <summary>
    ///   Actually called once for each row removed, even when multiple selected rows are
    ///   removed at once.
    /// </summary>
    /// <remarks>
    ///   For unknown reason, the RowsRemoved event is raised 2 or 3 times while data is
    ///   being loaded into the grid.
    ///   <para>
    ///     Overriding the corresponding protected method in   <see cref="MainGrid" />
    ///     does not work. The event has to be handled, otherwise the program will crash
    ///     in some circumstances.
    ///   </para>
    /// </remarks>
    private void
      MainGrid_RowsRemoved(object? sender, DataGridViewRowsRemovedEventArgs e) {
      MainGrid.Controller.OnRowValidated(e.RowIndex);
    }

    /// <summary>
    ///   Handles the main grid's RowValidated event, which is raised when the user exits
    ///   a row on the grid, even when nothing has changed.
    /// </summary>
    /// <remarks>
    ///   Overriding the corresponding protected method in <see cref="MainGrid" /> does
    ///   not work. The event has to be handled, otherwise the program will crash in some
    ///   circumstances.
    /// </remarks>
    private void MainGrid_RowValidated(object? sender, DataGridViewCellEventArgs e) {
      Debug.WriteLine($"EditorView.MainGrid_RowValidated: row {e.RowIndex}");
      if (IsFixingFocus) {
        IsFixingFocus = false;
        return;
      }
      MainGrid.Controller.OnRowValidated(e.RowIndex);
    }

    /// <summary>
    ///   Enables and focuses the grid when the window is activated.
    /// </summary>
    /// <remarks>
    ///   This is necessary because of the workaround implemented in
    ///   <see cref="OnDeactivate" />.
    /// </remarks>
    protected override void OnActivated(EventArgs e) {
      base.OnActivated(e);
      //Debug.WriteLine("EditorView.OnActivated: " + this.Text);
      MainGrid.Enabled = true;
      if (Controller.IsParentGridToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        FocusGrid(ParentGrid);
      } else {
        MainGrid.Focus();
        FocusedGrid = MainGrid;
      }
    }

    /// <summary>
    ///   Disable the grid when another table window is activated.
    /// </summary>
    /// <remarks>
    ///   For unknown reason, without this workaround, when an editor window with date
    ///   columns is deactivated and another table window is activated, it is impossible
    ///   to navigate or edit the grid on the active window. To be safe, disable the grid
    ///   even if there aren't date columns: maybe there are other data types that would
    ///   cause similar problems.
    /// </remarks>
    protected override void OnDeactivate(EventArgs e) {
      base.OnDeactivate(e);
      //Debug.WriteLine("EditorView.OnDeactivate: " + this.Text);
      MainGrid.Enabled = false;
      if (Controller.IsParentGridToBeShown) {
        // A read-only related grid for the parent table is shown
        // above the main grid.
        ParentGrid.Enabled = false;
      }
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
      IsClosed = true;
      base.OnFormClosed(e);
      SizeableFormOptions.Save();
      // if (Entities is ArtistInImageList
      //     || Entities is ImageList) {
      //   Controller.ImageSplitterDistance = ImageSplitContainer.SplitterDistance;
      // }
      if (Controller.IsParentGridToBeShown) {
        Controller.GridSplitterDistance = GridSplitContainer.SplitterDistance;
      }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      base.OnKeyDown(e);
      switch (e.KeyData) {
        case Keys.F6:
          if (Controller.IsParentGridToBeShown) {
            FocusGrid(FocusedGrid == ParentGrid ? (GridBase)MainGrid : ParentGrid);
          }
          break;
      } //End of switch
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);
      MainGrid.SetController(new MainGridController(MainGrid, this));
      MainGrid.MainView = MainView;
      MainGrid.CellBeginEdit += MainGrid_CellBeginEdit;
      MainGrid.DataError += MainGrid_DataError;
      MainGrid.MouseDown += Grid_MouseDown;
      MainGrid.RowsRemoved += MainGrid_RowsRemoved;
      MainGrid.RowValidated += MainGrid_RowValidated;
      if (Controller.IsParentGridToBeShown) {
        ParentGrid.SetController(new ParentGridController(this));
        ParentGrid.MainView = MainView;
        ParentGrid.MouseDown += Grid_MouseDown;
      }
      // Has to be done here rather than in constructor in order to tell that this is an
      // MDI child form.
      SizeableFormOptions = SizeableFormOptions.Create(this);
      // And better to do this here than in SetController, where any exception would be
      // indirectly reported, due to being thrown in the controller's constructor.
      ShowData();
    }

    private void OnMainGridPopulatedAsync() {
      if (Controller.IsParentGridToBeShown) {
        ParentGrid.Focus();
      } else { // No parent grid
        MainGrid.Focus();
      }
    }

    protected override void OnMove(EventArgs e) {
      base.OnMove(e);
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    /// <summary>
    ///   Sets the position of the horizontal splitter between the two grids to the
    ///   previously saved value or, if this is the first time the user has opened an
    ///   editor window, a default.  Then sets the parent grid's current row, ensuring
    ///   that the row is scrolled into view.
    /// </summary>
    /// <remarks>
    ///   Setting the horizontal splitter position has to be called asynchronously to
    ///   ensure it is positioned correctly if the editor window is opened maximised.
    ///   Setting the parent grid's current row after the grid splitter has been
    ///   positioned ensures that the row is scrolled into view.
    /// </remarks>
    private void OnParentAndMainGridsShownAsync() {
      Debug.WriteLine("EditorView.OnParentAndMainGridsShownAsync");
      int savedGridSplitterDistance = Controller.GridSplitterDistance;
      GridSplitContainer.SplitterDistance =
        savedGridSplitterDistance > 0 ? savedGridSplitterDistance : 180;
      IsFixingFocus = true;
      if (ParentGrid.RowCount > 0) {
        ParentGrid.MakeRowCurrent(ParentGrid.RowCount - 1);
      }
      OnPopulatedAsync();
    }

    private void OnPopulatedAsync() {
      Debug.WriteLine("EditorView.OnPopulatedAsync");
      IsPopulating = false;
      if (MainGrid.RowCount > 0) {
        MainGrid.MakeRowCurrent(MainGrid.RowCount - 1);
      }
      MainView.Cursor = Cursors.Default;
    }

    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    private void Populate() {
      Debug.WriteLine("EditorView.Populate");
      IsPopulating = true;
      if (Controller.IsParentGridToBeShown) {
        ParentGrid.CellColorScheme.RestoreToDefault();
        MainGrid.CellColorScheme.Invert();
        ParentGrid.Populate();
        if (ParentGrid.RowCount > 0) {
          MainGrid.Populate(ParentGrid.Controller.GetChildrenForMainList(
            ParentGrid.RowCount - 1));
          // If the editor window is being loaded, the parent grid's current row is set
          // asynchronously in OnParentAndMainGridsShownAsync to ensure that it is
          // scrolled into view. Otherwise, i.e. if the grid contents are being
          // refreshed, we need to do it here.
          if (IsRefreshingData) {
            ParentGrid.MakeRowCurrent(ParentGrid.RowCount - 1);
            BeginInvoke((Action)OnPopulatedAsync);
          } else { // Showing for first time
            BeginInvoke((Action)OnParentAndMainGridsShownAsync);
          }
        }
      } else {
        MainGrid.Populate();
        BeginInvoke((Action)OnPopulatedAsync);
      }
      Debug.WriteLine("EditorView.Populate END");
    }

    public void PopulateMainGridOnParentRowChanged(int parentRowIndex) {
      Debug.WriteLine(
        $"EditView.PopulateMainGridOnParentRowChanged: parent row {parentRowIndex}");
      IsPopulating = true;
      MainGrid.Populate(ParentGrid.Controller.GetChildrenForMainList(parentRowIndex));
      IsFixingFocus = true;
      BeginInvoke((Action)OnPopulatedAsync);
    }

    /// <summary>
    ///   Shows the data for the first time, on an new grid or grids.
    /// </summary>
    private void ShowData() {
      GridSplitContainer.Panel1Collapsed = !Controller.IsParentGridToBeShown;
      Populate();
      Text = MainGrid.Controller.TableName;
    }

    private void ShowMessage(string text, MessageBoxIcon icon) {
      MainView.Cursor = Cursors.Default;
      MessageBox.Show(
        this, text, Application.ProductName, MessageBoxButtons.OK, icon);
    }

    /// <summary>
    ///   Handle's a SplitContainer's GotFocus event to shift the focus to the current
    ///   grid when the SplitContainer gets the focus.
    /// </summary>
    /// <remarks>
    ///   For unknown reason, when an existing table form is activated,the grid
    ///   SplitContainer gets focused. In any case, we want focus to return to the
    ///   current grid after the user has grabbed the splitter.
    /// </remarks>
    private void SplitContainer_GotFocus(object? sender, EventArgs e) {
      Debug.WriteLine("EditorView.GridSplitContainer_GotFocus");
      BeginInvoke((Action)delegate { FocusGrid(FocusedGrid ?? ParentGrid); });
    }

    protected override void WndProc(ref Message m) {
      // ReSharper disable once InconsistentNaming
      const int WM_CLOSE = 0x0010;
      if (m.Msg == WM_CLOSE) {
        if (IsClosed) {
          // The editor window has already been closed programatically (in
          // MainView.WindowsCloseAllMenuItem_Click), when the main window is closed
          // while the editor window is still open). As a result, the WM_CLOSE message is
          // not received till AFTER the window has closed. So we need to block the
          // editor closure procedures from being run again.
          return;
        }
        // Attempting to close Form
        Controller.IsClosing = true;
      }
      base.WndProc(ref m);
    }
  } //End of class
} //End of namespace