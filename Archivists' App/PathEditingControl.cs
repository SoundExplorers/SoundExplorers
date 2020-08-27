using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SoundExplorers.Data;
using Image = SoundExplorers.Data.Image;

namespace SoundExplorers {
  /// <summary>
  ///   Editing control for a path cell of a DataGridView.
  /// </summary>
  internal class PathEditingControl : UserControl, IDataGridViewEditingControl {
    public PathEditingControl() {
      TextBox = new TextBox();
      TextBox.KeyDown += TextBox_KeyDown;
      Controls.Add(TextBox);
      Button = new Button();
      Controls.Add(Button);
      RenderControl();
    }

    private Button Button { get; }

    private string FileType =>
      EditingControlDataGridView.CurrentCell.OwningColumn.Name.Replace(
        "Path", string.Empty);

    private TextBox TextBox { get; }

    /// <summary>
    /// Changes the control's user interface (UI) to be consistent with the specified
    //  cell style.
    /// </summary>
    /// <param name="dataGridViewCellStyle">
    ///   The System.Windows.Forms.DataGridViewCellStyle to use as the model for the
    ///   UI.
    /// </param>
    public void ApplyCellStyleToEditingControl(
      DataGridViewCellStyle dataGridViewCellStyle) {
      TextBox.BackColor = dataGridViewCellStyle.BackColor;
      TextBox.Font = dataGridViewCellStyle.Font;
      TextBox.ForeColor = dataGridViewCellStyle.ForeColor;
    }

    /// <summary>
    ///   Gets or sets the System.Windows.Forms.DataGridView that contains the cell.
    /// </summary>
    public DataGridView EditingControlDataGridView { get; set; }

    /// <summary>
    ///   Gets or sets the formatted value of the cell being modified by the editor.
    /// </summary>
    public object EditingControlFormattedValue {
      get => TextBox.Text;
      set {
        TextBox.Text = value.ToString();
        TextBox.TextChanged += TextBox_TextChanged;
      }
    }

    /// <summary>
    ///   Gets or sets the index of the hosting cell's parent row.
    /// </summary>
    public int EditingControlRowIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value of the editing control
    //  differs from the value of the hosting cell.
    /// </summary>
    public bool EditingControlValueChanged { get; set; }

    /// <summary>
    /// Determines whether the specified key is a regular input key that the editing
    //  control should process or a special key that the System.Windows.Forms.DataGridView
    //  should process.
    /// </summary>
    /// <param name="keyData">
    /// A System.Windows.Forms.Keys that represents the key that was pressed.
    /// </param>
    /// <param name="dataGridViewWantsInputKey">
    /// True when the System.Windows.Forms.DataGridView wants to process the System.Windows.Forms.Keys
    //  in keyData; otherwise, False.
    /// </param>
    /// <returns>
    /// True if the specified key is a regular input key that should be handled by
    //  the editing control; otherwise, False.
    /// </returns>
    public bool EditingControlWantsInputKey(Keys keyData,
      bool dataGridViewWantsInputKey) {
      // Can't be captured.
      //if (keyData == (Keys.Control | Keys.Enter)) {
      //    return true;
      //}
      //switch (keyData & Keys.KeyCode) {
      //case Keys.ControlKey:
      //    Debug.WriteLine("ControlKey");
      //    break;
      //}//End of switch
      switch (keyData & Keys.KeyCode) {
        case Keys.F2:
        case Keys.Left:
        case Keys.Right:
        case Keys.Home:
        case Keys.End:
          //case Keys.Tab: // Can't be captured.  Can tab to button anyway.
          //case Keys.Enter: // Can't be captured.
          return true;
        default:
          return !dataGridViewWantsInputKey;
      } //End of switch
    }

    /// <summary>
    /// Gets the cursor used when the mouse pointer is over the System.Windows.Forms.DataGridView.EditingPanel
    //  but not over the editing control.
    /// </summary>
    public Cursor EditingPanelCursor => base.Cursor;

    /// <summary>
    /// Retrieves the formatted value of the cell.
    /// </summary>
    /// <param name="context">
    /// A bitwise combination of System.Windows.Forms.DataGridViewDataErrorContexts
    //  values that specifies the context in which the data is needed.
    /// </param>
    /// <returns>
    ///   An System.Object that represents the formatted version of the cell contents.
    /// </returns>
    public object GetEditingControlFormattedValue(
      DataGridViewDataErrorContexts context) {
      return EditingControlFormattedValue;
    }

    /// <summary>
    ///   Prepares the currently selected cell for editing.
    /// </summary>
    /// <param name="selectAll">
    ///   True to select all of the cell's content; otherwise, False.
    /// </param>
    /// <remarks>
    ///   No preparation needs to be done.
    /// </remarks>
    public void PrepareEditingControlForEdit(bool selectAll) { }

    /// <summary>
    /// Gets or sets a value indicating whether the cell contents need to be repositioned
    //  whenever the value changes.
    /// </summary>
    public bool RepositionEditingControlOnValueChange => false;

    /// <summary>
    ///   Called when the button gets the focus,
    ///   either by being tabbed to or by being clicked.
    ///   Shows an open file dialogue box
    ///   to allow a file to be selected.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    ///   The button's GotFocus event handler is used
    ///   instead of its Click event handler to
    ///   show the open file dialogue box because
    ///   pressing Enter when the button (or text box)
    ///   is in focused causes the edit to end and
    ///   the focus to move within the grid to the cell below.
    ///   I tried capturing the Enter key in
    ///   EditingControlWantsInputKey to get round that.
    ///   But it did not work.
    ///   <para>
    ///     If the cell currently contains a path that includes
    ///     a specific folder that exists,
    ///     the initial folder for the Open dialogue
    ///     will be set to that folder.
    ///     Otherwise the initial folder for the Open dialogue
    ///     will be set to a column-specific default folder.
    ///     If the cell currently contains a path of a file
    ///     that exists in the initial folder,
    ///     the initial file name for the Open dialogue
    ///     will be set to the name of that file.
    ///   </para>
    /// </remarks>
    private void Button_GotFocus(object sender, EventArgs e) {
      TextBox.Focus();
      var openFileDialog = new OpenFileDialog();
      openFileDialog.Title =
        ("Select " + FileType + " File").Replace("  ", " ");
      var path = EditingControlFormattedValue.ToString();
      if (path != string.Empty) {
        FileInfo file = null;
        DirectoryInfo folder = null;
        if (path.Contains(Path.DirectorySeparatorChar.ToString())) {
          file = new FileInfo(path);
          folder = file.Directory;
        }
        if (folder == null
            || !folder.Exists) {
          var pathCell = EditingControlDataGridView.CurrentCell as PathCell;
          switch (pathCell.OwningColumn.Name) {
            case "AudioPath": // Piece.AudioPath
              folder = Piece.DefaultAudioFolder;
              break;
            case "Path":
              if (pathCell.Column.TableName == "Image") {
                folder = Image.DefaultFolder; // Image.Path
              } else if (pathCell.Column.TableName == "Newsletter") {
                // Newsletter.Path
                folder = Newsletter.DefaultFolder; // Newsletter.Path
              } else {
                throw new NotSupportedException(
                  pathCell.Column.TableName + ".Path is not supported.");
              }
              break;
            case "VideoPath": // Piece.VideoPath
              folder = Piece.DefaultVideoFolder;
              break;
          } //End of switch
          if (folder != null
              && folder.Exists) {
            if (path.Contains(Path.DirectorySeparatorChar.ToString())) {
              file = new FileInfo(
                folder.FullName
                + Path.DirectorySeparatorChar
                + file.Name);
            } else {
              file = new FileInfo(
                folder.FullName
                + Path.DirectorySeparatorChar
                + path);
            }
          }
        }
        if (folder.Exists) {
          openFileDialog.InitialDirectory = folder.FullName;
          if (file.Exists) {
            openFileDialog.FileName = file.Name;
          }
        }
      }
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        EditingControlFormattedValue = openFileDialog.FileName;
      }
    }

    public virtual void Copy() {
      if (string.IsNullOrEmpty(TextBox.SelectedText)) {
        // Clipboard.SetText throws an exception
        // if passed an empty string.
        return;
      }
      Clipboard.SetText(TextBox.SelectedText);
    }

    public virtual void Cut() {
      if (string.IsNullOrEmpty(TextBox.SelectedText)) {
        // Clipboard.SetText throws an exception
        // if passed an empty string.
        return;
      }
      Clipboard.SetText(TextBox.SelectedText);
      TextBox.SelectedText = string.Empty;
      ;
    }

    public virtual void Paste() {
      TextBox.SelectedText = Clipboard.GetText();
    }

    protected void RenderControl() {
      Button.BackColor = SystemColors.Control;
      Button.ForeColor = SystemColors.ControlText;
      Button.Width = 32;
      Button.Location = new Point(Width - 32, 0);
      Button.Dock = DockStyle.Right;
      Button.Text = "..";
      Button.GotFocus += Button_GotFocus;
      TextBox.Location = new Point(0, 0);
      TextBox.Multiline = true;
      TextBox.Dock = DockStyle.Fill;
    }

    /// <summary>
    ///   Raise the KeyDown event in the event of
    ///   a key down on the text box.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_KeyDown(object sender, KeyEventArgs e) {
      //Debug.WriteLine("TextBox_KeyDown");
      OnKeyDown(e);
    }

    private void TextBox_TextChanged(object sender, EventArgs e) {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      EditingControlValueChanged = true;
      EditingControlDataGridView.NotifyCurrentCellDirty(true);
    }
  } //End of class
} //End of namespace