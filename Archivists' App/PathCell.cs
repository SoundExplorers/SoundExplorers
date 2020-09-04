using System;
using System.IO;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers {
  /// <summary>
  ///   A DataGridView Cell that supports the editing of a file path.
  /// </summary>
  /// <remarks>
  ///   The cell displays dates in ordinary text box cells,
  ///   but when the user edits a cell, a text box and
  ///   a browse button to show an open dialogue appear.
  ///   <para>
  ///     After updating the path in the text box,
  ///     to complete the edit of the cell with the keyboard
  ///     without showing the open dialogue,
  ///     press F2.
  ///   </para>
  ///   <para>
  ///     After updating the path in the text box,
  ///     Control+Enter, like F2,
  ///     completes the edit of the cell with the keyboard
  ///     without showing the open dialogue.
  ///     But, unlike F2,
  ///     Control+Enter also attempts to update the database.
  ///     Control+Enter is not recommended because,
  ///     for unknown reason,
  ///     when there's a database update error and the rejected values
  ///     are restored to the row and the cell put into edit mode,
  ///     if the edit is then cancelled (by pressing Esc),
  ///     the original values do not get restored.
  ///     That's why it is worth implementing F2 to end edit here.
  ///   </para>
  ///   <para>
  ///     On a Mac keyboard,
  ///     ⌘+Enter completes the edit and goes to next row.
  ///     (Command (⌘) key on a Mac keyboard does the same
  ///     as Windows/Start key on a Windows keyboard.)
  ///   </para>
  /// </remarks>
  internal class PathCell : DataGridViewTextBoxCell, IView<PathCellController> {
    public PathCellController Controller { get; private set; }

    public override Type EditType =>
      // Return the type of the editing control that PathCell uses.
      typeof(PathEditingControl);

    /// <summary>
    ///   Gets whether the file, if any, specified by
    ///   the path in the cell exists.
    ///   False if a path is not specified or the file does not exist.
    /// </summary>
    public bool FileExists =>
      !string.IsNullOrEmpty(Path)
      && File.Exists(Path);

    /// <summary>
    ///   Gets the file path contained in the cell.
    ///   Null only if the path cell is on the new row and had not been edited.
    /// </summary>
    public string Path => Value?.ToString();

    public void SetController(PathCellController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Creates a PathCell and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    ///   The parameters are passed to the controller's constructor.
    /// </summary>
    /// <param name="tableController">
    ///   The controller of the table editor.
    /// </param>
    /// <param name="columnName">
    ///   The name of the column that is edited with the path cell.
    /// </param>
    [NotNull]
    public static PathCell Create([NotNull] TableController tableController,
      [NotNull] string columnName) {
      return (PathCell)ViewFactory.Create<PathCell, PathCellController>(
        tableController, columnName);
    }

    public override void InitializeEditingControl(
      int rowIndex,
      object initialFormattedValue,
      DataGridViewCellStyle dataGridViewCellStyle) {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
        dataGridViewCellStyle);
      var pathEditingControl = (PathEditingControl)DataGridView.EditingControl;
      pathEditingControl.KeyDown += PathEditingControl_KeyDown;
      if (Value == null
          || Value == DBNull.Value) {
        pathEditingControl.EditingControlFormattedValue = string.Empty;
      } else {
        pathEditingControl.EditingControlFormattedValue = Value.ToString();
      }
    }

    /// <summary>
    ///   End edit on F2.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PathEditingControl_KeyDown(object sender, KeyEventArgs e) {
      //Debug.WriteLine("PathEditingControl_KeyDown");
      if (e.KeyData == Keys.F2) {
        //Debug.WriteLine(e.KeyCode);
        DataGridView.EndEdit();
      }
      //// Navigation to next cell is erratic, for unknown reason.
      //if (e.Modifiers == Keys.Control) {
      //    switch (e.KeyCode) {
      //    case Keys.Left:
      //        //Debug.WriteLine("Control & " + e.KeyCode);
      //        DataGridView.EndEdit();
      //        Debug.WriteLine(DataGridView.CurrentCell.ColumnIndex);
      //        if (DataGridView.CurrentCell.ColumnIndex > 0) {
      //            DataGridViewCell cellToLeft = DataGridView.Rows[
      //                DataGridView.CurrentCell.RowIndex].Cells[
      //                    DataGridView.CurrentCell.ColumnIndex - 1];
      //            Debug.WriteLine(cellToLeft.ColumnIndex);
      //            if (cellToLeft.Visible) {
      //                DataGridView.CurrentCell = cellToLeft;
      //            }
      //        }
      //        break;
      //    case Keys.Right:
      //        DataGridView.EndEdit();
      //        Debug.WriteLine(DataGridView.CurrentCell.ColumnIndex);
      //        if (DataGridView.CurrentCell.ColumnIndex < DataGridView.ColumnCount - 1) {
      //            DataGridViewCell cellToRight = DataGridView.Rows[
      //                DataGridView.CurrentCell.RowIndex].Cells[
      //                    DataGridView.CurrentCell.ColumnIndex + 1];
      //            Debug.WriteLine(cellToRight.ColumnIndex);
      //            if (cellToRight.Visible) {
      //                DataGridView.CurrentCell = cellToRight;
      //            }
      //        }
      //        break;
      //    }//End of switch
      //}
    }
  } //End of class
} //End of namespace