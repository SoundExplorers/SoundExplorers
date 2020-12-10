using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal abstract class EditContextMenuBase : ContextMenuStrip {
    protected EditContextMenuBase() {
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
      DeleteSelectedRowsMenuItem = new DeleteSelectedRowsMenuItem();
      DeleteSelectedRowsMenuItem.Click += DeleteSelectedRowsMenuItem_Click;
      // The menu properties must be set after creating the menu items,
      // as their setters call the Items getter (implemented in derived classes),
      // which needs all the menu items to have been created.
      Size = new Size(61, 4);
      ShowImageMargin = false;
    }
    
    [NotNull] protected UndoMenuItem UndoMenuItem { get; }
    [NotNull] public CutMenuItem CutMenuItem { get; }
    [NotNull] protected CopyMenuItem CopyMenuItem { get; }
    [NotNull] protected DeleteMenuItem DeleteMenuItem { get; }
    [NotNull] public PasteMenuItem PasteMenuItem { get; }
    [NotNull] public SelectAllMenuItem SelectAllMenuItem { get; }
    [NotNull] public DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }

    protected static void DeleteTextBoxSelectedText([NotNull] TextBox textBox) {
      int selectionStart = textBox.SelectionStart;
      int count = textBox.SelectionLength;
      textBox.Text = textBox.Text.Remove(selectionStart, count);
      textBox.SelectionStart = selectionStart;
    }

    protected virtual void Undo() {
      throw new NotSupportedException();
    }

    public abstract void Cut();

    public abstract void Copy();

    public abstract void Paste();

    public abstract void Delete();

    public abstract void SelectAll();

    public virtual void DeleteSelectedRows() {
      throw new NotSupportedException();
    }

    private void UndoMenuItem_Click(object sender, EventArgs e) {
      Undo();
    }

    private void CutMenuItem_Click(object sender, EventArgs e) {
      Cut();
    }

    private void CopyMenuItem_Click(object sender, EventArgs e) {
      Copy();
    }

    private void PasteMenuItem_Click(object sender, EventArgs e) {
      Paste();
    }

    private void DeleteMenuItem_Click(object sender, EventArgs e) {
      Delete();
    }

    private void SelectAllMenuItem_Click(object sender, EventArgs e) {
      SelectAll();
    }

    private void DeleteSelectedRowsMenuItem_Click(object sender, EventArgs e) {
      DeleteSelectedRows();
    }
  }
}