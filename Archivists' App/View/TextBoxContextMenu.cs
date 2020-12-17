using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class TextBoxContextMenu : EditContextMenuBase {
    public TextBoxContextMenu([NotNull] TextBox textBox) {
      TextBox = textBox;
    }

    private bool CanCutOrDelete => !TextBox.ReadOnly && TextBox.SelectedText.Length > 0;
    private bool CanCopy => TextBox.SelectedText.Length > 0;
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
      CutMenuItem.Enabled = DeleteMenuItem.Enabled = CanCutOrDelete;
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
      if (CanCutOrDelete) {
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
      if (CanCutOrDelete) {
        int selectionStart = TextBox.SelectionStart;
        int selectionLength = TextBox.SelectionLength;
        TextBox.Text = TextBox.Text.Remove(selectionStart, selectionLength);
        TextBox.SelectionStart = selectionStart;
      }
    }

    public override void SelectAll() {
      if (CanSelectAll) {
        TextBox.SelectAll();
      }
    }
  }
}