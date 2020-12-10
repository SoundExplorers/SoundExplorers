using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class CellTextBoxContextMenu : GridContextMenu {
    public CellTextBoxContextMenu([NotNull] DataGridView grid) : base(grid) {
    }

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

    public override void Undo() {
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
      DeleteTextBoxSelectedText(TextBox);
    }

    public override void SelectAll() {
      TextBox.SelectAll();
    }
  }
}