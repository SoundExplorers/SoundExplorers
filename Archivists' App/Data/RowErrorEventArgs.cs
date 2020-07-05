using System;
using System.Data;
using System.Diagnostics;

namespace SoundExplorers.Data {

    /// <summary>
    /// Provides data for a RowError event
    /// that is raised when there is an error on 
    /// attempting to insert, update or delete a <see cref="DataRow"/>.
    /// </summary>
    internal class RowErrorEventArgs : EventArgs {

        #region Properties
        /// <summary>
        /// Gets the index of the <see cref="DataColumn"/> 
        /// whose field value in the <see cref="DataRow"/> 
        /// caused the 
        /// insertion, update or deletion to fail.
        /// Zero if the failure cannot be attributed to a specific field value.
        /// </summary>
        public virtual int ColumnIndex { get; private set; }

        /// <summary>
        /// Gets an <see cref="Exception"/> that provides
        /// error diagnostics.
        /// </summary>
        /// <remarks>
        /// This is a modified version of the <see cref="Exception"/> 
        /// that was thrown on
        /// attempting to insert, update or delete the row.
        /// Where the SQL error message has been replaced
        /// with a more meaningful error message, 
        /// this will be an <see cref="ApplicationException"/>.
        /// Where the SQL error message is passed on unmodified,
        /// this will be a <see cref="DataException"/>.
        /// In the unlikely event of any other type of 
        /// <see cref="Exception"/> than an SQL exception having been thrown,
        /// the <see cref="Exception"/>, 
        /// this will be that <see cref="Exception"/> unchanged.
        /// </remarks>
        public virtual Exception Exception { get; private set; }

        /// <summary>
        /// Gets an array of the values of the row's fields
        /// as at just before the change was rejected.
        /// If the user had tried to delete the row,
        /// All the values will be DBNull.
        /// </summary>
        public virtual object[] RejectedValues { get; private set; }

        /// <summary>
        /// Gets the index of the <see cref="DataRow"/> whose 
        /// insertion, update or deletion failed.
        /// </summary>
        public virtual int RowIndex { get; private set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initialises an instance of the 
        /// <see cref="RowErrorEventArgs"/> class.
        /// </summary>
        /// <param name="rowIndex">
        /// The index of the <see cref="DataRow"/> whose 
        /// insertion, update or deletion failed.
        /// </param>
        /// <param name="columnIndex">
        /// The index of the <see cref="DataColumn"/> 
        /// whose field value in the <see cref="DataRow"/> 
        /// caused the 
        /// insertion, update or deletion to fail.
        /// Zero if the failure cannot be attributed to a specific field value.
        /// </param>
        /// <param name="rejectedValues">
        /// An array of the values of the row's fields
        /// as at just before the change was rejected.
        /// </param>
        /// <param name="exception">
        /// An <see cref="Exception"/> that provides
        /// error diagnostics.
        /// </param>
        public RowErrorEventArgs(
                int rowIndex,
                int columnIndex,
                object[] rejectedValues,
                Exception exception) {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RejectedValues = rejectedValues;
            Exception = exception;
        }
        #endregion Constructors
    }//End of class
}//End of namespace