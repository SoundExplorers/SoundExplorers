﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  public partial class MainView : Form, IView<MainController> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="MainView" /> class.
    /// </summary>
    public MainView() {
      SplashManager.Status = "Building window...";
      InitializeComponent();
      HideMainMenuImageMargins();
    }

    private EditorView EditorView => ActiveMdiChild as EditorView ??
                                     throw new NullReferenceException(nameof(EditorView));

    private MainController Controller { get; set; }
    private SelectEditorView SelectEditorView { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; }

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
      } catch (Exception ex) {
        MessageBox.Show(
          SplashManager.SplashForm,
          ex is ApplicationException ? ex.Message : ex.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        Environment.Exit(0);
      }
    }

    /// <summary>
    ///   Creates a MainView and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    /// </summary>
    [NotNull]
    public static MainView Create() {
      return (MainView)ViewFactory.Create<MainView, MainController>();
    }

    [NotNull]
    private EditorView CreateEditorView() {
      WindowsSeparator2.Visible = true; // See comment in EditorView_FormClosed 
      var result = EditorView.Create(SelectEditorView.Controller.SelectedEntityListType,
        Controller);
      result.FormClosed += EditorView_FormClosed;
      return result;
    }

    [NotNull]
    private SelectEditorView CreateSelectEditorView() {
      return SelectEditorView.Create(Controller.TableName);
    }

    private void EditorView_FormClosed(object sender, FormClosedEventArgs e) {
      // Creating and then manually showing and hiding the separator above
      // the MDI child list in the Windows menu (= MenuStrip.MdiWindowListItem)
      // prevents the separator from remaining visible
      // when all children have been closed, which is what happens when we allow
      // the separator to be created and shown automatically.  See
      // https://stackoverflow.com/questions/12951820/extra-separator-after-mdiwindowlistitem-when-no-child-windows-are-open
      BeginInvoke((Action)delegate { WindowsSeparator2.Visible = MdiChildren.Any(); }
      );
    }

    internal void EditCopyMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Copy();
      }
    }

    internal void EditCutMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Cut();
      }
    }

    internal void EditDeleteSelectedRowsMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.DeleteSelectedRows();
      }
    }

    internal void EditPasteMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Paste();
      }
    }

    internal void EditSelectAllMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.SelectAll();
      }
    }

    private void FileExitMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void FileNewMenuItem_Click(object sender, EventArgs e) {
      SelectEditorView.Text = "Select Table for New Editor";
      if (SelectEditorView.ShowDialog(this) == DialogResult.Cancel) {
        return;
      }
      try {
        var editorView = CreateEditorView();
        editorView.MdiParent = this;
        editorView.Show();
      } catch (ApplicationException ex) {
        MessageBox.Show(
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      } catch (DataException ex) {
        MessageBox.Show(
          ex.Message,
          //ex.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        //} catch (Exception ex) {
        //    MessageBox.Show(
        //        ex.ToString(),
        //        Application.ProductName,
        //        MessageBoxButtons.OK,
        //        MessageBoxIcon.Error);
      }
    }

    private void FileOpenMenuItem_Click(object sender, EventArgs e) {
      if (!MdiChildren.Any()) {
        FileNewMenuItem_Click(sender, e);
        return;
      }
      SelectEditorView.Text = "Select Table for Current Editor";
      if (SelectEditorView.ShowDialog(this) == DialogResult.Cancel) {
        return;
      }
      // When the grid is bound to a second or subsequent table,
      // the type of each cell, which determines the cell editor to be used,
      // is always DataGridViewTextBoxCell,
      // irrespective of what the column's CellTemplate,
      // as shown by the column's CellType property,
      // has been set.
      // This appears to be an error in DataGridView.
      // Even disposing of the grid and re-instantiating it
      // did not solve the problem!
      // To get round this,
      // emulate repopulating the existing table form
      // by replacing it with a new table form
      // with the same location and size.
      //EditorView.OpenTable(
      //    SelectTableForm.EntityTypeName);
      var oldEditorView = EditorView;
      try {
        var newEditorView = CreateEditorView();
        newEditorView.Location = oldEditorView.Location;
        newEditorView.WindowState = oldEditorView.WindowState;
        oldEditorView.Close();
        newEditorView.MdiParent = this;
        newEditorView.Show();
        newEditorView.Size = oldEditorView.Size;
        oldEditorView.Dispose();
        // This won't always work, because SizeableFormOptions
        // copies the size and state of an MDI child form
        // from the last active MDI child form, if any,
        // rather than from the database:
        // if we close the old table form to force it
        // to save its size to the database,
        // it won't be an MDI child any more.
        //Point oldLocation = oldTableForm.Location;
        //oldTableForm.Close();
        //EditorView newTableForm = new EditorView(
        //    SelectTableForm.TableName);
        //newTableForm.MdiParent = this;
        //newTableForm.Show();
        //newTableForm.Location = oldLocation;
        //oldTableForm.Dispose();
      } catch (ApplicationException ex) {
        MessageBox.Show(
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      } catch (DataException ex) {
        MessageBox.Show(
          ex.Message,
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        //} catch (Exception ex) {
        //    MessageBox.Show(
        //        ex.ToString(),
        //        Application.ProductName,
        //        MessageBoxButtons.OK,
        //        MessageBoxIcon.Error);
      }
    }

    private void FileRefreshMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Refresh();
      }
    }

    private void HelpAboutMenuItem_Click(object sender, EventArgs e) {
      new AboutForm().ShowDialog();
    }

    /// <summary>
    ///   As we are not showing images to the left of main menu item texts,
    ///   hides the image margins, except for on the View menu,
    ///   where we need to be able to show ticks to the left of the item texts.
    /// </summary>
    private void HideMainMenuImageMargins() {
      foreach (ToolStripMenuItem menu in MenuStrip.Items) {
        if (menu.Name != nameof(ViewMenu)) {
          ((ToolStripDropDownMenu)menu.DropDown).ShowImageMargin = false;
        }
      }
    }

    private void MainView_FormClosed(object sender, FormClosedEventArgs e) {
      Controller.IsToolBarVisible = ToolStrip.Visible;
      Controller.TableName = MdiChildren.Any()
        ? EditorView.Controller.MainTableName
        : SelectEditorView.Controller.SelectedTableName;
      // Explicitly closing all the MIDI child forms
      // fixes a problem where, 
      // if multiple child forms were open and maximized
      // and the most recently opened child form was not active,
      // the child form window state was incorrectly saved
      // as Normal.
      WindowsCloseAllMenuItem_Click(this, EventArgs.Empty);
      SizeableFormOptions.Save();
    }

    /// <summary>
    ///   Handles the <see cref="Form" />'s
    ///   <see cref="Control.VisibleChanged" />
    ///   event to close the splash form and
    ///   bring the main form to the foreground
    ///   when the load is complete.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   If you do not do this,
    ///   the main window will not show in foreground
    ///   unless a message box has previously been shown
    ///   in front of the splash form.
    /// </remarks>
    private void MainView_VisibleChanged(object sender, EventArgs e) {
      Activate();
      SplashManager.Close();
    }

    /// <summary>
    ///   Circumvents a known problem in WinForms where,
    ///   when an MDI child form is maximized,
    ///   a wrong (VS default) and unwanted icon is shown in the menu strip,
    ///   even when ShowIcon is false for the child form.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   http://social.msdn.microsoft.com/forums/en-US/winforms/thread/3c7c1bea-7f37-4786-acb4-5685f827f8f2/
    /// </remarks>
    private void MenuStrip_ItemAdded(object sender, ToolStripItemEventArgs e) {
      if (e.Item.Text == string.Empty) {
        e.Item.Visible = false;
      }
    }

    private void ViewToolBarMenuItem_Click(object sender, EventArgs e) {
      ToolStrip.Visible = ViewToolBarMenuItem.Checked;
    }

    private void WindowsArrangeIconsMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.ArrangeIcons);
    }

    private void WindowsCascadeMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.Cascade);
    }

    private void WindowsCloseAllMenuItem_Click(object sender, EventArgs e) {
      foreach (var childForm in MdiChildren) {
        childForm.Close();
      }
    }

    private void
      WindowsCloseCurrentTableEditorMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Close();
      }
    }

    private void WindowsTileSideBySideMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileVertical);
    }

    private void WindowsTileStackedMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileHorizontal);
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