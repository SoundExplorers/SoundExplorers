using System;
using System.Collections.Generic;
using System.Data;

namespace SoundExplorers.Common {
  /// <summary>
  ///   Raised when there is an error on
  ///   attempting to insert, update or delete a <see cref="DataRow" />.
  /// </summary>
  public class DatabaseUpdateErrorException : ApplicationException {
    /// <summary>
    ///   Initialises an instance of the
    ///   <see cref="DatabaseUpdateErrorException" /> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="rowIndex">
    ///   The index of the <see cref="DataRow" /> whose
    ///   insertion, update or deletion failed.
    /// </param>
    /// <param name="columnIndex">
    ///   The index of the <see cref="DataColumn" />
    ///   whose field value in the <see cref="DataRow" />
    ///   caused the
    ///   insertion, update or deletion to fail.
    ///   Zero if the failure cannot be attributed to a specific field value.
    /// </param>
    /// <param name="rejectedValues">
    ///   An array of the values of the row's fields
    ///   as at just before the change was rejected.
    /// </param>
    /// <param name="innerException">
    ///   An <see cref="Exception" /> that provides
    ///   error diagnostics.
    /// </param>
    public DatabaseUpdateErrorException(
      string message,
      int rowIndex,
      int columnIndex,
      IList<object> rejectedValues,
      Exception innerException) : base(message, innerException) {
      RowIndex = rowIndex;
      ColumnIndex = columnIndex;
      RejectedValues = rejectedValues;
    }

    /// <summary>
    ///   Gets the index of the <see cref="DataColumn" />
    ///   whose field value in the <see cref="DataRow" />
    ///   caused the
    ///   insertion, update or deletion to fail.
    ///   Zero if the failure cannot be attributed to a specific field value.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    ///   Gets a list of the values of the row's fields
    ///   as at just before the change was rejected.
    ///   If the user had tried to delete the row,
    ///   All the values will be DBNull.
    /// </summary>
    public IList<object> RejectedValues { get; }

    /// <summary>
    ///   Gets the index of the <see cref="DataRow" /> whose
    ///   insertion, update or deletion failed.
    /// </summary>
    public int RowIndex { get; }
  }
}