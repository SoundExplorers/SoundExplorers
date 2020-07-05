using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {

    /// <summary>
    /// Represents an SQL INSERT statement to execute against the 
    /// SoundExplorers database for an Entity
    /// of the specified type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal class InsertCommand<T> : OurSqlCommand<T>
        where T : Entity<T> {

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="InsertCommand"/> class, 
        /// creating its <see cref="PgSqlConnection.Connection"/> 
        /// and <see cref="PgSqlConnection.Parameters"/>.
        /// </summary>
        public InsertCommand() {
            Parameters.AddRange(
                CreateParameters(keyColumsOnly: false));
        }
        #endregion Constructors

        #region Protected Methods
        /// <summary>
        /// Generates the SQL command text 
        /// from metadata derived from the type of
        /// Entity.
        /// </summary>
        /// <returns>
        /// The SQL text generated.
        /// </returns>
        protected override string GenerateSql() {
            var sql = new StringWriter();
            sql.WriteLine("insert into " + EntityType.Name + " (");
            sql.Write(GenerateSqlColumnList());
            sql.WriteLine(") values (");
            for (int i = 0; i < Columns.Count(); i++) {
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
                        "DataType " + Columns[i].DataType.ToString()
                        + " (for column " + Columns[i].ColumnName
                        + " of " + EntityType.Name + ") is not currently supported.");
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
        #endregion Protected Methods
    }//End of class
}//End of namespace