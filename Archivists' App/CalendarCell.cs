using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SoundExplorers {

    /// <summary>
    /// Calendar cell of a DataGridView.
    /// </summary>
    /// <remarks>
    /// The cell displays dates in ordinary text box cells, 
    /// but when the user edits a cell, a DateTimePicker control appears.
    /// The default date when inserting a new row will be today's date.
    /// <para>
    /// Code modified from
    /// MSDN/"How to: Host Controls in Windows Forms DataGridView Cells"
    /// http://msdn.microsoft.com/en-us/library/7tas5c80.aspx.
    /// </para>
    /// </remarks>
    internal class CalendarCell : DataGridViewTextBoxCell {

        public CalendarCell()
            : base() {
        }

        public override void InitializeEditingControl(
                int rowIndex, 
                object initialFormattedValue, 
                DataGridViewCellStyle dataGridViewCellStyle) {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            CalendarEditingControl ctl =
                DataGridView.EditingControl as CalendarEditingControl;
            // Use the default row value when Value property is null
            // or, more to the point in this case, empty.
            if (this.Value == null
            ||  this.Value == DBNull.Value) {
                ctl.Value = DateTime.Now;
                this.Value = ctl.Value;
            } else {
                ctl.Value = (DateTime)this.Value;
            }
        }

        public override Type EditType {
            get {
                // Return the type of the editing control that CalendarCell uses.
                return typeof(CalendarEditingControl);
            }
        }

        //public override Type ValueType {
        //    get {
        //        // Return the type of the value that CalendarCell contains.
        //        return typeof(DateTime);
        //    }
        //}
    }//End of class
}//End of namespace
