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
    /// Represents an SQL DELETE statement to execute against the 
    /// SoundExplorers database for an Entity
    /// of the specified type.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal class DeleteCommand<T> : OurSqlCommand<T>
        where T : Entity<T> {

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteCommand"/> class, 
        /// creating its <see cref="PgSqlConnection.Connection"/> 
        /// and <see cref="PgSqlConnection.Parameters"/>.
        /// </summary>
        public DeleteCommand() {
            Parameters.AddRange(
                CreateParameters(keyColumsOnly: true));
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
            sql.WriteLine("delete from " + EntityType.Name);
            sql.Write(GenerateSqlWhereClause());
            return sql.ToString();
        }
        #endregion Protected Methods
    }//End of class
}//End of namespace