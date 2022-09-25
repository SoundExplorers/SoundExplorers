using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundExplorers.View;

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

  public override void Cut() {
    if (Grid.CanCut) {
      if (BeginCellEdit()) {
        Grid.BeginInvoke(() => {
          TextBox.Cut();
          Grid.EndEdit();
        });
      } else {
        TextBox.Cut();
      }
    }
  }

  public override void Copy() {
    if (Grid.CanCopy) {
      Clipboard.SetText(Grid.CopyableText);
    }
  }

  public override void Paste() {
    if (Grid.CanPaste) {
      if (Grid.IsTextBoxCellCurrent) {
        if (BeginCellEdit()) {
          Grid.BeginInvoke(() => {
            TextBox.SelectedText = Clipboard.GetText();
            Grid.EndEdit();
          });
        } else {
          TextBox.SelectedText = Clipboard.GetText();
        }
      } else {
        Grid.CurrentCell.Value = Clipboard.GetText();
      }
    }
  }

  public override void Delete() {
    if (Grid.CanDelete) {
      if (BeginCellEdit()) {
        Grid.BeginInvoke(() => {
          DeleteTextBoxSelectedText(TextBox);
          Grid.EndEdit();
        });
      } else {
        DeleteTextBoxSelectedText(TextBox);
      }
    }
  }

  public override void SelectAll() {
    if (Grid.CanSelectAll) {
      if (BeginCellEdit()) {
        Grid.BeginInvoke(() => {
          TextBox.SelectAll();
        });
      } else {
        TextBox.SelectAll();
      }
    }
  }

  public override void SelectRow() {
    if (Grid.CanSelectRow) {
      Grid.CurrentRow!.Selected = true;
    }
  }

  public override void DeleteSelectedRows() {
    if (Grid.CanDeleteSelectedRows) {
      foreach (DataGridViewRow row in Grid.SelectedRows) {
        Grid.Rows.Remove(row);
      }
    }
  }

  /// <summary>
  ///   The grid context menu does not do edits on TextBox cells that are already in
  ///   edit mode.  But the Edit menu on the main window menu bar,
  ///   which used the same commands, does, as it combines the functions of the
  ///   grid context menu and the TextBox context menu.
  ///   So we need to allow for the cell either already being in edit mode or not.
  /// </summary>
  private bool BeginCellEdit() {
    if (Grid.IsCurrentCellInEditMode) {
     return false;
    }
    Grid.BeginEdit(true);
    return true;
  }
}