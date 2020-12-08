using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  public class TextBoxContextMenu : ContextMenuStrip {

    public TextBoxContextMenu([NotNull] MainView mainView, [NotNull] TextBox textBox) {
      TextBox = textBox;
      UndoMenuItem = new UndoMenuItem();
      UndoMenuItem.Click += UndoMenuItem_Click;
      CutMenuItem = new CutMenuItem();
      CutMenuItem.Click += mainView.EditCutMenuItem_Click;
      CopyMenuItem = new CopyMenuItem();
      CopyMenuItem.Click += mainView.EditCopyMenuItem_Click;
      PasteMenuItem = new PasteMenuItem();
      PasteMenuItem.Click += mainView.EditPasteMenuItem_Click;
      DeleteMenuItem = new DeleteMenuItem();
      DeleteMenuItem.Click += DeleteMenuItem_Click;
      SelectAllMenuItem = new SelectAllMenuItem();
      SelectAllMenuItem.Click += mainView.EditSelectAllMenuItem_Click;
      ShowImageMargin = false;
      Size = new Size(61, 4);
    }

    internal CopyMenuItem CopyMenuItem { get; }
    internal CutMenuItem CutMenuItem { get; }
    internal DeleteMenuItem DeleteMenuItem { get; }
    internal PasteMenuItem PasteMenuItem { get; }
    internal SelectAllMenuItem SelectAllMenuItem { get; }
    internal UndoMenuItem UndoMenuItem { get; }
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

    private void DeleteMenuItem_Click(object sender, EventArgs e) {
      int selectionStart = TextBox.SelectionStart;
      int selectionLength = TextBox.SelectionLength;
      TextBox.Text = TextBox.Text.Remove(selectionStart, selectionLength);
      TextBox.SelectionStart = selectionStart;
    }

    private void UndoMenuItem_Click(object sender, EventArgs e) {
      if (TextBox.CanUndo) {
        TextBox.Undo();
      }
    }
  }
}