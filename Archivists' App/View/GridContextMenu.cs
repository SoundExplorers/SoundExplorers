using System;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class GridContextMenu : EditContextMenuBase {
    public GridContextMenu([NotNull] DataGridView grid) {
      Grid = grid;
    }

    [NotNull] private DataGridView Grid { get; }

    [NotNull]
    private TextBox TextBox =>
      (TextBox)Grid.EditingControl ??
      throw new InvalidOperationException(
        "The current grid cell's editor is not a TextBox.");

    private bool IsGridInsertionRowCurrent =>
      Grid.CurrentRow?.Index == Grid.Rows.Count - 1;

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

    public override void Cut() {
      Grid.BeginEdit(true);
      TextBox.Cut();
      Grid.EndEdit();
    }

    public override void Copy() {
      Clipboard.SetText(Grid.CurrentCell.Value.ToString());
    }

    public override void Paste() {
      Grid.BeginEdit(true);
      Grid.CurrentCell.Value = Clipboard.GetText();
      Grid.EndEdit();
    }

    public override void Delete() {
      Grid.BeginEdit(true);
      DeleteTextBoxSelectedText(TextBox);
      Grid.EndEdit();
    }

    public override void SelectAll() {
      Grid.BeginEdit(true);
    }

    public override void DeleteSelectedRows() {
      if (Grid.SelectedRows.Count == 0) {
        if (Grid.CurrentRow != null) {
          Grid.CurrentRow.Selected = true;
        }
      }
      foreach (DataGridViewRow row in Grid.SelectedRows) {
        Grid.Rows.Remove(row);
      }
    }
  }
}