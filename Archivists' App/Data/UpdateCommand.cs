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
    /// Represents an SQL UPDATE statement to execute against the 
    /// SoundExplorers database for an Entity
    /// of the specified type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal class UpdateCommand<T> : OurSqlCommand<T>
        where T : Entity<T> {

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateCommand"/> class, 
        /// creating its <see cref="PgSqlConnection.Connection"/> 
        /// and <see cref="PgSqlConnection.Parameters"/>.
        /// </summary>
        public UpdateCommand() {
            Parameters.AddRange(
                CreateParameters());
        }
        #endregion Constructors

        #region Protected Methods
        protected new PgSqlParameter[] CreateParameters() {
            var parameters = new List<PgSqlParameter>();
            foreach (EntityColumn<T> column in (Entity.Columns)) {
                if (column.IsInPrimaryKey) {
                    var parameter = new PgSqlParameter();
                    parameter.ParameterName = "@OLD_" + column.ColumnName;
                    parameter.DbType = TypeToDbType(column.DataType);
                    parameters.Add(parameter);
                }
            } // End of foreach
            parameters.AddRange(base.CreateParameters(false));
            //foreach (PgSqlParameter parameter in parameters) {
            //    Debug.WriteLine(parameter.ParameterName);
            //}
            return parameters.ToArray();
        }

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
            sql.WriteLine("update " + EntityType.Name + " set");
            for (int i = 0; i < Columns.Count(); i++) {
                sql.Write("    " + Columns[i].NameOnDb + " = @" + Columns[i].ColumnName);
                //if (AliasedNames.ContainsKey(Columns[i].ColumnName)) {
                //    sql.Write("    " + AliasedNames[Columns[i].ColumnName] + " = @" + Columns[i].ColumnName);
                //} else {
                //    sql.Write("    " + Columns[i].ColumnName + " = @" + Columns[i].ColumnName);
                //}
                if (i < Columns.Count() - 1) {
                    sql.WriteLine(",");
                } else {
                    sql.WriteLine();
                }
            } // End of for
            sql.Write(GenerateSqlWhereClause());
            return sql.ToString();
        }

        /// <summary>
        /// Generates the SQL WHERE clause.
        /// </summary>
        /// <returns>
        /// The SQL fragment generated.
        /// </returns>
        protected override string GenerateSqlWhereClause() {
            return base.GenerateSqlWhereClause().Replace("@", "@OLD_");
        }
        #endregion Protected Methods
    }//End of class
}//End of namespace