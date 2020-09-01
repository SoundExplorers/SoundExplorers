using System;
using System.Data;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Credits.
  /// </summary>
  public class CreditList : EntityList<Credit> {
    /// <overloads>
    ///   Initialises a new instance of the <see cref="CreditList" /> class.
    /// </overloads>
    /// <summary>
    ///   Initialises a new instance of the <see cref="CreditList" /> class
    ///   and, when the <paramref name="empty" /> parameter is False (default),
    ///   populating its list
    ///   with all the Credit records on the database
    ///   and including all the Piece records as a parent list.
    /// </summary>
    /// <param name="empty">
    ///   Whether an empty list is to be created.
    ///   Default False.
    /// </param>
    public CreditList(bool empty = false)
      : this(null, empty) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="CreditList" /> class,
    ///   populating its list
    ///   with all the Credit records on the database
    ///   and including all the Piece records as a parent list.
    /// </summary>
    public CreditList()
      : this(typeof(PieceList), false) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="CreditList" /> class
    ///   and, when the <paramref name="empty" /> parameter is False (default),
    ///   populating its list
    ///   with all the Credit records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    /// <param name="empty">
    ///   Whether an empty list is to be created.
    ///   Default False.
    /// </param>
    public CreditList(
      Type parentListType = null,
      bool empty = false)
      : base(parentListType, empty: empty) { }

    public CreditList(
      Type parentListType = null, Piece piece = null)
      : base(selectCommand: CreateSelectCommand(piece)) { }

    /// <summary>
    ///   An indexer that returns
    ///   the Credit at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Credit to get.
    /// </param>
    /// <returns>
    ///   The Credit at the specified index in the list.
    /// </returns>
    public new Credit this[int index] => base[index] as Credit;

    private static SelectCommand<Credit> CreateSelectCommand(Piece piece) {
      var selectCommand = new SelectCommand<Credit>();
      if (piece != null) {
        selectCommand.CommandText =
          SqlHelper.GetSql("Select Credits for Piece.sql");
        var parameter = new PgSqlParameter();
        parameter.ParameterName = "@Date";
        parameter.Value = piece.Date;
        parameter.DbType = DbType.DateTime;
        selectCommand.Parameters.Add(parameter);
        parameter = new PgSqlParameter();
        parameter.ParameterName = "@Location";
        parameter.Value = piece.Location;
        parameter.DbType = DbType.String;
        selectCommand.Parameters.Add(parameter);
        parameter = new PgSqlParameter();
        parameter.ParameterName = "@Set";
        parameter.Value = piece.Set;
        parameter.DbType = DbType.Int32;
        selectCommand.Parameters.Add(parameter);
        parameter = new PgSqlParameter();
        parameter.ParameterName = "@Piece";
        parameter.Value = piece.PieceNo;
        parameter.DbType = DbType.Int32;
        selectCommand.Parameters.Add(parameter);
      }
      return selectCommand;
    }
  } //End of class
} //End of namespace