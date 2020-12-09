using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  public class TextBoxContextMenu : ContextMenuStrip {

    public TextBoxContextMenu([NotNull] TextBox textBox) {
      TextBox = textBox;
      UndoMenuItem = new UndoMenuItem();
      UndoMenuItem.Click += UndoMenuItem_Click;
      CutMenuItem = new CutMenuItem();
      CutMenuItem.Click += CutMenuItem_Click;
      CopyMenuItem = new CopyMenuItem();
      CopyMenuItem.Click += CopyMenuItem_Click;
      PasteMenuItem = new PasteMenuItem();
      PasteMenuItem.Click += PasteMenuItem_Click;
      DeleteMenuItem = new DeleteMenuItem();
      DeleteMenuItem.Click += DeleteMenuItem_Click;
      SelectAllMenuItem = new SelectAllMenuItem();
      SelectAllMenuItem.Click += SelectAllMenuItem_Click;
      ShowImageMargin = false;
      Size = new Size(61, 4);
    }

    private CopyMenuItem CopyMenuItem { get; }
    private CutMenuItem CutMenuItem { get; }
    private DeleteMenuItem DeleteMenuItem { get; }
    private PasteMenuItem PasteMenuItem { get; }
    private SelectAllMenuItem SelectAllMenuItem { get; }
    private UndoMenuItem UndoMenuItem { get; }
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

    private void CopyMenuItem_Click(object sender, EventArgs e) {
      TextBox.Copy();
    }

    private void CutMenuItem_Click(object sender, EventArgs e) {
      TextBox.Cut();
    }

    private void DeleteMenuItem_Click(object sender, EventArgs e) {
      int selectionStart = TextBox.SelectionStart;
      int selectionLength = TextBox.SelectionLength;
      TextBox.Text = TextBox.Text.Remove(selectionStart, selectionLength);
      TextBox.SelectionStart = selectionStart;
    }

    private void PasteMenuItem_Click(object sender, EventArgs e) {
      TextBox.Paste();
    }

    private void SelectAllMenuItem_Click(object sender, EventArgs e) {
    }

    private void UndoMenuItem_Click(object sender, EventArgs e) {
      TextBox.Undo();
    }
  }
}