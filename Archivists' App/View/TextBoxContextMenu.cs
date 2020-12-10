using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class TextBoxContextMenu : EditContextMenuBase {

    public TextBoxContextMenu([NotNull] TextBox textBox) {
      TextBox = textBox;
    }
    
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
      if (TextBox.SelectedText.Length == 0) {
        CutMenuItem.Enabled = false;
        CopyMenuItem.Enabled = false;
        DeleteMenuItem.Enabled = false;
      } else {
        CutMenuItem.Enabled = true;
        CopyMenuItem.Enabled = true;
        DeleteMenuItem.Enabled = true;
      }
      PasteMenuItem.Enabled = Clipboard.ContainsText();
      SelectAllMenuItem.Enabled = TextBox.Text.Length > 0;
    }

    protected override void Undo() {
      TextBox.Undo();
    }

    public override void Cut() {
      TextBox.Cut();
    }

    public override void Copy() {
      TextBox.Copy();
    }

    public override void Paste() {
      TextBox.Paste();
    }

    public override void Delete() {
      int selectionStart = TextBox.SelectionStart;
      int selectionLength = TextBox.SelectionLength;
      TextBox.Text = TextBox.Text.Remove(selectionStart, selectionLength);
      TextBox.SelectionStart = selectionStart;
    }

    public override void SelectAll() {
      TextBox.SelectAll();
    }
  }
}