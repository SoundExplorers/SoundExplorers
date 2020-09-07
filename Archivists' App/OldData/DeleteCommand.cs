using System.IO;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Represents an SQL DELETE statement to execute against the
  ///   SoundExplorers database for an Entity
  ///   of the specified type.
  /// </summary>
  /// <remarks>
  /// </remarks>
  public class DeleteCommand<T> : OurSqlCommand<T>
    where T : Entity<T> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="DeleteCommand" /> class,
    ///   creating its <see cref="PgSqlConnection.Connection" />
    ///   and <see cref="PgSqlConnection.Parameters" />.
    /// </summary>
    public DeleteCommand() {
      Parameters.AddRange(
        CreateParameters(true));
    }

    /// <summary>
    ///   Generates the SQL command text
    ///   from metadata derived from the type of
    ///   Entity.
    /// </summary>
    /// <returns>
    ///   The SQL text generated.
    /// </returns>
    protected override string GenerateSql() {
      var sql = new StringWriter();
      sql.WriteLine("delete from " + EntityType.Name);
      sql.Write(GenerateSqlWhereClause());
      return sql.ToString();
    }
  } //End of class
} //End of namespace