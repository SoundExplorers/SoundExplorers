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
      GridSplitContainer.MouseCaptureChanged += GridSplitContainer_MouseCaptureChanged;
      // THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER
      ImageSplitContainer.MouseCaptureChanged += GridSplitContainer_MouseCaptureChanged;
    }

    private EditorController Controller { get; set; } = null!;
    public GridBase? CurrentGrid { get; internal set; }
    private MainView MainView => (MainView)MdiParent;
    private bool IsClosed { get; set; }

    private SizeableFormOptions SizeableFormOptions { get; set; } = null!;
    IGrid? IEditorView.CurrentGrid => CurrentGrid;
    IMainGrid IEditorView.MainGrid => MainGrid;
    IParentGrid IEditorView.ParentGrid => ParentGrid;

    void IEditorView.Close() {
      Close();
    }

    public void OnError() {
      Debug.WriteLine("EditorView.OnError");
      SetMouseCursorToWait();
      BeginInvoke((Action)MainGrid.OnError);
    }

    /// <summary>
    ///   Sets the position of the horizontal splitter between the two grids to the
    ///   previously saved value or, if this is the first time the user has opened an
    ///   editor window, a default.
    /// </summary>
    /// <remarks>
    ///   Setting the horizontal splitter position has to be called asynchronously to
    ///   ensure it is positioned correctly if the editor window is opened maximised.
    /// </remarks>
    public void OnParentAndMainGridsShown() {
      Debug.WriteLine("EditorView.OnParentAndMainGridsShown");
      int savedGridSplitterDistance = Controller.GridSplitterDistance;
      GridSplitContainer.SplitterDistance =
        savedGridSplitterDistance > 0 ? savedGridSplitterDistance : 180;
    }

    public void SetMouseCursorToDefault() {
      Debug.WriteLine("EditorView.SetMouseCursorToDefault");
      MainView.Cursor = Cursors.Default;
    }

    public void SetMouseCursorToWait() {
      Debug.WriteLine("EditorView.SetMouseCursorToWait");
      MainView.Cursor = Cursors.WaitCursor;
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

    // public void PopulateMainGridOnParentRowChanged(int parentRowIndex) {
    //   Debug.WriteLine(
    //     $"EditorView.PopulateMainGridOnParentRowChanged: parent row {parentRowIndex}");
    //   Controller.PopulateMainGridOnParentRowChanged(parentRowIndex);
    // }

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
      // THIS CAN BE USED INSTEAD FOR EASE OF EXCEPTION HANDLING:
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
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
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
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
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
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
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
    ///   Shifts the focus back to the current grid when the user lets go of the grid
    ///   splitter with the mouse.
    /// </summary>
    private void GridSplitContainer_MouseCaptureChanged(object? sender, EventArgs e) {
      BeginInvoke((Action)Controller.FocusCurrentGrid);
    }

    /// <summary>
    ///   Raised when an exception is thrown on ending a cell edit.
    /// </summary>
    /// <remarks>
    ///   The event has to be handled for error handling to work. Overriding the
    ///   corresponding protected method in <see cref="MainGrid" /> does not work.
    /// </remarks>
    private void MainGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e) {
      // Debug.WriteLine("MainGrid_DataError");
      string columnName = MainGrid.Columns[e.ColumnIndex].Name;
      MainGrid.Controller.OnCellEditException(e.RowIndex, columnName,
        e.Exception);
    }

    /// <summary>
    ///   Handles the main grid's <see cref="DataGridView.RowsRemoved" /> event, which is
    ///   actually called once for each row removed, even when multiple selected rows are
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
    private void MainGrid_RowRemoved(
      object? sender, DataGridViewRowsRemovedEventArgs e) {
      MainGrid.Controller.OnRowRemoved(e.RowIndex);
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
      // if (IsFixingFocus) {
      //   IsFixingFocus = false;
      //   // Stops the main grid from being focused when the user changes row on the parent
      //   // grid, repopulating the main grid.  See also the comments for IsFixingFocus. 
      //   return;
      // }
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
        ParentGrid.Enabled = true;
        // When an existing parent /child editor window is activated,the grid splitter
        // gets focused. So we need to switch focus to the parent grid.
        BeginInvoke((Func<bool>)ParentGrid.Focus);
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
        ParentGrid.Enabled = false;
      }
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
      IsClosed = true;
      base.OnFormClosed(e);
      SizeableFormOptions.Save();
      // THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
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
          Debug.WriteLine(
            $"EditorView.OnKeyDown F6: ParentGrid.Enabled = {ParentGrid.Enabled}");
          Controller.FocusUnfocusedGridIfAny();
          break;
      } //End of switch
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);
      MainGrid.SetController(new MainGridController(MainGrid, Controller));
      MainGrid.EditorView = this;
      MainGrid.MainView = MainView;
      MainGrid.DataError += MainGrid_DataError;
      MainGrid.RowsRemoved += MainGrid_RowRemoved;
      MainGrid.RowValidated += MainGrid_RowValidated;
      if (Controller.IsParentGridToBeShown) {
        ParentGrid.SetController(new ParentGridController(ParentGrid, Controller));
        ParentGrid.EditorView = this;
        ParentGrid.MainView = MainView;
      }
      // Has to be done here rather than in constructor in order to tell that this is an
      // MDI child form.
      SizeableFormOptions = SizeableFormOptions.Create(this);
      // And better to do this here than in SetController, where any exception would be
      // indirectly reported, due to being thrown in the controller's constructor.
      ShowData();
    }

    protected override void OnMove(EventArgs e) {
      base.OnMove(e);
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      // Stop ghost border lines appearing on main window background.
      ParentForm?.Refresh();
    }

    public void RefreshData() {
      Controller.Populate();
    }

    /// <summary>
    ///   Shows the data for the first time, on an new grid or grids.
    /// </summary>
    private void ShowData() {
      GridSplitContainer.Panel1Collapsed = !Controller.IsParentGridToBeShown;
      Controller.Populate();
      Text = MainGrid.Controller.TableName;
    }

    private void ShowMessage(string text, MessageBoxIcon icon) {
      MainView.Cursor = Cursors.Default;
      MessageBox.Show(
        this, text, Application.ProductName, MessageBoxButtons.OK, icon);
    }

    protected override void WndProc(ref Message m) {
      if (m.Msg == (int)WindowsMessage.WM_CLOSE) {
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