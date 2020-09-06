using System;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Pieces.
  /// </summary>
  public class PieceList : EntityList<Piece> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="PieceList" /> class,
    ///   so that a subsequent call of the Fetch method will populate its list
    ///   with all the Piece records on the database
    ///   and including all the Set records as a parent list.
    /// </summary>
    public PieceList()
      : this(typeof(SetList)) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="PieceList" /> class,
    ///   so that a subsequent call of the Fetch method will populate its list
    ///   with all the Piece records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    public PieceList(Type parentListType = null)
      : base(parentListType) { }

    public PieceList(
      Type parentListType = null, IPieceOwningEntity owner = null)
      : base(selectCommand: CreateSelectCommand(owner)) { }

    /// <summary>
    ///   An indexer that returns
    ///   the Piece at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Piece to get.
    /// </param>
    /// <returns>
    ///   The Piece at the specified index in the list.
    /// </returns>
    public new Piece this[int index] => base[index] as Piece;

    /// <summary>
    ///   Creates a SELECT command to fetch from the database
    ///   all the Pieces that directly or indirectly
    ///   reference the specified owning entiy.
    /// </summary>
    /// <param name="owner">
    ///   An entity representing a database record
    ///   for which all the referencing Pieces are to be fetched.
    /// </param>
    /// <returns>
    ///   The created SELECT command.
    /// </returns>
    private static SelectCommand<Piece> CreateSelectCommand(IEntity owner) {
      var selectCommand = new SelectCommand<Piece>();
      if (owner != null) {
        selectCommand.CommandText =
          SqlHelper.GetSql("Select Pieces for " + owner.TableName + ".sql");
        foreach (var column in new Piece().Columns) {
          IEntityColumn referencedColumn = null;
          if (owner.PrimaryKeyColumns.ContainsKey(column.ColumnName)) {
            // True for 
            //   Performance.Date, Performance.Location,
            //   Set.Date, Set.Location,
            referencedColumn = owner.PrimaryKeyColumns[column.ColumnName];
          } else if (column.ReferencedColumnName != null
                     && column.ColumnName == owner.TableName) {
            if (owner.PrimaryKeyColumns.ContainsKey(column.ReferencedColumnName)
            ) {
              // True for
              //   Act.Name,
              //   Set.SetNo
              referencedColumn =
                owner.PrimaryKeyColumns[column.ReferencedColumnName];
            } else if (owner.UniqueKeyColumns.ContainsKey(
              column.ReferencedColumnName)) {
              // True for Location.Name
              referencedColumn =
                owner.UniqueKeyColumns[column.ReferencedColumnName];
            }
          }
          if (referencedColumn != null) {
            var parameter = new PgSqlParameter();
            parameter.ParameterName = "@" + column.ColumnName;
            parameter.Value = referencedColumn.GetValue(owner);
            parameter.DbType =
              SelectCommand<Piece>.TypeToDbType(column.DataType);
            selectCommand.Parameters.Add(parameter);
          }
        } //End of foreach
      }
      if (selectCommand.Parameters.Count == 0) {
        // Piece does not contain any columns that reference owner.
        // This applies to Artist, 
        // because it has am many to many relationship with Piece.
        foreach (var ownerPrimaryKeyColumn in owner.PrimaryKeyColumns) {
          // Artist.Name
          var parameter = new PgSqlParameter();
          parameter.ParameterName = "@" + ownerPrimaryKeyColumn.ColumnName;
          parameter.Value = ownerPrimaryKeyColumn.GetValue(owner);
          parameter.DbType =
            SelectCommand<Piece>.TypeToDbType(ownerPrimaryKeyColumn.DataType);
          selectCommand.Parameters.Add(parameter);
        } //End of foreach
      }
      return selectCommand;
    }
  } //End of class
} //End of namespace