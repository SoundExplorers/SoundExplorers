using System;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class GridContextMenu : EditContextMenuBase {
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
      Grid.EnableMenuItems(CutMenuItem, CopyMenuItem, PasteMenuItem, DeleteMenuItem,
        SelectAllMenuItem, DeleteSelectedRowsMenuItem);
    }

    private bool IsAlreadyInEditMode { get; set; }

    /// <summary>
    ///   The grid context menu does not do edits on TextBox cells that are already in
    ///   edit mode.  But the Edit menu on the main window menu,
    ///   which used the same commands, does, as it combines the functions of the
    ///   grid context menu and the TextBox context menu.
    ///   So we need to allow for the cell either already being in edit mode or not.
    /// </summary>
    private void BeginCellEditIfRequired() {
      IsAlreadyInEditMode = Grid.IsCurrentCellInEditMode;
      if (!IsAlreadyInEditMode) {
        Grid.BeginEdit(true);
      }
    }
    
    private void EndCellEditIfRequired() {
      if (!IsAlreadyInEditMode) {
        Grid.EndEdit();
      }
    }

    public override void Cut() {
      BeginCellEditIfRequired();
      TextBox.Cut();
      EndCellEditIfRequired();
    }

    public override void Copy() {
      Clipboard.SetText(Grid.CopyableText);
    }

    public override void Paste() {
      if (Grid.IsTextBoxCellCurrent) {
        BeginCellEditIfRequired();
        TextBox.SelectedText = Clipboard.GetText();
        EndCellEditIfRequired();
      } else {
        Grid.CurrentCell.Value = Clipboard.GetText();
      }
    }

    public override void Delete() {
      BeginCellEditIfRequired();
      DeleteTextBoxSelectedText(TextBox);
      EndCellEditIfRequired();
    }

    public override void SelectAll() {
      BeginCellEditIfRequired();
      TextBox.SelectAll();
      EndCellEditIfRequired();
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