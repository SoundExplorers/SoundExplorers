using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using SoundExplorers.Common;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal abstract class GridBase : DataGridView, IGrid {
    private GridContextMenu? _contextMenu;

    protected GridBase() {
      AllowUserToOrderColumns = true;
      ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      Margin = new Padding(4);
      RowHeadersWidth = 51;
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

    bool IGrid.Enabled {
      get => Enabled;
      set => Enabled = value;
    }

    bool IGrid.Focused => Focused;

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [ExcludeFromCodeCoverage]
    string IGrid.Name => Name;

    int IGrid.RowCount => RowCount;

    void IGrid.BeginInvoke(Action action) {
      Debug.WriteLine($"GridBase.BeginInvoke {Name}");
      BeginInvoke(action);
    }

    void IGrid.Focus() {
      Debug.WriteLine($"GridBase.Focus {Name}");
      Focus();
    }

    /// <summary>
    ///   Makes the specified row current, which will set focus and raise
    ///   <see cref="OnRowEnter" />. If the new row index is specified, the insertion
    ///   binding item will be added (unless the new row is already current).
    /// </summary>
    public void MakeRowCurrent(int rowIndex, bool async = false) {
      Debug.WriteLine($"GridBase.MakeRowCurrent {Name}: row {rowIndex}");
      if (async) {
        BeginInvoke((Action)delegate { MakeRowCurrentAsync(rowIndex); });
      } else {
        CurrentCell = Rows[rowIndex].Cells[Controller.FirstVisibleColumnIndex];
      }
    }

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    public void Populate() {
      Debug.WriteLine($"GridBase.Populate {Name}");
      Controller.Populate();
      if (ColumnCount == 0) {
        AddColumns();
      }
      DataSource = Controller.BindingList;
      AutoResizeColumns();
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

    private void AddColumns() {
      foreach (var bindingColumn in Controller.BindingColumns) {
        AddColumn(bindingColumn);
      }
    }

    protected virtual DataGridViewColumn AddColumn(IBindingColumn bindingColumn) {
      DataGridViewColumn result = bindingColumn.ValueType == typeof(bool)
        ? new DataGridViewCheckBoxColumn {FlatStyle = FlatStyle.Flat}
        : new DataGridViewTextBoxColumn();
      {
        result.DataPropertyName = bindingColumn.PropertyName;
        result.HeaderText = bindingColumn.DisplayName;
        result.Name = bindingColumn.PropertyName;
        // Making every column explicitly not sortable prevents the program from crashing
        // if F3 in pressed while the grid is focused.
        result.SortMode = DataGridViewColumnSortMode.NotSortable;
        result.ValueType = bindingColumn.ValueType;
      }
      if (result.ValueType == typeof(DateTime)) {
        result.DefaultCellStyle.Format = EditorController.DateFormat;
      }
      Columns.Add(result);
      return result;
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
    ///   When the mouse is right-clicked, the grid will be focused. When mouse is
    ///   right-clicked on a cell, the cell will become the current cell and, unless the
    ///   cell is already being edited, the grid context menu will be shown. (Showing
    ///   the context menu for a TextBox cell's embedded TextBox when in edit mode is
    ///   done elsewhere.)
    /// </summary>
    /// <remarks>
    ///   THE FOLLOWING RELATES TO A FEATURE THAT IS NOT YET IN USE BUT MAY BE LATER:
    ///   I tried to initiate a drag-and-drop operation when a cell is clicked with the
    ///   mouse button 1. But it did not work, possibly because it puts the cell into
    ///   edit mode and also, when dragged, selects multiple rows. Perhaps we could
    ///   initiate a drag-and-drop operation on Control + mouse button 1.
    /// </remarks>
    protected override void OnMouseDown(MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        if (this != EditorView.CurrentGrid) {
          Debug.WriteLine("======================================================");
          Debug.WriteLine(
            $"GridBase.OnMouseDown {Name}: focusing grid with right mouse button");
          Debug.WriteLine("======================================================");
          Focus();
        }
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
      if (m.Msg == (int)WindowsMessage.WM_SETFOCUS) {
        // ReSharper disable once StringLiteralTypo
        Debug.WriteLine($"GridBase.WndProc {Name}: WM_SETFOCUS");
        // We need to make sure we can switch the grid cell colour schemes when switching
        // focus between two grids. To get this to work even when focus is changed by a
        // mouse left-click, we would ideally like to override the Focus method. As that
        // method does not support overriding, we intercept the corresponding Windows
        // message here instead, which has the same effect.
        Controller.PrepareForFocus();
      }
      base.WndProc(ref m);
    }
  }
}