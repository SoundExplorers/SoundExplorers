using System;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal class GridContextMenu : EditContextMenuBase {
    public GridContextMenu([NotNull] GridBase grid) {
      Grid = grid;
    }
    
    private TextBox TextBox =>
      (Grid as MainGrid)?.TextBox ??
      throw new InvalidOperationException(
        "ParentGrid does not support the TextBox property.");

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
      (Grid.CurrentRow ?? throw new NullReferenceException()).Selected = true;
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