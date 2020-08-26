using System.IO;
using System.Linq;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
    /// <summary>
    ///   Represents an SQL SELECT statement to execute against the
    ///   SoundExplorers database for an Entity
    ///   of the specified type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal class SelectCommand<T> : OurSqlCommand<T>
    where T : Entity<T> {
        /// <summary>
        ///   Initialises a new instance of the <see cref="SelectCommand" /> class,
        ///   creating its <see cref="PgSqlConnection.Connection" />
        ///   and <see cref="PgSqlConnection.Parameters" />.
        /// </summary>
        /// <param name="all">
        ///   Whether all records are to be fetched.
        ///   If False, just one record will be fetched.
        /// </param>
        public SelectCommand(bool all = true) {
      All = all;
      if (All) {
        SqlFilename = "Select All From " + EntityType.Name + ".sql";
      } else {
        Parameters.AddRange(
          CreateParameters(true));
        SqlFilename = "Select " + EntityType.Name + ".sql";
      }
    }

        /// <summary>
        ///   Gets whether all records are to be fetched.
        ///   If False, just one record will be fetched.
        /// </summary>
        public bool All { get; }

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
      sql.WriteLine("select");
      string columnList = GenerateSqlColumnList();
      foreach (EntityColumn<T> column in Columns) {
        if (column.ColumnName != column.NameOnDb) {
          columnList = columnList.Replace(
            "    " + column.NameOnDb,
            "    " + column.NameOnDb + " as " + column.ColumnName);
        }
      } // End of foreach
      //foreach (KeyValuePair<string, string> aliasedName in AliasedNames) {
      //    columnList = columnList.Replace(
      //    "    " + aliasedName.Value,
      //    "    " + aliasedName.Value + " as " + aliasedName.Key);
      //} // End of foreach
      sql.Write(columnList);
      sql.WriteLine("from " + EntityType.Name);
      if (All) {
        sql.WriteLine("order by");
        for (var i = 0; i < PrimaryKeyColumns.Count(); i++) {
          sql.Write("    " + PrimaryKeyColumns[i].ColumnName);
          if (i < PrimaryKeyColumns.Count() - 1) {
            sql.WriteLine(",");
          } else {
            sql.WriteLine();
          }
        } // End of for
      } else {
        sql.Write(GenerateSqlWhereClause());
      }
      return sql.ToString();
    }
  } //End of class
} //End of namespace