using System;
using System.Collections.Generic;
using System.Data;
using SoundExplorers.Data;

namespace SoundExplorers.Model; 

/// <summary>
///   Raised when there is an error on
///   attempting to insert, update or delete an entity on the database.
/// </summary>
public class DatabaseUpdateErrorException : ApplicationException {
  static DatabaseUpdateErrorException() {
    ErrorTypes = CreateErrorTypes();
  }

  /// <summary>
  ///   Initialises an instance of the
  ///   <see cref="DatabaseUpdateErrorException" /> class.
  /// </summary>
  /// <param name="changeAction">
  ///   The type of database change action that caused the error:
  ///   deletion, insertion or update;
  /// </param>
  /// <param name="message">Error message.</param>
  /// <param name="rowIndex">
  ///   The index of the row whose insertion, update or deletion failed.
  /// </param>
  /// <param name="columnIndex">
  ///   The index of the column whose field value in the row
  ///   caused the insertion, update or deletion to fail.
  ///   Zero if the failure cannot be attributed to a specific field value.
  /// </param>
  /// <param name="innerException">
  ///   An <see cref="Exception" /> that provides
  ///   error diagnostics.
  /// </param>
  public DatabaseUpdateErrorException(
    StatementType changeAction,
    string message,
    int rowIndex,
    int columnIndex,
    Exception innerException) : base(message, innerException) {
    ChangeAction = changeAction;
    RowIndex = rowIndex;
    ColumnIndex = columnIndex;
  }

  public StatementType ChangeAction { get; }

  /// <summary>
  ///   Gets the index of the column
  ///   whose field value in the row
  ///   caused the
  ///   insertion, update or deletion to fail.
  ///   Zero if the failure cannot be attributed to a specific field value.
  /// </summary>
  public int ColumnIndex { get; }

  public ErrorType ErrorType => ErrorTypes[InnerException!.GetType()];
  private static IDictionary<Type, ErrorType> ErrorTypes { get; }

  /// <summary>
  ///   Gets the index of the whose
  ///   insertion, update or deletion failed.
  /// </summary>
  public int RowIndex { get; }

  private static IDictionary<Type, ErrorType> CreateErrorTypes() {
    return new Dictionary<Type, ErrorType> {
      {typeof(ConstraintException), ErrorType.Database},
      {typeof(DuplicateNameException), ErrorType.Duplicate},
      {typeof(FormatException), ErrorType.Format},
      {typeof(PropertyConstraintException), ErrorType.Database},
      {typeof(RowNotInTableException), ErrorType.ReferencingValueNotFound}
    };
  }
}