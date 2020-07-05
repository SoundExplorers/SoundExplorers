using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SoundExplorers.Data;

namespace SoundExplorers {

    /// <summary>
    /// Path cell of a DataGridView.
    /// </summary>
    /// <remarks>
    /// The cell displays dates in ordinary text box cells, 
    /// but when the user edits a cell, a text box and 
    /// a browse button to show an open dialogue appear.
    /// <para>
    /// After updating the path in the text box,
    /// to complete the edit of the cell with the keyboard
    /// without showing the open dialogue,
    /// press F2.
    /// </para>
    /// <para>
    /// After updating the path in the text box,
    /// Control+Enter, like F2,
    /// completes the edit of the cell with the keyboard
    /// without showing the open dialogue.
    /// But, unlike F2, 
    /// Control+Enter also attempts to update the database.
    /// Control+Enter is not recommended because,
    /// for unknown reason,
    /// when there's a database update error and the rejected values
    /// are restored to the row and the cell put into edit mode,
    /// if the edit is then cancelled (by pressing Esc),
    /// the original values do not get restored.
    /// That's why it is worth implementing F2 to end edit here.
    /// </para>
    /// <para>
    /// On a Mac keyboard,
    /// ⌘+Enter completes the edit and goes to next row.
    /// (Command (⌘) key on a Mac keyboard does the same 
    /// as Windows/Start key on a Windows keyboard.)
    /// </para>
    /// </remarks>
    internal class PathCell : DataGridViewTextBoxCell {

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="PathCell"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor cannot have properties.
        /// Otherwise nothing gets shown on the grid!
        /// </remarks>
        public PathCell()
            : base() {
        }
        #endregion Constructors

        /// <summary>
        /// Gets or sets the entity column metadata.
        /// </summary>
        /// <remarks>
        /// For unknown reason,
        /// non-inherited properties of a PathCell
        /// (i.e. that are not inherited from DataGridViewTextBoxCell)
        /// to which a DataGridViewColumn.CellTemplate
        /// has been set
        /// don't persist when a cell is edited.
        /// So we are going to store this property in the Tag.
        /// That solves the problem.
        /// </remarks>
        public virtual IEntityColumn Column {
            get {
                return this.Tag as IEntityColumn;
            }
            set {
                this.Tag = value;
            }
        }

        /// <summary>
        /// Gets whether the file, if any, specified by
        /// the path in the cell exists.
        /// False if a path is not specified or the file does not exist.
        /// </summary>
        public virtual bool FileExists {
            get {
                return 
                    !string.IsNullOrEmpty(Path) 
                    && File.Exists(Path);
            }
        }

        /// <summary>
        /// Gets or sets the file path contained in the cell.
        /// Null only if the path cell is on the new row and had not been edited.
        /// </summary>
        public virtual string Path {
            get {
                if (Value != null) {
                    return Value.ToString();
                } else {
                    return null;
                }
            }
            set {
                if (value != null) {
                    Value = value;
                } else {
                    Value = string.Empty;
                }
            }
        }

        public override void InitializeEditingControl(
                int rowIndex, 
                object initialFormattedValue, 
                DataGridViewCellStyle dataGridViewCellStyle) {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            PathEditingControl pathEditingControl =
                this.DataGridView.EditingControl as PathEditingControl;
            pathEditingControl.KeyDown += new KeyEventHandler(PathEditingControl_KeyDown);
            if (this.Value == null
            ||  this.Value == DBNull.Value) {
                pathEditingControl.EditingControlFormattedValue = string.Empty;
            } else {
                pathEditingControl.EditingControlFormattedValue = this.Value.ToString();
            }
        }

        public override Type EditType {
            get {
                // Return the type of the editing control that PathCell uses.
                return typeof(PathEditingControl);
            }
        }

        /// <summary>
        /// End edit on F2.
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
    }//End of class
}//End of namespace
