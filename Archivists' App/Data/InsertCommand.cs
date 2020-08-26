using System;
using System.IO;
using System.Linq;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
    /// <summary>
    ///   Represents an SQL INSERT statement to execute against the
    ///   SoundExplorers database for an Entity
    ///   of the specified type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal class InsertCommand<T> : OurSqlCommand<T>
    where T : Entity<T> {
        /// <summary>
        ///   Initialises a new instance of the <see cref="InsertCommand" /> class,
        ///   creating its <see cref="PgSqlConnection.Connection" />
        ///   and <see cref="PgSqlConnection.Parameters" />.
        /// </summary>
        public InsertCommand() {
      Parameters.AddRange(
        CreateParameters(false));
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
      sql.WriteLine("insert into " + EntityType.Name + " (");
      sql.Write(GenerateSqlColumnList());
      sql.WriteLine(") values (");
      for (var i = 0; i < Columns.Count(); i++) {
        // A default is required in case the parameter value is null.
        sql.Write("    coalesce(@" + Columns[i].ColumnName + ", ");
        if (Columns[i].DataType == typeof(string)) {
          sql.Write("''");
        } else if (Columns[i].DataType == typeof(DateTime)) {
          sql.Write("cast('01 Jan 1900' as date)");
        } else if (Columns[i].DataType == typeof(int)) {
          sql.Write("0");
        } else {
          throw new InvalidOperationException(
            "DataType " + Columns[i].DataType
                        + " (for column " + Columns[i].ColumnName
                        + " of " + EntityType.Name +
                        ") is not currently supported.");
        }
        sql.Write(")");
        if (i < Columns.Count() - 1) {
          sql.WriteLine(",");
        } else {
          sql.WriteLine();
        }
      } // End of for
      sql.WriteLine(")");
      return sql.ToString();
    }
  } //End of class
} //End of namespace