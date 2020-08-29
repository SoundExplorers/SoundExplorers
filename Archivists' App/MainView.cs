using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers {
  public partial class MainView : Form, IMainView {
    /// <summary>
    ///   Initialises a new instance of the <see cref="MainView" /> class.
    /// </summary>
    public MainView() {
      SplashManager.Status = "Building window...";
      InitializeComponent();
      try {
        StatusLabel.Text = string.Empty;
        SplashManager.Status = "Positioning window...";
        SizeableFormOptions = new SizeableFormOptions(this);
        SplashManager.Status = "Getting options...";
        StatusStrip.Visible = Controller.IsStatusBarVisible;
        SelectTableForm = new SelectTableForm(Controller.TableName);
        ToolStrip.Visible = Controller.IsToolBarVisible;
        SplashManager.Status = "Creating shortcuts...";
        EventShortcuts = CreateEventShortcuts();
      } catch (Exception ex) {
        MessageBox.Show(
          ex.ToString(),
          Application.ProductName,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        Environment.Exit(0);
      }
    }
    private MainController Controller { get; set; }
    private EventShortcutList EventShortcuts { get; }
    private bool LWinIsDown { get; set; }
    private bool RWinIsDown { get; set; }
    private SelectTableForm SelectTableForm { get; }
    private SizeableFormOptions SizeableFormOptions { get; }

    private TableForm TableView => ActiveMdiChild as TableForm ??
                                   throw new NullReferenceException(nameof(TableView));

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
      new AboutForm().ShowDialog();
    }

    private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.ArrangeIcons);
    }

    private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) {
      LayoutMdi(MdiLayout.Cascade);
    }

    //private void ChildForm_Activated(object sender, EventArgs e) {
    //    var activeTableForm = sender as TableForm;
    //    Debug.WriteLine("ChildForm_Activated: " + activeTableForm.Text);
    //}

    private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var childForm in MdiChildren) {
        childForm.Close();
      }
    }

    private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        TableView.Copy();
      }
    }

    /// <summary>
    ///   For each menu item whose keyboard shortcut includes the Control key,
    ///   create an keyboard alternative shortcut to use the Command (⌘) key
    ///   on a Mac keyboard instead of the Control key
    ///   and alter the text next to the menu item that shows the keyboard shortcut
    ///   to show the Command key version of the shortcut
    ///   instead of the Control key version of the shortcut.
    /// </summary>
    /// <returns>
    ///   The list of alternative keyboard shortcuts.
    /// </returns>
    /// <remarks>
    ///   It would be nice to only show the Mac shortcuts
    ///   on the menu when we are running on a Mac.
    ///   That's probably difficult or impossible to
    ///   be certain of, as we will be running in a Windows VM
    ///   on the Mac.
    ///   One way might be by detecting whether a Mac keyboard
    ///   is plugged into the computer.  But that's no reliable,
    ///   as Device Manager just calls the Mac keyboard "HID Keyboard Device",
    ///   which it would call any keyboard that did not provide its model name
    ///   via plug and play.  Besides, I've so far not been able to find out how to
    ///   enumerate the device manager's keyboard list in code.
    ///   Another way would be to detect whether we are running in a VM.
    ///   It looks like there could be several possible ways of doing that,
    ///   sometimes depending on VM publisher and sometimes not 100% reliable.
    /// </remarks>
    private EventShortcutList CreateEventShortcuts() {
      var result = new EventShortcutList();
      foreach (ToolStripMenuItem menu in MenuStrip.Items) {
        foreach (ToolStripItem dropDownItem in menu.DropDownItems) {
          var menuItem = (ToolStripMenuItem)dropDownItem;
          if (menuItem != null) {
            if (menuItem.ShortcutKeys.HasFlag(Keys.Control)) {
              var letterKeyCode = (Keys)(menuItem.ShortcutKeys - Keys.Control);
              result.Add(new EventShortcut(letterKeyCode, menuItem, "Click"));
              menuItem.ShortcutKeyDisplayString = "⌘+" + letterKeyCode;
            }
          }
        } //End of foreach
      } //End of foreach
      return result;
    }

    private void CutToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MdiChildren.Any()) {
        TableView.Cut();
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
        TableView.EditAudioFileTags();
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
      if (MdiChildren.Any()) {
        Controller.TableName = TableView.Entities.TableName;
      } else {
        Controller.TableName = SelectTableForm.TableName;
      }
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
    ///   <see cref="Control.KeyDown" /> event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   In order for this event handler to be triggered,
    ///   the <see cref="Form" />'s <see cref="Form.KeyPreview" />
    ///   property must be set to <b>True</b>.
    ///   <para>
    ///     Mac keyboard:
    ///     Delete = Keys.Back (Backspace)
    ///     Left Command (⌘) = Keys.LWin ((Left) Start)
    ///     Right Command (⌘) = Keys.RWin (Right Start - not on most Windows keyboards)
    ///   </para>
    ///   <para>
    ///     This is used to provide alternative shortcuts
    ///     using a command (⌘) key on a Mac keyboard
    ///     to substitute for menu item shortcuts
    ///     that use a control key on a Windows keyboard.
    ///   </para>
    /// </remarks>
    private void MainView_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.LWin:
          LWinIsDown = true;
          break;
        case Keys.RWin:
          RWinIsDown = true;
          break;
        default:
          if (LWinIsDown
              || RWinIsDown) {
            if (EventShortcuts.ContainsKey(e.KeyCode)) {
              e.SuppressKeyPress = true;
              EventShortcuts[e.KeyCode].RaiseEvent(EventArgs.Empty);
            }
          }
          break;
      } //End of switch
    }

    /// <summary>
    ///   Handles the <see cref="Form" />'s
    ///   <see cref="Control.KeyUp" /> event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   In order for this event handler to be triggered,
    ///   the <see cref="Form" />'s <see cref="Form.KeyPreview" />
    ///   property must be set to <b>True</b>.
    /// </remarks>
    private void MainView_KeyUp(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.LWin:
          LWinIsDown = false;
          break;
        case Keys.RWin:
          RWinIsDown = false;
          break;
      } //End of switch
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
      SelectTableForm.Text = "Select Table for New Editor";
      if (SelectTableForm.ShowDialog(this) == DialogResult.Cancel) {
        return;
      }
      try {
        var tableForm = new TableForm(
          SelectTableForm.TableName) {MdiParent = this};
        tableForm.Show();
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
      SelectTableForm.Text = "Select Table for Current Editor";
      if (SelectTableForm.ShowDialog(this) == DialogResult.Cancel) {
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
      //TableView.OpenTable(
      //    SelectTableForm.EntityTypeName);
      var oldTableForm = TableView;
      try {
        var newTableForm = new TableForm(
          SelectTableForm.TableName) {
          Location = oldTableForm.Location, WindowState = oldTableForm.WindowState
        };
        oldTableForm.Close();
        newTableForm.MdiParent = this;
        newTableForm.Show();
        newTableForm.Size = oldTableForm.Size;
        oldTableForm.Dispose();
        // This won't always work, because SizeableFormOptions
        // copies the size and state of an MDI child form
        // from the last active MDI child form, if any,
        // rather than from the database:
        // if we close the old table form to force it
        // to save its size to the database,
        // it won't be an MDI child any more.
        //Point oldLocation = oldTableForm.Location;
        //oldTableForm.Close();
        //TableForm newTableForm = new TableForm(
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
        TableView.Paste();
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
        TableView.PlayAudio();
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
        TableView.PlayVideo();
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
        TableView.Refresh();
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
        TableView.ShowNewsletter();
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

    public void SetController(MainController controller) {
      Controller = controller;
    }
  } //End of class
} //End of namespace