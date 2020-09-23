using System;

namespace SoundExplorers.Common {
  /// <summary>
  ///   Raised when there is an error on
  ///   attempting to insert, update or delete an entity on the database.
  /// </summary>
  public class DatabaseUpdateErrorException : ApplicationException {
    /// <summary>
    ///   Initialises an instance of the
    ///   <see cref="DatabaseUpdateErrorException" /> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="rowIndex">
    ///   The index of the row whose
    ///   insertion, update or deletion failed.
    /// </param>
    /// <param name="columnIndex">
    ///   The index of the column
    ///   whose field value in the row
    ///   caused the
    ///   insertion, update or deletion to fail.
    ///   Zero if the failure cannot be attributed to a specific field value.
    /// </param>
    /// <param name="innerException">
    ///   An <see cref="Exception" /> that provides
    ///   error diagnostics.
    /// </param>
    public DatabaseUpdateErrorException(
      string message,
      int rowIndex,
      int columnIndex,
      Exception innerException) : base(message, innerException) {
      RowIndex = rowIndex;
      ColumnIndex = columnIndex;
    }

    /// <summary>
    ///   Gets the index of the column
    ///   whose field value in the row
    ///   caused the
    ///   insertion, update or deletion to fail.
    ///   Zero if the failure cannot be attributed to a specific field value.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    ///   Gets the index of the whose
    ///   insertion, update or deletion failed.
    /// </summary>
    public int RowIndex { get; }
  }
}