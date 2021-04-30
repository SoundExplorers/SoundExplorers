using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  public partial class MainView : Form, IMainView {
    /// <summary>
    ///   Initialises a new instance of the <see cref="MainView" /> class.
    /// </summary>
    public MainView() {
      SplashManager.Status = "Building window...";
      InitializeComponent();
      HideMainMenuImageMargins();
      DisableGridToolStripButtons();
      StatusLabel.Text = string.Empty;
      FileMenu.DropDown.Opening += FileMenu_DropDown_Opening;
      EditMenu.DropDown.Opening += EditMenu_DropDown_Opening;
      WindowsMenu.DropDown.Opening += WindowsMenu_DropDown_Opening;
    }

    private EditorView? EditorView => ActiveMdiChild as EditorView;
    private MainController Controller { get; set; } = null!;
    private SelectEditorView SelectEditorView { get; set; } = null!;
    private SizeableFormOptions SizeableFormOptions { get; set; } = null!;

    public void SetController(MainController controller) {
      Controller = controller;
      try {
        SplashManager.Status = "Connecting to database...";
        Controller.ConnectToDatabase();
        SplashManager.Status = "Positioning window...";
        SizeableFormOptions = SizeableFormOptions.Create(this);
        SplashManager.Status = "Getting options...";
        SelectEditorView = CreateSelectEditorView();
        ToolStrip.Visible = Controller.IsToolBarVisible;
        StatusLabel.Visible = Controller.IsStatusBarVisible;
      } catch (Exception exception) {
        // If you put a breakpoint in this catch block, don't forget to Alt+Tab to see 
        // and click OK on the message!
        if (exception is ApplicationException) {
          MessageBox.Show(
            SplashManager.SplashForm,
            exception.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        } else {
          MessageWindow.Show(SplashManager.SplashForm, exception.ToString(),
            $"{Application.ProductName} - Terminal Error");
        }
        Environment.Exit(0);
      }
    }

    public void ShowErrorMessage(string text) {
      ShowMessage(text, MessageBoxIcon.Error);
    }

    /// <summary>
    ///   Creates a MainView and its associated controller, as per the
    ///   Model-View-Controller design pattern, returning the view instance created.
    /// </summary>
    public static MainView Create() {
      return (MainView)ViewFactory.Create<MainView, MainController>();
    }

    private EditorView CreateEditorView() {
      WindowsSeparator3.Visible = true; // See comment in OnEditorClosed 
      var result = EditorView.Create(SelectEditorView.Controller.SelectedEntityListType!,
        Controller);
      result.FormClosed += EditorView_FormClosed;
      return result;
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
      base.OnFormClosed(e);
      Controller.IsToolBarVisible = ToolStrip.Visible;
      Controller.IsStatusBarVisible = StatusLabel.Visible;
      Controller.TableName = (MdiChildren.Any()
        ? EditorView?.MainGrid.Controller.TableName
        : SelectEditorView.Controller.SelectedTableName)!;
      // Explicitly closing all the MIDI child forms fixes a problem where, if multiple
      // child forms were open and maximized and the most recently opened child form was
      // not active, the child form window state was incorrectly saved as Normal.
      WindowsCloseAllMenuItem_Click(this, EventArgs.Empty);
      SizeableFormOptions.Save();
    }

    /// <summary>
    ///   If you do not do this, the main window will not show in foreground unless a
    ///   message box has previously been shown in front of the splash form.
    /// </summary>
    protected override void OnVisibleChanged(EventArgs e) {
      base.OnVisibleChanged(e);
      Activate();
      SplashManager.Close();
    }

    private SelectEditorView CreateSelectEditorView() {
      return SelectEditorView.Create(Controller.TableName);
    }

    private void DisableGridToolStripButtons() {
      RefreshToolStripButton.Enabled = CutToolStripButton.Enabled =
        CopyToolStripButton.Enabled = PasteToolStripButton.Enabled =
          ToolsLinkMenuItem.Enabled = LinkToolStripButton.Enabled = false;
    }

    private void OnEditorClosed() {
      // Creating and then manually showing and hiding the separator above the MDI child
      // list in the Windows menu (= MenuStrip.MdiWindowListItem) prevents the separator
      // from remaining visible when all children have been closed, which is what happens
      // when we allow the separator to be created and shown automatically.  See
      // https://stackoverflow.com/questions/12951820/extra-separator-after-mdiwindowlistitem-when-no-child-windows-are-open
      if (!MdiChildren.Any()) {
        WindowsSeparator3.Visible = false;
        DisableGridToolStripButtons();
      }
    }

    private void OnNewEditorOpened() {
      RefreshToolStripButton.Enabled = true;
    }

    private void EditorView_FormClosed(object? sender, FormClosedEventArgs e) {
      StatusLabel.Text = string.Empty;
      BeginInvoke((Action)OnEditorClosed);
    }

    private void EditMenu_DropDown_Opening(object? sender, CancelEventArgs e) {
      if (MdiChildren.Any()) {
        EditorView?.CurrentGrid?.EnableOrDisableMenuItems(
          EditCutMenuItem, EditCopyMenuItem, EditPasteMenuItem,
          EditDeleteMenuItem,
          EditSelectAllMenuItem, EditSelectRowMenuItem, EditDeleteSelectedRowsMenuItem);
      } else {
        foreach (ToolStripItem item in EditMenu.DropDownItems) {
          item.Enabled = false;
        }
      }
    }

    private void EditCutMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.Cut();
    }

    private void EditCopyMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.Copy();
    }

    private void EditPasteMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.Paste();
    }

    private void EditDeleteMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.Delete();
    }

    private void EditSelectAllMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.SelectAll();
    }

    private void EditSelectRowMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.SelectRow();
    }

    private void EditDeleteSelectedRowsMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.ContextMenu.DeleteSelectedRows();
    }

    private void FileMenu_DropDown_Opening(object? sender, CancelEventArgs e) {
      FileRefreshMenuItem.Enabled = MdiChildren.Any();
    }

    private void FileExitMenuItem_Click(object? sender, EventArgs e) {
      Close();
    }

    private void FileNewMenuItem_Click(object? sender, EventArgs e) {
      SelectEditorView.Text = "Select Table for New Editor";
      if (SelectEditorView.ShowDialog(this) == DialogResult.Cancel) {
        return;
      }
      Cursor = Cursors.WaitCursor;
      BeginInvoke((Action)NewEditorAsync);
    }

    private void NewEditorAsync() {
      var editorView = CreateEditorView();
      editorView.MdiParent = this;
      editorView.Show();
      BeginInvoke((Action)OnNewEditorOpened);
    }

    private void FileOpenMenuItem_Click(object? sender, EventArgs e) {
      if (!MdiChildren.Any()) {
        FileNewMenuItem_Click(sender, e);
        return;
      }
      SelectEditorView.Text = "Select Table for Current Editor";
      if (SelectEditorView.ShowDialog(this) == DialogResult.Cancel) {
        return;
      }
      Cursor = Cursors.WaitCursor;
      BeginInvoke((Action)OpenEditorAsync);
    }

    private void OpenEditorAsync() {
      // When the grid is bound to a second or subsequent table, the type of each cell,
      // which determines the cell editor to be used, is always DataGridViewTextBoxCell,
      // irrespective of what the column's CellTemplate, as shown by the column's
      // CellType property, has been set. This appears to be an error in DataGridView.
      // Even disposing of the grid and re-instantiating it did not solve the problem!
      // To get round this, emulate repopulating the existing table form by replacing it
      // with a new table form with the same location and size.
      var oldEditorView = EditorView!;
      if (oldEditorView.WindowState == FormWindowState.Maximized) {
        oldEditorView.Close();
        oldEditorView.Dispose();
        NewEditorAsync();
        return;
      }
      var newEditorView = CreateEditorView();
      newEditorView.WindowState = oldEditorView.WindowState;
      oldEditorView.Close();
      newEditorView.MdiParent = this;
      newEditorView.Show();
      newEditorView.Location = oldEditorView.Location;
      newEditorView.Size = oldEditorView.Size;
      oldEditorView.Dispose();
      // This won't always work, because SizeableFormOptions copies the size and state of
      // an MDI child form from the last active MDI child form, if any, rather than from
      // the database: if we close the old table form to force it to save its size to the
      // database, it won't be an MDI child any more.
      //Point oldLocation = oldTableForm.Location;
      //oldTableForm.Close();
      //EditorView newTableForm = new EditorView(
      //    SelectTableForm.TableName);
      //newTableForm.MdiParent = this;
      //newTableForm.Show();
      //newTableForm.Location = oldLocation;
      //oldTableForm.Dispose();
    }

    private void FileRefreshMenuItem_Click(object? sender, EventArgs e) {
      if (MdiChildren.Any()) {
        Cursor = Cursors.WaitCursor;
        EditorView?.Controller.Populate();
      }
    }

    private void HelpEntityRelationshipDiagramMenuItem_Click(
      object? sender, EventArgs e) {
      Controller.ShowEntityRelationshipDiagram();
    }

    private void HelpKeyboardShortcutsMenuItem_Click(object? sender, EventArgs e) {
      Controller.ShowKeyboardShortcuts();
    }

    private void HelpAboutMenuItem_Click(object? sender, EventArgs e) {
      new AboutView().ShowDialog();
    }

    /// <summary>
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
    ///   As we are not showing images to the left of main menu item texts, hides the
    ///   image margins, except for on the View menu, where we need to be able to show
    ///   ticks to the left of the item texts.
    /// </summary>
    private void HideMainMenuImageMargins() {
      foreach (ToolStripMenuItem menu in MenuStrip.Items) {
        if (menu.Name != nameof(ViewMenu)) {
          ((ToolStripDropDownMenu)menu.DropDown).ShowImageMargin = false;
        }
      }
    }

    /// <summary>
    ///   Circumvents a known problem in WinForms where, when an MDI child form is
    ///   maximized, a wrong (VS default) and unwanted icon is shown in the menu strip,
    ///   even when ShowIcon is false for the child form.
    /// </summary>
    /// <remarks>
    ///   http://social.msdn.microsoft.com/forums/en-US/winforms/thread/3c7c1bea-7f37-4786-acb4-5685f827f8f2/
    /// </remarks>
    private void MenuStrip_ItemAdded(object? sender, ToolStripItemEventArgs e) {
      if (e.Item.Text == string.Empty) {
        e.Item.Visible = false;
      }
    }

    private void ShowMessage(string text, MessageBoxIcon icon) {
      Cursor = Cursors.Default;
      MessageBox.Show(
        this, text, Application.ProductName, MessageBoxButtons.OK, icon);
    }

    private void ToolsLinkMenuItem_Click(object? sender, EventArgs e) {
      EditorView?.CurrentGrid?.Controller.FollowLink();
    }

    private void ToolsOptionsMenuItem_Click(object? sender, EventArgs e) {
      var optionsView = OptionsView.Create();
      optionsView.ShowDialog();
    }

    private void ViewToolBarMenuItem_Click(object? sender, EventArgs e) {
      ToolStrip.Visible = ViewToolBarMenuItem.Checked;
    }

    private void ViewStatusBarMenuItem_Click(object? sender, EventArgs e) {
      StatusLabel.Visible = ViewStatusBarMenuItem.Checked;
    }

    private void WindowsMenu_DropDown_Opening(object? sender, CancelEventArgs e) {
      bool hasChildren = MdiChildren.Any();
      foreach (ToolStripItem item in WindowsMenu.DropDownItems) {
        item.Enabled = hasChildren;
      }
      if (!hasChildren) {
        return;
      }
      bool hasMinimizedChildren = (
        from child in MdiChildren
        where child.WindowState == FormWindowState.Minimized
        select child).Any();
      WindowsArrangeIconsMenuItem.Enabled = hasMinimizedChildren;
      WindowsNextMenuItem.Enabled =
        WindowsPreviousMenuItem.Enabled = MdiChildren.Length > 1;
    }

    private void WindowsCascadeMenuItem_Click(object? sender, EventArgs e) {
      LayoutMdi(MdiLayout.Cascade);
    }

    private void WindowsTileSideBySideMenuItem_Click(object? sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileVertical);
    }

    private void WindowsTileStackedMenuItem_Click(object? sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileHorizontal);
    }

    private void WindowsArrangeIconsMenuItem_Click(object? sender, EventArgs e) {
      LayoutMdi(MdiLayout.ArrangeIcons);
    }

    private void WindowsNextMenuItem_Click(object? sender, EventArgs e) {
      // ReSharper disable once CommentTypo
      // Just emulating the MDI's built-in Next behaviour (Ctrl+F6), which backtracks
      // through the child editor windows in the order in which they were opened.
      // Previous (Ctrl+Shift+F6) does the converse. It looks like they are the wrong way
      // round. And no attempt is made to progress through the order in which the user
      // actually accessed the windows, i.e. by clicking on them or by selecting a
      // numbered window from the Windows menu. So there is arguably room for
      // improvement, though, for consistency, it would then be necessary to change the
      // behaviour of the keyboard shortcuts and, if possible, Next on the Editor
      // window's system menu.
      var childList = MdiChildren.ToList();
      int currentChildIndex = childList.IndexOf(ActiveMdiChild);
      int nextChildIndex =
        currentChildIndex > 0 ? currentChildIndex - 1 : childList.Count - 1;
      childList[nextChildIndex].Activate();
    }

    private void WindowsPreviousMenuItem_Click(object? sender, EventArgs e) {
      // Se comment in WindowsNextMenuItem_Click.
      var childList = MdiChildren.ToList();
      int currentChildIndex = childList.IndexOf(ActiveMdiChild);
      int previousChildIndex =
        currentChildIndex < childList.Count - 1 ? currentChildIndex + 1 : 0;
      childList[previousChildIndex].Activate();
    }

    private void WindowsCloseCurrentTableEditorMenuItem_Click(
      object? sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView?.Close();
      }
    }

    private void WindowsCloseAllMenuItem_Click(object? sender, EventArgs e) {
      foreach (var childForm in MdiChildren) {
        childForm.Close();
      }
    }

    protected override void WndProc(ref Message m) {
      if (m.Msg == (int)WindowsMessage.WM_CLOSE) {
        // Attempting to close Form
        Controller.IsClosing = true;
      }
      base.WndProc(ref m);
    }
  } //End of class
} //End of namespace