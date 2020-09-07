// Written by Simon O'Rorke, April 2012.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Helper methods for SQL execution.
  /// </summary>
  /// <remarks>
  /// </remarks>
  internal class SqlHelper {
    public static readonly string ConnectionString =
      "Server=localhost;" +
      "Database=soundexplorers;" +
      // This Protocol parameter ensures that a meaningful
      // 'database "xxx" does not exist' 
      // error message is shown.
      // It would not be required in a later version of
      // Devart.Data.PostgreSql.
      // See http://forums.devart.com/viewtopic.php?t=23874.
      "Protocol=Ver20;" +
      "User ID=fred;" +
      "Password=!q";

    private PgSqlCommand _command;

    /// <summary>
    ///   Gets or sets the wait time, in seconds,
    ///   before terminating the attempt to
    ///   execute a method and generating an error.
    /// </summary>
    /// <remarks>
    ///   The default is 30 seconds.
    ///   A value of 0 indicates no limit, and should be avoided.
    /// </remarks>
    public int Timeout { get; set; } = 30;

    /// <summary>
    ///   Tries to cancel the execution of the last
    ///   command, if any.
    /// </summary>
    /// <remarks>
    ///   If the attempt to cancel fails,
    ///   no exception is generated.
    ///   Attempting to cancel the command
    ///   has to be done on a different thread
    ///   from the one on which the command was executed.
    ///   This method is thread-safe.
    ///   <para>
    ///     The method whose command is cancelled will throw
    ///     an <see cref="AbortedException" />.
    ///   </para>
    /// </remarks>
    public virtual void CancelLastCommand() {
      lock (_command) {
        if (_command != null) {
          _command.Cancel();
        }
      } //End of lock
    }

    /// <summary>
    ///   Closes the specified <see cref="SqlConnection" />.
    /// </summary>
    /// <param name="connection">
    ///   The <see cref="SqlConnection" /> to be closed.
    /// </param>
    /// <exception cref="DataException">
    ///   The specified <see cref="SqlConnection" />
    ///   could not be closed.
    /// </exception>
    private void CloseConnection(PgSqlConnection connection) {
      try {
        connection.Close();
      } catch (PgSqlException ex) {
        // SqlException's Message
        // just says "System error" unless caught!
        throw new DataException(ex.Message);
      }
    }

    /// <summary>
    ///   Returns a suitable exception.
    /// </summary>
    /// <param name="exception">
    ///   The <see cref="SqlException" />
    ///   that was thrown when the SQL's execution failed.
    /// </param>
    /// <param name="sql">The SQL that was executed.</param>
    /// <returns>
    ///   A suitable exception.
    /// </returns>
    /// <remarks>
    ///   If a command was cancelled by the
    ///   <see cref="CancelLastCommand" /> method,
    ///   an <see cref="AbortedException" /> will be returned.
    ///   Otherwise a <see cref="DataException" />,
    ///   with a <see cref="Exception.Message" />
    ///   that combines the specified exception's
    ///   <see cref="Exception.Message" />
    ///   with the specified SQL,
    ///   will be returned.
    ///   <see cref="SqlException" />'s
    ///   <see cref="Exception.Message" />
    ///   just says "System error" unless caught!
    /// </remarks>
    private Exception CreateException(
      PgSqlException exception, string sql) {
      string message = exception.Message;
      //if (message.ToLower().Contains("operation cancelled by user")) {
      //    return new AbortedException(message);
      //}
      if (message.ToLower().Contains("timeout expired")) {
        message += Environment.NewLine
                   + " (SqlHelper.Timeout = "
                   + Timeout.ToString("#,0") + " seconds.)";
      }
      return new DataException(
        message
        + Environment.NewLine + Environment.NewLine
        + "on executing the following SQL:"
        + Environment.NewLine + Environment.NewLine
        + sql,
        exception);
    }

    /// <summary>
    ///   Deletes all data from the specified table.
    /// </summary>
    /// <param name="tableName">
    ///   Table name.
    /// </param>
    /// <remarks>
    ///   The <b>TRUNCATE TABLE</b> SQL statement is used to delete the
    ///   data if the user has permission
    ///   to do TRUNCATE TABLE.
    ///   Otherwise the <b>DELETE</b> SQL statement
    ///   is used.
    ///   <para>
    ///   </para>
    ///   Both DELETE with no WHERE clause and TRUNCATE TABLE
    ///   remove all rows from the table.
    ///   But TRUNCATE TABLE is faster and uses fewer
    ///   system and transaction log resources than DELETE.
    ///   In addition, TRUNCATE TABLE resets
    ///   the counter used by an identity column for new rows
    ///   to the seed for the column.
    ///   Unfortunately, ordinary users cannot
    ///   be given permissions to do TRUNCATE TABLE:
    ///   TRUNCATE TABLE permissions default to the table owner,
    ///   members of the sysadmin fixed server role,
    ///   and the db_owner and db_ddladmin fixed database roles,
    ///   and are not transferable.
    /// </remarks>
    /// <exception cref="AbortedException">
    ///   The attempt to delete all data from
    ///   the table was cancelled by the
    ///   <see cref="CancelLastCommand" /> method.
    /// </exception>
    /// <exception cref="DataException">
    ///   The attempt to delete all data from
    ///   the table failed.
    /// </exception>
    public virtual void DeleteAllFromTable(string tableName) {
      var parameters = new Dictionary<string, string>(1);
      parameters.Add("#TableName", tableName);
      try {
        Execute("Truncate Table.sql", parameters);
      } catch {
        Execute("Delete From Table.sql", parameters);
      }
    }

    /// <summary>
    ///   Drops the specified table if it exists.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table to be dropped.
    /// </param>
    /// <exception cref="AbortedException">
    ///   The the attempt to
    ///   drop the table if it exists was cancelled by the
    ///   <see cref="CancelLastCommand" /> method.
    /// </exception>
    /// <exception cref="DataException">
    ///   Either the check for the
    ///   existence of the table failed or the attempt to
    ///   drop the table if it exists failed.
    /// </exception>
    public virtual void DropTableIfExists(string tableName) {
      var parameters = new Dictionary<string, string>(1);
      parameters.Add("#TableName", tableName);
      Execute("Drop Table If Exists.sql", parameters);
    }

    /// <summary>
    ///   Executes the specified SQL.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   within the Sql folder of the Data folder
    ///   of the executing assembly's project
    ///   containing the SQL.
    /// </param>
    /// <param name="parameters">
    ///   A dictionary of parameters to be substituted
    ///   in the SQL.
    ///   A null reference (<b>Nothing</b> in Visual Basic)
    ///   if parameter substitution is not required.
    /// </param>
    /// <returns>
    ///   The number of rows affected.
    /// </returns>
    /// <remarks>
    ///   If the specified SQL's execution fails, the
    ///   the SQL will be included,
    ///   in addition to the reason for failure,
    ///   in the
    ///   <see cref="Exception.Message" />
    ///   of the <see cref="DataException" />
    ///   that will be thrown.
    ///   <para>
    ///     To prevent SQL injection fraud,
    ///     if the specified SQL contains multiple
    ///     statements delimited by semicolons,
    ///     an <see cref="ArgumentException" /> will be thrown
    ///     and none of the statements will be executed.
    ///     A single statement may optionally
    ///     be terminated by a semicolon.
    ///   </para>
    /// </remarks>
    /// <exception cref="AbortedException">
    ///   The execution of the SQL was cancelled by the
    ///   <see cref="CancelLastCommand" /> method.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The specified SQL contains multiple
    ///   statements.
    /// </exception>
    /// <exception cref="DataException">
    ///   The execution of the SQL failed.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found within the Sql folder of the Data folder
    ///   of the executing assembly.
    /// </exception>
    public virtual int Execute(
      string filename,
      Dictionary<string, string> parameters = null) {
      string sql = GetSql(filename, parameters);
      _command =
        new PgSqlCommand(sql, new PgSqlConnection(ConnectionString));
      OpenConnection(_command.Connection);
      _command.CommandTimeout = Timeout;
      int recordsAffected;
      try {
        recordsAffected = _command.ExecuteNonQuery();
      } catch (PgSqlException ex) {
        throw CreateException(ex, sql);
      }
      CloseConnection(_command.Connection);
      return recordsAffected;
    }

    /// <summary>
    ///   Executes the specified SQL query and
    ///   returns the result set.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   within the Sql folder of the Data folder
    ///   of the executing assembly's project
    ///   containing the query's SQL.
    /// </param>
    /// <param name="parameters">
    ///   A dictionary of parameters to be substituted
    ///   in the SQL.
    ///   A null reference (<b>Nothing</b> in Visual Basic)
    ///   if parameter substitution is not required.
    /// </param>
    /// <returns>
    ///   The result set of the specified SQL query.
    /// </returns>
    /// <remarks>
    ///   If the specified SQL's execution fails, the
    ///   the SQL will be included,
    ///   in addition to the reason for failure,
    ///   in the
    ///   <see cref="Exception.Message" />
    ///   of the <see cref="DataException" />
    ///   that will be thrown.
    ///   <para>
    ///     To prevent SQL injection fraud,
    ///     if the specified SQL contains multiple
    ///     statements delimited by semicolons,
    ///     an <see cref="ArgumentException" /> will be thrown
    ///     and none of the statements will be executed.
    ///     A single statement may optionally
    ///     be terminated by a semicolon.
    ///   </para>
    /// </remarks>
    /// <exception cref="AbortedException">
    ///   The execution of the query was cancelled by the
    ///   <see cref="CancelLastCommand" /> method.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The specified SQL contains multiple
    ///   statements.
    /// </exception>
    /// <exception cref="DataException">
    ///   The execution of the query failed.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found within the Sql folder of the Data folder
    ///   of the executing assembly.
    /// </exception>
    public virtual DataTable GetData(
      string filename,
      Dictionary<string, string> parameters = null) {
      string sql = GetSql(filename, parameters);
      var adapter =
        new PgSqlDataAdapter(sql, new PgSqlConnection(ConnectionString));
      _command = adapter.SelectCommand;
      OpenConnection(_command.Connection);
      _command.CommandTimeout = Timeout;
      var table = new DataTable();
      try {
        adapter.Fill(table);
      } catch (PgSqlException ex) {
        throw CreateException(ex, sql);
      }
      CloseConnection(_command.Connection);
      return table;
    }

    /// <summary>
    ///   Returns the text contained in the
    ///   the specified embedded resource file
    ///   in the executing assembly,
    ///   optionally specifying
    ///   a dictionary of parameters to be substituted
    ///   in the text.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   in the executing assembly.
    /// </param>
    /// <param name="parameters">
    ///   A dictionary of parameters to be substituted
    ///   in the text.
    ///   A null reference (<b>Nothing</b> in Visual Basic)
    ///   if parameter substitution is not required.
    /// </param>
    /// <returns>
    ///   The text contained in the specified
    ///   embedded resource file
    ///   in the executing assembly,
    ///   optionally with parameters substituted.
    /// </returns>
    /// <remarks>
    ///   The key of each dictionary entry
    ///   should be a parameter that may be found in the text.
    ///   The value of each dictionary entry
    ///   should be the value with which each occurence of
    ///   the corresponding parameter is to replaced.
    ///   <para>
    ///     To embed a file in an assembly,
    ///     add it to the assembly's project
    ///     and set the file's Build Action property to
    ///     "Embedded Resource".
    ///     To access an embedded resource file that is
    ///     in a folder within the project,
    ///     <paramref name="filename" /> must be prefixed
    ///     by the folder name followed a dot.
    ///     For example, to get the text in file "Test.sql"
    ///     in folder "Embedded" in the executing assembly's project:
    ///     <code>
    /// // C#
    /// string text = GetEmbeddedText("Embedded.Test.sql");
    /// </code>
    ///   </para>
    /// </remarks>
    /// <example>
    ///   <code>
    /// // C#
    /// using System;
    /// using System.Collections.Generic;
    /// 
    /// var parameters = new Dictionary&lt;string, string&gt;();
    /// parameters.Add("#Date", DateTime.Today.ToString("yyyyMMdd"));
    /// parameters.Add("#Time", DateTime.Now.ToString("hh:mm"));
    /// string text = GetEmbeddedText(
    ///     "Embedded.Test.sql", parameters);
    /// </code>
    /// </example>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found in the executing assembly.
    /// </exception>
    private static string GetEmbeddedText(
      //private string GetEmbeddedText(
      string filename,
      Dictionary<string, string> parameters = null) {
      var assembly = Assembly.GetExecutingAssembly();
      string assemblyName = assembly.GetName().Name;
      var stream = assembly.GetManifestResourceStream(
        assemblyName + "." + filename);
      StreamReader reader;
      try {
        reader = new StreamReader(stream);
      } catch (ArgumentNullException) {
        throw new FileNotFoundException(
          "Embedded resource file \"" + filename
                                      + "\" cannot be found in assembly "
                                      + assemblyName
                                      + ". (To be included in an assembly as "
                                      + "an embedded resource, a file's "
                                      + "Build Action property must be set to " +
                                      "\"Embedded Resource\" in the assembly's project.)",
          filename);
      }
      string embeddedText = reader.ReadToEnd();
      reader.Close();
      if (parameters == null) {
        return embeddedText;
      }
      var builder = new StringBuilder(embeddedText);
      foreach (var parameter in parameters) {
        builder.Replace(parameter.Key, parameter.Value);
      } // End of foreach
      return builder.ToString();
    }

    /// <summary>
    ///   Executes the specified SQL query and
    ///   returns the first column of the first row
    ///   in the result set returned by the query.
    ///   Extra columns or rows are ignored.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   within the Sql folder of the Data folder
    ///   of the executing assembly's project
    ///   containing the query's SQL.
    /// </param>
    /// <param name="parameters">
    ///   A dictionary of parameters to be substituted
    ///   in the SQL.
    ///   A null reference (<b>Nothing</b> in Visual Basic)
    ///   if parameter substitution is not required.
    /// </param>
    /// <returns>
    ///   The first column of the first row in the
    ///   result set, or a null reference if the
    ///   result set is empty.
    /// </returns>
    /// <remarks>
    ///   If the specified SQL's execution fails, the
    ///   the SQL will be included,
    ///   in addition to the reason for failure,
    ///   in the
    ///   <see cref="Exception.Message" />
    ///   of the <see cref="DataException" />
    ///   that will be thrown.
    ///   <para>
    ///     To prevent SQL injection fraud,
    ///     if the specified SQL contains multiple
    ///     statements delimited by semicolons,
    ///     an <see cref="ArgumentException" /> will be thrown
    ///     and none of the statements will be executed.
    ///     A single statement may optionally
    ///     be terminated by a semicolon.
    ///   </para>
    /// </remarks>
    /// <exception cref="AbortedException">
    ///   The execution of the query was cancelled by the
    ///   <see cref="CancelLastCommand" /> method.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   The specified SQL contains multiple
    ///   statements.
    /// </exception>
    /// <exception cref="DataException">
    ///   The execution of the query failed.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found within the Sql folder of the Data folder
    ///   of the executing assembly.
    /// </exception>
    public virtual object GetScalar(
      string filename,
      Dictionary<string, string> parameters = null) {
      string sql = GetSql(filename, parameters);
      _command =
        new PgSqlCommand(sql, new PgSqlConnection(ConnectionString));
      OpenConnection(_command.Connection);
      _command.CommandTimeout = Timeout;
      object result;
      try {
        result = _command.ExecuteScalar();
      } catch (PgSqlException ex) {
        throw CreateException(ex, sql);
      }
      CloseConnection(_command.Connection);
      return result;
    }

    /// <summary>
    ///   Returns the SQL contained in the
    ///   the specified embedded resource file
    ///   in the executing assembly,
    ///   optionally specifying
    ///   a dictionary of parameters to be substituted
    ///   in the SQL.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   within the Sql folder of the Data folder
    ///   of the executing assembly's project
    ///   containing the SQL.
    /// </param>
    /// <param name="parameters">
    ///   A dictionary of parameters to be substituted
    ///   in the SQL.
    ///   A null reference (<b>Nothing</b> in Visual Basic)
    ///   if parameter substitution is not required.
    /// </param>
    /// <returns>
    ///   The SQL contained in the specified
    ///   embedded resource file,
    ///   optionally with parameters substituted.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   The specified SQL contains multiple
    ///   statements.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found within the Sql folder of the Data folder
    ///   of the executing assembly.
    /// </exception>
    public static string GetSql(
      //private string GetSql(
      string filename,
      Dictionary<string, string> parameters = null) {
      string sql = GetEmbeddedText("Sql." + filename, parameters);
      RejectMultipleSqlStatements(sql);
      return sql;
    }

    /// <summary>
    ///   Opens the specified <see cref="SqlConnection" />.
    /// </summary>
    /// <param name="connection">
    ///   The <see cref="SqlConnection" /> to be opened.
    /// </param>
    /// <exception cref="DataException">
    ///   The specified <see cref="SqlConnection" />
    ///   could not be opened.
    /// </exception>
    private void OpenConnection(PgSqlConnection connection) {
      try {
        connection.Open();
      } catch (PgSqlException ex) {
        // SqlException's Message
        // just says "System error" unless caught!
        throw new DataException(ex.Message, ex);
      }
    }

    /// <summary>
    ///   Throws an exception if
    ///   the specified SQL contains multiple
    ///   statements delimited by semicolons.
    /// </summary>
    /// <param name="sql">
    ///   The SQL to be checked.
    /// </param>
    /// <remarks>
    ///   The specified SQL will be judged
    ///   to contain multiple statements
    ///   if it contains any semicolons that
    ///   are not enclosed in single-quotes
    ///   and are followed by any non-white-space
    ///   characters.
    ///   So a single statement may optionally
    ///   be terminated by a semicolon.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///   The specified SQL contains multiple
    ///   statements.
    /// </exception>
    private static void RejectMultipleSqlStatements(string sql) {
      //private void RejectMultipleSqlStatements(string sql) {
      // The even-numbered chunks of text 
      // in this array will contain 
      // those portions of the SQL that are outside
      // any string literals.
      // The odd-numbered chunks will be
      // any string literals.
      var chunks = sql.Split("'"[0]);
      var builder = new StringBuilder(
        sql.Length, sql.Length);
      for (var i = 0; i < chunks.Length; i++) {
        if (i % 2 == 0) {
          // Even - this chunk is NOT part
          // of a string literal
          builder.Append(chunks[i]);
        } else {
          // Odd - this chunk IS part
          // of a string literal
          builder.Append("''");
        }
      } // End of for
      // This will be an array of statements
      // with any string literals emptied.
      var statements = builder.ToString().Split(';');
      if (statements.Length > 1) {
        for (var i = 1; i < statements.Length; i++) {
          if (statements[i].Trim() != string.Empty) {
            throw new ArgumentException(
              "Multiple SQL statements were submitted "
              + "but are not supported. "
              + "None of the statements have been executed.",
              "sql");
          }
        } // End of for
      }
    }
  } //End of class
} //End of namespace