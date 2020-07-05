using System;
using System.Windows.Forms;

namespace SoundExplorers {

    /// <summary>
    /// Editing control for a calendar cell of a DataGridView.
    /// </summary>
    internal class ComboBoxEditingControl : ComboBox, IDataGridViewEditingControl {

        public ComboBoxEditingControl() {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnSelectedIndexChanged(EventArgs e) {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnSelectedIndexChanged(e);
        }

        #region IDataGridViewEditingControl Members
        /// <summary>
        /// Changes the control's user interface (UI) to be consistent with the specified
        //  cell style.
        /// </summary>
        /// <param name="dataGridViewCellStyle">
        /// The System.Windows.Forms.DataGridViewCellStyle to use as the model for the
        /// UI.
        /// </param>
        public void ApplyCellStyleToEditingControl(
                DataGridViewCellStyle dataGridViewCellStyle) {
            this.Font = dataGridViewCellStyle.Font;
            //this.FormatString = dataGridViewCellStyle.Format;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        /// <summary>
        /// Gets or sets the System.Windows.Forms.DataGridView that contains the cell.
        /// </summary>
        public DataGridView EditingControlDataGridView { get; set; }

        /// <summary>
        /// Gets or sets the formatted value of the cell being modified by the editor.
        /// </summary>
        public object EditingControlFormattedValue {
            get {
                return this.Text;
            }
            set {
                this.Text = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the index of the hosting cell's parent row.
        /// </summary>
        public int EditingControlRowIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value of the editing control
        //  differs from the value of the hosting cell.
        /// </summary>
        public bool EditingControlValueChanged { get; set; }

        /// <summary>
        /// Determines whether the specified key is a regular input key that the editing
        //  control should process or a special key that the System.Windows.Forms.DataGridView
        //  should process.
        /// </summary>
        /// <param name="keyData">
        /// A System.Windows.Forms.Keys that represents the key that was pressed.
        /// </param>
        /// <param name="dataGridViewWantsInputKey">
        /// True when the System.Windows.Forms.DataGridView wants to process the System.Windows.Forms.Keys
        //  in keyData; otherwise, False.
        /// </param>
        /// <returns>
        /// True if the specified key is a regular input key that should be handled by
        //  the editing control; otherwise, False.
        /// </returns>
        public bool EditingControlWantsInputKey(
                Keys key, 
                bool dataGridViewWantsInputKey) {
            // Let the ComboBox handle the keys listed.
            switch (key & Keys.KeyCode) {
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Right:
            case Keys.Home:
            case Keys.End:
            case Keys.PageDown:
            case Keys.PageUp:
                return true;
            default:
                return !dataGridViewWantsInputKey;
            }
        }

        /// <summary>
        /// Gets the cursor used when the mouse pointer is over the System.Windows.Forms.DataGridView.EditingPanel
        //  but not over the editing control.
        /// </summary>
        public Cursor EditingPanelCursor {
            get {
                return base.Cursor;
            }
        }

        /// <summary>
        /// Retrieves the formatted value of the cell.
        /// </summary>
        /// <param name="context">
        /// A bitwise combination of System.Windows.Forms.DataGridViewDataErrorContexts
        //  values that specifies the context in which the data is needed.
        /// </param>
        /// <returns>
        /// An System.Object that represents the formatted version of the cell contents.
        /// </returns>
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context) {
            return EditingControlFormattedValue;
        }

        /// <summary>
        /// Prepares the currently selected cell for editing.
        /// </summary>
        /// <param name="selectAll">
        /// True to select all of the cell's content; otherwise, False.
        /// </param>
        /// <remarks>
        /// No preparation needs to be done.
        /// </remarks>
        public void PrepareEditingControlForEdit(bool selectAll) {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell contents need to be repositioned
        //  whenever the value changes.
        /// </summary>
        public bool RepositionEditingControlOnValueChange {
            get {
                return false;
            }
        }
        #endregion IDataGridViewEditingControl Members
    }//End of class
}//End of namespace
