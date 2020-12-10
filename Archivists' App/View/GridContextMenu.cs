using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class GridContextMenu : ContextMenuStrip {
    public GridContextMenu([NotNull] DataGridView grid) {
      Grid = grid;
      ShowImageMargin = false;
      Size = new Size(61, 4);
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
    }

    [NotNull] private DataGridView Grid { get; }

    [NotNull] protected TextBox TextBox => 
      (TextBox)Grid.EditingControl ?? 
      throw new InvalidOperationException(
        "The current grid cell's editor is not a TextBox.");

    private bool IsGridInsertionRowCurrent =>
      Grid.CurrentRow?.Index == Grid.Rows.Count - 1;
    
    [NotNull] public UndoMenuItem UndoMenuItem { get; }
    [NotNull] public CutMenuItem CutMenuItem { get; }
    [NotNull] public CopyMenuItem CopyMenuItem { get; }
    [NotNull] public DeleteMenuItem DeleteMenuItem { get; }
    [NotNull] public PasteMenuItem PasteMenuItem { get; }
    [NotNull] public SelectAllMenuItem SelectAllMenuItem { get; }
    [NotNull] public DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }

    public override ToolStripItemCollection Items {
      get {
        if (base.Items.Count == 0) {
          base.Items.AddRange(new ToolStripItem[] {
            CutMenuItem,
            CopyMenuItem,
            PasteMenuItem,
            DeleteMenuItem,
            new ToolStripSeparator(),
            SelectAllMenuItem,
            DeleteSelectedRowsMenuItem
          });
        }
        return base.Items;
      }
    }

    protected static void DeleteTextBoxSelectedText([NotNull] TextBox textBox) {
      int selectionStart = textBox.SelectionStart;
      int count = textBox.SelectionLength;
      textBox.Text = textBox.Text.Remove(selectionStart, count);
      textBox.SelectionStart = selectionStart;
    }

    protected override void OnOpening(CancelEventArgs e) {
      if (Grid.IsCurrentCellInEditMode) {
        e.Cancel = true;
      }
      CutMenuItem.Enabled = DeleteMenuItem.Enabled = SelectAllMenuItem.Enabled =
        !Grid.CurrentCell.ReadOnly &&
        Grid.CurrentCell.OwningColumn.CellTemplate is TextBoxCell &&
        Grid.CurrentCell.Value.ToString().Length > 0;
      CopyMenuItem.Enabled = Grid.CurrentCell.Value?.ToString().Length > 0;
      PasteMenuItem.Enabled = !Grid.CurrentCell.ReadOnly && Clipboard.ContainsText();
      DeleteSelectedRowsMenuItem.Enabled = !Grid.ReadOnly && !IsGridInsertionRowCurrent;
    }

    public virtual void Undo() { }

    public virtual void Cut() {
      Grid.BeginEdit(true);
      TextBox.Cut();
      Grid.EndEdit();
    }

    public virtual void Copy() {
      Clipboard.SetText(Grid.CurrentCell.Value.ToString());
    }

    public virtual void Paste() {
      Grid.BeginEdit(true);
      Grid.CurrentCell.Value = Clipboard.GetText();
      Grid.EndEdit();
    }

    public virtual void Delete() {
      Grid.BeginEdit(true);
      DeleteTextBoxSelectedText(TextBox);
      Grid.EndEdit();
    }

    public virtual void SelectAll() {
      Grid.BeginEdit(true);
    }

    public void DeleteSelectedRows() {
      if (Grid.SelectedRows.Count == 0) {
        if (Grid.CurrentRow != null) {
          Grid.CurrentRow.Selected = true;
        }
      }
      foreach (DataGridViewRow row in Grid.SelectedRows) {
        Grid.Rows.Remove(row);
      }
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