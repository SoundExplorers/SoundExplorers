using System.ComponentModel;
using System.Windows.Forms;

namespace SoundExplorers.View {
  internal class GridContextMenu : EditContextMenuBase {
    public GridContextMenu(GridBase grid) {
      Grid = grid;
    }

    private TextBox TextBox => (Grid as MainGrid)?.TextBox!;

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
            SelectRowMenuItem,
            DeleteSelectedRowsMenuItem
          });
        }
        return base.Items;
      }
    }

    private GridBase Grid { get; }
    private bool IsAlreadyInEditMode { get; set; }

    protected override void OnOpening(CancelEventArgs e) {
      if (Grid.IsCurrentCellInEditMode) {
        // A context menu specific to the cell editor type
        // will be shown instead if available:
        // currently this only applies to TextBoxCells.
        e.Cancel = true; // Stops the context menu from being shown.
      }
      Grid.EnableOrDisableMenuItems(CutMenuItem, CopyMenuItem, PasteMenuItem,
        DeleteMenuItem,
        SelectAllMenuItem, SelectRowMenuItem, DeleteSelectedRowsMenuItem);
    }

    /// <summary>
    ///   The grid context menu does not do edits on TextBox cells that are already in
    ///   edit mode.  But the Edit menu on the main window menu bar,
    ///   which used the same commands, does, as it combines the functions of the
    ///   grid context menu and the TextBox context menu.
    ///   So we need to allow for the cell either already being in edit mode or not.
    /// </summary>
    /// <remarks>
    ///   For unknown reason, this does not work any more: the TexBox EditorControl
    ///   can no longer be found, which crashes the program. So, for now, to stop the
    ///   program from crashing, <see cref="GridBase.CanPaste" /> and
    ///   <see cref="GridBase.CanSelectAll" /> are overridden in <see cref="MainGrid"/>
    ///   to disable editing a main grid TextBox cell unless it is already in edit
    ///   mode, with the Cut, Paste, Delete and Select All items disabled on both the
    ///   edit menu on the menu bar and the context menu.
    /// </remarks>
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
      if (!Grid.CanCut) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      BeginCellEditIfRequired();
      TextBox.Cut();
      EndCellEditIfRequired();
    }

    public override void Copy() {
      if (!Grid.CanCopy) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      Clipboard.SetText(Grid.CopyableText);
    }

    public override void Paste() {
      if (!Grid.CanPaste) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      if (Grid.IsTextBoxCellCurrent) {
        BeginCellEditIfRequired();
        TextBox.SelectedText = Clipboard.GetText();
        EndCellEditIfRequired();
      } else {
        Grid.CurrentCell.Value = Clipboard.GetText();
      }
    }

    public override void Delete() {
      if (!Grid.CanDelete) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      BeginCellEditIfRequired();
      DeleteTextBoxSelectedText(TextBox);
      EndCellEditIfRequired();
    }

    public override void SelectAll() {
      if (!Grid.CanSelectAll) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      BeginCellEditIfRequired();
      TextBox.SelectAll();
      EndCellEditIfRequired();
    }

    public override void SelectRow() {
      if (!Grid.CanSelectRow) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      Grid.CurrentRow!.Selected = true;
    }

    public override void DeleteSelectedRows() {
      if (!Grid.CanDeleteSelectedRows) {
        // Prevents use of keyboard shortcut outside valid context. 
        return;
      }
      foreach (DataGridViewRow row in Grid.SelectedRows) {
        Grid.Rows.Remove(row);
      }
    }
  }
}