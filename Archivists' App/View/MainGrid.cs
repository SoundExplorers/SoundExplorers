using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  public class MainGrid : DataGridView {
    private bool _allowDrop;
    private ContextMenuStrip _contextMenuStrip;

    public MainGrid() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      CutMenuItem = new CutMenuItem();
      CopyMenuItem = new CopyMenuItem();
      PasteMenuItem = new PasteMenuItem();
      SelectAllMenuItem = new SelectAllMenuItem();
      DeleteSelectedRowsMenuItem = new DeleteSelectedRowsMenuItem();
    }

    public override bool AllowDrop {
      get => _allowDrop ? _allowDrop : base.AllowDrop = _allowDrop = true;
      set => base.AllowDrop = value;
    }

    public new ContextMenuStrip ContextMenuStrip =>
      _contextMenuStrip ?? (_contextMenuStrip = CreateContextMenuStrip());

    public MainGridController Controller { get; set; }
    public CopyMenuItem CopyMenuItem { get; }

    private new DataGridViewRow CurrentRow =>
      base.CurrentRow ?? throw new NullReferenceException(nameof(CurrentRow));

    public CutMenuItem CutMenuItem { get; }
    public DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }
    public PasteMenuItem PasteMenuItem { get; }
    public SelectAllMenuItem SelectAllMenuItem { get; }

    [NotNull]
    private ContextMenuStrip CreateContextMenuStrip() {
      var result = new ContextMenuStrip();
      result.Items.AddRange(new ToolStripItem[] {
        CutMenuItem,
        CopyMenuItem,
        PasteMenuItem,
        SelectAllMenuItem,
        DeleteSelectedRowsMenuItem
      });
      result.ShowImageMargin = false;
      result.Size = new Size(61, 4);
      return result;
    }

    protected override void OnCellValueChanged(DataGridViewCellEventArgs e) {
      base.OnCellValueChanged(e);
      if (CurrentCell is ComboBoxCell comboBoxCell) {
        // Debug.WriteLine("MainGrid_CellValueChanged, ComboBoxCell");
        var cellValue = CurrentCell.Value;
        int rowIndex = CurrentRow.Index;
        comboBoxCell.Controller.OnCellValueChanged(rowIndex, cellValue);
      }
    }

    /// <summary>
    ///   Emulates the ComboBox's SelectedIndexChanged event.
    /// </summary>
    /// <remarks>
    ///   A known problem with <see cref="DataGridView" /> is that,
    ///   where there are multiple ComboBox columns,
    ///   ComboBox events can get spuriously raised against the ComboBoxes
    ///   in multiple cells of the row that is being edited.
    ///   So this event handler provides a workaround by
    ///   emulating a cell ComboBox's SelectedIndexChange event
    ///   but without the spurious occurrences.
    ///   The fix is based on the second answer here:
    ///   https://stackoverflow.com/questions/11141872/event-that-fires-during-MainGridcomboboxcolumn-selectedindexchanged
    /// </remarks>
    protected override void OnCurrentCellDirtyStateChanged(EventArgs e) {
      base.OnCurrentCellDirtyStateChanged(e);
      //Debug.WriteLine($"MainGrid_CurrentCellDirtyStateChanged: IsCurrentCellDirty = {MainGrid.IsCurrentCellDirty}");
      if (CurrentCell is ComboBoxCell && IsCurrentCellDirty) {
        // Debug.WriteLine(
        //   "MainGrid_CurrentCellDirtyStateChanged: ComboBoxCell, IsCurrentCellDirty");
        // This fires the cell value changed handler MainGrid_CellValueChanged.
        CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
      }
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.F2:
          if (CurrentCell != null) {
            BeginEdit(true);
          }
          break;
        case Keys.Apps: // Context menu key
        case Keys.Shift | Keys.F10:
          var cellRectangle = GetCellDisplayRectangle(
            CurrentCell.ColumnIndex,
            CurrentRow.Index, true);
          var point = new Point(cellRectangle.Right, cellRectangle.Bottom);
          ContextMenuStrip.Show(this, point);
          // When base.ContextMenuStrip was set to (new) ContextMenuStrip,
          // neither e.Handled nor e.SuppressKeyPress nor not calling base.OnKeyDown
          // stopped the context menu from being shown a second time
          // immediately afterwards in the default wrong position.
          // The solution is not to set
          // base.ContextMenuStrip to (new) GridContextMenuStrip and instead to
          // show the context menu in grid event handlers:
          // here when shown with one of the two standard keyboard shortcuts:
          // EditorView.Grid_MouseDown when shown with a right mouse click.
          e.Handled = e.SuppressKeyPress = true;
          break;
        default:
          base.OnKeyDown(e);
          break;
      } //End of switch
    }

    /// <summary>
    ///   The initial state of the row will be saved to a detached row
    ///   to allow comparison with any changes if the row gets edited.
    /// </summary>
    /// <remarks>
    ///   If the row represents an Image whose Path
    ///   specifies a valid image file,
    ///   the image will be shown below the main grid.
    ///   If the row represents an Image whose Path
    ///   does not specifies a valid image file,
    ///   a Missing Image label containing an appropriate message will be displayed.
    /// </remarks>
    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      base.OnRowEnter(e);
      // This is the safe way of checking whether we have entered the insertion (new) row:
      //if (e.RowIndex == MainGrid.RowCount - 1) {
      //   // Controller.OnEnteringInsertionRow();
      //   // // if (Entities is ImageList) {
      //   // //   ShowImageOrMessage(null);
      //   // // }
      //}
      Controller.OnRowEnter(e.RowIndex);
    }
  }
}