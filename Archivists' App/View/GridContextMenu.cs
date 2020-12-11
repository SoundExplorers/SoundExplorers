using System;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class GridContextMenu : EditContextMenuBase, IGridMenu {
    public GridContextMenu([NotNull] GridBase grid) {
      Grid = grid;
    }

    [NotNull]
    private TextBox TextBox =>
      (TextBox)Grid.EditingControl ??
      throw new InvalidOperationException(
        "The current grid cell's editor is not a TextBox.");

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

    private GridBase Grid { get; }

    protected override void OnOpening(CancelEventArgs e) {
      if (Grid.IsCurrentCellInEditMode) {
        // A context menu specific to the cell editor type
        // will be shown instead if available:
        // currently this only applies to TextBoxCells.
        e.Cancel = true;
      }
      Grid.EnableMenuItems(this);
    }

    /// <summary>
    ///   The grid context menu does not do edits on TextBox cells that are already in
    ///   edit mode.  But the Edit menu on the main window menu,
    ///   which used the same commands, does, as it combines the functions of the
    ///   grid context menu and the TextBox context menu.
    ///   So we need to allow for the cell either already being in edit mode or not.
    /// </summary>
    private void DoCellEdit([NotNull] Action action) {
      bool isAlreadyInEditMode = Grid.IsCurrentCellInEditMode;
      if (!isAlreadyInEditMode) {
        Grid.BeginEdit(true);
      }
      action.Invoke();
      if (!isAlreadyInEditMode) {
        Grid.EndEdit();
      }
    }

    public override void Cut() {
      DoCellEdit(TextBox.Cut);
    }

    public override void Copy() {
      Clipboard.SetText(Grid.CopyableText);
    }

    public override void Paste() {
      DoCellEdit(() => Grid.CurrentCell.Value = Clipboard.GetText());
    }

    public override void Delete() {
      DoCellEdit(() => DeleteTextBoxSelectedText(TextBox));
    }

    public override void SelectAll() {
      if (Grid.IsCurrentCellInEditMode) {
        DoCellEdit(TextBox.SelectAll);
      } else {
        Grid.BeginEdit(true);
      }
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