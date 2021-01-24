using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal abstract class GridBase : DataGridView, IGrid {
    private GridContextMenu? _contextMenu;

    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      Margin = new Padding(4);
      RowHeadersWidth = 51;
      // Fixes a .Net 5 problem where, when the user cancelled (by pressing Esc) an edit
      // of a text cell on the insertion row after typing text, the program would crash.
      ShowCellErrors = false;
      ShowCellToolTips = false; // Before .Net 5, tooltips were off by default.
      CellColorScheme = new GridCellColorScheme(this);
    }

    public bool CanCut => CanSelectAll && CopyableText.Length > 0;
    public bool CanCopy => CopyableText.Length > 0;
    public bool CanPaste => !CurrentCell.ReadOnly && Clipboard.ContainsText();
    public bool CanDelete => CanCut;
    public bool CanSelectAll => !CurrentCell.ReadOnly && IsTextBoxCellCurrent;
    public bool CanSelectRow => CurrentRow != null;

    public bool CanDeleteSelectedRows =>
      !ReadOnly && !IsCurrentCellInEditMode && SelectedRows.Count > 0 &&
      !SelectedRows.Contains(NewRow);

    protected GridControllerBase Controller { get; set; } = null!;
    protected int FirstVisibleColumnIndex { get; set; }

    public GridContextMenu ContextMenu =>
      _contextMenu ??= new GridContextMenu(this);

    public EditorView EditorView { get; set; } = null!;

    public bool IsTextBoxCellCurrent =>
      CurrentCell?.OwningColumn.CellTemplate is TextBoxCell;

    public MainView MainView { get; set; } = null!;

    /// <summary>
    ///   Gets the new (i.e. empty) row at the bottom of an editable grid.
    /// </summary>
    private DataGridViewRow NewRow => !ReadOnly
      ? Rows[^1]
      : throw new InvalidOperationException(
        "A read-only grid does not contain a new row.");

    /// <summary>
    ///   Gets the cell text that can be copied to the clipboard or, if there is none,
    ///   an empty string.
    /// </summary>
    public string CopyableText {
      get {
        if (IsTextBoxCellCurrent && IsCurrentCellInEditMode) {
          return ((TextBox)EditingControl).SelectedText;
        }
        return CurrentCell.Value?.ToString() ?? string.Empty;
      }
    }

    public int CurrentRowIndex => CurrentRow?.Index ?? -1;
    public IGridCellColorScheme CellColorScheme { get; }
    bool IGrid.Focused => Focused;

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [ExcludeFromCodeCoverage]
    string IGrid.Name => Name;

    int IGrid.RowCount => RowCount;

    /// <summary>
    ///   Makes the specified row current, which will set focus and raise
    ///   <see cref="OnRowEnter" />.
    /// </summary>
    public void MakeRowCurrent(int rowIndex, bool async = false) {
      Debug.WriteLine($"GridBase.MakeRowCurrent {Name}: row {rowIndex}");
      if (async) {
        BeginInvoke((Action)delegate { MakeRowCurrentAsync(rowIndex); });
      } else {
        CurrentCell = Rows[rowIndex].Cells[FirstVisibleColumnIndex];
      }
    }

    public virtual void OnPopulated() {
      Debug.WriteLine($"GridBase.OnPopulated {Name}");
      BeginInvoke((Action)Controller.OnPopulatedAsync);
    }

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities that will populate the grid.
    ///   If null, the default, all entities of the class's entity type will be fetched
    ///   from the database.
    /// </param>
    public virtual void Populate(IList? list = null) {
      Debug.WriteLine($"GridBase.Populate {Name}");
      Controller.Populate(list);
      DataSource = Controller.BindingList;
      ConfigureColumns();
      AutoResizeColumns();
    }

    /// <summary>
    ///   Focuses the grid.
    /// </summary>
    /// <remarks>
    ///   Where two grids are shown, their colour schemes are swapped round, indicating
    ///   which is now the current grid by having the usual colour scheme inverted on the
    ///   other grid.
    /// </remarks>
    public virtual void SetFocus() {
      Debug.WriteLine($"GridBase.SetFocus {Name}");
      Controller.IsFocusingProgramatically = true;
      Controller.PrepareForFocus();
      if (!EditorView.Controller.IsParentGridToBeShown) {
        Focus();
        return;
      }
      // A read-only related grid for the parent table is shown above the main grid.
      var unfocusedGrid = GetOtherGrid();
      // By trial an error, I found that this complicated rigmarole was required to
      // properly shift the focus programatically, i.e. in EditorView_KeyDown to
      // implement doing it with the F6 key.
      unfocusedGrid.Enabled = false;
      Enabled = true;
      Refresh();
      Focus();
      Refresh();
      unfocusedGrid.Enabled = true;
    }

    /// <summary>
    ///   Enables or disables the menu items of the grid's context menu or the
    ///   corresponding items of the Edit menu on the main window menu bar.
    /// </summary>
    public void EnableOrDisableMenuItems(
      ToolStripMenuItem cutMenuItem,
      ToolStripMenuItem copyMenuItem,
      ToolStripMenuItem pasteMenuItem,
      ToolStripMenuItem deleteMenuItem,
      ToolStripMenuItem selectAllMenuItem,
      ToolStripMenuItem selectRowMenuItem,
      ToolStripMenuItem deleteSelectedRowsMenuItem) {
      selectAllMenuItem.Enabled = CanSelectAll;
      cutMenuItem.Enabled = CanCut;
      deleteMenuItem.Enabled = CanDelete;
      copyMenuItem.Enabled = CanCopy;
      pasteMenuItem.Enabled = CanPaste;
      selectRowMenuItem.Enabled = CanSelectRow;
      deleteSelectedRowsMenuItem.Enabled = CanDeleteSelectedRows;
    }

    protected virtual void ConfigureColumn(DataGridViewColumn column) {
      // Making every column explicitly not sortable prevents the program
      // from crashing if F3 in pressed while the grid is focused.
      column.SortMode = DataGridViewColumnSortMode.NotSortable;
      column.HeaderText = Controller.GetColumnDisplayName(column.Name);
      if (column.ValueType == typeof(DateTime)) {
        column.DefaultCellStyle.Format = EditorController.DateFormat;
      }
    }

    private void ConfigureColumns() {
      foreach (DataGridViewColumn column in Columns) {
        ConfigureColumn(column);
      }
    }

    /// <summary>
    ///   Returns the cell that is at the specified client co-ordinates of the main grid.
    ///   Null if there is no cell at the coordinates.
    /// </summary>
    /// <param name="x">
    ///   The x co-ordinate relative to the main grid's client rectangle.
    /// </param>
    /// <param name="y">
    ///   The y co-ordinate relative to the main grid's client rectangle.
    /// </param>
    private DataGridViewCell? GetCellAtClientCoOrdinates(int x, int y) {
      var hitTestInfo = HitTest(x, y);
      if (hitTestInfo.Type == DataGridViewHitTestType.Cell) {
        return Rows[
          hitTestInfo.RowIndex].Cells[
          hitTestInfo.ColumnIndex];
      }
      return null;
    }
    private GridBase GetOtherGrid() {
      return (GridBase)Controller.GetOtherGrid();
    }

    private void MakeRowCurrentAsync(int rowIndex) {
      Debug.WriteLine($"GridBase.MakeRowCurrentAsync {Name}: row {rowIndex}");
      MakeRowCurrent(rowIndex);
    }

    protected override void OnCurrentCellChanged(EventArgs e) {
      base.OnCurrentCellChanged(e);
      if (CurrentCell != null) {
        MainView.CopyToolStripButton.Enabled = CanCopy;
      }
    }

    protected override void OnGotFocus(EventArgs e) {
      Debug.WriteLine($"GridBase.OnGotFocus {Name}");
      base.OnGotFocus(e);
      EditorView.CurrentGrid = this;
      Controller.OnGotFocus();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      switch (e.KeyData) {
        case Keys.Apps: // Context menu key
        case Keys.Shift | Keys.F10: // Alternate context menu keyboard shortcut
          var cellRectangle = GetCellDisplayRectangle(
            CurrentCell.ColumnIndex,
            CurrentRow!.Index, true);
          var point = new Point(cellRectangle.Right, cellRectangle.Bottom);
          ContextMenu.Show(this, point);
          // When base.ContextMenuStrip was set to ContextMenu, neither e.Handled nor
          // e.SuppressKeyPress nor not calling base.OnKeyDown stopped the context menu
          // from being shown a second time immediately afterwards in the default wrong
          // position. The solution is not to set base.ContextMenuStrip to ContextMenu
          // and instead to show the context menu in grid event handlers: here when shown
          // with one of the two standard keyboard shortcuts; EditorView.Grid_MouseDown
          // when shown with a right mouse click.
          e.Handled = e.SuppressKeyPress = true;
          break;
        default:
          base.OnKeyDown(e);
          break;
      } //End of switch
    }

    /// <summary>
    ///   When either mouse button is clicked, the grid will be focused if it is not
    ///   already. When mouse button 2 is clicked on a cell, the cell will become the
    ///   current cell and, unless the cell is already being edited, the context menu
    ///   will be shown.
    /// </summary>
    /// <remarks>
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
    ///   I tried to initiate a drag-and-drop operation when a cell is clicked with the
    ///   mouse button 1. But it did not work, possibly because it puts the cell into
    ///   edit mode and also, when dragged, selects multiple rows. Perhaps we could
    ///   initiate a drag-and-drop operation on Control + mouse button 1.
    /// </remarks>
    protected override void OnMouseDown(MouseEventArgs e) {
      if (this != EditorView.CurrentGrid) {
        // When focusing the grid with the left mouse button, this is not always
        // executed. It can be if the user has just been navigating the other grid with
        // the mouse.
        Debug.WriteLine("======================================================");
        Debug.WriteLine(
          $"GridBase.OnMouseDown {Name}: focusing grid with {e.Button} mouse button");
        Debug.WriteLine("======================================================");
        SetFocus();
      }
      if (e.Button == MouseButtons.Right) {
        // Find the cell, if any, that mouse button 2 clicked.
        var cell = GetCellAtClientCoOrdinates(e.X, e.Y);
        if (cell != null) { // Cell found
          CurrentCell = cell;
          if (!IsCurrentCellInEditMode) {
            ContextMenu.Show(this, e.Location);
          }
        }
      }
      base.OnMouseDown(e);
    }

    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      Debug.WriteLine($"GridBase.OnRowEnter {Name}: row {e.RowIndex}");
      base.OnRowEnter(e);
      Controller.OnRowEnter(e.RowIndex);
    }

    protected override void WndProc(ref Message m) {
      // ReSharper disable once InconsistentNaming
      // ReSharper disable once IdentifierTypo
      const int WM_SETFOCUS = 0x0007;
      if (m.Msg == WM_SETFOCUS) {
        // ReSharper disable once StringLiteralTypo
        Debug.WriteLine($"GridBase.WndProc {Name}: WM_SETFOCUS");
        // We need to make sure we can switch the grid cell colour schemes when switching
        // focus between two grids.  To get this to work even when focus is changed by a
        // mouse click, we would ideally like to override the SetFocus method.
        // As that method does not support overriding, we intercept the corresponding
        // Windows message here instead, which has the same effect.
        Controller.OnFocusing();
      }
      base.WndProc(ref m);
    }
  }
}