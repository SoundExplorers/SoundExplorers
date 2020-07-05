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
    /// A data adapter for accessing 
    /// the table on the SoundExplorers database 
    /// that corresponds to the Entity
    /// of the specified type.
    /// </summary>
    internal class OurSqlDataAdapter<T> : PgSqlDataAdapter     
        where T : Entity<T> {

        #region Public Properties
        /// <summary>
        /// Gets or sets an SQL DELETE statement to execute against the 
        /// SoundExplorers database for an Entity
        /// of the specified type.
        /// </summary>
        public new DeleteCommand<T> DeleteCommand {
            get {
                return (DeleteCommand<T>)base.DeleteCommand;
            }
            set {
                base.DeleteCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets an SQL INSERT statement to execute against the 
        /// SoundExplorers database for an Entity
        /// of the specified type.
        /// </summary>
        public new InsertCommand<T> InsertCommand {
            get {
                return (InsertCommand<T>)base.InsertCommand;
            }
            set {
                base.InsertCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets an SQL SELECT statement to execute against the 
        /// SoundExplorers database for an Entity
        /// of the specified type.
        /// </summary>
        public new SelectCommand<T> SelectCommand {
            get {
                return (SelectCommand<T>)base.SelectCommand;
            }
            set {
                base.SelectCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets an SQL UPDATE statement to execute against the 
        /// SoundExplorers database for an Entity
        /// of the specified type.
        /// </summary>
        public new UpdateCommand<T> UpdateCommand {
            get {
                return (UpdateCommand<T>)base.UpdateCommand;
            }
            set {
                base.UpdateCommand = value;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="OurSqlDataAdapter"/> class.
        /// </summary>
        /// <param name="selectCommand">
        /// SELECT command.
        /// Null to to get the SQL from an embedded resource file,
        /// if found, or generate it
        /// </param>
        /// <param name="insertCommand">
        /// INSERT command.
        /// Null to to get the SQL from an embedded resource file,
        /// if found, or generate it
        /// </param>
        /// <param name="updateCommand">
        /// UPDATE command.
        /// Null to to get the SQL from an embedded resource file,
        /// if found, or generate it
        /// </param>
        /// <param name="deleteCommand">
        /// DELETE command.
        /// Null to to get the SQL from an embedded resource file,
        /// if found, or generate it
        /// </param>
        /// <exception cref="DataException">
        /// Error on preparing one of the SQL commands.
        /// </exception>
        public OurSqlDataAdapter(
                SelectCommand<T> selectCommand = null,
                InsertCommand<T> insertCommand = null,
                UpdateCommand<T> updateCommand = null,
                DeleteCommand<T> deleteCommand = null) {
            if (selectCommand != null) {
                this.SelectCommand = selectCommand;
            } else {
                this.SelectCommand = new SelectCommand<T>();
            }
            this.SelectCommand.Prepare();
            if (insertCommand != null) {
                this.InsertCommand = insertCommand;
            } else {
                this.InsertCommand = new InsertCommand<T>();
            }
            this.InsertCommand.Prepare();
            if (updateCommand != null) {
                this.UpdateCommand = updateCommand;
            } else {
                this.UpdateCommand = new UpdateCommand<T>();
            }
            this.UpdateCommand.Prepare();
            if (deleteCommand != null) {
                this.DeleteCommand = deleteCommand;
            } else {
                this.DeleteCommand = new DeleteCommand<T>();
            }
            this.DeleteCommand.Prepare();
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Calls the respective INSERT, UPDATE, or DELETE 
        /// statements for each inserted, updated, 
        /// or deleted row in the specified <see cref="DataTable"/>.
        /// </summary>
        /// <param name="dataTable">
        /// The <see cref="DataTable"/> used to update the data source. 
        /// </param>
        /// <param name="oldKeyFields">
        /// The names and original values of the primary key fields.
        /// </param>
        /// <exception cref="DataException">
        /// Thrown if
        /// there is an error on attempting to access the database.
        /// </exception>
        public virtual void Update(
                DataTable dataTable, 
                Dictionary<string, object> oldKeyFields = null) {
            try {
                if (oldKeyFields != null) {
                    foreach (KeyValuePair<string, object> oldKeyField in oldKeyFields) {
                        this.UpdateCommand.Parameters[
                            "@OLD_" + oldKeyField.Key].Value = oldKeyField.Value;
                    } // End of foreach
                }
                base.Update(dataTable);
            } catch (PgSqlException ex) {
                throw new DataException(ex.Message, ex);
            }
        }
        #endregion Public Methods
    }//End of class
}//End of namespace