using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SoundExplorers.View {
  internal class EditMenu : ToolStripMenuItem, IGridMenu {
    
    public EditMenu() {
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
      DropDown.Opening += DropDown_Opening;
    }

    private void DropDown_Opening(object sender, CancelEventArgs e) {
      if (MainView.MdiChildren.Any()) {
        Grid.EnableMenuItems(this);
      } else {
        foreach (ToolStripItem menuItem in DropDownItems) {
          menuItem.Enabled = false;
        }
      }
    }

    private GridBase Grid => MainView.EditorView.FocusedGrid;     
    public CutMenuItem CutMenuItem { get; }
    public CopyMenuItem CopyMenuItem { get; }
    public DeleteMenuItem DeleteMenuItem { get; }
    public PasteMenuItem PasteMenuItem { get; }
    public SelectAllMenuItem SelectAllMenuItem { get; }
    public DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }
    public MainView MainView { get; set; }

    public void CutMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.Cut();
    }

    public void CopyMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.Copy();
    }

    public void PasteMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.Paste();
    }

    private void DeleteMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.Delete();
    }

    private void SelectAllMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.SelectAll();
    }

    private void DeleteSelectedRowsMenuItem_Click(object sender, EventArgs e) {
      Grid.ContextMenu.DeleteSelectedRows();
    }
  }
}