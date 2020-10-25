using System;
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
      StatusLabel.Text = string.Empty;
    }

    private MainController Controller { get; set; }
    private SelectEditorView SelectEditorView { get; set; }
    private SizeableFormOptions SizeableFormOptions { get; set; }

    private EditorView EditorView => ActiveMdiChild as EditorView ??
                                   throw new NullReferenceException(nameof(EditorView));

    public void SetController(MainController controller) {
      Controller = controller;
      try {
        SplashManager.Status = "Connecting to database...";
        Controller.ConnectToDatabase();
        SplashManager.Status = "Positioning window...";
        SizeableFormOptions = SizeableFormOptions.Create(this);
        SplashManager.Status = "Getting options...";
        StatusStrip.Visible = Controller.IsStatusBarVisible;
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

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
      new AboutForm().ShowDialog();
    }

    private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.ArrangeIcons);
    }

    private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.Cascade);
    }

    private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var childForm in MdiChildren) {
        childForm.Close();
      }
    }

    private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Copy();
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
    private SelectEditorView CreateSelectEditorView() {
      return SelectEditorView.Create(Controller.TableName);
    }

    [NotNull]
    private EditorView CreateEditorView() {
      return EditorView.Create(SelectEditorView.Controller.SelectedEntityListType);
    }

    private void CutToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Cut();
      }
    }

    /// <summary>
    ///   Handles the
    ///   <see cref="Control.Click" /> event
    ///   of the Edit Audio File Tags menu item and toolbar button to
    ///   edit the tags of the audio file, if found,
    ///   of the current Piece, if any,
    ///   relative to the active Table widow
    ///   or otherwise show an informative message box.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void EditAudioFileTagsToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        try {
          EditorView.Controller.EditAudioFileTags();
        } catch (ApplicationException ex) {
          MessageBox.Show(
            this,
            ex.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        }
      } else {
        MessageBox.Show(
          this,
          "To edit the tags of a piece's audio file, "
          + "first open the Credit or Piece table and select a row.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void MainView_FormClosed(object sender, FormClosedEventArgs e) {
      Controller.IsStatusBarVisible = StatusStrip.Visible;
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
      CloseAllToolStripMenuItem_Click(this, EventArgs.Empty);
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

    private void NewToolStripMenuItem_Click(object sender, EventArgs e) {
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

    private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
      if (!MdiChildren.Any()) {
        NewToolStripMenuItem_Click(sender, e);
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

    private void PasteToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Paste();
      }
    }

    /// <summary>
    ///   Handles the
    ///   <see cref="Control.Click" /> event
    ///   of the Play Audio menu item and toolbar button to
    ///   play the audio, if found,
    ///   of the current Piece, if any,
    ///   relative to the active Table widow
    ///   or otherwise show an informative message box.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void PlayAudioToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        try {
          EditorView.Controller.PlayAudio();
        } catch (ApplicationException ex) {
          MessageBox.Show(
            this,
            ex.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }
      } else {
        MessageBox.Show(
          this,
          "To play a piece's audio, first open the Credit or Piece table and select a row.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    /// <summary>
    ///   Handles the
    ///   <see cref="Control.Click" /> event
    ///   of the Play Video menu item and toolbar button to
    ///   play the video, if found,
    ///   of the current Piece, if any,
    ///   relative to the active Table widow
    ///   or otherwise show an informative message box.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void PlayVideoToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        try {
          EditorView.Controller.PlayVideo();
        } catch (ApplicationException ex) {
          MessageBox.Show(
            this,
            ex.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }
      } else {
        MessageBox.Show(
          this,
          "To play a piece's video, first open the Credit or Piece table and select a row.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    private void RefreshToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        EditorView.Refresh();
      }
    }

    /// <summary>
    ///   Handles the
    ///   <see cref="Control.Click" /> event
    ///   of the Show Newsletter menu item and toolbar button to
    ///   show the current newsletter, if any,
    ///   relative to the active Table widow
    ///   or otherwise show an informative message box.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void ShowNewsletterToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        try {
          EditorView.Controller.ShowNewsletter();
        } catch (ApplicationException exception) {
          MessageBox.Show(
            this,
            exception.Message,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }
      } else {
        MessageBox.Show(
          this,
          "To show a newsletter, first show the "
          + "ArtistInImage, Credit, Image, Newsletter, "
          + "Performance, Piece or Set table and select a row.",
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
    }

    private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
      StatusStrip.Visible = StatusBarToolStripMenuItem.Checked;
    }

    private void TileSideBySideToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileVertical);
    }

    private void TileStackedToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.TileHorizontal);
    }

    private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
      ToolStrip.Visible = ToolBarToolStripMenuItem.Checked;
    }
  } //End of class
} //End of namespace