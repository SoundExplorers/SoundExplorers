using System.ComponentModel;
using System.Windows.Forms;

namespace SoundExplorers.View {
  internal class TextBoxContextMenu : EditContextMenuBase {
    public TextBoxContextMenu(TextBox textBox) {
      TextBox = textBox;
      TextBox.MouseDown += TextBox_MouseDown;
    }

    private bool CanCut => !TextBox.ReadOnly && TextBox.SelectedText.Length > 0;
    private bool CanCopy => TextBox.SelectedText.Length > 0;

    private bool CanDelete => 
      !TextBox.ReadOnly && 
      (TextBox.SelectedText.Length > 0 || TextBox.SelectionStart < TextBox.Text.Length);

    private bool CanPaste => !TextBox.ReadOnly && Clipboard.ContainsText();
    private bool CanSelectAll => TextBox.Text.Length > 0;
    private TextBox TextBox { get; }

    public override ToolStripItemCollection Items {
      get {
        if (base.Items.Count == 0) {
          base.Items.AddRange(new ToolStripItem[] {
            UndoMenuItem,
            new ToolStripSeparator(),
            CutMenuItem,
            CopyMenuItem,
            PasteMenuItem,
            DeleteMenuItem,
            new ToolStripSeparator(),
            SelectAllMenuItem
          });
        }
        return base.Items;
      }
    }

    protected override void OnOpening(CancelEventArgs e) {
      base.OnOpening(e);
      UndoMenuItem.Enabled = TextBox.CanUndo;
      CutMenuItem.Enabled = CanCut;
      DeleteMenuItem.Enabled = CanDelete;
      CopyMenuItem.Enabled = CanCopy;
      PasteMenuItem.Enabled = CanPaste;
      SelectAllMenuItem.Enabled = CanSelectAll;
    }

    protected override void Undo() {
      if (TextBox.CanUndo) {
        TextBox.Undo();
      }
    }

    public override void Cut() {
      if (CanCut) {
        TextBox.Cut();
      }
    }

    public override void Copy() {
      if (CanCopy) {
        TextBox.Copy();
      }
    }

    public override void Paste() {
      if (CanPaste) {
        TextBox.Paste();
      }
    }

    public override void Delete() {
      if (CanDelete) {
        int selectionStart = TextBox.SelectionStart;
        int selectionLength = TextBox.SelectionLength;
        TextBox.Text = TextBox.Text.Remove(
          selectionStart, 
          selectionLength > 0 ? selectionLength : 1);
        TextBox.SelectionStart = selectionStart;
      }
    }

    public override void SelectAll() {
      if (CanSelectAll) {
        TextBox.SelectAll();
      }
    }

    /// <summary>
    ///   Allows right-clicking the TextBox to focus it and then show its context menu.
    /// </summary>
    private void TextBox_MouseDown(object? sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right && !TextBox.Focused) {
        TextBox.Focus();
      }
    }
  }
}